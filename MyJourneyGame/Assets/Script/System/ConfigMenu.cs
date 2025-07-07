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

        // ���f�i��: ���ʐݒ�j
        AudioListener.volume = config.bgmVolume;

        // �t���X�N���[���؂�ւ�
        Screen.fullScreen = config.isFullScreen;

        // �𑜓x�K�p�i�K�v�Ȃ玖�O�Ƀ��X�g�擾�j
    }
}
