using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// プレイヤー専用のステータスと処理（CharacterData を利用）
/// </summary>
public class PlayerCharacter : CharacterBase
{
   
    [Header("UI 表示")]
    public TextMeshProUGUI m_spText, m_nameText;
    public GameObject m_selectionFrame;
    public Slider m_hpSlider;

    [Header("表情スプライト")]
    public Image m_iconImage;


    protected override void Start()
    {
        base.Start();

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
        float randomMultiplier = Random.Range(m_data.m_addPowerMin, m_data.m_addPowerMax);
        int damage = Mathf.RoundToInt(m_data.m_normalAttack * randomMultiplier);
        Debug.Log($"{m_data.m_characterName} の攻撃！ → {damage} ダメージ");
        target.TakeDamage(damage);
        m_iconImage.sprite = m_data.m_iconAttack;
    }

    public void UseSkill(CharacterBase target)
    {
        if (m_currentSP >= 5)
        {
            float randomMultiplier = Random.Range(m_data.m_addPowerMin, m_data.m_addPowerMax);
            int damage = Mathf.RoundToInt(m_data.m_skillAttack * randomMultiplier);
            m_iconImage.sprite = m_data.m_iconAttack;
            Debug.Log($"{m_data.m_characterName} のスキル発動！ → {damage} ダメージ");
            m_currentSP -= 5;
            target.TakeDamage(damage);

            if (target is Enemy enemy)
            {
                float rand = Random.value;
                if (rand < 0.2f) enemy.ApplyStatusEffect("poison");
                else if (rand < 0.35f) enemy.ApplyStatusEffect("burn");
                else if (rand < 0.45f) enemy.ApplyStatusEffect("freeze");
            }

            UpdateStatusDisplay();
        }
        else
        {
            Debug.Log($"{m_data.m_characterName} は SP が足りない！");
        }
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
}
