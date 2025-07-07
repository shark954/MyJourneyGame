using System.IO;
using UnityEngine;

public static class ConfigManager
{
    private static string path = Application.persistentDataPath + "/config.json";

    public static void SaveConfig(ConfigData config)
    {
        string json = JsonUtility.ToJson(config, true);
        File.WriteAllText(path, json);
        Debug.Log("コンフィグ保存完了");
    }

    public static ConfigData LoadConfig()
    {
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<ConfigData>(json);
        }
        else
        {
            Debug.Log("新規コンフィグ生成");
            return new ConfigData(); // デフォルト設定
        }
    }
}
