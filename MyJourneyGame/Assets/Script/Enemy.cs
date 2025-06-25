using UnityEngine;
using UnityEngine.UI;
using TMPro;  // TextMeshPro �p

/// <summary>
/// �G��p�̏���
/// </summary>
public class Enemy : CharacterBase
{
    [Header("UI�\��")]
    public TextMeshProUGUI m_nameText;
    public Slider m_hpSlider;
    public Image m_hpFillImage;

    [Header("�G�A�C�R���Ə�ԃX�v���C�g")]
    public Image m_iconImage;
    public Sprite m_normal, m_damaged, m_lowHP;

    protected override void Start()
    {
        base.Start();
        UpdateUI();
    }

    /// <summary>
    /// �G�̍U������
    /// </summary>
    public void Attack(PlayerCharacter target)
    {
        Debug.Log("�G�̍U���I");
        target.TakeDamage(10);
    }

    /// <summary>
    /// �_���[�W���󂯂��Ƃ��̏���
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
            Debug.Log("�G��|�����I");
            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// ���݂�HP�E���O�ɉ�����UI�X�V
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
    /// �\���HP�󋵂ɉ����Đ؂�ւ���
    /// </summary>
    private void UpdateIconState()
    {
        if (m_iconImage == null) return;

        m_iconImage.sprite = m_currentHP <= m_maxHP * 0.3f ? m_lowHP : m_normal;
    }
}
