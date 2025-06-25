using UnityEngine;
using UnityEngine.UI;

public class CharacterIconController : MonoBehaviour
{
    public Image m_iconImage;

    [Header("�X�v���C�g�ݒ�")]
    public Sprite m_idleSprite;
    public Sprite m_selectedSprite;
    public Sprite m_attackSprite;
    public Sprite m_damagedSprite;
    public Sprite m_lowHPSprite;

    [Header("HP���")]
    public int m_maxHP = 100;
    public int m_currentHP = 100;

    public void SetIdle() => m_iconImage.sprite = m_idleSprite;

    public void SetSelected() => m_iconImage.sprite = m_selectedSprite;

    public void SetAttacking() => m_iconImage.sprite = m_attackSprite;

    public void SetDamaged()
    {
        m_iconImage.sprite = m_damagedSprite;
        Invoke(nameof(UpdateHPState), 0.4f); // 0.4�b���HP��Ԃɖ߂�
    }

    public void SetHP(int newHP)
    {
        m_currentHP = Mathf.Clamp(newHP, 0, m_maxHP);
        UpdateHPState();
    }

    private void UpdateHPState()
    {
        float hpRatio = (float)m_currentHP / m_maxHP;
        if (hpRatio < 0.3f)
            m_iconImage.sprite = m_lowHPSprite;
        else
            m_iconImage.sprite = m_idleSprite;
    }
}
