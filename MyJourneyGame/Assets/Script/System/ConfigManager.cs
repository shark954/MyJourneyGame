using UnityEngine;
using UnityEngine.UI;
using System.IO;

/// <summary>
/// ゲーム設定（BGM音量、SE音量、フルスクリーンなど）を管理するクラス。
/// Escキーで表示切替、設定をJsonファイルに保存・読み込み。
/// </summary>
public class ConfigManager : MonoBehaviour
{
    [Header("UI参照")]
    public GameObject m_configPanel;         // 設定画面のパネル
    public Slider m_bgmSlider;               // BGM音量スライダー
    public Slider m_seSlider;                // SE音量スライダー
    public Toggle m_fullscreenToggle;        // フルスクリーン切替トグル

    private bool m_isOpen = false;           // 設定画面の表示状態
    private ConfigData m_currentConfig;      // 現在の設定値

    private static string m_path => Application.persistentDataPath + "/config.json"; // 保存パス

    /// <summary>
    /// 起動時に設定を読み込み、UIおよびシステムに反映。
    /// </summary>
    void Start()
    {
        m_currentConfig = LoadConfig();
        ApplyConfigToUI();
        ApplyConfigToSystem();
    }

    /// <summary>
    /// Escキーで設定画面を開閉。
    /// </summary>
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleConfig();
        }
    }

    /// <summary>
    /// 設定画面の表示・非表示を切り替え、TimeScaleを操作。
    /// </summary>
    void ToggleConfig()
    {
        m_isOpen = !m_isOpen;
        m_configPanel.SetActive(m_isOpen);
        Time.timeScale = m_isOpen ? 0f : 1f; // 一時停止
    }

    /// <summary>
    /// [適用]ボタン押下時に呼ばれ、UIの設定値を保存＆適用。
    /// </summary>
    public void OnApplyButtonPressed()
    {
        // UI → データへ反映
        m_currentConfig.bgmVolume = m_bgmSlider.value;
        m_currentConfig.seVolume = m_seSlider.value;
        m_currentConfig.isFullScreen = m_fullscreenToggle.isOn;

        SaveConfig(m_currentConfig);
        ApplyConfigToSystem();
    }

    /// <summary>
    /// 保存されているConfigDataをUIスライダーなどに反映。
    /// </summary>
    private void ApplyConfigToUI()
    {
        m_bgmSlider.value = m_currentConfig.bgmVolume;
        m_seSlider.value = m_currentConfig.seVolume;
        m_fullscreenToggle.isOn = m_currentConfig.isFullScreen;
    }

    /// <summary>
    /// 設定データをシステム（音量・画面）に反映。
    /// </summary>
    private void ApplyConfigToSystem()
    {
        AudioListener.volume = m_currentConfig.bgmVolume; // BGM音量（全体用）
        Screen.fullScreen = m_currentConfig.isFullScreen; // フルスクリーン切替
    }

    /// <summary>
    /// 設定ファイルを保存（JSON形式）。
    /// </summary>
    public static void SaveConfig(ConfigData config)
    {
        string json = JsonUtility.ToJson(config, true);
        File.WriteAllText(m_path, json);
        Debug.Log("コンフィグ保存完了: " + m_path);
    }

    /// <summary>
    /// 設定ファイルを読み込み（無ければデフォルト生成）。
    /// </summary>
    public static ConfigData LoadConfig()
    {
        if (File.Exists(m_path))
        {
            string json = File.ReadAllText(m_path);
            return JsonUtility.FromJson<ConfigData>(json);
        }
        else
        {
            Debug.Log("新規コンフィグ生成");
            return new ConfigData(); // デフォルト値
        }
    }
}
