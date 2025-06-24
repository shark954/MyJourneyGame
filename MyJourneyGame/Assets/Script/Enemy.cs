using UnityEngine;

/// <summary>
/// “Gê—p‚Ìˆ—
/// </summary>
public class Enemy : CharacterBase
{
    public void Attack(PlayerCharacter target)
    {
        Debug.Log("“G‚ÌUŒ‚I");
        target.TakeDamage(10);
    }

    public override void TakeDamage(int amount)
    {
        base.TakeDamage(amount);

        if (m_currentHP <= 0)
        {
            Debug.Log("“G‚ğ“|‚µ‚½I");
            // Œ‚”j‰‰o‚È‚Ç’Ç‰Á‰Â”\
        }
    }
}
