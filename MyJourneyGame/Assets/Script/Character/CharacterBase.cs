using UnityEngine;

public class CharacterBase : MonoBehaviour
{
    public CharacterData m_data;

    public int m_currentHP;
    public int m_currentSP;

    protected virtual void Start()
    {
        if (m_data != null)
        {
            m_currentHP = m_data.m_maxHP;
            m_currentSP = m_data.m_maxSP;
        }
    }

    public virtual void TakeDamage(int amount)
    {
        m_currentHP -= amount;
        if (m_currentHP < 0) m_currentHP = 0;
        Debug.Log($"{m_data.m_characterName} は {amount} ダメージを受けた！（残HP: {m_currentHP}）");
    }

    public virtual void ResetStatus()
    {
        if (m_data != null)
        {
            m_currentHP = m_data.m_maxHP;
            m_currentSP = m_data.m_maxSP;
        }
    }
}
