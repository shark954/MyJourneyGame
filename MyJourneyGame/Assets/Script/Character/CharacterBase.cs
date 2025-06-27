using UnityEngine;

/// <summary>
/// �v���C���[�E�G���ʂ̊��X�e�[�^�X�N���X
/// </summary>
public class CharacterBase : MonoBehaviour
{
    public string m_characterName;
    public int m_maxHP = 100;
    public int m_currentHP;
    public int m_maxSP = 30;
    public int m_currentSP;
    public int m_nomalAttack;
    public int m_skillAttack;

    protected virtual void Start()
    {
        m_currentHP = m_maxHP;
        m_currentSP = m_maxSP;
    }

    /// <summary>
    /// �_���[�W����
    /// </summary>
    public virtual void TakeDamage(int amount)
    {
        m_currentHP -= amount;
        if (m_currentHP < 0) m_currentHP = 0;
        Debug.Log($"{m_characterName} �� {amount} �_���[�W���󂯂��I�i�cHP: {m_currentHP}�j");
    }
}
