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
    private string m_statusEffect = null;
    private int m_effectTurns = 0;


    [Header("攻撃力")]
    public int m_attackPower;
    public float m_addPower;
    public int m_chageCount;


    protected override void Start()
    {
        base.Start();
        m_skillAttack = m_attackPower;
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
    public void Attack(PlayerCharacter target)
    {
        if (m_currentHP <= 0) return; // 死亡中は行動できない
        int damage = CalculateDamage(m_skillAttack, m_addPower, m_chageCount); // ランダム補正あり
        Debug.Log($"damage ダメージ");
        target.TakeDamage(damage);
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
            m_iconImage.sprite = m_damaged;

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
            m_hpSlider.maxValue = m_maxHP;   
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
