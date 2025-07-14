using UnityEngine;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEditor; // Add this for AssetDatabase

public class CreatePremade : MonoBehaviour
{
    // Event to create a new premade
    public static event Action OnCreatePremadeRequested;
    public static void RequestCreatePremade() => OnCreatePremadeRequested?.Invoke();

    private void OnEnable()
    {
        OnCreatePremadeRequested += SaveModelLocally;
    }

    // Sanitize transcription name for file naming
    private string SanitizeTranscriptionName(string transcriptionName)
    {
        if (string.IsNullOrEmpty(transcriptionName))
            return string.Empty;

        // Remove leading/trailing spaces and convert internal spaces to underscores
        string sanitized = transcriptionName.Trim();

        // Remove leading articles (A, a, An, an)
        string[] articles = { "A ", "a ", "An ", "an " };
        foreach (string article in articles)
        {
            if (sanitized.StartsWith(article))
            {
                sanitized = sanitized.Substring(article.Length).Trim();
                break; // Only remove the first occurrence
            }
        }

        // Replace spaces with underscores
        sanitized = sanitized.Replace(" ", "_");

        // Remove periods
        sanitized = sanitized.Replace(".", "");

        // Remove other problematic characters for file names
        char[] invalidChars = System.IO.Path.GetInvalidFileNameChars();
        foreach (char c in invalidChars)
        {
            sanitized = sanitized.Replace(c.ToString(), "");
        }

        return sanitized;
    }

    public void SaveModelLocally()
    {
        Debug.Log("Saving current model locally...");

        // string modelName = SanitizeTranscriptionName(TranscriptionUIManager.Instance.TranscriptionResult);
        // byte[] rembgResult = ImageGenerationManager.Instance.RembgResult;
        // byte[] SPAR3DResult = ObjectGenerationManager.Instance.SPAR3DResult;
        string modelName = SanitizeTranscriptionName(TranscriptionUIModel.Instance.TranscriptionResult);
        byte[] rembgResult = ImageGenerationUIModel.Instance.RembgResult;
        byte[] SPAR3DResult = ObjectGenerationUIModel.Instance.SPAR3DResult;

        // Create directory path if it doesn't exist
        string directoryPath = Path.Combine(Application.dataPath, LoadPremadesModel.Instance.PremadesBaseDirectory, modelName);
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
        else
        {
            Debug.LogWarning($"Directory already exists: {directoryPath}");
            // Add the unique timestamp to avoid overwriting
            directoryPath = Path.Combine(directoryPath, $"{modelName}_{DateTime.Now:yyyyMMdd_HHmmss}");
            Directory.CreateDirectory(directoryPath); // Ensure the directory is created even if it exists
        }

        // Save rembg result
        if (rembgResult != null && rembgResult.Length > 0)
        {
            string rembgPath = Path.Combine(directoryPath, $"rembg_result_{DateTime.Now:yyyyMMdd_HHmmss}.png");
            File.WriteAllBytes(rembgPath, rembgResult);
            Debug.Log($"Rembg result saved to: {rembgPath}");
        }

        // Save SPAR3D result
        if (SPAR3DResult != null && SPAR3DResult.Length > 0)
        {
            string spar3dPath = Path.Combine(directoryPath, $"SPAR3D_result_{DateTime.Now:yyyyMMdd_HHmmss}.glb");
            File.WriteAllBytes(spar3dPath, SPAR3DResult);
            Debug.Log($"SPAR3D result saved to: {spar3dPath}");
        }
        #if UNITY_EDITOR
            // Refresh the AssetDatabase to make files appear instantly in Unity
            AssetDatabase.Refresh();
        #endif

        // Update the button list to reflect new premade
        LoadPremadesPresenter.RequestRefreshPremadeList();
    }

    // public async void SaveModelLocally()
    // {
    //      Debug.Log("Saving current model locally...");

    //     string modelName = SanitizeTranscriptionName(TranscriptionUIManager.Instance.TranscriptionResult);
    //     byte[] rembgResult = ImageGenerationManager.Instance.RembgResult;
    //     byte[] SPAR3DResult = ObjectGenerationManager.Instance.SPAR3DResult;

    //     // Create directory path if it doesn't exist
    //     string directoryPath = Path.Combine(Application.dataPath, LoadPremadesModel.Instance.PremadesBaseDirectory, modelName);
    //     if (!Directory.Exists(directoryPath))
    //     {
    //         Directory.CreateDirectory(directoryPath);

    //     }

    //     // Save both files concurrently using tasks
    //     var tasks = new List<Task>();

    //     // Save rembg result async
    //     if (rembgResult != null && rembgResult.Length > 0)
    //     {
    //         string rembgPath = Path.Combine(directoryPath, $"rembg_result_{DateTime.Now:yyyyMMdd_HHmmss}.png");
    //         tasks.Add(File.WriteAllBytesAsync(rembgPath, rembgResult));
    //         Debug.Log($"Started saving rembg result to: {rembgPath}");
    //     }

    //     // Save SPAR3D result async
    //     if (SPAR3DResult != null && SPAR3DResult.Length > 0)
    //     {
    //         string spar3dPath = Path.Combine(directoryPath, $"SPAR3D_result_{DateTime.Now:yyyyMMdd_HHmmss}.glb");
    //         tasks.Add(File.WriteAllBytesAsync(spar3dPath, SPAR3DResult));
    //         Debug.Log($"Started saving SPAR3D result to: {spar3dPath}");
    //     }

    //     // Wait for all saves to complete
    //     await Task.WhenAll(tasks);
    //     Debug.Log("All files saved successfully!");

    //     // Update the button list to reflect new premade
    //     LoadPremadesPresenter.RequestRefreshPremadeList();
    // }
}
