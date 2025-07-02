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
    protected override void Awake()
    {
        base.Awake();

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
        if (m_currentSP < 5 || m_data == null || m_deathFlag) return;

        float multiplier = Random.Range(m_data.m_addPowerMin, m_data.m_addPowerMax);
        int baseDamage = m_data.m_skillAttack;
        int damage;

        switch (m_data.m_skillType)
        {
            case SkillType.Slash:
                damage = Mathf.RoundToInt(baseDamage * multiplier);
                target.TakeDamage(damage);
                target.ApplyStatusEffect(StatusEffect.Bleed);
                Debug.Log($"{m_data.m_characterName} �̎a���I �� {damage} �_���[�W�i�o���j");
                break;

            case SkillType.HeavyBlow:
                damage = Mathf.RoundToInt(baseDamage * 1.5f * multiplier);
                target.TakeDamage(damage);
                Debug.Log($"{m_data.m_characterName} �̏d���I �� {damage} �_���[�W");
                break;

            case SkillType.MultiHit:
                for (int i = 0; i < 2; i++)
                {
                    damage = Mathf.RoundToInt(baseDamage * multiplier);
                    target.TakeDamage(damage);
                    Debug.Log($"{m_data.m_characterName} �̘A���I �� {damage} �_���[�W");
                }
                break;

            case SkillType.Debuff:
                Debug.Log($"{m_data.m_characterName} ����̂��������I");
                // ��̌��ʂ̋�̓I�����͕ʓr����
                break;

            case SkillType.Fire:
                damage = Mathf.RoundToInt(baseDamage * 1.2f * multiplier);
                target.TakeDamage(damage);
                target.ApplyStatusEffect(StatusEffect.Burn);
                Debug.Log($"{m_data.m_characterName} �̉��U���I �� {damage} �_���[�W�i�Ώ��j");
                break;

            case SkillType.Poison:
                damage = Mathf.RoundToInt(baseDamage * multiplier);
                target.TakeDamage(damage);
                target.ApplyStatusEffect(StatusEffect.Poison);
                Debug.Log($"{m_data.m_characterName} �̓ōU���I �� {damage} �_���[�W�i�Łj");
                break;

            case SkillType.Shield:
                ApplyStatusEffect(StatusEffect.Shield); // �����ɃV�[���h�t�^
                Debug.Log($"{m_data.m_characterName} �̓V�[���h�𒣂����I");
                break;

            case SkillType.Heal:
                if (target == null || target.m_data == null || target.m_deathFlag)
                {
                    Debug.Log($"{m_data.m_characterName} �͉񕜑Ώۂ����Ȃ����߃X�L���𖳌����I");
                    break;
                }

                int healAmount = Mathf.RoundToInt(baseDamage * multiplier);
                target.m_currentHP = Mathf.Min(target.m_currentHP + healAmount, target.m_data.m_maxHP);
                Debug.Log($"{m_data.m_characterName} �� {target.m_data.m_characterName} �� {healAmount} �񕜂����I");
                if (target is PlayerCharacter player)
                {
                    player.UpdateStatusDisplay();
                }
                break;

            default:
                damage = Mathf.RoundToInt(baseDamage * multiplier);
                target.TakeDamage(damage);
                Debug.Log($"{m_data.m_characterName} �̃X�L���I �� {damage} �_���[�W");
                break;
        }

        if (m_data.m_skillSE != null)
            m_audioSource.PlayOneShot(m_data.m_skillSE);

        m_currentSP -= 5;
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
        Debug.Log($"{name} ResetStatus ���s, m_data = {(m_data != null)}");

        if (m_data != null)
        {
            m_currentSP = m_data.m_maxSP;
            m_currentHP = m_data.m_maxHP;
        }

        m_deathFlag = false;

        if (m_iconImage != null)
            m_iconImage.sprite = m_data?.m_iconNormal;

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
