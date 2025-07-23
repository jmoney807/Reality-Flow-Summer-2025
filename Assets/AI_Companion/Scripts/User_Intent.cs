using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Neocortex.Data;
using System.Text.RegularExpressions;
using System.IO;

namespace Neocortex.Samples
{
    public class User_Intent : MonoBehaviour
    {
        //[SerializeField] private NeocortexChatPanel chatPanel;
        //[SerializeField] private OllamaModelDropdown modelDropdown;
        [SerializeField, TextArea] private string systemPrompt;
        [SerializeField, TextArea] private string responsePrompt;
        [SerializeField] private User_Intent user_Intent;//User STT

        public class LLMResponse
        {
            public string answer;
            public string reason;
        }

        public bool isProcessing = false; // To track the last final message sent to the LLM
        private OllamaRequest request;
        private Queue<string> messageQueue = new();
        private Coroutine messageLoopCoroutine;
        private string modelName = "llama3.2:3b"; // Default model name


        private void Awake()
        {
            // Load the system prompt from the text file
            TextAsset promptAsset = Resources.Load<TextAsset>("SystemPrompt");
            string promptPath = Path.Combine(Application.dataPath, "AI_Companion/Prompts/SystemPrompt.txt");
            systemPrompt = File.ReadAllText(promptPath);
        }

        void Start()
        {
            request = new OllamaRequest();
            messageLoopCoroutine = StartCoroutine(ProcessMessageQueue());
            request.ModelName = modelName; // Set the default model name
            //request.ModelName = modelDropdown.options[0].text;
            //modelDropdown.onValueChanged.AddListener(OnDropdownValueChanged);
            request.AddSystemMessage(systemPrompt);
        }

        /*private void OnDropdownValueChanged(int index)
        {
            request.ModelName = "llama3.2:1b";
            //request.ModelName = modelDropdown.options[index].text;
        }*/

        private void OnChatResponseReceived(ChatResponse response)
        {
            string message = response.message.Trim();

            //chatPanel.AddMessage(message, false);
        }

        private IEnumerator HandleUserIntent(string userMessage)
        {
            // 🛑 Skip any user message over 500 characters
            if (userMessage.Length > 200)
            {
                Debug.LogWarning("⛔ Skipped LLM check — user message too long (" + userMessage.Length + " chars).");
                yield break;
            }

            // Load the response prompt from the text file
            //TextAsset promptAsset = Resources.Load<TextAsset>("ResponsePrompt");
            string promptPath = Path.Combine(Application.dataPath, "AI_Companion/Prompts/ResponsePrompt.txt");
            string promptText = File.ReadAllText(promptPath);


            // Combine the user's message with the prompt template for intent checking
            string responsePrompt = $"{promptText}\n\nUser message: {userMessage}";

            // Variables to track when the AI response is received and to store the result
            bool isDone = false;
            string result = "";
            string reason = "";

            // Local function to handle the AI's response to the intent check
            void OnResponse(ChatResponse response)
            {
                // Store the AI's response (lowercased and trimmed) and mark as done
                result = response.message.ToLower().Trim();

                //Debug.LogWarning("⚠️ Raw LLM response:\n" + result);

                // Extract first JSON block using regex
                Match match = Regex.Match(result, @"\{\s*""answer""\s*:\s*""(yes|no)""\s*,\s*""reason""\s*:\s*""[^""]*""\s*\}");

                if (match.Success)
                {
                    string json = match.Value;
                    Debug.Log($"✅ Cleaned JSON: {json}");

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

            
            Debug.Log($"🔍 User message: \"{userMessage}");
            if (result == "yes")
            {
                Debug.Log("✅ Detected: Creation-related command.");

                // Add Alejandro's UI Canvas message
                //chatPanel.AddMessage(userMessage, true);//show user input in chat panel

                // Update the transcription UI model with the final result
                TranscriptionUIModel.Instance.TranscriptionResult = userMessage;
                TranscriptionUIModel.Instance.IsFinalResult = true;
            }
            else
            {
                Debug.Log("🛑 Ignored: Not related to creation.");
            }
            Debug.Log($"Reason: {reason}");
        }

        public void HandleSpeechTranscription(string message)
        {
            // Ignore empty, duplicate, or obviously bad input
            if (string.IsNullOrWhiteSpace(message)) return;

            string lower = message.ToLower().Trim();
            int wordCount = lower.Split(' ').Length;

            // Skip 1-2 word messages (usually not valid creation commands)
            if (wordCount <= 2 || lower.Length < 4)
            {
                Debug.Log($"⏭️ Skipping short or duplicate message: \"{message}\"");
                return;
            }

            messageQueue.Enqueue(message);
            Debug.Log($"📥 Queued: \"{message}\" | Queue size: {messageQueue.Count}");
        }

        private IEnumerator ProcessMessageQueue()
        {
            while (true)
            {
                if (!isProcessing && messageQueue.Count > 0)
                {
                    string message = messageQueue.Dequeue();
                    Debug.Log($"📤 Dequeued: \"{message}\" | Remaining queue: {messageQueue.Count}");
                    isProcessing = true; // Set flag to indicate processing has started 
                    yield return HandleUserIntent(message);
                    isProcessing = false; // Reset flag after processing
                }
                yield return null; // wait for next frame
            }
        }

        private void OnDestroy()
        {
            request.OnChatResponseReceived -= OnChatResponseReceived;
            //modelDropdown.onValueChanged.RemoveListener(OnDropdownValueChanged);
        }
    }
}