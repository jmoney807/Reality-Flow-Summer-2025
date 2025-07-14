using UnityEngine;

public class UpdateLoadPremadesUI : MonoBehaviour
{
    // public void PopulateButtonList(string subDirectory)
    // {
    //     CreateButtonsForPremades(subDirectory);
    // }
    
    // public void CreateButtonsForPremades(string subDirectory)
    // {
    //     string basePath = Path.Combine(Application.dataPath, premadeBaseDirectory, subDirectory);

    //     string favoritesPath = Path.Combine(Application.dataPath, premadeBaseDirectory, "All Premades", "Favorites");

    //     // Get all directories in the base path
    //     string[] allDirectories = Directory.GetDirectories(basePath);

    //     bool isFavoritesDirectory = false;

    //     if (subDirectory == "All Premades/Favorites")
    //     {
    //         Debug.Log("Creating buttons for Favorites directory");
    //         isFavoritesDirectory = true;
    //     }
        
    //     // Create buttons for each directory
    //     foreach (string dirPath in allDirectories)
    //     {
    //         Debug.Log($"Processing directory: {dirPath}");
    //         // Create buttons for Favorites directory as well
    //         if (dirPath == favoritesPath)
    //         {
    //             Debug.Log("Creating buttons for Favorites directory");

    //             string[] allFavorites = Directory.GetDirectories(favoritesPath);

    //             foreach (string favoritePremade in allFavorites)
    //             {
    //                 string favoritePremadeName = Path.GetFileName(favoritePremade);

    //                 string[] glbFilesOfFavorites = Directory.GetFiles(favoritePremade, "*.glb");
    //                 string glbPathOfFavorites = glbFilesOfFavorites[0];

    //                 CreateButton("All Premades/Favorites", favoritePremadeName, glbPathOfFavorites, true);
    //             }
    //             continue;
    //         }

    //         string modelName = Path.GetFileName(dirPath);

    //         // Look for GLB files in the directory to verify it has model data
    //         string[] glbFiles = Directory.GetFiles(dirPath, "*.glb");
    //         if (glbFiles.Length > 0)
    //         {
    //             // Get the first GLB file path
    //             string glbPath = glbFiles[0];
    //             CreateButton(subDirectory, modelName, glbPath, isFavoritesDirectory);
    //         }
    //     }
    //     Debug.Log($"Created buttons for {allDirectories.Length} premade sub directories");
        
    // }
}
