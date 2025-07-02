using UnityEngine;
using UnityEngine.UI;
using System;

/// <summary>
/// ゲーム全体の進行管理クラス。
/// タイトル画面 → ストーリー → 戦闘 → エンディング の流れを制御。
/// 画面表示の切り替えやフラグ管理を行う。
/// </summary>
public class GameManager : MonoBehaviour
{
    // === 各種サブマネージャー（Inspectorから割り当て） ===
    public TitleManager m_titleManager;
    public BattleSystem m_battleSystem;
    public EndingManager m_endingManager;

    // === UI オブジェクト ===
    [Header("タイトル画面")]
    public GameObject m_titlePanel;

    [Header("エンディング画面")]
    public GameObject m_endingPanel;

    [Header("ゲーム（メイン）画面")]
    public GameObject m_gamePanel;

    [Header("ストーリー画面")]
    public GameObject m_storyPanel;

    [Header("戦闘用UI")]
    public GameObject m_battlePanel; // 戦闘全体を囲むUIパネル

    [Header("テキストシステム（ストーリー管理）")]
    public TextAdventureSystem m_adventureSystem;

    [Header("ゲーム進行制御")]
    public bool m_gameEnd = false;      // trueならエンディングへ
    public bool m_resetFlag = false;    // 再開・リトライ処理用（未使用なら削除可）

    /// <summary>
    /// 起動時に各画面の初期状態を設定。
    /// 最初はタイトルのみ表示し、他は非表示にする。
    /// </summary>
    void Awake()
    {
        m_titlePanel.SetActive(true);         // タイトル画面 ON
        m_endingPanel.SetActive(false);       // エンディング OFF
        m_gamePanel.SetActive(false);         // ゲーム画面 OFF
        m_battlePanel.SetActive(false);       // バトルUI OFF
        m_storyPanel.SetActive(false);        // ストーリーUI OFF
        m_adventureSystem.enabled = false;    // ストーリー進行のスクリプトも停止
    }

    /// <summary>
    /// フレームごとにゲーム状態を確認し、エンディング条件なら切り替える
    /// </summary>
    private void Update()
    {
        if (m_gameEnd)
        {
            // エンディングフラグが立っていたら各UIを切り替える
            m_titlePanel.SetActive(false);
            m_endingPanel.SetActive(true);
            m_gamePanel.SetActive(false);
            m_battlePanel.SetActive(false);
            m_storyPanel.SetActive(false);
        }

        if (m_resetFlag)
        {
            m_adventureSystem.ResetScenario();  // シナリオを再ロード
            m_resetFlag = false;                // フラグを戻す
        }

    }
}
