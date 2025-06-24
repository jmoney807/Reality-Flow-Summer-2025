using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Neocortex.Data;

namespace Neocortex.Samples
{
    public class ChatSample : MonoBehaviour
    {
        [SerializeField] private NeocortexChatPanel chatPanel;
        [SerializeField] private NeocortexTextChatInput chatInput;
        [SerializeField] private OllamaModelDropdown modelDropdown;
        [SerializeField, TextArea] private string systemPrompt;

        private OllamaRequest request;

         private void Awake()
        {
            // Load the system prompt from the text file
            TextAsset promptAsset = Resources.Load<TextAsset>("SystemPrompt");
            if (promptAsset != null)
            {
                systemPrompt = promptAsset.text;
            }
            else
            {
                Debug.LogError("SystemPrompt.txt not found in Resources folder!");
            }
        }

        void Start()
        {

            request = new OllamaRequest();
            request.OnChatResponseReceived += OnChatResponseReceived;
            request.ModelName = modelDropdown.options[0].text;
            chatInput.OnSendButtonClicked.AddListener(OnUserMessageSent);
            modelDropdown.onValueChanged.AddListener(OnDropdownValueChanged);

            request.AddSystemMessage(systemPrompt);

            //StartCoroutine(RunTestCases());
        }

        private void OnDropdownValueChanged(int index)
        {
            request.ModelName = modelDropdown.options[index].text;
        }

        private void OnChatResponseReceived(ChatResponse response)
        {
            chatPanel.AddMessage(response.message, false);
        }

        private void OnUserMessageSent(string message)
        {
            //request.Send(message);
            chatPanel.AddMessage(message, true);
            StartCoroutine(HandleUserIntent(message));
        }
        private readonly List<string> positivePrompts = new()
        {
            "Create a cube in front of me.",
            "Spawn a tree right here.",
            "Make the room brighter.",
            "Can you rotate that object to the left?",
            "Build a wall between these two platforms.",
            "Add a chair near the window.",
            "Duplicate that box over there.",
            "Place a red cylinder on this table.",
            "Delete the lamp above my head.",
            "Move that screen closer."
        };

        private readonly List<string> negativePrompts = new()
        {
            "Hey, what’s up?",
            "Did you watch the game last night?",
            "Where’s John right now?",
            "That was so cool!",
            "I’m getting tired.",
            "Do you want to grab coffee after this?",
            "I can’t hear you.",
            "Let's take a break.",
            "How do I mute myself?",
            "What did you think of the last meeting?"
        };

        private IEnumerator HandleUserIntent(string userMessage)
        {
            string prompt = $"The user said: \"{userMessage}\"\n\nIs this an instruction related to creating, modifying, or deleting something in the Reality Flow VR environment? Answer with only \"yes\" or \"no\".";

            bool isDone = false;
            string result = "";

            request.OnChatResponseReceived += OnResponse;

            void OnResponse(ChatResponse response)    
            {
                result = response.message.ToLower().Trim();
                isDone = true;
            }

            request.Send(prompt);

            yield return new WaitUntil(() => isDone);
            request.OnChatResponseReceived -= OnResponse;

            if (result.Contains("yes"))
            {
                Debug.Log("✅ Detected: Creation-related command.");
                // Optional: continue to process the command
                request.Send(userMessage); // Send original input to get a real response
            }
            else
            {
                Debug.Log("🛑 Ignored: Not related to creation.");
                // Optional: respond with a default message
                chatPanel.AddMessage("(AI ignored message not related to creation/modification.)", false);
            }
        }

        private IEnumerator RunTestCases()
        {
            foreach (var prompt in positivePrompts)
                yield return TestPrompt(prompt, "yes");

            foreach (var prompt in negativePrompts)
                yield return TestPrompt(prompt, "no");
        }

        private IEnumerator TestPrompt(string userSpeech, string expected)
        {
            string testPrompt = $"The user said: \"{userSpeech}\"\n\nIs this an instruction related to creating or modifying something in the Reality Flow VR environment? Answer with only \"yes\" or \"no\".";

            bool isDone = false;
            string result = "";

            request.OnChatResponseReceived += (ChatResponse response) =>
            {
                result = response.message.ToLower().Trim();
                isDone = true;
            };

            request.Send(testPrompt);

            yield return new WaitUntil(() => isDone);

            string status = result.Contains(expected) ? "✅ PASS" : "❌ FAIL";
            Debug.Log($"[Input] {userSpeech}\n[Expected] {expected}\n[LLM Response] {result}\n{status}");
        }

            private void OnDestroy()
            {
                request.OnChatResponseReceived -= OnChatResponseReceived;
                chatInput.OnSendButtonClicked.RemoveListener(OnUserMessageSent);
                modelDropdown.onValueChanged.RemoveListener(OnDropdownValueChanged);
            }

    }
}