using UnityEngine;
using TMPro; // TextMeshPro を使う場合はこちら

/// <summary>
/// デバッグ用ボタン
/// </summary>
public class ModeChangeButton : MonoBehaviour
{
    public TextAdventureSystem textSystem;
    public TextMeshProUGUI labelText; // ボタンの表示用テキスト

    public void ToggleMode()
    {
        if (textSystem != null)
        {
            textSystem.modeChange = !textSystem.modeChange;

            if (labelText != null)
            {
                labelText.text = textSystem.modeChange ? "バトルモード" : "ノーマルモード";
            }
        }
    }

    private void Start()
    {
        // 初期表示
        if (textSystem != null && labelText != null)
        {
            labelText.text = textSystem.modeChange ? "バトルモード" : "ノーマルモード";
        }
    }
}
