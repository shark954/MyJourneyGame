using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �V�i���I���o�g���̊ȈՏ������s���Ǘ��N���X�B
/// �L�����I�� �� �R�}���h�I�� �� �A�N�V���� �� �I���܂ł𐧌�B
/// </summary>
public class BattleSystem : MonoBehaviour
{
    public List<Enemy> m_enemies;
    public List<PlayerCharacter> m_players;

    public TextMeshProUGUI m_TurnText;

    private int m_currentTurn = 0;
    // UI�v�f
    public GameObject m_battleUI;         // �퓬�S��UI�iON/OFF�Ńo�g���̕\������j
    public GameObject m_characterUI;      // �L�����N�^�[�I��UI�i�����L�����{�^���Ȃǁj
    public GameObject m_commandUI;        // �R�}���h�I��UI�i�U���A�h��Ȃǁj
    public GameObject m_actionUI;         // �s���I��UI�i�ʏ�U���A�X�L���Ȃǁj
    public List<GameObject> m_selectionFrames; // �e�L�����̔��w�i�i�I��g�A�_�ŗp�j

    // �I�𒆏��
    private string m_selectedCharacterName = ""; // ���ݑI������Ă���L������
    private string m_selectedCommand = "";       // ���ݑI�𒆂̃R�}���h���e

    /// <summary>
    /// �o�g���J�n���Ɏ��s�BUI��������g���Z�b�g�ȂǁB
    /// </summary>
    public void StartBattle()
    {
        InitSelectionFrames(); // �g�S�ē������_��OFF
        Debug.Log("�o�g���J�n�I");

        // UI�\������
        m_battleUI.SetActive(true);
        m_characterUI.SetActive(true);
        m_commandUI.SetActive(false);
        m_actionUI.SetActive(false);
    }

    /// <summary>
    /// �L�������I�����ꂽ�Ƃ��Ɏ��s�i�{�^�����當����œn�����j
    /// </summary>
    public void OnCharacterSelected(string charaName)
    {
        m_selectedCharacterName = charaName;
        int selectedIndex = ExtractIndexFromName(charaName); // "Character2" �� 2
        Debug.Log(selectedIndex);
        foreach (var frame in m_selectionFrames)
        {
            var blink = frame.GetComponent<BlinkEffect>();
            if (blink != null)
                blink.enabled = true;
        }
        SetSelectionFrame(selectedIndex); // �I�΂ꂽ�L���������g��_�ł�����

        m_commandUI.SetActive(true); // ���ɃR�}���h�I��UI��\��
    }

    /// <summary>
    /// �L����������C���f�b�N�X���擾�i�����̐����j
    /// </summary>
    private int ExtractIndexFromName(string name)
    {
        if (name.StartsWith("Character"))
        {
            string num = name.Substring("Character".Length);
            if (int.TryParse(num, out int result))
                return result;
        }
        return 0; // �f�t�H���g�͐擪�L����
    }

    /// <summary>
    /// �R�}���h�i�U���Ȃǁj���I�����ꂽ�Ƃ��Ɏ��s
    /// </summary>
    public void OnCommandSelected(string command)
    {
        Debug.Log("�I���R�}���h: " + command);
        m_selectedCommand = command;

        if (command == "�U��")
        {
            m_commandUI.SetActive(false);
            m_actionUI.SetActive(true);
        }
    }

    /// <summary>
    /// ���ۂ̍s���i�ʏ�U���E�X�L���Ȃǁj���I�΂ꂽ�Ƃ��̏���
    /// </summary>
    public void OnActionConfirmed(string action)
    {
        Debug.Log($"{m_selectedCharacterName} �� {action} ���s�I");

        // ���ۂ̍s�������Ȃǂ�}���i�GHP�����炷�Ȃǁj

        StopAllSelectionFrameBlink(); // �I���L�����̘g�_�ł������i�^�[���I���j
    }

    /// <summary>
    /// ���ׂĂ̑I��g�𓧖��{�_��OFF�ɂ���
    /// </summary>
    private void StopAllSelectionFrameBlink()
    {
        foreach (var frame in m_selectionFrames)
        {
            var image = frame.GetComponent<Image>();
            var blink = frame.GetComponent<BlinkEffect>();

            if (image != null) image.color = new Color(1, 1, 1, 0);
            if (blink != null) blink.SetRender(false);
        }
    }

    /// <summary>
    /// �w�肳�ꂽ�L�����̘g�̂ݕ\�����_�ŁA����ȊO�͔�\������_��
    /// </summary>
    private void SetSelectionFrame(int selectedIndex)
    {
        StopAllSelectionFrameBlink();
        var image = m_selectionFrames[selectedIndex].GetComponent<Image>();
        var blink = m_selectionFrames[selectedIndex].GetComponent<BlinkEffect>();
        if (image != null) image.color = new Color(1, 1, 1, 1); // �\��
        if (blink != null) blink.SetRender(true);              // �_��ON

    }

    /// <summary>
    /// �ŏ��ɂ��ׂẴL�����g�𓧖����_��OFF�ɏ�����
    /// </summary>
    private void InitSelectionFrames()
    {
        foreach (var frame in m_selectionFrames)
        {
            var img = frame.GetComponent<Image>();
            if (img != null) img.color = new Color(1, 1, 1, 0);
            var blink = frame.GetComponent<BlinkEffect>();
            if (blink != null) blink.enabled = false;
        }
    }


    public void EndPlayerTurn()
    {
        StartCoroutine(EnemyTurnRoutine());
    }

    private IEnumerator EnemyTurnRoutine()
    {
        m_TurnText.text = "�G�̃^�[��";

        foreach (var enemy in m_enemies)
        {
            if (enemy.gameObject.activeSelf)
            {
                var target = SelectRandomPlayer();
                if (target != null)
                {
                    enemy.Attack(target);
                    yield return new WaitForSeconds(1.0f); // �����Ԃ�u��
                }
            }
        }


        Debug.Log("�v���C���[�̃^�[���ɖ߂�");
        // �v���C���[�̃^�[���J�n�i�K�v�ȏ����������Ɂj
        m_TurnText.text = "�v���C���[�̃^�[��";
    }

    private PlayerCharacter SelectRandomPlayer()
    {
        var alivePlayers = m_players.FindAll(p => p.m_currentHP > 0);
        if (alivePlayers.Count == 0) return null;
        return alivePlayers[Random.Range(0, alivePlayers.Count)];
    }



    /// <summary>
    /// �o�g���I������UI���\�����_�ŉ����B�X�g�[���[�ɖ߂�B
    /// </summary>
    public void EndBattle()
    {
        m_battleUI.SetActive(false);

        foreach (var frame in m_selectionFrames)
        {
            var blink = frame.GetComponent<BlinkEffect>();
            if (blink != null)
                blink.enabled = false;
        }

        FindObjectOfType<TextAdventureSystem>().ContinueFromBattle(); // �X�g�[���[�i�s�ĊJ
    }
}
