using UnityEngine;
using UnityEngine.UI;
using TMPro;  // TextMeshPro �p
using static CharacterData;

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
    private string m_statusEffect = null;
    private int m_effectTurns = 0;




    protected override void Start()
    {
        if (m_data != null)
        {
            m_currentSP = m_data.m_maxSP;
            m_currentHP = m_data.m_maxHP;
        }

        base.Start();
        m_iconImage.sprite = m_data.m_iconNormal;
        m_hpSlider.interactable = false;
        UpdateUI();
    }

    public void ApplyStatusEffect(string type)
    {
        m_statusEffect = type;
        m_effectTurns = 3;
    }

    /// <summary>
    /// �G�̍U�������A�ꉞ�X�L������
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

                // ���̃X�L���^�C�v���K�v�ɉ����Ēǉ�
        }

        if (m_data.m_skillSE != null)
            m_audioSource.PlayOneShot(m_data.m_skillSE);

        m_iconImage.sprite = m_data.m_iconAttack;
        UpdateUI();
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
            m_iconImage.sprite = m_data.m_iconDamaged;

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
            Die();
        }
    }

    public void Die()
    {
        Debug.Log("�G��|�����I");
        m_deathFlag = true;
        gameObject.SetActive(false);
        BattleUIManager.m_Instance.CheckAllEnemiesDefeated();
    }

    /// <summary>
    /// ���݂�HP�E���O�ɉ�����UI�X�V
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
    /// �\���HP�󋵂ɉ����Đ؂�ւ���
    /// </summary>
    private void UpdateIconState()
    {
        if (m_iconImage == null) return;

        m_iconImage.sprite = m_currentHP <= m_data.m_maxHP * 0.3f ? m_data.m_iconLowHP : m_data.m_iconNormal;
    }

    public override void ResetStatus()
    {
        if (m_data != null)
        {
            m_currentSP = m_data.m_maxSP;
            m_currentHP = m_data.m_maxHP;
        }

        m_statusEffect = null;
        m_effectTurns = 0;
        m_deathFlag = false;
        gameObject.SetActive(true);
        m_iconImage.sprite = m_data.m_iconNormal;

        UpdateUI();
    }

    public void UpdateTurn()
    {
        if (m_effectTurns <= 0) return;

        switch (m_statusEffect)
        {
            case "poison":
                TakeDamage(10);
                Debug.Log("�ł̃_���[�W�I");
                break;
            case "burn":
                TakeDamage(15);
                Debug.Log("�Ώ��̃_���[�W�I");
                break;
            case "freeze":
                Debug.Log("�������ōs���s�\�I");
                // �����̓_���[�W�ł͂Ȃ��s������
                break;
        }

        m_effectTurns--;
        if (m_effectTurns <= 0)
            m_statusEffect = null;
    }
}
