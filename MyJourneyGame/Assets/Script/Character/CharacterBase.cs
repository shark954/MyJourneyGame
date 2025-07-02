using UnityEngine;

/// <summary>
/// プレイヤー・敵を問わず、共通するキャラクター機能を定義する基底クラス。
/// ・HP/SPの管理
/// ・ステータス異常処理
/// ・ダメージ処理など
/// PlayerCharacter / Enemy がこのクラスを継承して機能を拡張。
/// </summary>
public class CharacterBase : MonoBehaviour
{
    public CharacterData m_data; // キャラごとのステータス情報（ScriptableObject）

    public int m_currentHP;      // 現在のHP
    public int m_currentSP;      // 現在のSP
    public bool m_deathFlag = false; // 死亡判定（戦闘不能状態か）

    protected AudioSource m_audioSource; // 効果音用オーディオソース
    protected StatusEffect m_statusEffect = StatusEffect.None; // 現在の状態異常
    protected int m_effectTurns = 0; // 状態異常の残りターン数

    /// <summary>
    /// 状態異常の種類を列挙
    /// </summary>
    public enum StatusEffect
    {
        None,    // 無効
        Poison,  // 毒：毎ターン10ダメージ
        Burn,    // 火傷：毎ターン15ダメージ
        Freeze,  // 凍結：行動不能（ダメージなし）
        Bleed,   // 出血：毎ターン8ダメージ
        Shield   // シールド：防御状態（ダメージ軽減などに利用）
    }

    /// <summary>
    /// オーディオソースをアタッチ（自動的に追加）
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
    /// ダメージを受ける処理（HPを減少させる）
    /// </summary>
    /// <param name="amount">受けるダメージの量</param>
    public virtual void TakeDamage(int amount)
    {
        m_currentHP -= amount;
        if (m_currentHP < 0) m_currentHP = 0;

        Debug.Log($"{m_data.m_characterName} は {amount} ダメージを受けた！（残HP: {m_currentHP}）");
    }

    /// <summary>
    /// HP/SPを最大値にリセット（戦闘開始時などに使用）
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
    /// 状態異常を新たに適用
    /// </summary>
    /// <param name="effect">適用する状態異常の種類</param>
    /// <param name="turns">効果ターン数（デフォルトは3）</param>
    public virtual void ApplyStatusEffect(StatusEffect effect, int turns = 3)
    {
        m_statusEffect = effect;
        m_effectTurns = turns;
    }

    /// <summary>
    /// ターン経過時に状態異常の影響を処理
    /// </summary>
    public virtual void UpdateTurnStatusEffect()
    {
        if (m_effectTurns <= 0) return;

        switch (m_statusEffect)
        {
            case StatusEffect.Poison:
                TakeDamage(10);
                Debug.Log("毒ダメージ！");
                break;

            case StatusEffect.Burn:
                TakeDamage(15);
                Debug.Log("火傷ダメージ！");
                break;

            case StatusEffect.Freeze:
                Debug.Log("凍結中で行動不能！");
                // 行動不能ロジックは呼び出し元で判断
                break;

            case StatusEffect.Bleed:
                TakeDamage(8);
                Debug.Log("出血ダメージ！");
                break;

            case StatusEffect.Shield:
                Debug.Log("シールド中！");
                // ダメージ軽減ロジックは受け手側で実装
                break;
        }

        m_effectTurns--;
        if (m_effectTurns <= 0)
            m_statusEffect = StatusEffect.None;
    }
}
