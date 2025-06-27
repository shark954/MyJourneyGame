using UnityEngine;

/// <summary>
/// プレイヤー・敵共通の基底ステータスクラス
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
    /// ダメージ処理
    /// </summary>
    public virtual void TakeDamage(int amount)
    {
        m_currentHP -= amount;
        if (m_currentHP < 0) m_currentHP = 0;
        Debug.Log($"{m_characterName} は {amount} ダメージを受けた！（残HP: {m_currentHP}）");
    }
}
