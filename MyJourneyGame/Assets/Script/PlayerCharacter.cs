using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �v���C���[��p�̃X�e�[�^�X�Ə���
/// </summary>
public class PlayerCharacter : CharacterBase
{
    public Text m_hpText;
    public Text m_spText;
    public GameObject m_selectionFrame;

    protected override void Start()
    {
        base.Start();
        UpdateStatusDisplay();
    }

    public void SetSelected(bool selected)
    {
        m_selectionFrame.SetActive(selected);
    }

    public void Attack(CharacterBase target)
    {
        Debug.Log($"{m_characterName} �̍U���I");
        target.TakeDamage(10);
    }

    public void UseSkill(CharacterBase target)
    {
        if (m_currentSP >= 5)
        {
            Debug.Log($"{m_characterName} �̃X�L�������I");
            m_currentSP -= 5;
            target.TakeDamage(20);
            UpdateStatusDisplay();
        }
        else
        {
            Debug.Log($"{m_characterName} �� SP ������Ȃ��I");
        }
    }

    public override void TakeDamage(int amount)
    {
        base.TakeDamage(amount);
        UpdateStatusDisplay();
    }

    public void UpdateStatusDisplay()
    {
        m_hpText.text = $"HP: {m_currentHP}/{m_maxHP}";
        m_spText.text = $"SP: {m_currentSP}/{m_maxSP}";
    }
}
