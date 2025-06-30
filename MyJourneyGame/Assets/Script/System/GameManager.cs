using UnityEngine;
using UnityEngine.UI;
using System;

/// <summary>
/// ゲーム全体の進行管理クラス。ストーリー表示・選択肢制御・戦闘管理などを含む。
/// </summary>
public class GameManager : MonoBehaviour
{
    // === UI参照 ===

    public TitleManager m_titleManager; // Inspectorで接続
    public BattleSystem m_battleSystem;
    public EndingManager m_endingManager; // Inspectorで接続

    [Header("タイトル画面")]
    public GameObject m_titlePanel;
    [Header("エンディング画面")]
    public GameObject m_endingPanel;
    [Header("ゲーム画面")]
    public GameObject m_gamePanel;
    [Header("ストーリー画面")]
    public GameObject m_storyPanel;
    [Header("戦闘用UI")]
    public GameObject m_battlePanel; // 戦闘パネル全体（切替用）
    [Header("テキストシステム")]
    public TextAdventureSystem m_adventureSystem;

    [Header("true=ゲーム終了")]
    public bool m_gameEnd = false;

    public bool m_resetFlag = false;
 
    /// <summary>
    /// ゲーム開始時に最初のシーンを表示
    /// </summary>
    void Awake()
    {
        m_titlePanel.SetActive(true);
        m_endingPanel.SetActive(false);
        m_gamePanel.SetActive(false);
        m_battlePanel.SetActive(false);
        m_storyPanel.SetActive(false);
        m_adventureSystem.enabled = false;
    }

    private void Update()
    {
        if (m_gameEnd)
        {

            m_titlePanel.SetActive(false);
            m_endingPanel.SetActive(true);
            m_gamePanel.SetActive(false);
            m_battlePanel.SetActive(false);
            m_storyPanel.SetActive(false);
        }
    }


}