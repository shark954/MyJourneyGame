using UnityEngine;
using TMPro;
using UnityEngine.UI;


/// <summary>
/// �v���C���[��p�̃X�e�[�^�X�Ə���
/// </summary>
public class PlayerCharacter : CharacterBase
{
    [Header("UI �\��")]
    //public Text m_hpText;
    public TextMeshProUGUI m_spText,m_nameText;

    public GameObject m_selectionFrame;
    public Slider m_hpSlider;
    public Image m_hpFillImage; // �� Slider�� Fill �Ɋ��蓖�Ă�

    [Header("�\��X�v���C�g")]
    public Image m_iconImage;
    public Sprite m_normal, m_selected, m_damaged, m_lowHP;

    [Header("�U���́A�ʏ�E�X�L��")]
    public int m_nomal;
    public int m_skill;

    public float m_addPower;
    public int m_chageCount;

    /// <summary>
    /// �����������iHP/SP�Ȃǁj
    /// </summary>
    protected override void Start()
    {
        base.Start();
        m_hpSlider.interactable = false;
        m_nomalAttack = m_nomal;
        m_skillAttack = m_skill;
        UpdateStatusDisplay();
    }

    /// <summary>
    /// �ʏ�U��
    /// </summary>
    public void Attack(CharacterBase target)
    {
        int damage = CalculateDamage(m_nomalAttack, m_addPower, m_chageCount); // addPower=1, chargeCount=1 �Ɖ���
        Debug.Log($"{m_characterName} �̍U���I �� {damage} �_���[�W");
        target.TakeDamage(damage);
    }

    /// <summary>
    /// �X�L���U��
    /// </summary>
    public void UseSkill(CharacterBase target)
    {
        if (m_currentSP >= 5)
        {
            int damage = CalculateDamage(m_skillAttack, 1.5f, 2); // ���ɃX�L���͋��߂ɐݒ�
            Debug.Log($"{m_characterName} �̃X�L�������I �� {damage} �_���[�W");
            m_currentSP -= 5;
            target.TakeDamage(damage);
            UpdateStatusDisplay();
        }
        else
        {
            Debug.Log($"{m_characterName} �� SP ������Ȃ��I");
        }
    }


    /// <summary>
    /// �_���[�W�̌v�Z����
    /// </summary>
    private int CalculateDamage(float baseDamage, float addPower, int chargeCount)
    {
        float chargePower = addPower * chargeCount;
        float randomFactor = Random.Range((baseDamage * chargePower) / 2f, baseDamage * chargePower);
        return Mathf.RoundToInt(baseDamage + randomFactor);
    }

    /// <summary>
    /// �L�����I�����̏�ԕύX�i�_�Ł{�A�C�R���j
    /// </summary>
    public void SetSelected(bool selected)
    {
        m_selectionFrame.SetActive(selected);
        SetStateIcon(selected ? "selected" : "normal");
    }

    /// <summary>
    /// �_���[�W���󂯂����̏����ƌ����ڕω�
    /// </summary>
    public override void TakeDamage(int amount)
    {
        base.TakeDamage(amount);

        if (m_iconImage != null)
        {
            m_iconImage.sprite = m_damaged;
            Invoke(nameof(UpdateStatusDisplay), 0.4f); //�_���[�W��ɏ�Ԃ�߂�
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

        //HP�̊����ɉ����ăA�C�R���ύX
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
    /// ��Ԃɉ������A�C�R���ύX
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
