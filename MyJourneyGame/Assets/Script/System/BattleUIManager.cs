using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// バトルUIを操作・管理するクラス（主にプレイヤーと敵のクリック処理）
/// - プレイヤーや敵の選択
/// - 攻撃コマンドUIの表示/非表示
/// - 攻撃/スキルの発動
/// - 勝利判定（全敵撃破）など
/// </summary>
public class BattleUIManager : MonoBehaviour
{
    public static BattleUIManager m_Instance; // シングルトン（他クラスからアクセスする用）

    private int m_currentTurn = 0;

    private PlayerCharacter m_currentPlayer; // 今選択中のプレイヤー（未使用）
    private PlayerCharacter m_attacker;      // 攻撃者（現在ターン中のプレイヤー）
    private Enemy m_selectedTarget;          // クリックされた敵

    public GameObject m_attackChoicePanel;   // 通常攻撃/スキル選択パネル
    public List<PlayerCharacter> m_allPlayers; // 全プレイヤーリスト
    public BattleSystem m_battleSystem;      // バトル制御用クラスへの参照

    /// <summary>
    /// ゲーム起動時に攻撃パネルを非表示にし、シングルトンを初期化
    /// </summary>
    private void Awake()
    {
        m_Instance = this;
        m_attackChoicePanel.SetActive(false); // 最初は非表示
    }

    /// <summary>
    /// 敵キャラをクリックしたときに呼ばれる処理
    /// </summary>
    public void OnEnemyClicked(Enemy target)
    {
        // すでに倒されている敵なら即勝利
        if (target.m_deathFlag)
        {
            m_battleSystem.EndBattle(!target.m_deathFlag);
        }

        // 生きている敵なら攻撃対象として設定
        if (!target.m_deathFlag)
        {
            m_selectedTarget = target;
            m_attackChoicePanel.SetActive(true); // 通常/スキル選択UIを表示
        }
    }

    /// <summary>
    /// プレイヤーキャラをクリックしたときの処理（操作キャラを決定）
    /// </summary>
    public void OnPlayerCharacterSelected(PlayerCharacter player)
    {
        // すでに敵が全滅していれば勝利処理
        if (!FindObjectOfType<Enemy>())
        {
            m_battleSystem.EndBattle(true);
        }

        // HPが0なら選択不可
        if (player.m_currentHP <= 0)
        {
            Debug.Log("このキャラは行動不能です");
            return;
        }

        // 選択されたキャラのUI枠だけ選択状態に
        foreach (var pc in m_allPlayers)
        {
            bool isSelected = (pc == player);
            pc.SetSelected(isSelected);
        }

        SetCurrentAttacker(player); // 攻撃者設定
    }

    /// <summary>
    /// 通常攻撃ボタン押下時：攻撃処理を行い、ターンを終了
    /// </summary>
    public void OnAttackButtonPressed()
    {
        if (m_attacker == null || m_selectedTarget == null)
        {
            Debug.LogWarning("攻撃者またはターゲットが未設定です！");
            return;
        }

        m_attacker.Attack(m_selectedTarget);
        m_attackChoicePanel.SetActive(false);
        m_battleSystem.EndPlayerTurn(); // 敵のターンへ移行
    }

    /// <summary>
    /// スキルボタン押下時：スキル発動処理を行い、ターンを終了
    /// </summary>
    public void OnSkillButtonPressed()
    {
        if (m_attacker == null || m_selectedTarget == null)
        {
            Debug.LogWarning("スキル使用者またはターゲットが未設定です！");
            return;
        }

        m_attacker.UseSkill(m_selectedTarget);
        m_attackChoicePanel.SetActive(false);
        m_battleSystem.EndPlayerTurn(); // 敵のターンへ移行
    }

    /// <summary>
    /// 現在のプレイヤーアクターをセット（次の行動キャラ）
    /// </summary>
    public void SetCurrentAttacker(PlayerCharacter pc)
    {
        m_attacker = pc;
    }

    /// <summary>
    /// 現在のターゲットに対する敵の反撃（未使用 or 拡張用）
    /// </summary>
    private void EnemyCounterAttack()
    {
        Debug.Log("反撃処理開始");

        var alivePlayers = m_allPlayers.FindAll(p => p.m_currentHP > 0);

        if (alivePlayers.Count == 0 || m_selectedTarget == null)
        {
            Debug.Log("反撃対象がいない");
            m_battleSystem.EndPlayerTurn();
            return;
        }

        var randomPlayer = alivePlayers[Random.Range(0, alivePlayers.Count)];
        Debug.Log($"敵が {randomPlayer.m_data.m_characterName} に反撃！");
        m_selectedTarget.Attack(randomPlayer);
    }

    /// <summary>
    /// 全ての敵が撃破されているかチェックし、勝利処理を行う
    /// </summary>
    public void CheckAllEnemiesDefeated()
    {
        Enemy[] remainingEnemies = FindObjectsOfType<Enemy>();
        if (remainingEnemies.Length == 0)
        {
            m_battleSystem.EndBattle(true); // 勝利
        }
    }

    /// <summary>
    /// プレイヤーキャラのステータス更新（拡張用）
    /// </summary>
    private void UpdateAllPlayerStatus()
    {
        foreach (var pc in m_allPlayers)
        {
            pc.RefreshStatus();
        }
    }
}
