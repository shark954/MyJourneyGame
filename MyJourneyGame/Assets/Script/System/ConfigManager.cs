using System.IO;
using UnityEngine;

public static class ConfigManager
{
    private static string path = Application.persistentDataPath + "/config.json";

    public static void SaveConfig(ConfigData config)
    {
        string json = JsonUtility.ToJson(config, true);
        File.WriteAllText(path, json);
        Debug.Log("�R���t�B�O�ۑ�����");
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
            Debug.Log("�V�K�R���t�B�O����");
            return new ConfigData(); // �f�t�H���g�ݒ�
        }
    }
}
