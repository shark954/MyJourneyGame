using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Battle/EnemyData")]
public class EnemyData : ScriptableObject
{
    public string m_enemyName;
    public int m_maxHP;
    public int m_basePower;
    public float m_addPowerMin;
    public float m_addPowerMax;

    public Sprite m_iconNormal;
    public Sprite m_iconDamaged;
    public Sprite m_iconLowHP;
}
