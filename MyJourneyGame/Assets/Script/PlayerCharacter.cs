using UnityEngine;
using TMPro;
using UnityEngine.UI;


/// <summary>
/// プレイヤー専用のステータスと処理
/// </summary>
public class PlayerCharacter : CharacterBase
{
    [Header("UI 表示")]
    //public Text m_hpText;
    public TextMeshProUGUI m_spText,m_nameText;

    public GameObject m_selectionFrame;
    public Slider m_hpSlider;
    public Image m_hpFillImage; // ← Sliderの Fill に割り当てる

    [Header("表情スプライト")]
    public Image m_iconImage;
    public Sprite m_normal, m_selected, m_damaged, m_lowHP;

    /// <summary>
    /// 初期化処理（HP/SPなど）
    /// </summary>
    protected override void Start()
    {
        base.Start();
        UpdateStatusDisplay();
    }

    /// <summary>
    /// 通常攻撃
    /// </summary>
    public void Attack(CharacterBase target)
    {
        Debug.Log($"{m_characterName} の攻撃！");
        target.TakeDamage(10);
    }

    /// <summary>
    /// スキル攻撃
    /// </summary>
    public void UseSkill(CharacterBase target)
    {
        if (m_currentSP >= 5)
        {
            Debug.Log($"{m_characterName} のスキル発動！");
            m_currentSP -= 5;
            target.TakeDamage(20);
            UpdateStatusDisplay();
        }
        else
        {
            Debug.Log($"{m_characterName} は SP が足りない！");
        }
    }

    /// <summary>
    /// キャラ選択時の状態変更（点滅＋アイコン）
    /// </summary>
    public void SetSelected(bool selected)
    {
        m_selectionFrame.SetActive(selected);
        SetStateIcon(selected ? "selected" : "normal");
    }

    /// <summary>
    /// ダメージを受けた時の処理と見た目変化
    /// </summary>
    public override void TakeDamage(int amount)
    {
        base.TakeDamage(amount);

        if (m_iconImage != null)
        {
            m_iconImage.sprite = m_damaged;
            Invoke(nameof(UpdateStatusDisplay), 0.4f); //ダメージ後に状態を戻す
        }
        else
        {
            UpdateStatusDisplay();
        }
    }

    public void UpdateStatusDisplay()
    {
        m_hpSlider.value = m_currentHP;

        float hpRatio = (float)m_currentHP / m_maxHP;
        Color color = hpRatio <= 0.3f ? Color.red : hpRatio <= 0.6f ? Color.yellow : Color.green;
        m_hpSlider.fillRect.GetComponent<Image>().color = color;
        m_spText.text = $"SP: {m_currentSP}/{m_maxSP}";
        m_nameText.text = m_characterName;

        //HPの割合に応じてアイコン変更
        if (hpRatio <= 0.3f)
        {
            m_iconImage.sprite = m_lowHP;
        }
        else
        {
            m_iconImage.sprite = m_normal;
        }
    }

    /// <summary>
    /// 状態に応じたアイコン変更
    /// </summary>
    public void SetStateIcon(string state)
    {
        switch (state)
        {
            case "normal": m_iconImage.sprite = m_normal; break;
            case "selected": m_iconImage.sprite = m_selected; break;
            case "damaged": m_iconImage.sprite = m_damaged; break;
            case "low": m_iconImage.sprite = m_lowHP; break;
        }
    }
}
