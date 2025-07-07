using System.IO;
using UnityEngine;
using UnityEditor;

public class ScenarioEncryptor
{
    [MenuItem("Tools/Encrypt Scenario")]
    public static void EncryptScenario()
    {
        string inputPath = "Assets/StreamingAssets/scenario.txt"; // 元ファイル
        string outputPath = "Assets/StreamingAssets/scenario.bytes"; // バイナリ出力先

        if (!File.Exists(inputPath))
        {
            Debug.LogError("scenario.txt が見つかりません");
            return;
        }

        string text = File.ReadAllText(inputPath);
        byte[] data = System.Text.Encoding.UTF8.GetBytes(text);

        // 簡易暗号化（XOR）
        byte key = 0x5A;
        for (int i = 0; i < data.Length; i++)
        {
            data[i] ^= key;
        }

        File.WriteAllBytes(outputPath, data);
        AssetDatabase.Refresh();
        Debug.Log("暗号化完了: " + outputPath);
    }
}
