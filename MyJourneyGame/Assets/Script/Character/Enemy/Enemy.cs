using UnityEngine;
using UnityEngine.UI;
using TMPro;  // TextMeshPro 用
using static CharacterData;

/// <summary>
/// 敵専用の処理
/// </summary>
public class Enemy : CharacterBase
{

    [Header("UI表示")]
    public TextMeshProUGUI m_nameText;
    public Slider m_hpSlider;
    public Image m_hpFillImage;

    [Header("敵アイコンと状態スプライト")]
    public Image m_iconImage;
    public Sprite m_normal, m_damaged, m_lowHP;
    private string m_statusEffect = null;
    private int m_effectTurns = 0;




    protected override void Start()
    {
        if (m_data != null)
        {
            m_currentSP = m_data.m_maxSP;
            m_currentHP = m_data.m_maxHP;
        }

        base.Start();
        m_iconImage.sprite = m_data.m_iconNormal;
        m_hpSlider.interactable = false;
        UpdateUI();
    }

    public void ApplyStatusEffect(string type)
    {
        m_statusEffect = type;
        m_effectTurns = 3;
    }

    /// <summary>
    /// 敵の攻撃処理、一応スキル判定
    /// </summary>
    public void Attack(CharacterBase target)
    {
        if (m_currentHP <= 0) return;
        float randomMultiplier = Random.Range(m_data.m_addPowerMin, m_data.m_addPowerMax);
        int damage = Mathf.RoundToInt(m_data.m_normalAttack * randomMultiplier);
        Debug.Log($"{m_data.m_characterName} の攻撃！ → {damage} ダメージ");
        target.TakeDamage(damage);

        if (m_data.m_attackSE != null)
            m_audioSource.PlayOneShot(m_data.m_attackSE);

        m_iconImage.sprite = m_data.m_iconAttack;
    }

    public void UseSkill(CharacterBase target)
    {
        if (m_currentSP < 5)
        {
            Debug.Log($"{m_data.m_characterName} は SP が足りない！");
            return;
        }

        m_currentSP -= 5;

        switch (m_data.m_skillType)
        {
            case SkillType.Slash:
                int slashDamage = Mathf.RoundToInt(m_data.m_skillAttack * Random.Range(m_data.m_addPowerMin, m_data.m_addPowerMax));
                target.TakeDamage(slashDamage);
                Debug.Log($"{m_data.m_characterName} の斬撃！ → {slashDamage} ダメージ");
                break;

            case SkillType.Heal:
                int healAmount = Mathf.RoundToInt(m_data.m_skillAttack * Random.Range(m_data.m_addPowerMin, m_data.m_addPowerMax));
                m_currentHP = Mathf.Min(m_currentHP + healAmount, m_data.m_maxHP);
                Debug.Log($"{m_data.m_characterName} は {healAmount} 回復した！");
                break;

            case SkillType.Fire:
                int fireDamage = Mathf.RoundToInt(m_data.m_skillAttack * Random.Range(m_data.m_addPowerMin, m_data.m_addPowerMax));
                target.TakeDamage(fireDamage);
                Debug.Log($"{m_data.m_characterName} のファイアスキル！ → {fireDamage} ダメージ");
                break;

                // 他のスキルタイプも必要に応じて追加
        }

        if (m_data.m_skillSE != null)
            m_audioSource.PlayOneShot(m_data.m_skillSE);

        m_iconImage.sprite = m_data.m_iconAttack;
        UpdateUI();
    }



    /// <summary>
    /// ランダム補正付きダメージ計算
    /// </summary>
    private int CalculateDamage(float baseDamage, float addPower, int chargeCount)
    {
        float chargePower = addPower * chargeCount;
        float randomFactor = Random.Range((baseDamage * chargePower) / 2f, baseDamage * chargePower);
        return Mathf.RoundToInt(baseDamage + randomFactor);
    }

    /// <summary>
    /// ダメージを受けたときの処理
    /// </summary>
    public override void TakeDamage(int amount)
    {
        base.TakeDamage(amount);
        UpdateUI();

        if (m_iconImage != null)
        {
            m_iconImage.sprite = m_data.m_iconDamaged;

            if (m_currentHP > 0)
            {
                // 生存中なら通常アイコンに戻す
                Invoke(nameof(UpdateIconState), 0.4f);
            }
            else
            {
                // 死亡時はすぐ切り替え（Invoke は無視されるので）
                UpdateIconState();
            }
        }

        if (m_currentHP <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Debug.Log("敵を倒した！");
        m_deathFlag = true;
        gameObject.SetActive(false);
        BattleUIManager.m_Instance.CheckAllEnemiesDefeated();
    }

    /// <summary>
    /// 現在のHP・名前に応じてUI更新
    /// </summary>
    public void UpdateUI()
    {
        if (m_hpSlider != null)
        {
            m_hpSlider.maxValue = m_data.m_maxHP;   
            m_hpSlider.value = m_currentHP;

            float ratio = (float)m_currentHP / m_data.m_maxHP;
            if (m_hpFillImage != null)
                m_hpFillImage.color = ratio <= 0.3f ? Color.red :
                                      ratio <= 0.6f ? Color.yellow :
                                      Color.green;
        }

        if (m_nameText != null)
            m_nameText.text = m_data.m_characterName;
    }

    /// <summary>
    /// 表情をHP状況に応じて切り替える
    /// </summary>
    private void UpdateIconState()
    {
        if (m_iconImage == null) return;

        m_iconImage.sprite = m_currentHP <= m_data.m_maxHP * 0.3f ? m_data.m_iconLowHP : m_data.m_iconNormal;
    }

    public override void ResetStatus()
    {
        if (m_data != null)
        {
            m_currentSP = m_data.m_maxSP;
            m_currentHP = m_data.m_maxHP;
        }

        m_statusEffect = null;
        m_effectTurns = 0;
        m_deathFlag = false;
        gameObject.SetActive(true);
        m_iconImage.sprite = m_data.m_iconNormal;

        UpdateUI();
    }

    public void UpdateTurn()
    {
        if (m_effectTurns <= 0) return;

        switch (m_statusEffect)
        {
            case "poison":
                TakeDamage(10);
                Debug.Log("毒のダメージ！");
                break;
            case "burn":
                TakeDamage(15);
                Debug.Log("火傷のダメージ！");
                break;
            case "freeze":
                Debug.Log("凍結中で行動不能！");
                // 凍結はダメージではなく行動封じ
                break;
        }

        m_effectTurns--;
        if (m_effectTurns <= 0)
            m_statusEffect = null;
    }
}
