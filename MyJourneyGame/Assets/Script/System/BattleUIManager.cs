using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �o�g������UI�����S������}�l�[�W���[
/// �E�v���C���[�L�����̑I��
/// �E�G���N���b�N�������̏���
/// �E�U����X�L���I���̕\���E��\��
/// �E�P�ޏ����Ȃ�
/// </summary>
public class BattleUIManager : MonoBehaviour
{
    public static BattleUIManager m_Instance; // �V���O���g���i���N���X����A�N�Z�X�p�j
    public List<Enemy> m_enemies;
    public List<PlayerCharacter> m_players;

    private int m_currentTurn = 0;

    private PlayerCharacter m_currentPlayer; // ���ݑ��쒆�̃v���C���[�L����
    private PlayerCharacter m_attacker;      // ���ۂɍU������v���C���[
    private Enemy m_selectedTarget;          // �I�����ꂽ�G

    public GameObject m_attackChoicePanel;   // �u�ʏ�U���v�u�X�L���v�{�^�����܂ރp�l��
    public List<PlayerCharacter> m_allPlayers; // �S�v���C���[�L�����ꗗ�i�I�𔻒�p�j

    /// <summary>
    /// �������F�U���R�}���h�p�l�����\���ɂ���
    /// </summary>
    private void Awake()
    {
        m_Instance = this;
        m_attackChoicePanel.SetActive(false); // �Q�[���J�n���͔�\��
    }

    /// <summary>
    /// �G���N���b�N�����Ƃ��ɌĂяo����A�U���I���p�l����\������
    /// </summary>
    public void OnEnemyClicked(Enemy target)
    {
        m_selectedTarget = target;
        m_attackChoicePanel.SetActive(true); // �ʏ�^�X�L���I���p�l���\��
    }

    /// <summary>
    /// �v���C���[�L�������N���b�N���đI�������Ƃ��̏���
    /// </summary>
    public void OnPlayerCharacterSelected(PlayerCharacter player)
    {
        foreach (var pc in m_allPlayers)
        {
            bool isSelected = (pc == player);
            pc.SetSelected(isSelected); // �I���L���������_�Ł{�A�C�R���ύX
        }

        SetCurrentAttacker(player); // �N���b�N���ꂽ�L�������U���҂ɐݒ�
    }

    /// <summary>
    /// �P�ރ{�^���������ꂽ�Ƃ��̏���
    /// </summary>
    public void OnRetreatButtonPressed()
    {
        // BattleSystem �� EndBattle ���Ăяo���ďI������
        FindObjectOfType<BattleSystem>().EndBattle();
    }

    /// <summary>
    /// �u�ʏ�U���v�{�^���������ꂽ�Ƃ��̏���
    /// </summary>
    public void OnAttackButtonPressed()
    {
        if (m_attacker == null || m_selectedTarget == null)
        {
            Debug.LogWarning("�U���҂܂��̓^�[�Q�b�g�����ݒ�ł��I");
            return;
        }

        m_attacker.Attack(m_selectedTarget);
        m_attackChoicePanel.SetActive(false);

        FindObjectOfType<BattleSystem>().EndPlayerTurn(); // �������œG�^�[����
    }

    /// <summary>
    /// �u�X�L���v�{�^���������ꂽ�Ƃ��̏���
    /// </summary>
    public void OnSkillButtonPressed()
    {
        if (m_attacker == null || m_selectedTarget == null)
        {
            Debug.LogWarning("�X�L���g�p�҂܂��̓^�[�Q�b�g�����ݒ�ł��I");
            return;
        }

        m_attacker.UseSkill(m_selectedTarget);
        m_attackChoicePanel.SetActive(false);

        FindObjectOfType<BattleSystem>().EndPlayerTurn(); // �������œG�^�[����
    }


    /// <summary>
    /// ���݂̍s���L�����i�v���C���[�j���w�肷��
    /// </summary>
    public void SetCurrentAttacker(PlayerCharacter pc)
    {
        m_attacker = pc;
    }



    private void EnemyCounterAttack()
    {
        Debug.Log("���������J�n");

        var alivePlayers = m_allPlayers.FindAll(p => p.m_currentHP > 0);

        if (alivePlayers.Count == 0 || m_selectedTarget == null)
        {
            Debug.Log("�����Ώۂ����Ȃ�");
            return;
        }

        var randomPlayer = alivePlayers[Random.Range(0, alivePlayers.Count)];
        Debug.Log($"�G�� {randomPlayer.m_characterName} �ɔ����I");
        m_selectedTarget.Attack(randomPlayer);
    }
}



