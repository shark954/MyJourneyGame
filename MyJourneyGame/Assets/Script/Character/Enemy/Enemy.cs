using UnityEngine;
using UnityEngine.UI;
using TMPro;  // TextMeshPro 用
using static CharacterData; // SkillType の参照を簡略化

/// <summary>
/// 敵キャラクターに関する処理を行うクラス（攻撃、スキル、ステータスなど）
/// </summary>
public class Enemy : CharacterBase
{
    [Header("UI表示")]
    public TextMeshProUGUI m_nameText;
    public Slider m_hpSlider;
    public Image m_hpFillImage;

    [Header("敵アイコンと状態スプライト")]
    public Image m_iconImage;
    public Sprite m_normal, m_damaged, m_lowHP;

  
    /// <summary>
    /// 初期化処理（HP/SPの設定とUI初期化）
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
    /// 状態異常を付与する
    /// </summary>
    public void ApplyStatusEffect(StatusEffect effect)
    {
        m_statusEffect = effect;
        m_effectTurns = 3; // 状態異常は3ターン継続
    }

    /// <summary>
    /// 通常攻撃処理
    /// </summary>
    public void Attack(CharacterBase target)
    {
        if (m_currentHP <= 0) return;

        float randomMultiplier = Random.Range(m_data.m_addPowerMin, m_data.m_addPowerMax);
        int damage = Mathf.RoundToInt(m_data.m_normalAttack * randomMultiplier);

        Debug.Log($"{m_data.m_characterName} の攻撃！ → {damage} ダメージ");
        target.TakeDamage(damage);

        if (m_data.m_attackSE != null)
            m_audioSource.PlayOneShot(m_data.m_attackSE);

        m_iconImage.sprite = m_data.m_iconAttack;
    }

    /// <summary>
    /// スキル使用処理（タイプによって挙動が異なる）
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
                Debug.Log($"{m_data.m_characterName} の斬撃！ → {damage} ダメージ（出血）");
                break;

            case SkillType.HeavyBlow:
                damage = Mathf.RoundToInt(baseDamage * 1.5f * multiplier);
                target.TakeDamage(damage);
                PlaySkillEffect(target.transform.position);
                Debug.Log($"{m_data.m_characterName} の重撃！ → {damage} ダメージ");
                break;

            case SkillType.MultiHit:
                for (int i = 0; i < 2; i++)
                {
                    damage = Mathf.RoundToInt(baseDamage * multiplier);
                    target.TakeDamage(damage);
                    PlaySkillEffect(target.transform.position);
                    Debug.Log($"{m_data.m_characterName} の連撃！ → {damage} ダメージ");
                }
                break;

            case SkillType.Debuff:
                PlaySkillEffect(target.transform.position);
                Debug.Log($"{m_data.m_characterName} が弱体をかけた！");
                // 弱体効果の具体的処理は別途実装
                break;

            case SkillType.Fire:
                damage = Mathf.RoundToInt(baseDamage * 1.2f * multiplier);
                target.TakeDamage(damage);
                target.ApplyStatusEffect(StatusEffect.Burn);
                PlaySkillEffect(target.transform.position);
                Debug.Log($"{m_data.m_characterName} の炎攻撃！ → {damage} ダメージ（火傷）");
                break;

            case SkillType.Poison:
                damage = Mathf.RoundToInt(baseDamage * multiplier);
                target.TakeDamage(damage);
                target.ApplyStatusEffect(StatusEffect.Poison);
                PlaySkillEffect(target.transform.position);
                Debug.Log($"{m_data.m_characterName} の毒攻撃！ → {damage} ダメージ（毒）");
                break;

            case SkillType.Shield:
                ApplyStatusEffect(StatusEffect.Shield); // 自分にシールド付与
                PlaySkillEffect(target.transform.position);
                Debug.Log($"{m_data.m_characterName} はシールドを張った！");
                break;

            default:
                damage = Mathf.RoundToInt(baseDamage * multiplier);
                target.TakeDamage(damage);
                Debug.Log($"{m_data.m_characterName} のスキル！ → {damage} ダメージ");
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
            Vector3 spawnPos = new Vector3(targetPosition.x, targetPosition.y, -1f); // カメラより手前に出す
            GameObject effect = Instantiate(m_skillEffectPrefab, spawnPos, Quaternion.identity);
            Destroy(effect, 2f); // 2秒後に自動削除
        }
    }

    /// <summary>
    /// ステータスリセット（戦闘前などに呼ぶ）
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
    /// 状態異常効果の毎ターン処理
    /// </summary>
    public void UpdateTurn()
    {
        if (m_effectTurns <= 0) return;

        switch (m_statusEffect)
        {
            case StatusEffect.Poison:
                TakeDamage(10);
                Debug.Log("毒のダメージ！");
                break;
            case StatusEffect.Burn:
                TakeDamage(15);
                Debug.Log("火傷のダメージ！");
                break;
            case StatusEffect.Freeze:
                Debug.Log("凍結中で行動不能！");
                break;
        }

        m_effectTurns--;
        if (m_effectTurns <= 0)
            m_statusEffect = StatusEffect.None;
    }

    /// <summary>
    /// 敵がダメージを受けたときの処理
    /// </summary>
    public override void TakeDamage(int amount)
    {
        base.TakeDamage(amount);
        UpdateUI();

        if (m_iconImage != null)
        {
            m_iconImage.sprite = m_data.m_iconDamaged;

            if (m_currentHP > 0)
                Invoke(nameof(UpdateIconState), 0.4f); // 一時的にダメージ顔を表示
            else
                UpdateIconState(); // 死亡時は即切替
        }

        if (m_currentHP <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// 敵が倒れた時の処理
    /// </summary>
    public void Die()
    {
        Debug.Log("敵を倒した！");
        m_deathFlag = true;
        gameObject.SetActive(false);
        BattleUIManager.m_Instance.CheckAllEnemiesDefeated();
    }

    /// <summary>
    /// HPや色・名前などUIの更新
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
    /// HP状況に応じて顔グラ変更
    /// </summary>
    private void UpdateIconState()
    {
        if (m_iconImage == null) return;

        m_iconImage.sprite = m_currentHP <= m_data.m_maxHP * 0.3f ? m_data.m_iconLowHP : m_data.m_iconNormal;
    }

    /// <summary>
    /// 補助関数：重ね攻撃用などのダメージ計算
    /// </summary>
    private int CalculateDamage(float baseDamage, float addPower, int chargeCount)
    {
        float chargePower = addPower * chargeCount;
        float randomFactor = Random.Range((baseDamage * chargePower) / 2f, baseDamage * chargePower);
        return Mathf.RoundToInt(baseDamage + randomFactor);
    }
}
