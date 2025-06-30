using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// バトル時のUI制御を担当するマネージャー
/// ・プレイヤーキャラの選択
/// ・敵をクリックした時の処理
/// ・攻撃やスキル選択の表示・非表示
/// ・撤退処理など
/// </summary>
public class BattleUIManager : MonoBehaviour
{
    public static BattleUIManager m_Instance; // シングルトン（他クラスからアクセス用）
    public List<Enemy> m_enemies;
    public List<PlayerCharacter> m_players;

    private int m_currentTurn = 0;

    private PlayerCharacter m_currentPlayer; // 現在操作中のプレイヤーキャラ
    private PlayerCharacter m_attacker;      // 実際に攻撃するプレイヤー
    private Enemy m_selectedTarget;          // 選択された敵

    public GameObject m_attackChoicePanel;   // 「通常攻撃」「スキル」ボタンを含むパネル
    public List<PlayerCharacter> m_allPlayers; // 全プレイヤーキャラ一覧（選択判定用）
    public BattleSystem m_battleSystem;

    /// <summary>
    /// 初期化：攻撃コマンドパネルを非表示にする
    /// </summary>
    private void Awake()
    {
        m_Instance = this;
        m_attackChoicePanel.SetActive(false); // ゲーム開始時は非表示
    }

    /// <summary>
    /// 敵をクリックしたときに呼び出され、攻撃選択パネルを表示する
    /// EndBattleのboolは勝ちがtrueだからEnemyの死亡フラグを反転させる
    /// </summary>
    public void OnEnemyClicked(Enemy target)
    {
        if (target.m_deathflag)
        {
            m_battleSystem.EndBattle(!target.m_deathflag);
        }

        if (!target.m_deathflag)
        {
            m_selectedTarget = target;
            m_attackChoicePanel.SetActive(true); // 通常／スキル選択パネル表示
        }
        
    }

    /// <summary>
    /// プレイヤーキャラをクリックして選択したときの処理
    /// </summary>
    public void OnPlayerCharacterSelected(PlayerCharacter player)
    {
        if (!FindObjectOfType<Enemy>())
        {
            m_battleSystem.EndBattle(true);
        }

        if (player.m_currentHP <= 0)
        {
            Debug.Log("このキャラは行動不能です");
            return;
        }

        foreach (var pc in m_allPlayers)
        {
            bool isSelected = (pc == player);
            pc.SetSelected(isSelected);
        }

        SetCurrentAttacker(player);
    }

    /// <summary>
    /// 撤退ボタンが押されたときの処理
    /// </summary>
    /*public void OnRetreatButtonPressed()
    {
        // BattleSystem の EndBattle を呼び出して終了処理
         m_battleSystem.EndBattle();
    }*/

    /// <summary>
    /// 「通常攻撃」ボタンが押されたときの処理
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

         m_battleSystem.EndPlayerTurn(); // ←ここで敵ターンへ
    }

    /// <summary>
    /// 「スキル」ボタンが押されたときの処理
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

         m_battleSystem.EndPlayerTurn(); // ←ここで敵ターンへ
    }


    /// <summary>
    /// 現在の行動キャラ（プレイヤー）を指定する
    /// </summary>
    public void SetCurrentAttacker(PlayerCharacter pc)
    {
        m_attacker = pc;
    }



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
    public void CheckAllEnemiesDefeated()
    {
        Enemy[] remainingEnemies = FindObjectsOfType<Enemy>();
        if (remainingEnemies.Length == 0)
        {
            m_battleSystem.EndBattle(true); // 勝利判定
        }
    }

    private void UpdateAllPlayerStatus()
    {
        foreach (var pc in m_allPlayers)
        {
            pc.RefreshStatus();
        }
    }
}



