using UnityEngine;

/// <summary>
/// �G��p�̏���
/// </summary>
public class Enemy : CharacterBase
{
    public void Attack(PlayerCharacter target)
    {
        Debug.Log("�G�̍U���I");
        target.TakeDamage(10);
    }

    public override void TakeDamage(int amount)
    {
        base.TakeDamage(amount);

        if (m_currentHP <= 0)
        {
            Debug.Log("�G��|�����I");
            // ���j���o�Ȃǒǉ��\
        }
    }
}
