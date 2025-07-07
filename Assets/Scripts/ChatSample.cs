using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Neocortex.Data;
using System.Text.RegularExpressions;

namespace Neocortex.Samples
{
    public class ChatSample : MonoBehaviour
    {
        [SerializeField] private NeocortexChatPanel chatPanel;
        [SerializeField] private NeocortexTextChatInput chatInput;
        [SerializeField] private OllamaModelDropdown modelDropdown;
        [SerializeField, TextArea] private string systemPrompt;
        [SerializeField, TextArea] private string responsePrompt;
        [SerializeField] private ChatSample chatSample;//User STT

        //[System.Serializable]
        public class LLMResponse
        {
            public string answer;
            public string reason;
        }


        private bool isGenerating = false; // To prevent multiple simultaneous requests
        private string lastFinalMessage = ""; // To track the last final message sent to the LLM
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
            //request.OnChatResponseReceived += OnChatResponseReceived;
            request.ModelName = modelDropdown.options[0].text;
            chatInput.OnSendButtonClicked.AddListener(OnUserMessageSent);
            modelDropdown.onValueChanged.AddListener(OnDropdownValueChanged);

            request.AddSystemMessage(systemPrompt);
        }

        private void OnDropdownValueChanged(int index)
        {
            request.ModelName = modelDropdown.options[index].text;
        }

        private void OnChatResponseReceived(ChatResponse response)
        {
            string message = response.message.Trim();

            chatPanel.AddMessage(message, false);
        }

        private void OnUserMessageSent(string message)
        {
            // Start HandleUserIntent as as coroutine because it waits asynchronously for the AI response before proceeding.
            StartCoroutine(HandleUserIntent(message));
        }

        private IEnumerator HandleUserIntent(string userMessage)
        {
            // Load the response prompt from the text file
            TextAsset promptAsset = Resources.Load<TextAsset>("ResponsePrompt");

            // Combine the user's message with the prompt template for intent checking
            string responsePrompt = $"The user said: \"{userMessage}\"\n\n" + promptAsset.text;

            // Variables to track when the AI response is received and to store the result
            bool isDone = false;
            string result = "";
            string reason = "";

            // Local function to handle the AI's response to the intent check
            void OnResponse(ChatResponse response)
            {
                // Store the AI's response (lowercased and trimmed) and mark as done
                result = response.message.ToLower().Trim();

                // Extract first JSON block using regex
                Match match = Regex.Match(result, @"\{[\s\S]*?\}");

                if (match.Success)
                {
                    string json = match.Value;
                    Debug.Log($"✅ Extracted JSON: {json}");

                    try
                    {
                        LLMResponse parsed = JsonUtility.FromJson<LLMResponse>(json);
                        result = parsed.answer;
                        reason = parsed.reason;
                        Debug.Log($"🧠 Reason: {reason}");
                    }
                    catch
                    {
                        Debug.LogError("❌ Failed to parse extracted JSON.");
                        result = "unknown";
                    }
                }
                else
                {
                    Debug.LogError("❌ No JSON found in LLM response.");
                    result = "unknown";
                }

                isDone = true;
            }

            request.OnChatResponseReceived += OnResponse;
            request.Send(responsePrompt);

            // Wait until the AI response is received
            yield return new WaitUntil(() => isDone);

            // Unsubscribe from the response event to avoid duplicate handling
            request.OnChatResponseReceived -= OnResponse;

            Debug.Log($"LLM Filter Result: {result}");
            if (result == "yes")
            {
                Debug.Log("✅ Detected: Creation-related command.");
                isGenerating = true; // Set flag to prevent further requests while generating

                chatPanel.AddMessage(userMessage, true);//show user input in chat panel
                chatPanel.AddMessage(reason, false);
                //request.Send(userMessage); // Send message to the LLM

                //⏳ Optional: wait until generation is done or a cooldown ends
                yield return new WaitForSeconds(5f); // Adjust to match your generation time

                // this will be called to 3D model creation
                isGenerating = false;
            }
            else
            {
                Debug.Log("🛑 Ignored: Not related to creation.");
                Debug.Log($"Reason: {reason}");
                yield return new WaitForSeconds(5f);
                //chatPanel.AddMessage("(AI ignored message not related to creation/modification.)", false);
            }
        }

        public void HandleSpeechTranscription(string message)
        {
            // Ignore empty, duplicate, or obviously bad input
            if (string.IsNullOrWhiteSpace(message)) return;

            string lower = message.ToLower().Trim();

            // Skip 1-2 word messages (usually not valid creation commands)
            int wordCount = lower.Split(' ').Length;
            if (wordCount <= 2)
            {
                Debug.Log($"⏭️ Skipping short message: \"{message}\"");
                return;
            }

            if (lower == "the" || lower == "a" || lower.Length < 4) return;
            if (lower == lastFinalMessage) return;

            if (isGenerating) return; //Don't process if already generating a response

            lastFinalMessage = lower;
            StartCoroutine(HandleUserIntent(message));
        }
        

        private void OnDestroy()
        {
            request.OnChatResponseReceived -= OnChatResponseReceived;
            chatInput.OnSendButtonClicked.RemoveListener(OnUserMessageSent);
            modelDropdown.onValueChanged.RemoveListener(OnDropdownValueChanged);
        }
    }
}