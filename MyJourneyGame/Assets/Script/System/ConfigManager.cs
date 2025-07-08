using UnityEngine;
using UnityEngine.UI;
using System.IO;

/// <summary>
/// �Q�[���ݒ�iBGM���ʁASE���ʁA�t���X�N���[���Ȃǁj���Ǘ�����N���X�B
/// Esc�L�[�ŕ\���ؑցA�ݒ��Json�t�@�C���ɕۑ��E�ǂݍ��݁B
/// </summary>
public class ConfigManager : MonoBehaviour
{
    [Header("UI�Q��")]
    public GameObject m_configPanel;         // �ݒ��ʂ̃p�l��
    public Slider m_bgmSlider;               // BGM���ʃX���C�_�[
    public Slider m_seSlider;                // SE���ʃX���C�_�[
    public Toggle m_fullscreenToggle;        // �t���X�N���[���ؑփg�O��

    private bool m_isOpen = false;           // �ݒ��ʂ̕\�����
    private ConfigData m_currentConfig;      // ���݂̐ݒ�l

    private static string m_path => Application.persistentDataPath + "/config.json"; // �ۑ��p�X

    /// <summary>
    /// �N�����ɐݒ��ǂݍ��݁AUI����уV�X�e���ɔ��f�B
    /// </summary>
    void Start()
    {
        m_currentConfig = LoadConfig();
        ApplyConfigToUI();
        ApplyConfigToSystem();
    }

    /// <summary>
    /// Esc�L�[�Őݒ��ʂ��J�B
    /// </summary>
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleConfig();
        }
    }

    /// <summary>
    /// �ݒ��ʂ̕\���E��\����؂�ւ��ATimeScale�𑀍�B
    /// </summary>
    void ToggleConfig()
    {
        m_isOpen = !m_isOpen;
        m_configPanel.SetActive(m_isOpen);
        Time.timeScale = m_isOpen ? 0f : 1f; // �ꎞ��~
    }

    /// <summary>
    /// [�K�p]�{�^���������ɌĂ΂�AUI�̐ݒ�l��ۑ����K�p�B
    /// </summary>
    public void OnApplyButtonPressed()
    {
        // UI �� �f�[�^�֔��f
        m_currentConfig.bgmVolume = m_bgmSlider.value;
        m_currentConfig.seVolume = m_seSlider.value;
        m_currentConfig.isFullScreen = m_fullscreenToggle.isOn;

        SaveConfig(m_currentConfig);
        ApplyConfigToSystem();
    }

    /// <summary>
    /// �ۑ�����Ă���ConfigData��UI�X���C�_�[�Ȃǂɔ��f�B
    /// </summary>
    private void ApplyConfigToUI()
    {
        m_bgmSlider.value = m_currentConfig.bgmVolume;
        m_seSlider.value = m_currentConfig.seVolume;
        m_fullscreenToggle.isOn = m_currentConfig.isFullScreen;
    }

    /// <summary>
    /// �ݒ�f�[�^���V�X�e���i���ʁE��ʁj�ɔ��f�B
    /// </summary>
    private void ApplyConfigToSystem()
    {
        AudioListener.volume = m_currentConfig.bgmVolume; // BGM���ʁi�S�̗p�j
        Screen.fullScreen = m_currentConfig.isFullScreen; // �t���X�N���[���ؑ�
    }

    /// <summary>
    /// �ݒ�t�@�C����ۑ��iJSON�`���j�B
    /// </summary>
    public static void SaveConfig(ConfigData config)
    {
        string json = JsonUtility.ToJson(config, true);
        File.WriteAllText(m_path, json);
        Debug.Log("�R���t�B�O�ۑ�����: " + m_path);
    }

    /// <summary>
    /// �ݒ�t�@�C����ǂݍ��݁i������΃f�t�H���g�����j�B
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
            Debug.Log("�V�K�R���t�B�O����");
            return new ConfigData(); // �f�t�H���g�l
        }
    }
}
