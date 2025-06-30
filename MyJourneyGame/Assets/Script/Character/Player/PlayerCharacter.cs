using UnityEngine;
using TMPro;
using UnityEngine.UI;
using static CharacterData;

/// <summary>
/// プレイヤー専用のステータスと処理（CharacterData を利用）
/// </summary>
public class PlayerCharacter : CharacterBase
{
    public GameObject m_disabledOverlay; // 黒の半透明UIをアタッチする

    [Header("UI 表示")]
    public TextMeshProUGUI m_spText, m_nameText;
    public GameObject m_selectionFrame;
    public Slider m_hpSlider;

    [Header("表情スプライト")]
    public Image m_iconImage;

    private string m_statusEffect = null;
    private int m_effectTurns = 0;

 

    protected override void Start()
    {
        base.Start();

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

    public void Attack(CharacterBase target)
    {
        if (m_currentHP <= 0) return;
        float multiplier = Random.Range(m_data.m_addPowerMin, m_data.m_addPowerMax);
        int damage = Mathf.RoundToInt(m_data.m_normalAttack * multiplier);
        target.TakeDamage(damage);

        // ★ 効果音再生
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
        UpdateStatusDisplay();
    }

    public void SetSelected(bool selected)
    {
        m_selectionFrame.SetActive(selected);
        UpdateStatusDisplay();
        if (selected && m_iconImage) m_iconImage.sprite = m_data.m_iconSelected;
    }

    public override void TakeDamage(int amount)
    {
        base.TakeDamage(amount);
        if (m_iconImage != null)
        {
            m_iconImage.sprite = m_data.m_iconDamaged;
            Invoke(nameof(UpdateStatusDisplay), 0.4f);
        }
        else
        {
            UpdateStatusDisplay();
        }

        RefreshStatus();
    }

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
            if (hpRatio <= 0.3f)
                m_iconImage.sprite = m_data.m_iconLowHP;
            else
                m_iconImage.sprite = m_data.m_iconNormal;
        }
    }

    public override void ResetStatus()
    {
        if (m_data != null)
        {
            m_currentSP = m_data.m_maxSP;
            m_currentHP = m_data.m_maxHP;
        }

        m_deathFlag = false;
        m_iconImage.sprite = m_data.m_iconNormal;

        UpdateStatusDisplay();
    }

    public void RefreshStatus()
    {
        if (m_currentHP > 0)
        {
            m_selectionFrame.SetActive(true); // 点滅フレームは生存時のみ
            if (m_disabledOverlay != null)
                m_disabledOverlay.SetActive(false); // 黒ぼかし非表示
        }
        else
        {
            if (m_disabledOverlay != null)
                m_disabledOverlay.SetActive(true); // 黒フェードUIの切り替え
            m_deathFlag = true;
         
        }
    }
}
