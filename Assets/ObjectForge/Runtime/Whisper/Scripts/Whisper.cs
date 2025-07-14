// using OpenAI;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

namespace GenerativeAI.Whisper
{
    public class Whisper : MonoBehaviour
    {
        // public static Whisper Instance { get; private set; } // I dont think we need this singleton pattern here, as Whisper is not a manager class.
        public static event Action OnRecordingStarted;
        public static void RequestRecordingStart() => OnRecordingStarted?.Invoke();

        public static event Action OnRecordingEnded;
        public static void RequestRecordingEnd() => OnRecordingEnded?.Invoke();

        // [SerializeField] private string _whisperServerUrl = "http://localhost:8001/transcribe";
        // [SerializeField] private string _whisperServerUrl = "http://127.0.0.1:8001/transcribe";
        // [SerializeField] private string _whisperServerUrl = "http://0.0.0.0:8001/transcribe";
        [SerializeField] private string _whisperServerUrl = "http://host.docker.internal:8001/transcribe";

        private readonly string fileName = "output.wav";
        private readonly int duration = 5;  
        
        private AudioClip clip;

        private void Start()
        {
        }

        private void OnEnable()
        {
            OnRecordingStarted += RecordSpeech;
            OnRecordingEnded += EndRecording;
        }
        private void OnDisable()
        {
            OnRecordingStarted -= RecordSpeech;
            OnRecordingEnded -= EndRecording;
        }

        public void RecordSpeech()
        {
            Debug.Log("Recording User Speech...");

            // Instead the model will invoke an event that shows the transcription UI
            TranscriptionUIModel.Instance.TranscriptionResult = "Recording...";
            TranscriptionUIModel.Instance.IsFinalResult = false; 

            string selectedMic = ChangeMicrophoneModel.Instance.SelectedMicrophone; // Use the new model to get the selected microphone

#if !UNITY_WEBGL
            Debug.Log("Recording with microphone: " + selectedMic);
            clip = Microphone.Start(selectedMic, false, duration, 44100);
#endif
        }

        public void EndRecording()
        {
            Debug.Log("End recording");

            TranscriptionUIModel.Instance.TranscriptionResult = "Transcribing...";
            TranscriptionUIModel.Instance.IsFinalResult = false; 

            #if !UNITY_WEBGL
            // Use the same microphone that we started recording from
            string selectedMic = ChangeMicrophoneModel.Instance.SelectedMicrophone;

            Microphone.End(selectedMic);
            #endif

            byte[] data = SaveWav.Save(fileName, clip);

            StartCoroutine(SendTranscriptionRequest(data));
        }

        private IEnumerator SendTranscriptionRequest(byte[] audioData)
        {
            // Create form and add audio data
            WWWForm form = new WWWForm();
            form.AddBinaryData("file", audioData, "audio.wav", "audio/wav");

            using (UnityWebRequest www = UnityWebRequest.Post(_whisperServerUrl, form))
            {
                yield return www.SendWebRequest();

                Debug.Log($"Request completed. Result: {www.result}");
                Debug.Log($"Response Code: {www.responseCode}");
                Debug.Log($"Error (if any): {www.error}");
                Debug.Log($"Response text: {www.downloadHandler.text}");

                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"Request failed with result: {www.result}");
                    Debug.LogError($"Error: {www.error}");
                    Debug.LogError($"Response Code: {www.responseCode}");

                    TranscriptionUIModel.Instance.TranscriptionResult = "Transcription failed: " + www.error;
                    TranscriptionUIModel.Instance.IsFinalResult = false; 
                }
                else
                {
                    // Parse the JSON result
                    string json = www.downloadHandler.text;

                    var result = JsonUtility.FromJson<TranscriptionResponse>(json);

                    TranscriptionUIModel.Instance.TranscriptionResult = result.text;
                    TranscriptionUIModel.Instance.IsFinalResult = true; 
                }
                Debug.Log("Transcription completed successfully.");
            }
        }
    }
}

[System.Serializable]
public class TranscriptionResponse
{
    public string text;
}
