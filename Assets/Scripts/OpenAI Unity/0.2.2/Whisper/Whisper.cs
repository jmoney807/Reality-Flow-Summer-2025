using OpenAI;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // Assuming you are using TextMeshPro for text rendering

// allows us to use the OpenAI API locally
using UnityEngine.Networking;

namespace Samples.Whisper
{
    public class Whisper : MonoBehaviour
    {
        [SerializeField] private Button recordButton;
        [SerializeField] private Image progressBar;
        [SerializeField] private Text message;
        [SerializeField] private Dropdown dropdown;
        [SerializeField] private InputField LLM_message;
        
        private readonly string fileName = "output.wav";
        private readonly int duration = 5;
        
        private AudioClip clip;
        private bool isRecording;
        private float time;
        //private OpenAIApi openai = new OpenAIApi();

        private void Start()
        {
            #if UNITY_WEBGL && !UNITY_EDITOR
            dropdown.options.Add(new Dropdown.OptionData("Microphone not supported on WebGL"));
            #else
            foreach (var device in Microphone.devices)
            {
                dropdown.options.Add(new Dropdown.OptionData(device));
            }
            recordButton.onClick.AddListener(StartRecording);
            dropdown.onValueChanged.AddListener(ChangeMicrophone);
            
            var index = PlayerPrefs.GetInt("user-mic-device-index");
            dropdown.SetValueWithoutNotify(index);
            #endif
        }

        private void ChangeMicrophone(int index)
        {
            PlayerPrefs.SetInt("user-mic-device-index", index);
        }
        
        private void StartRecording()
        {
            isRecording = true;
            recordButton.enabled = false;

            var index = PlayerPrefs.GetInt("user-mic-device-index");
            
            #if !UNITY_WEBGL
            clip = Microphone.Start(dropdown.options[index].text, false, duration, 44100);
            #endif
        }

        private async void EndRecording()
        {
            message.text = "Transcripting...";
            
            #if !UNITY_WEBGL
            Microphone.End(null);
            #endif
            
            byte[] data = SaveWav.Save(fileName, clip);
            
            var req = new CreateAudioTranscriptionsRequest
            {
                FileData = new FileData() {Data = data, Name = "audio.wav"},
                // File = Application.persistentDataPath + "/" + fileName,
                Model = "whisper-1",
                Language = "en"
            };

            // Create form and add audio data
            WWWForm form = new WWWForm();
            form.AddBinaryData("file", data, "audio.wav", "audio/wav");

           using (UnityWebRequest www = UnityWebRequest.Post("http://127.0.0.1:8000/transcribe", form))
            {
                // www.SetRequestHeader("Content-Type", "multipart/form-data");
                await www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError("Error: " + www.error);
                    message.text = "Transcription failed.";
                }
                else
                {
                    // Parse the JSON result
                    var json = www.downloadHandler.text;
                    var result = JsonUtility.FromJson<TranscriptionResponse>(json);
                    message.text = result.text;
                    LLM_message.text = result.text;

                }

                progressBar.fillAmount = 0;
                recordButton.enabled = true;
            }
        }

        private void Update()
        {
            if (isRecording)
            {
                time += Time.deltaTime;
                progressBar.fillAmount = time / duration;

                Debug.Log("Fill Amount: " + progressBar.fillAmount);
                
                if (time >= duration)
                {
                    time = 0;
                    isRecording = false;
                    EndRecording();
                }
            }
        }
    }
}

[System.Serializable]
public class TranscriptionResponse
{
    public string text;
}