using UnityEngine.UI;
using UnityEngine;

public class ConfigMenu : MonoBehaviour
{
    public Slider bgmSlider;
    public Slider seSlider;
    public Toggle fullScreenToggle;
    public Dropdown resolutionDropdown;

    private ConfigData config;

    void Start()
    {
        config = ConfigManager.LoadConfig();

        bgmSlider.value = config.bgmVolume;
        seSlider.value = config.seVolume;
        fullScreenToggle.isOn = config.isFullScreen;
        resolutionDropdown.value = config.resolutionIndex;
    }

    public void ApplyAndSave()
    {
        config.bgmVolume = bgmSlider.value;
        config.seVolume = seSlider.value;
        config.isFullScreen = fullScreenToggle.isOn;
        config.resolutionIndex = resolutionDropdown.value;

        ConfigManager.SaveConfig(config);

        // 反映（例: 音量設定）
        AudioListener.volume = config.bgmVolume;

        // フルスクリーン切り替え
        Screen.fullScreen = config.isFullScreen;

        // 解像度適用（必要なら事前にリスト取得）
    }
}
