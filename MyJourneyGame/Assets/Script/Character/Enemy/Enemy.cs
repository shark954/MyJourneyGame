using UnityEngine;
using UnityEngine.UI;
using TMPro;  // TextMeshPro �p
using static CharacterData; // SkillType �̎Q�Ƃ��ȗ���

/// <summary>
/// �G�L�����N�^�[�Ɋւ��鏈�����s���N���X�i�U���A�X�L���A�X�e�[�^�X�Ȃǁj
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

  
    /// <summary>
    /// �����������iHP/SP�̐ݒ��UI�������j
    /// </summary>
    protected override void Awake()
    {
        base.Awake();

        if (m_data != null)
        {
            m_currentSP = m_data.m_maxSP;
            m_currentHP = m_data.m_maxHP;
        }

        m_iconImage.sprite = m_data.m_iconNormal;
        m_hpSlider.interactable = false;
        UpdateUI();
    }

    /// <summary>
    /// ��Ԉُ��t�^����
    /// </summary>
    public void ApplyStatusEffect(StatusEffect effect)
    {
        m_statusEffect = effect;
        m_effectTurns = 3; // ��Ԉُ��3�^�[���p��
    }

    /// <summary>
    /// �ʏ�U������
    /// </summary>
    public void Attack(CharacterBase target)
    {
        if (m_currentHP <= 0) return;

        float randomMultiplier = Random.Range(m_data.m_addPowerMin, m_data.m_addPowerMax);
        int damage = Mathf.RoundToInt(m_data.m_normalAttack * randomMultiplier);

        Debug.Log($"{m_data.m_characterName} �̍U���I �� {damage} �_���[�W");
        target.TakeDamage(damage);

        if (m_data.m_attackSE != null)
            m_audioSource.PlayOneShot(m_data.m_attackSE);

        m_iconImage.sprite = m_data.m_iconAttack;
    }

    /// <summary>
    /// �X�L���g�p�����i�^�C�v�ɂ���ċ������قȂ�j
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
                PlaySkillEffect(target.transform.position);
                Debug.Log($"{m_data.m_characterName} �̎a���I �� {damage} �_���[�W�i�o���j");
                break;

            case SkillType.HeavyBlow:
                damage = Mathf.RoundToInt(baseDamage * 1.5f * multiplier);
                target.TakeDamage(damage);
                PlaySkillEffect(target.transform.position);
                Debug.Log($"{m_data.m_characterName} �̏d���I �� {damage} �_���[�W");
                break;

            case SkillType.MultiHit:
                for (int i = 0; i < 2; i++)
                {
                    damage = Mathf.RoundToInt(baseDamage * multiplier);
                    target.TakeDamage(damage);
                    PlaySkillEffect(target.transform.position);
                    Debug.Log($"{m_data.m_characterName} �̘A���I �� {damage} �_���[�W");
                }
                break;

            case SkillType.Debuff:
                PlaySkillEffect(target.transform.position);
                Debug.Log($"{m_data.m_characterName} ����̂��������I");
                // ��̌��ʂ̋�̓I�����͕ʓr����
                break;

            case SkillType.Fire:
                damage = Mathf.RoundToInt(baseDamage * 1.2f * multiplier);
                target.TakeDamage(damage);
                target.ApplyStatusEffect(StatusEffect.Burn);
                PlaySkillEffect(target.transform.position);
                Debug.Log($"{m_data.m_characterName} �̉��U���I �� {damage} �_���[�W�i�Ώ��j");
                break;

            case SkillType.Poison:
                damage = Mathf.RoundToInt(baseDamage * multiplier);
                target.TakeDamage(damage);
                target.ApplyStatusEffect(StatusEffect.Poison);
                PlaySkillEffect(target.transform.position);
                Debug.Log($"{m_data.m_characterName} �̓ōU���I �� {damage} �_���[�W�i�Łj");
                break;

            case SkillType.Shield:
                ApplyStatusEffect(StatusEffect.Shield); // �����ɃV�[���h�t�^
                PlaySkillEffect(target.transform.position);
                Debug.Log($"{m_data.m_characterName} �̓V�[���h�𒣂����I");
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
        UpdateUI();
    }


    private void PlaySkillEffect(Vector3 targetPosition)
    {
        if (m_skillEffectPrefab != null)
        {
            Vector3 spawnPos = new Vector3(targetPosition.x, targetPosition.y, -1f); // �J��������O�ɏo��
            GameObject effect = Instantiate(m_skillEffectPrefab, spawnPos, Quaternion.identity);
            Destroy(effect, 2f); // 2�b��Ɏ����폜
        }
    }

    /// <summary>
    /// �X�e�[�^�X���Z�b�g�i�퓬�O�ȂǂɌĂԁj
    /// </summary>
    public override void ResetStatus()
    {
        if (m_data != null)
        {
            m_currentSP = m_data.m_maxSP;
            m_currentHP = m_data.m_maxHP;
        }

        m_statusEffect = StatusEffect.None;
        m_effectTurns = 0;
        m_deathFlag = false;
        gameObject.SetActive(true);
        m_iconImage.sprite = m_data.m_iconNormal;

        UpdateUI();
    }

    /// <summary>
    /// ��Ԉُ���ʂ̖��^�[������
    /// </summary>
    public void UpdateTurn()
    {
        if (m_effectTurns <= 0) return;

        switch (m_statusEffect)
        {
            case StatusEffect.Poison:
                TakeDamage(10);
                Debug.Log("�ł̃_���[�W�I");
                break;
            case StatusEffect.Burn:
                TakeDamage(15);
                Debug.Log("�Ώ��̃_���[�W�I");
                break;
            case StatusEffect.Freeze:
                Debug.Log("�������ōs���s�\�I");
                break;
        }

        m_effectTurns--;
        if (m_effectTurns <= 0)
            m_statusEffect = StatusEffect.None;
    }

    /// <summary>
    /// �G���_���[�W���󂯂��Ƃ��̏���
    /// </summary>
    public override void TakeDamage(int amount)
    {
        base.TakeDamage(amount);
        UpdateUI();

        if (m_iconImage != null)
        {
            m_iconImage.sprite = m_data.m_iconDamaged;

            if (m_currentHP > 0)
                Invoke(nameof(UpdateIconState), 0.4f); // �ꎞ�I�Ƀ_���[�W���\��
            else
                UpdateIconState(); // ���S���͑��ؑ�
        }

        if (m_currentHP <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// �G���|�ꂽ���̏���
    /// </summary>
    public void Die()
    {
        Debug.Log("�G��|�����I");
        m_deathFlag = true;
        gameObject.SetActive(false);
        BattleUIManager.m_Instance.CheckAllEnemiesDefeated();
    }

    /// <summary>
    /// HP��F�E���O�Ȃ�UI�̍X�V
    /// </summary>
    public void UpdateUI()
    {
        if (m_hpSlider != null)
        {
            m_hpSlider.maxValue = m_data.m_maxHP;
            m_hpSlider.value = m_currentHP;

            float ratio = (float)m_currentHP / m_data.m_maxHP;
            if (m_hpFillImage != null)
                m_hpFillImage.color = ratio <= 0.3f ? Color.red :
                                      ratio <= 0.6f ? Color.yellow :
                                      Color.green;
        }

        if (m_nameText != null)
            m_nameText.text = m_data.m_characterName;
    }

    /// <summary>
    /// HP�󋵂ɉ����Ċ�O���ύX
    /// </summary>
    private void UpdateIconState()
    {
        if (m_iconImage == null) return;

        m_iconImage.sprite = m_currentHP <= m_data.m_maxHP * 0.3f ? m_data.m_iconLowHP : m_data.m_iconNormal;
    }

    /// <summary>
    /// �⏕�֐��F�d�ˍU���p�Ȃǂ̃_���[�W�v�Z
    /// </summary>
    private int CalculateDamage(float baseDamage, float addPower, int chargeCount)
    {
        float chargePower = addPower * chargeCount;
        float randomFactor = Random.Range((baseDamage * chargePower) / 2f, baseDamage * chargePower);
        return Mathf.RoundToInt(baseDamage + randomFactor);
    }
}
