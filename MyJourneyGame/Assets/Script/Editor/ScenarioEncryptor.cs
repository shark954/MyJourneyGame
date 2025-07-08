using System.IO;
using UnityEngine;
using UnityEditor;

public class ScenarioEncryptor
{
    [MenuItem("Tools/Encrypt Scenario")]
    public static void EncryptScenario()
    {
        string inputPath = "Assets/StreamingAssets/scenario.txt"; // ���t�@�C��
        string outputPath = "Assets/StreamingAssets/scenario.bytes"; // �o�C�i���o�͐�

        if (!File.Exists(inputPath))
        {
            Debug.LogError("scenario.txt ��������܂���");
            return;
        }

        string text = File.ReadAllText(inputPath);
        byte[] data = System.Text.Encoding.UTF8.GetBytes(text);

        // �ȈՈÍ����iXOR�j
        byte key = 0x5A;
        for (int i = 0; i < data.Length; i++)
        {
            data[i] ^= key;
        }

        File.WriteAllBytes(outputPath, data);
        AssetDatabase.Refresh();
        Debug.Log("�Í�������: " + outputPath);
    }
}
