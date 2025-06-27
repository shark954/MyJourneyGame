using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterData", menuName = "Game/Character Data")]
public class CharacterData : ScriptableObject
{
    public string m_characterName;
    public int m_maxHP = 100;
    public int m_maxSP = 30;
    public int m_normalAttack;
    public int m_skillAttack;
    public float m_addPowerMin;
    public float m_addPowerMax;
    public Sprite m_iconNormal;
    public Sprite m_iconAttack;
    public Sprite m_iconLowHP;
    public Sprite m_iconDamaged;
    public Sprite m_iconSelected;
}
