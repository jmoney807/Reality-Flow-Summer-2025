using UnityEngine;
using Diagnostics = System.Diagnostics;


public class VoskLauncher : MonoBehaviour
{
    private Diagnostics.Process voskProcess;


    void Start()
    {
        // pythonw is used to run Python scripts without opening a console window
        // python is used to run Python scripts with a console window
        string pythonPath = "pythonw";
        string Vosk_Script_path = Application.dataPath + "/AI_Companion/Scripts/Python/Vosk_speech_to_text.py";
        var psi = new Diagnostics.ProcessStartInfo
        {
            FileName = pythonPath,
            Arguments = $"\"{Vosk_Script_path}\"",
        };

        voskProcess = Diagnostics.Process.Start(psi);
        Debug.Log("🚀 Vosk Python script started");
    }

    void OnApplicationQuit()
    {
        try
        {
            if (voskProcess != null && !voskProcess.HasExited)
            {
                voskProcess.Kill(); // ✅ Kills process and subprocesses (requires .NET 5+ or Unity 2021+)
                voskProcess.Dispose();
                UnityEngine.Debug.Log("🛑 Vosk Python process killed (with tree)");
            }
        }
        catch (System.Exception ex)
        {
            UnityEngine.Debug.LogError("❌ Failed to kill Vosk process: " + ex.Message);
        }
    }
}