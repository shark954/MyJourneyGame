using UnityEngine;
using UnityEngine.UI;
using TMPro;  // TextMeshPro 用

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

    protected override void Start()
    {
        base.Start();
        UpdateUI();
    }

    /// <summary>
    /// 敵の攻撃処理
    /// </summary>
    public void Attack(PlayerCharacter target)
    {
        Debug.Log("敵の攻撃！");
        target.TakeDamage(10);
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
            m_iconImage.sprite = m_damaged;
            Invoke(nameof(UpdateIconState), 0.4f);
        }

        if (m_currentHP <= 0)
        {
            Debug.Log("敵を倒した！");
            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 現在のHP・名前に応じてUI更新
    /// </summary>
    private void UpdateUI()
    {
        if (m_hpSlider != null)
        {
            m_hpSlider.value = m_currentHP;

            float ratio = (float)m_currentHP / m_maxHP;
            if (m_hpFillImage != null)
                m_hpFillImage.color = ratio <= 0.3f ? Color.red :
                                      ratio <= 0.6f ? Color.yellow :
                                      Color.green;
        }

        if (m_nameText != null)
            m_nameText.text = m_characterName;
    }

    /// <summary>
    /// 表情をHP状況に応じて切り替える
    /// </summary>
    private void UpdateIconState()
    {
        if (m_iconImage == null) return;

        m_iconImage.sprite = m_currentHP <= m_maxHP * 0.3f ? m_lowHP : m_normal;
    }
}
