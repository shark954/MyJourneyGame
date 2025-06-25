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

    private PlayerCharacter m_currentPlayer; // 現在操作中のプレイヤーキャラ
    private PlayerCharacter m_attacker;      // 実際に攻撃するプレイヤー
    private Enemy m_selectedTarget;          // 選択された敵

    public GameObject m_attackChoicePanel;   // 「通常攻撃」「スキル」ボタンを含むパネル
    public List<PlayerCharacter> m_allPlayers; // 全プレイヤーキャラ一覧（選択判定用）

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
    /// </summary>
    public void OnEnemyClicked(Enemy target)
    {
        m_selectedTarget = target;
        m_attackChoicePanel.SetActive(true); // 通常／スキル選択パネル表示
    }

    /// <summary>
    /// プレイヤーキャラをクリックして選択したときの処理
    /// </summary>
    public void OnPlayerCharacterSelected(PlayerCharacter player)
    {
        foreach (var pc in m_allPlayers)
        {
            bool isSelected = (pc == player);
            pc.SetSelected(isSelected); // 選択キャラだけ点滅＋アイコン変更
        }

        SetCurrentAttacker(player); // クリックされたキャラを攻撃者に設定
    }

    /// <summary>
    /// 撤退ボタンが押されたときの処理
    /// </summary>
    public void OnRetreatButtonPressed()
    {
        // BattleSystem の EndBattle を呼び出して終了処理
        FindObjectOfType<BattleSystem>().EndBattle();
    }

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
    }

    /// <summary>
    /// 現在の行動キャラ（プレイヤー）を指定する
    /// </summary>
    public void SetCurrentAttacker(PlayerCharacter pc)
    {
        m_attacker = pc;
    }

}
