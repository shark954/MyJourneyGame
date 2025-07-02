using UnityEngine;

/// <summary>
/// �v���C���[�E�G���킸�A���ʂ���L�����N�^�[�@�\���`������N���X�B
/// �EHP/SP�̊Ǘ�
/// �E�X�e�[�^�X�ُ폈��
/// �E�_���[�W�����Ȃ�
/// PlayerCharacter / Enemy �����̃N���X���p�����ċ@�\���g���B
/// </summary>
public class CharacterBase : MonoBehaviour
{
    public CharacterData m_data; // �L�������Ƃ̃X�e�[�^�X���iScriptableObject�j

    public int m_currentHP;      // ���݂�HP
    public int m_currentSP;      // ���݂�SP
    public bool m_deathFlag = false; // ���S����i�퓬�s�\��Ԃ��j

    protected AudioSource m_audioSource; // ���ʉ��p�I�[�f�B�I�\�[�X
    protected StatusEffect m_statusEffect = StatusEffect.None; // ���݂̏�Ԉُ�
    protected int m_effectTurns = 0; // ��Ԉُ�̎c��^�[����

    /// <summary>
    /// ��Ԉُ�̎�ނ��
    /// </summary>
    public enum StatusEffect
    {
        None,    // ����
        Poison,  // �ŁF���^�[��10�_���[�W
        Burn,    // �Ώ��F���^�[��15�_���[�W
        Freeze,  // �����F�s���s�\�i�_���[�W�Ȃ��j
        Bleed,   // �o���F���^�[��8�_���[�W
        Shield   // �V�[���h�F�h���ԁi�_���[�W�y���Ȃǂɗ��p�j
    }

    /// <summary>
    /// �I�[�f�B�I�\�[�X���A�^�b�`�i�����I�ɒǉ��j
    /// </summary>
    protected virtual void Awake()
    {
        m_audioSource = gameObject.AddComponent<AudioSource>();
        if (m_data != null)
        {
            m_currentHP = m_data.m_maxHP;
            m_currentSP = m_data.m_maxSP;
        }
    }

    /// <summary>
    /// �_���[�W���󂯂鏈���iHP������������j
    /// </summary>
    /// <param name="amount">�󂯂�_���[�W�̗�</param>
    public virtual void TakeDamage(int amount)
    {
        m_currentHP -= amount;
        if (m_currentHP < 0) m_currentHP = 0;

        Debug.Log($"{m_data.m_characterName} �� {amount} �_���[�W���󂯂��I�i�cHP: {m_currentHP}�j");
    }

    /// <summary>
    /// HP/SP���ő�l�Ƀ��Z�b�g�i�퓬�J�n���ȂǂɎg�p�j
    /// </summary>
    public virtual void ResetStatus()
    {
        if (m_data != null)
        {
            m_currentHP = m_data.m_maxHP;
            m_currentSP = m_data.m_maxSP;
        }
    }

    /// <summary>
    /// ��Ԉُ��V���ɓK�p
    /// </summary>
    /// <param name="effect">�K�p�����Ԉُ�̎��</param>
    /// <param name="turns">���ʃ^�[�����i�f�t�H���g��3�j</param>
    public virtual void ApplyStatusEffect(StatusEffect effect, int turns = 3)
    {
        m_statusEffect = effect;
        m_effectTurns = turns;
    }

    /// <summary>
    /// �^�[���o�ߎ��ɏ�Ԉُ�̉e��������
    /// </summary>
    public virtual void UpdateTurnStatusEffect()
    {
        if (m_effectTurns <= 0) return;

        switch (m_statusEffect)
        {
            case StatusEffect.Poison:
                TakeDamage(10);
                Debug.Log("�Ń_���[�W�I");
                break;

            case StatusEffect.Burn:
                TakeDamage(15);
                Debug.Log("�Ώ��_���[�W�I");
                break;

            case StatusEffect.Freeze:
                Debug.Log("�������ōs���s�\�I");
                // �s���s�\���W�b�N�͌Ăяo�����Ŕ��f
                break;

            case StatusEffect.Bleed:
                TakeDamage(8);
                Debug.Log("�o���_���[�W�I");
                break;

            case StatusEffect.Shield:
                Debug.Log("�V�[���h���I");
                // �_���[�W�y�����W�b�N�͎󂯎葤�Ŏ���
                break;
        }

        m_effectTurns--;
        if (m_effectTurns <= 0)
            m_statusEffect = StatusEffect.None;
    }
}
