using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class EnvLoader
{
    private static Dictionary<string, string> _envVars;

    public static string Get(string key)
    {
        if (_envVars == null)
            LoadEnv();

        return _envVars.TryGetValue(key, out string value) ? value : null;
    }

    private static void LoadEnv()
    {
        _envVars = new Dictionary<string, string>();
        // string path = Path.Combine(Application.streamingAssetsPath, ".env");
         string path = Path.Combine(Application.dataPath, ".env");

        foreach (string line in File.ReadAllLines(path))
        {
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#")) continue;
            string[] split = line.Split('=', 2);
            if (split.Length == 2)
                _envVars[split[0].Trim()] = split[1].Trim();
        }
    }
}
