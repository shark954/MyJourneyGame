using UnityEngine;
using TMPro;
using UnityEngine.UI;
using static CharacterData;

/// <summary>
/// �v���C���[��p�̃L�����N�^�[�N���X�i�\����X�L�������܂ށj
/// CharacterBase ���p�����ċ��ʏ����������p��
/// </summary>
public class PlayerCharacter : CharacterBase
{
    public GameObject m_disabledOverlay; // �퓬�s�\���̍��ڂ���UI

    [Header("UI �\��")]
    public TextMeshProUGUI m_spText, m_nameText; // SP�\���A���O�\��
    public GameObject m_selectionFrame;          // �I���t���[���i�_�Řg�j
    public Slider m_hpSlider;                    // HP�Q�[�W

    [Header("�\��X�v���C�g")]
    public Image m_iconImage; // �\��A�C�R���̐؂�ւ�

    /// <summary>
    /// �������iHP/SP�EUI�ݒ�j
    /// </summary>
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

    /// <summary>
    /// �ʏ�U���i�G�L�����Ƀ_���[�W�j
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
    /// �X�L���g�p�����i�^�C�v�ɉ����čs���𕪊�j
    /// </summary>
    public void UseSkill(CharacterBase target)
    {
        if (m_currentSP < 5)
        {
            Debug.Log($"{m_data.m_characterName} �� SP ������Ȃ��I");
            return;
        }

        m_currentSP -= 5;

        switch (m_data.m_skillType)
        {
            case SkillType.Slash:
                int slashDamage = Mathf.RoundToInt(m_data.m_skillAttack * Random.Range(m_data.m_addPowerMin, m_data.m_addPowerMax));
                target.TakeDamage(slashDamage);
                Debug.Log($"{m_data.m_characterName} �̎a���I �� {slashDamage} �_���[�W");
                break;

            case SkillType.Heal:
                int healAmount = Mathf.RoundToInt(m_data.m_skillAttack * Random.Range(m_data.m_addPowerMin, m_data.m_addPowerMax));
                m_currentHP = Mathf.Min(m_currentHP + healAmount, m_data.m_maxHP);
                Debug.Log($"{m_data.m_characterName} �� {healAmount} �񕜂����I");
                break;

            case SkillType.Fire:
                int fireDamage = Mathf.RoundToInt(m_data.m_skillAttack * Random.Range(m_data.m_addPowerMin, m_data.m_addPowerMax));
                target.TakeDamage(fireDamage);
                Debug.Log($"{m_data.m_characterName} �̃t�@�C�A�X�L���I �� {fireDamage} �_���[�W");
                break;

                // ���X�L���͕K�v�ɉ����Ēǉ�
        }

        if (m_data.m_skillSE != null)
            m_audioSource.PlayOneShot(m_data.m_skillSE);

        m_iconImage.sprite = m_data.m_iconAttack;
        UpdateStatusDisplay();
    }

    /// <summary>
    /// �L�������I�����ꂽ���ɌĂ΂��iUI�ƕ\��ؑցj
    /// </summary>
    public void SetSelected(bool selected)
    {
        m_selectionFrame.SetActive(selected);
        UpdateStatusDisplay();

        if (selected && m_iconImage)
            m_iconImage.sprite = m_data.m_iconSelected;
    }

    /// <summary>
    /// �_���[�W�󂯂��ۂ̏����i�\��X�V�j
    /// </summary>
    public override void TakeDamage(int amount)
    {
        base.TakeDamage(amount);

        if (m_iconImage != null)
        {
            m_iconImage.sprite = m_data.m_iconDamaged;
            Invoke(nameof(UpdateStatusDisplay), 0.4f); // ��u�\��ω�
        }
        else
        {
            UpdateStatusDisplay();
        }

        RefreshStatus();
    }

    /// <summary>
    /// UI�iHP�Q�[�W�ASP�A�\��j���X�V
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
    /// �퓬�J�n�E�ĊJ����HP/SP�Ȃǂ�S��
    /// </summary>
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

    /// <summary>
    /// ������Ԃɉ�����UI�ؑցi���ڂ����Ȃǁj
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
