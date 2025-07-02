using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �o�g��UI�𑀍�E�Ǘ�����N���X�i��Ƀv���C���[�ƓG�̃N���b�N�����j
/// - �v���C���[��G�̑I��
/// - �U���R�}���hUI�̕\��/��\��
/// - �U��/�X�L���̔���
/// - ��������i�S�G���j�j�Ȃ�
/// </summary>
public class BattleUIManager : MonoBehaviour
{
    public static BattleUIManager m_Instance; // �V���O���g���i���N���X����A�N�Z�X����p�j

    private int m_currentTurn = 0;

    private PlayerCharacter m_currentPlayer; // ���I�𒆂̃v���C���[�i���g�p�j
    private PlayerCharacter m_attacker;      // �U���ҁi���݃^�[�����̃v���C���[�j
    private Enemy m_selectedTarget;          // �N���b�N���ꂽ�G

    public GameObject m_attackChoicePanel;   // �ʏ�U��/�X�L���I���p�l��
    public List<PlayerCharacter> m_allPlayers; // �S�v���C���[���X�g
    public BattleSystem m_battleSystem;      // �o�g������p�N���X�ւ̎Q��

    /// <summary>
    /// �Q�[���N�����ɍU���p�l�����\���ɂ��A�V���O���g����������
    /// </summary>
    private void Awake()
    {
        m_Instance = this;
        m_attackChoicePanel.SetActive(false); // �ŏ��͔�\��
    }

    /// <summary>
    /// �G�L�������N���b�N�����Ƃ��ɌĂ΂�鏈��
    /// </summary>
    public void OnEnemyClicked(Enemy target)
    {
        // ���łɓ|����Ă���G�Ȃ瑦����
        if (target.m_deathFlag)
        {
            m_battleSystem.EndBattle(!target.m_deathFlag);
        }

        // �����Ă���G�Ȃ�U���ΏۂƂ��Đݒ�
        if (!target.m_deathFlag)
        {
            m_selectedTarget = target;
            m_attackChoicePanel.SetActive(true); // �ʏ�/�X�L���I��UI��\��
        }
    }

    /// <summary>
    /// �v���C���[�L�������N���b�N�����Ƃ��̏����i����L����������j
    /// </summary>
    public void OnPlayerCharacterSelected(PlayerCharacter player)
    {
        // ���łɓG���S�ł��Ă���Ώ�������
        if (!FindObjectOfType<Enemy>())
        {
            m_battleSystem.EndBattle(true);
        }

        // HP��0�Ȃ�I��s��
        if (player.m_currentHP <= 0)
        {
            Debug.Log("���̃L�����͍s���s�\�ł�");
            return;
        }

        // �I�����ꂽ�L������UI�g�����I����Ԃ�
        foreach (var pc in m_allPlayers)
        {
            bool isSelected = (pc == player);
            pc.SetSelected(isSelected);
        }

        SetCurrentAttacker(player); // �U���Ґݒ�
    }

    /// <summary>
    /// �ʏ�U���{�^���������F�U���������s���A�^�[�����I��
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
        m_battleSystem.EndPlayerTurn(); // �G�̃^�[���ֈڍs
    }

    /// <summary>
    /// �X�L���{�^���������F�X�L�������������s���A�^�[�����I��
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
        m_battleSystem.EndPlayerTurn(); // �G�̃^�[���ֈڍs
    }

    /// <summary>
    /// ���݂̃v���C���[�A�N�^�[���Z�b�g�i���̍s���L�����j
    /// </summary>
    public void SetCurrentAttacker(PlayerCharacter pc)
    {
        m_attacker = pc;
    }

    /// <summary>
    /// ���݂̃^�[�Q�b�g�ɑ΂���G�̔����i���g�p or �g���p�j
    /// </summary>
    private void EnemyCounterAttack()
    {
        Debug.Log("���������J�n");

        var alivePlayers = m_allPlayers.FindAll(p => p.m_currentHP > 0);

        if (alivePlayers.Count == 0 || m_selectedTarget == null)
        {
            Debug.Log("�����Ώۂ����Ȃ�");
            m_battleSystem.EndPlayerTurn();
            return;
        }

        var randomPlayer = alivePlayers[Random.Range(0, alivePlayers.Count)];
        Debug.Log($"�G�� {randomPlayer.m_data.m_characterName} �ɔ����I");
        m_selectedTarget.Attack(randomPlayer);
    }

    /// <summary>
    /// �S�Ă̓G�����j����Ă��邩�`�F�b�N���A�����������s��
    /// </summary>
    public void CheckAllEnemiesDefeated()
    {
        Enemy[] remainingEnemies = FindObjectsOfType<Enemy>();
        if (remainingEnemies.Length == 0)
        {
            m_battleSystem.EndBattle(true); // ����
        }
    }

    /// <summary>
    /// �v���C���[�L�����̃X�e�[�^�X�X�V�i�g���p�j
    /// </summary>
    private void UpdateAllPlayerStatus()
    {
        foreach (var pc in m_allPlayers)
        {
            pc.RefreshStatus();
        }
    }
}
