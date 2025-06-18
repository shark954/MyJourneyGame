using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using TMPro;

/// <summary>
/// �O���e�L�X�g�t�@�C������V�i���I��ǂݎ��A�R�}���h�����s����V�X�e��
/// </summary>
public class TextAdventureSystem : MonoBehaviour
{
    public enum Mode
    {
        Normal,
        Battle
    }

    public Mode currentMode = Mode.Normal;

    // �S�V�i���I�s��ێ��i�W�����v�����Ŏg���j
    private List<string> m_AllLines = new List<string>();

    // ���݂̓ǂݍ��݈ʒu�i���g�p�ł��������O�̂��߁j
    private int m_CurrentLineIndex = 0;

    // ���x�����Ƃ��̍s�ԍ��̃}�b�s���O
    private Dictionary<string, int> m_LabelLineIndex = new Dictionary<string, int>();

    [Header("�e�L�X�g��\������UI")]
    public Text m_DisplayText;
    [Header("�摜�\���p�~3")]
    public List<Image> m_DisplayImage;
    [Header("�w�i�摜")]
    public Image m_backGroundImage;
    [Header("�I�����{�^���Q")]
    public List<Button> m_ChoiceButtons;
    [Header("�L�������\�[�X�y�摜�z")]
    public string m_CharaImageResourcePath = "Images/Character";
    [Header("�w�i���\�[�X�y�摜�z")]
    public string m_BackGroundImageResourcePath = "Images/BackGround";
    [Header("BGM�p")]
    public AudioSource m_BgmSource;
    [Header("��荞�񂾃V�i���I")]
    public Queue<string> m_Commands = new Queue<string>();
    [Header("���N���b�N�҂��t���O")]
    public bool m_WaitingForClick = false;

    void Start()
    {
        // �L�����N�^�[�摜�����������ɂ���
        foreach (Image Dummy in m_DisplayImage)
            Dummy.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);

        // StreamingAssets���̃V�i���I�t�@�C�������[�h
        LoadScenario(Application.streamingAssetsPath + "/scenario.txt");

        // �ŏ��̃R�}���h�����s
        NextCommand();
    }

    void Update()
    {
        // m_WaitingForClick��true�̎��A���N���b�N�����o
        if (m_WaitingForClick && Input.GetMouseButtonDown(0))
        {
            // ���̃R�}���h�����s
            NextCommand();
        }
    }

    /// <summary>
    /// �O���e�L�X�g�t�@�C���i�V�i���I�j��ǂݍ��ޏ���
    /// </summary>
    /// <param name="filePath">�t�@�C���p�X</param>
    /// 
   /* void LoadScenario(string filePath)
    {
        if (!File.Exists(filePath))
        {
            // �t�@�C�������݂��Ȃ��ꍇ�G���[��\��
            Debug.LogError("�V�i���I�t�@�C����������˂���!: " + filePath);
            return;
        }

        // �t�@�C����1�s���ǂݍ���
        string[] lines = File.ReadAllLines(filePath);
        foreach (string line in lines)
        {
            // ��s�������ăL���[�ɒǉ�
            if (!string.IsNullOrWhiteSpace(line))
                m_Commands.Enqueue(line.Trim());
        }
    }*/

    void LoadScenario(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Debug.LogError("�V�i���I�t�@�C����������˂���!: " + filePath);
            return;
        }

        string[] lines = File.ReadAllLines(filePath);
        m_AllLines = new List<string>(lines); // �S�s��ێ�
        m_Commands.Clear();
        m_LabelLineIndex.Clear();

        for (int i = 0; i < m_AllLines.Count; i++)
        {
            string line = m_AllLines[i].Trim();

            // ���x���s�iLABEL:�j�̓W�����v�p�ɋL�^���āA����ȊO�̓R�}���h�Ƃ��ĕۑ�
            if (line.StartsWith("LABEL:"))
            {
                string label = line.Substring(6).Trim();
                m_LabelLineIndex[label] = i + 1; // ���̍s������s�ĊJ
            }
            else if (!string.IsNullOrWhiteSpace(line))
            {
                m_Commands.Enqueue(line);
            }
        }
    }

    /// <summary>
    /// ���̃R�}���h���L���[������o���A���s
    /// </summary>
    void NextCommand()
    {
        if (m_Commands.Count == 0)
        {
            // �V�i���I���I�������ꍇ
            m_DisplayText.text = "�V�i���I�I��";
            m_WaitingForClick = false; // �ҋ@����
            return;
        }

        // �L���[���玟�̃R�}���h�����o��
        string command = m_Commands.Dequeue();

        // �R�}���h�����s
        ExecuteCommand(command);
    }

    /// <summary>
    /// �R�}���h�̓��e�ɉ��������������s
    /// </summary>
    /// <param name="command">���s����R�}���h������</param>
    void ExecuteCommand(string command)
    {
        #region "TEXT:" �Ŏn�܂�ꍇ�F�e�L�X�g�\��
        // "TEXT:" �Ŏn�܂�ꍇ�F�e�L�X�g�\��
        if (command.StartsWith("TEXT:"))
        {
            //5�����폜��AMessage��\������
            TextRender(DataPatch(command, 5));
            // ���N���b�N�҂��ɂ���
            m_WaitingForClick = true;
        }
        #endregion
        #region "SHOW_IMAGE_S:" �Ŏn�܂�ꍇ�F�摜�\��
        // "SHOW_IMAGE_S:" �Ŏn�܂�ꍇ�F�摜�\��
        else if (command.StartsWith("SHOW_IMAGE_S:"))
        {
            //13�����폜��A�L�����C���[�W�y�����z��\������
            ImageCharacterRender(DataPatch(command, 13), 2);
            // �摜�\����͎����i�s
            NextCommand();
        }
        #endregion
        #region "SHOW_IMAGE_R:" �Ŏn�܂�ꍇ�F�摜�\��
        // "SHOW_IMAGE_R:" �Ŏn�܂�ꍇ�F�摜�\��
        else if (command.StartsWith("SHOW_IMAGE_R:"))
        {

            //13�����폜��A�L�����C���[�W�y�E�z��\������
            ImageCharacterRender(DataPatch(command, 13), 0);
            // �摜�\����͎����i�s
            NextCommand();
        }
        #endregion
        #region "SHOW_IMAGE_L:" �Ŏn�܂�ꍇ�F�摜�\��
        // "SHOW_IMAGE_L:" �Ŏn�܂�ꍇ�F�摜�\��
        else if (command.StartsWith("SHOW_IMAGE_L:"))
        {
            //13�����폜��A�L�����C���[�W�y���z��\������
            ImageCharacterRender(DataPatch(command, 13), 1);
            // �摜�\����͎����i�s
            NextCommand();
        }
        #endregion
        #region "SHOW_BGI:":�w�i�\��
        else if (command.StartsWith("SHOW_BGI:"))
        {
            // 9�����폜��A�w�i��\������
            ImageBackGroundRender(DataPatch(command, 9));
            // �摜�\����͎����i�s
            NextCommand();
        }
        #endregion
            #region "PLAY_BGM:" �Ŏn�܂�ꍇ�FBGM�Đ�
            // "PLAY_BGM:" �Ŏn�܂�ꍇ�FBGM�Đ�
        else if (command.StartsWith("PLAY_BGM:"))
        {
            // "PLAY_BGM:"�ȍ~�̕�����
            string bgmName = command.Substring(9);
            //9�����폜��A�w�肵���o�b�N�~���[�W�b�N�𗬂�
            BackGroundMusic(DataPatch(command, 9));
            // BGM�Đ���͎����i�s
            NextCommand();
        }
        #endregion
        #region "SHOW_IMAGE_S_CLS:" �Ŏn�܂�ꍇ�F�摜��\��
        // "SHOW_IMAGE_S_CLS:" �Ŏn�܂�ꍇ�F�摜�\��
        else if (command.StartsWith("SHOW_IMAGE_S_CLS:"))
        {
            // �����C���[�W�L�����N�^�[�p�l��������
            ImageCharacterCLS(2);
            // �摜�\����͎����i�s
            NextCommand();
        }
        #endregion
        #region "SHOW_IMAGE_R_CLS:" �Ŏn�܂�ꍇ�F�摜��\��
        // "SHOW_IMAGE_R_CLS:" �Ŏn�܂�ꍇ�F�摜�\��
        else if (command.StartsWith("SHOW_IMAGE_R_CLS:"))
        {
            // �����C���[�W�L�����N�^�[�p�l��������
            ImageCharacterCLS(0);
            // �摜�\����͎����i�s
            NextCommand();
        }
        #endregion
        #region "SHOW_IMAGE_L_CLS:" �Ŏn�܂�ꍇ�F�摜��\��
        // "SHOW_IMAGE_L_CLS:" �Ŏn�܂�ꍇ�F�摜�\��
        else if (command.StartsWith("SHOW_IMAGE_L_CLS:"))
        {
            // �����C���[�W�L�����N�^�[�p�l��������
            ImageCharacterCLS(1);
            // �摜�\����͎����i�s
            NextCommand();
        }
        #endregion
        #region "START_BATTLE"�Ŏn�܂�ꍇ�F�o�g���X�^�[�g 
        else if (command.StartsWith("START_BATTLE"))
        {
            // �퓬�V�X�e�����Ăяo��
            currentMode = Mode.Battle;
            FindObjectOfType<BattleSystem>().StartBattle();
            // ���̃N���X�ł͐i�s�X�g�b�v
            m_WaitingForClick = false;
        }
        #endregion
        #region "�{�^���I��"
        else if (command.StartsWith("CHOICE:"))
        {
            // �I�����F�e�L�X�g�ƃW�����v�惉�x���𕪗����Ċi�[
            string[] rawChoices = DataPatch(command, 7).Split('|');
            List<string> labels = new List<string>();
            List<string> texts = new List<string>();

            foreach (string c in rawChoices)
            {
                if (c.Contains("->"))
                {
                    string[] parts = c.Split(new string[] { "->" }, System.StringSplitOptions.None);
                    texts.Add(parts[0].Trim());   // �\�������e�L�X�g
                    labels.Add(parts[1].Trim());  // �W�����v�惉�x��
                }
                else
                {
                    texts.Add(c.Trim());
                    labels.Add(""); // ���x���Ȃ�
                }
            }

            ShowBranchingChoices(texts.ToArray(), labels.ToArray());
        }
        #endregion
        #region ���̑�
        else
        {
            // ���m�̃R�}���h
            Debug.LogWarning("���m�̃R�}���h�ɂ�?: " + command);
            // �s���ȃR�}���h�̓X�L�b�v
            NextCommand();
        }
        #endregion
    }
    /// <summary>
    /// �폜���������폜������̕�����Ԃ�
    /// </summary>
    /// <param name="Message">����ꂽ�����f�[�^</param>
    /// <param name="Count">�폜���镶����</param>
    /// <returns></returns>
    public string DataPatch(string Message, int Count)
    {
        //�w�肵�������񐔂��폜���ĕԂ�
        return Message.Substring(Count);
    }
    /// <summary>
    /// Message�����b�Z�[�W�E�B���h�D�ɕ\������
    /// </summary>
    /// <param name="Message"></param>
    public void TextRender(string Message)
    {
        // �e�L�X�g��\��
        m_DisplayText.text = Message;
        // ���N���b�N�҂��ɂ���
        m_WaitingForClick = true;

    }
    /// <summary>
    /// �L�����N�^�[�C���[�W��resource������o���Ďw�肵���ꏊ�ɕ\������
    /// </summary>
    /// <param name="Message">resource�t�H���_������o�������摜��</param>
    /// <param name="No">�\���������p�l���ԍ�[2.�����A1.�E�A0.��]</param>
    public void ImageCharacterRender(string Message, int No)
    {
        // Resources����摜�ǂݍ���
        Sprite sprite = Resources.Load<Sprite>(m_CharaImageResourcePath + "/" + Message);
        // �\���悪���邩�ǂ���
        if (sprite != null)
        {
            //�摜(Sprite)����
            m_DisplayImage[No].sprite = sprite;
            //�摜�̃��l��Max�ɂ���
            m_DisplayImage[No].color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        }
        else
            Debug.LogWarning("�摜���������!: " + Message);
    }

    public void ImageBackGroundRender(string Message)
    {
        Sprite sprite = Resources.Load<Sprite>(m_BackGroundImageResourcePath + "/" + Message);
        if (sprite != null)
        {
            m_backGroundImage.sprite = sprite;
            m_backGroundImage.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        }
        else
        {
            Debug.LogWarning("�摜���Ȃ�:" + Message);
        }
    }

    public void BackGroundMusic(string Message)
    {
        // Resources����BGM�ǂݍ���
        AudioClip clip = Resources.Load<AudioClip>("Audio/" + Message);
        if (clip != null)
        {
            //�ǂݍ���BGM����
            m_BgmSource.clip = clip;
            // BGM�Đ�
            m_BgmSource.Play();
        }
        else
        {
            Debug.LogWarning("BGM���������!: " + Message);
        }
    }

    /// <summary>
    /// �C���[�W�L�����N�^�[�p�l���̏�����
    /// </summary>
    /// <param name="No">���������C���[�W�L�����N�^�[�p�l���ԍ�</param>
    public void ImageCharacterCLS(int No)
    {
        //�Y���摜���폜
        m_DisplayImage[No].sprite = null;
        //���l��0�ɂ���(������)
        m_DisplayImage[No].color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
    }

    // ���ꂪ�uExecuteCommand�̊O�v�ɒu�����b�p�[�֐�
    public void ContinueFromBattle()
    {
        Debug.Log("�o�g���I���A�X�g�[���[�ĊJ");
        currentMode = Mode.Normal;
        NextCommand();
    }
    public void HideChoices()
    {
        foreach (Button btn in m_ChoiceButtons)
        {
            btn.gameObject.SetActive(false);
        }
    }

    public void ShowChoices(string[] choices)
    {
        m_WaitingForClick = false;

        for (int i = 0; i < m_ChoiceButtons.Count; i++)
        {
            if (i < choices.Length)
            {
                int index = i;
                m_ChoiceButtons[i].gameObject.SetActive(true);
                m_ChoiceButtons[i].GetComponentInChildren<Text>().text = choices[i];
                m_ChoiceButtons[i].onClick.RemoveAllListeners();
                m_ChoiceButtons[i].onClick.AddListener(() =>
                {
                    Debug.Log("�I�΂ꂽ: " + choices[index]);
                    HideChoices();

                    if (currentMode == Mode.Battle)
                    {
                        FindObjectOfType<BattleSystem>().OnChoiceSelected(choices[index]);
                    }
                    else
                    {
                        // �ʏ탂�[�h�̏���
                        TextRender($"���Ȃ��́u{choices[index]}�v��I�т܂����B");
                        m_WaitingForClick = true;
                    }
                });
            }
            else
            {
                m_ChoiceButtons[i].gameObject.SetActive(false);
            }
        }
    }

    public void JumpToLabel(string label)
    {
        if (m_LabelLineIndex.ContainsKey(label))
        {
            m_Commands.Clear();
            m_CurrentLineIndex = m_LabelLineIndex[label];

            // ���x���ȍ~�̃R�}���h�������Ă� m_Commands �ɓ����
            for (int i = m_CurrentLineIndex; i < m_AllLines.Count; i++)
            {
                string line = m_AllLines[i].Trim();
                if (!string.IsNullOrWhiteSpace(line) && !line.StartsWith("LABEL:"))
                    m_Commands.Enqueue(line);
            }

            // ���̃R�}���h�֐i�s
            NextCommand();
        }
        else
        {
            Debug.LogWarning("���x�����������!: " + label);
        }
    }

    public void ShowBranchingChoices(string[] texts, string[] labels)
    {
        m_WaitingForClick = false;

        if (texts == null || labels == null)
        {
            Debug.LogError("texts �܂��� labels �� null �ł�");
            return;
        }

        // �ʏ�̕���\��������

        for (int i = 0; i < m_ChoiceButtons.Count; i++)
        {
            if (i < texts.Length)
            {
                int index = i;
                m_ChoiceButtons[i].gameObject.SetActive(true);
                var tmpText = m_ChoiceButtons[i].GetComponentInChildren<TextMeshProUGUI>();
                if (tmpText != null)
                {
                    tmpText.text = texts[i];
                }
                else
                {
                    Debug.LogError($"Button[{i}] �� TextMeshProUGUI ��������܂���I");
                }

                m_ChoiceButtons[i].onClick.RemoveAllListeners();
                m_ChoiceButtons[i].onClick.AddListener(() =>
                {
                    HideChoices(); // ���̑I�������\��
                    UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(m_ChoiceButtons[index].gameObject);

                    if (!string.IsNullOrEmpty(labels[index]))
                    {
                        // ���x�����ݒ肳��Ă���΃W�����v
                        JumpToLabel(labels[index]);
                    }
                    else
                    {
                        // ���x�����Ȃ��ꍇ�͂��̂܂ܒʏ�i�s
                        TextRender($"���Ȃ��́u{texts[index]}�v��I�т܂����B");
                        m_WaitingForClick = true;
                    }
                });
            }
            else
            {
                m_ChoiceButtons[i].gameObject.SetActive(false);
            }
        }
    }

}

