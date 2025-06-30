using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using TMPro;
using System.Collections;

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

    [Header("�w�i�摜�i�V�j")]
    public Image m_backGroundImageNew; // �ʏ�̔w�i�\���p

    [Header("�w�i�摜�i���j")]
    public Image m_backGroundImageOld; // �t�F�[�h�A�E�g�p�d�˔w�i

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
    [Header("�Q�[���}�l�[�W���[")]
    public GameManager m_gameManager;

    private bool m_IsFading = false;
    private float m_FadeDuration = 1.0f;
    private float m_FadeElapsed = 0f;
    private int m_SelectedChoiceIndex = -1;
    private Sprite m_PendingBackground = null;
    [Header("�f�o�b�O�ϐ�")]
    public bool modeChange = false;

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
        if (m_IsFading)
        {
            m_FadeElapsed += Time.deltaTime;
            float t = Mathf.Clamp01(m_FadeElapsed / m_FadeDuration);

            m_backGroundImageOld.color = new Color(1, 1, 1, 1 - t);
            m_backGroundImageNew.color = new Color(1, 1, 1, t);

            if (t >= 1.0f)
            {
                m_IsFading = false;
                m_backGroundImageOld.color = new Color(1, 1, 1, 0);
                m_backGroundImageNew.color = new Color(1, 1, 1, 1);
                NextCommand();
            }

            return;
        }

        // �ʏ�̃N���b�N����
        if (m_WaitingForClick && Input.GetMouseButtonDown(0))
        {
            NextCommand();
        }

        //�f�o�b�O�p �O���藝
       // ChangeMode(modeChange ? true : false);
    }

    //�f�o�b�O�֐� modechange��true�Ȃ�o�g�����[�h�Afalse�Ȃ�m�[�}�����[�h
    public void ChangeMode(bool flag)
    {
        if (flag)
        {
            currentMode = Mode.Battle;
            m_WaitingForClick = false;
            m_gameManager.m_storyPanel.SetActive(false);
            m_gameManager.m_battlePanel.SetActive(true);
        }

        if (!flag)
        {
            currentMode = Mode.Normal;
            m_WaitingForClick = true;
            m_gameManager.m_battlePanel.SetActive(false);
            m_gameManager.m_storyPanel.SetActive(true);
        }
    }

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
        //HideChoices(); // �� �����Ŗ����\���ɂ���I

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
        #region "START_BATTLE:"�Ŏn�܂�ꍇ�F�o�g���X�^�[�g 
        else if (command.StartsWith("START_BATTLE:"))
        {
            // �퓬�V�X�e�����Ăяo��
            currentMode = Mode.Battle;
            FindObjectOfType<BattleSystem>().StartBattle();
            // ���̃N���X�ł͐i�s�X�g�b�v
            m_WaitingForClick = false;
            m_gameManager.m_storyPanel.SetActive(false);
        }
        #endregion
        #region "CHOICE:"�Ŏn�܂�ꍇ:�{�^���I��
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
        #region "SHOW_BGI:":�w�i�\��
        else if (command.StartsWith("SHOW_BGI:"))
        {
            string imageName = DataPatch(command, 9);
            StartFadeBackground(imageName, 1.0f); // �t�F�[�h1�b
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

    /// <summary>
    /// �w�i�摜��؂�ւ���i�t�F�[�h���o�t���j
    /// </summary>
    /// <param name="Message">�ǂݍ��ޔw�i�摜��</param>
    public void ImageBackGroundRender(string Message)
    {
        Sprite newSprite = Resources.Load<Sprite>(m_BackGroundImageResourcePath + "/" + Message);

        if (newSprite != null)
        {
            if (m_backGroundImageNew.sprite != null)
            {
                m_backGroundImageOld.sprite = m_backGroundImageNew.sprite;
                m_backGroundImageOld.color = new Color(1, 1, 1, 1);
            }
            else
            {
                m_backGroundImageOld.color = new Color(1, 1, 1, 0);
            }

            m_backGroundImageNew.sprite = newSprite;

            // �V�����摜�͓����ɂ��āA�t�F�[�h������ɕ\������
            m_backGroundImageNew.color = new Color(1, 1, 1, 0);

        }
        else
        {
            Debug.LogWarning("�w�i�摜���������!: " + Message);
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
        m_WaitingForClick = true;
        m_gameManager.m_storyPanel.SetActive(true);
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

                    Debug.Log("�I�΂ꂽ: " + texts[index]);

                    // �S�{�^����\��
                    HideChoices();

                    // �I�����ꂽ�{�^���ɐF���c���ݒ�i�K�v�Ȃ�j
                    var colors = m_ChoiceButtons[index].colors;
                    colors.normalColor = colors.pressedColor;
                    m_ChoiceButtons[index].colors = colors;

                    // ���x���փW�����v�܂��͂��̂܂ܕ\��
                    if (!string.IsNullOrEmpty(labels[index]))
                    {
                        JumpToLabel(labels[index]);
                    }
                    else
                    {
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


    /// <summary>
    /// �w�i�̌Â��摜���t�F�[�h�A�E�g������
    /// </summary>
    /// <param name="duration">�t�F�[�h�ɂ����鎞�ԁi�b�j</param>
    private void StartFadeBackground(string imageName, float duration)
    {
        m_PendingBackground = Resources.Load<Sprite>(m_BackGroundImageResourcePath + "/" + imageName);

        if (m_PendingBackground == null)
        {
            Debug.LogWarning("�w�i�摜���������!: " + imageName);
            NextCommand();
            return;
        }

        // �O�̔w�i���Ȃ��i������j�Ȃ瑦�؂�ւ�
        if (m_backGroundImageNew.sprite == null)
        {
            m_backGroundImageNew.sprite = m_PendingBackground;
            m_backGroundImageNew.color = new Color(1, 1, 1, 1); // �����ɂ������\��
            m_backGroundImageOld.color = new Color(1, 1, 1, 0); // �O�̂��ߔ�\��
            NextCommand(); // ���i�s
            return;
        }

        // �t�F�[�h���o����
        m_backGroundImageOld.sprite = m_backGroundImageNew.sprite;
        m_backGroundImageOld.color = new Color(1, 1, 1, 1);

        m_backGroundImageNew.sprite = m_PendingBackground;
        m_backGroundImageNew.color = new Color(1, 1, 1, 0);

        m_IsFading = true;
        m_FadeDuration = duration;
        m_FadeElapsed = 0f;
        m_WaitingForClick = false;
    }

}

