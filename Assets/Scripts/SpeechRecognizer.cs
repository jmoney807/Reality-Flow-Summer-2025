using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Neocortex.Samples;//ChatSample.cs
using System.Collections.Generic;
using System.Linq;

public class SpeechRecognizer : MonoBehaviour
{
    private TcpListener listener;
    private Thread listenerThread;
    [SerializeField] private ChatSample chatSample; // Reference to ChatSample to send transcriptions directly

    [SerializeField] private Text displayText;
    [SerializeField] private InputField LLM_message;
    private string incomingText = "";

    private List<string> pendingFinals = new();
    private float bufferTimer = 0f;
    private float bufferWindow = 1.5f; // tweak this if needed
    private bool buffering = false;


    void Start()
    {
        listenerThread = new Thread(ListenForData);
        listenerThread.IsBackground = true;
        listenerThread.Start();
    }

    void ListenForData()
    {
        try
        {
            listener = new TcpListener(IPAddress.Loopback, 65432);
            listener.Start();
            Debug.Log("🟢 Unity listening for Vosk...");

            while (true)
            {
                using (TcpClient client = listener.AcceptTcpClient())
                using (NetworkStream stream = client.GetStream())
                {
                    byte[] buffer = new byte[1024];
                    int bytesRead;

                    while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        incomingText = message.Trim();
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Socket error: " + e.Message);
        }
    }

    void Update()
    {
        // Handle buffering logic
        if (buffering)
        {
            bufferTimer += Time.deltaTime;
            if (bufferTimer >= bufferWindow)
            {
                buffering = false;
                bufferTimer = 0f;

                if (pendingFinals.Count > 0)
                {
                    // Choose the longest final transcript
                    string best = pendingFinals.OrderByDescending(s => s.Length).First();
                    chatSample.HandleSpeechTranscription(best);         // ✅ Only send one best message
                    LLM_message.text = best;
                    displayText.text = best;                            // (optional UI update)
                    pendingFinals.Clear();
                }
            }
        }
        if (!string.IsNullOrEmpty(incomingText))
        {
            displayText.text = incomingText;

            if (!incomingText.StartsWith("partial"))
            {
                buffering = true;
                bufferTimer = 0f;
                pendingFinals.Add(incomingText);  // ✅ Store it for later
            }

            incomingText = "";  // clear the buffer regardless
        }
        /*if (!string.IsNullOrEmpty(incomingText))
        {
            displayText.text = incomingText;

            if (!incomingText.StartsWith("partial"))
            {
                chatSample.HandleSpeechTranscription(incomingText); // 👈 Send it directly to LLM logic
                LLM_message.text = incomingText; // Update the LLM message input field
                incomingText = "";
            }
            //incomingText = "";
        }*/
    }

    void OnApplicationQuit()
    {
        listener?.Stop();
    }
}
