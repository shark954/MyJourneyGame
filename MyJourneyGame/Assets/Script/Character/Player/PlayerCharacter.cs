using UnityEngine;
using TMPro;
using UnityEngine.UI;
using static CharacterData;

/// <summary>
/// プレイヤー専用のキャラクタークラス（表示やスキル処理含む）
/// CharacterBase を継承して共通処理を引き継ぐ
/// </summary>
public class PlayerCharacter : CharacterBase
{
    public GameObject m_disabledOverlay; // 戦闘不能時の黒ぼかしUI

    [Header("UI 表示")]
    public TextMeshProUGUI m_spText, m_nameText; // SP表示、名前表示
    public GameObject m_selectionFrame;          // 選択フレーム（点滅枠）
    public Slider m_hpSlider;                    // HPゲージ

    [Header("表情スプライト")]
    public Image m_iconImage; // 表情アイコンの切り替え

    /// <summary>
    /// 初期化（HP/SP・UI設定）
    /// </summary>
    protected override void Awake()
    {
        base.Awake();

        if (m_disabledOverlay != null)
            m_disabledOverlay.SetActive(false);

        if (m_data != null)
        {
            m_currentHP = m_data.m_maxHP;
            m_currentSP = m_data.m_maxSP;
        }

        m_hpSlider.interactable = false;
        UpdateStatusDisplay();
    }

    /// <summary>
    /// 通常攻撃（敵キャラにダメージ）
    /// </summary>
    public void Attack(CharacterBase target)
    {
        if (m_currentHP <= 0) return;

        float multiplier = Random.Range(m_data.m_addPowerMin, m_data.m_addPowerMax);
        int damage = Mathf.RoundToInt(m_data.m_normalAttack * multiplier);
        target.TakeDamage(damage);

        if (m_data.m_attackSE != null)
            m_audioSource.PlayOneShot(m_data.m_attackSE);

        m_iconImage.sprite = m_data.m_iconAttack;
    }

    /// <summary>
    /// スキル使用処理（タイプに応じて行動を分岐）
    /// </summary>
    public void UseSkill(CharacterBase target)
    {
        if (m_currentSP < 5 || m_data == null || m_deathFlag) return;

        float multiplier = Random.Range(m_data.m_addPowerMin, m_data.m_addPowerMax);
        int baseDamage = m_data.m_skillAttack;
        int damage;

        switch (m_data.m_skillType)
        {
            case SkillType.Slash:
                damage = Mathf.RoundToInt(baseDamage * multiplier);
                target.TakeDamage(damage);
                target.ApplyStatusEffect(StatusEffect.Bleed);
                Debug.Log($"{m_data.m_characterName} の斬撃！ → {damage} ダメージ（出血）");
                break;

            case SkillType.HeavyBlow:
                damage = Mathf.RoundToInt(baseDamage * 1.5f * multiplier);
                target.TakeDamage(damage);
                Debug.Log($"{m_data.m_characterName} の重撃！ → {damage} ダメージ");
                break;

            case SkillType.MultiHit:
                for (int i = 0; i < 2; i++)
                {
                    damage = Mathf.RoundToInt(baseDamage * multiplier);
                    target.TakeDamage(damage);
                    Debug.Log($"{m_data.m_characterName} の連撃！ → {damage} ダメージ");
                }
                break;

            case SkillType.Debuff:
                Debug.Log($"{m_data.m_characterName} が弱体をかけた！");
                // 弱体効果の具体的処理は別途実装
                break;

            case SkillType.Fire:
                damage = Mathf.RoundToInt(baseDamage * 1.2f * multiplier);
                target.TakeDamage(damage);
                target.ApplyStatusEffect(StatusEffect.Burn);
                Debug.Log($"{m_data.m_characterName} の炎攻撃！ → {damage} ダメージ（火傷）");
                break;

            case SkillType.Poison:
                damage = Mathf.RoundToInt(baseDamage * multiplier);
                target.TakeDamage(damage);
                target.ApplyStatusEffect(StatusEffect.Poison);
                Debug.Log($"{m_data.m_characterName} の毒攻撃！ → {damage} ダメージ（毒）");
                break;

            case SkillType.Shield:
                ApplyStatusEffect(StatusEffect.Shield); // 自分にシールド付与
                Debug.Log($"{m_data.m_characterName} はシールドを張った！");
                break;

            case SkillType.Heal:
                if (target == null || target.m_data == null || target.m_deathFlag)
                {
                    Debug.Log($"{m_data.m_characterName} は回復対象がいないためスキルを無効化！");
                    break;
                }

                int healAmount = Mathf.RoundToInt(baseDamage * multiplier);
                target.m_currentHP = Mathf.Min(target.m_currentHP + healAmount, target.m_data.m_maxHP);
                Debug.Log($"{m_data.m_characterName} は {target.m_data.m_characterName} を {healAmount} 回復した！");
                if (target is PlayerCharacter player)
                {
                    player.UpdateStatusDisplay();
                }
                break;

            default:
                damage = Mathf.RoundToInt(baseDamage * multiplier);
                target.TakeDamage(damage);
                Debug.Log($"{m_data.m_characterName} のスキル！ → {damage} ダメージ");
                break;
        }

        if (m_data.m_skillSE != null)
            m_audioSource.PlayOneShot(m_data.m_skillSE);

        m_currentSP -= 5;
        UpdateStatusDisplay();
    }


    /// <summary>
    /// キャラが選択された時に呼ばれる（UIと表情切替）
    /// </summary>
    public void SetSelected(bool selected)
    {
        m_selectionFrame.SetActive(selected);
        UpdateStatusDisplay();

        if (selected && m_iconImage)
            m_iconImage.sprite = m_data.m_iconSelected;
    }

    /// <summary>
    /// ダメージ受けた際の処理（表情更新）
    /// </summary>
    public override void TakeDamage(int amount)
    {
        base.TakeDamage(amount);

        if (m_iconImage != null)
        {
            m_iconImage.sprite = m_data.m_iconDamaged;
            Invoke(nameof(UpdateStatusDisplay), 0.4f); // 一瞬表情変化
        }
        else
        {
            UpdateStatusDisplay();
        }

        RefreshStatus();
    }

    /// <summary>
    /// UI（HPゲージ、SP、表情）を更新
    /// </summary>
    public void UpdateStatusDisplay()
    {
        m_hpSlider.maxValue = m_data.m_maxHP;
        m_hpSlider.value = m_currentHP;

        float hpRatio = (float)m_currentHP / m_data.m_maxHP;
        m_hpSlider.fillRect.GetComponent<Image>().color =
            hpRatio <= 0.3f ? Color.red :
            hpRatio <= 0.6f ? Color.yellow : Color.green;

        m_spText.text = $"SP: {m_currentSP}/{m_data.m_maxSP}";
        m_nameText.text = m_data.m_characterName;

        if (m_iconImage != null)
        {
            m_iconImage.sprite = hpRatio <= 0.3f
                ? m_data.m_iconLowHP
                : m_data.m_iconNormal;
        }
    }

    /// <summary>
    /// 戦闘開始・再開時にHP/SPなどを全回復
    /// </summary>
    public override void ResetStatus()
    {
        Debug.Log($"{name} ResetStatus 実行, m_data = {(m_data != null)}");

        if (m_data != null)
        {
            m_currentSP = m_data.m_maxSP;
            m_currentHP = m_data.m_maxHP;
        }

        m_deathFlag = false;

        if (m_iconImage != null)
            m_iconImage.sprite = m_data?.m_iconNormal;

        UpdateStatusDisplay();
    }


    /// <summary>
    /// 生死状態に応じてUI切替（黒ぼかしなど）
    /// </summary>
    public void RefreshStatus()
    {
        if (m_currentHP > 0)
        {
            m_selectionFrame.SetActive(true);
            if (m_disabledOverlay != null)
                m_disabledOverlay.SetActive(false);
        }
        else
        {
            if (m_disabledOverlay != null)
                m_disabledOverlay.SetActive(true);

            m_deathFlag = true;
        }
    }
}
