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

    [Header("�U����")]
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

    /// <summary>
    /// �G�̍U�������A�ꉞ�X�L������
    /// </summary>
    public void Attack(PlayerCharacter target)
    {
        int damage = CalculateDamage(m_attackPower, m_addPower, m_chageCount); // �����_���␳����
        Debug.Log($"�G�̍U���I �� {damage} �_���[�W");
        target.TakeDamage(damage);
    }

    /// <summary>
    /// �����_���␳�t���_���[�W�v�Z
    /// </summary>
    private int CalculateDamage(float baseDamage, float addPower, int chargeCount)
    {
        float chargePower = addPower * chargeCount;
        float randomFactor = Random.Range((baseDamage * chargePower) / 2f, baseDamage * chargePower);
        return Mathf.RoundToInt(baseDamage + randomFactor);
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

            if (m_currentHP > 0)
            {
                // �������Ȃ�ʏ�A�C�R���ɖ߂�
                Invoke(nameof(UpdateIconState), 0.4f);
            }
            else
            {
                // ���S���͂����؂�ւ��iInvoke �͖��������̂Łj
                UpdateIconState();
            }
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
    /// �\���HP�󋵂ɉ����Đ؂�ւ���
    /// </summary>
    private void UpdateIconState()
    {
        if (m_iconImage == null) return;

        m_iconImage.sprite = m_currentHP <= m_maxHP * 0.3f ? m_lowHP : m_normal;
    }
}
