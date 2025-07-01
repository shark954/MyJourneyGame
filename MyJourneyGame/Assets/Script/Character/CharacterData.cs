using UnityEngine;

/// <summary>
/// キャラクター（プレイヤーや敵）のステータス・スキル・見た目・効果音などを
/// 一括管理する ScriptableObject データ定義。
/// Unityエディタ上で個別キャラクターの設定ファイルを作成できる。
/// </summary>
[CreateAssetMenu(fileName = "NewCharacterData", menuName = "Game/Character Data")]
public class CharacterData : ScriptableObject
{
    /// <summary>
    /// 使用可能なスキルの種類を定義（攻撃・回復・バフ・デバフなど）
    /// </summary>
    public enum SkillType
    {
        None,       // なし（スキル未使用）
        Slash,      // 斬撃（出血効果など）
        Heal,       // 回復スキル
        Fire,       // 火属性攻撃（火傷）
        Buff,       // 強化系
        Debuff,     // 弱体化
        HeavyBlow,  // 一撃大ダメージ
        MultiHit,   // 複数ヒット攻撃
        Poison,     // 毒を付与
        Bleed,      // 出血状態を付与
        Shield      // 防御アップ・シールド付与
    }

    [Header("スキル関連")]
    public SkillType m_skillType;        // このキャラが使うスキルの種類

    [Header("基本情報")]
    public string m_characterName;       // キャラクター名
    public int m_maxHP = 100;            // 最大HP
    public int m_maxSP = 30;             // 最大SP

    [Header("攻撃力")]
    public int m_normalAttack;           // 通常攻撃の基本ダメージ
    public int m_skillAttack;            // スキル攻撃の基本ダメージ
    public float m_addPowerMin;          // 最小ランダム倍率（例: 0.8）
    public float m_addPowerMax;          // 最大ランダム倍率（例: 1.2）

    [Header("見た目（表情スプライト）")]
    public Sprite m_iconNormal;          // 通常時のアイコン
    public Sprite m_iconAttack;          // 攻撃時のアイコン
    public Sprite m_iconLowHP;           // HPが低い時のアイコン
    public Sprite m_iconDamaged;         // ダメージを受けたときのアイコン
    public Sprite m_iconSelected;        // 選択中アイコン

    [Header("効果音")]
    public AudioClip m_attackSE;         // 通常攻撃時の効果音
    public AudioClip m_skillSE;          // スキル使用時の効果音
}
