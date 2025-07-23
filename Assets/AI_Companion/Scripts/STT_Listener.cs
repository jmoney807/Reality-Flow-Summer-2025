using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using Neocortex.Samples;//chatIntentProcessor.cs
using System.Collections.Generic;
using System.Linq;

public class STT_Listener : MonoBehaviour
{
    private TcpListener listener;
    private Thread listenerThread;
    [SerializeField] private User_Intent user_Intent; // Reference to chatIntentProcessor to send transcriptions directly
    [SerializeField] private Text displayText;
    private string incomingText = "";

    private List<string> pendingFinals = new List<string>();
    private float bufferTimer = 0f;
    private float bufferWindow = 0.7f; // tweak this if needed
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
            listener = new TcpListener(IPAddress.Loopback, 65434);
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
                    string best = pendingFinals.OrderByDescending(s => s.Length).First();
                    user_Intent.HandleSpeechTranscription(best);
                    displayText.text = best;
                    pendingFinals.Clear();
                }

            }
        }
        
        if (!string.IsNullOrEmpty(incomingText))
        {
            displayText.text = incomingText;

            if (!incomingText.StartsWith("partial"))
            {
                if (!pendingFinals.Contains(incomingText))
                {
                    pendingFinals.Add(incomingText);
                }

                buffering = true;
                bufferTimer = 0f;
            }


            incomingText = "";  // clear input
        }

    }

    void OnApplicationQuit()
    {
        listener?.Stop();
    }
}
