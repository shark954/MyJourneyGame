using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

/// <summary>
/// ゲームのエンディング演出を管理（成功/失敗共通）
/// ・フェードインで表示
/// ・一定時間後に「タイトルに戻る」ボタン表示
/// ・ボタンでタイトル画面に戻る
/// </summary>
public class EndingManager : MonoBehaviour
{
    [Header("UI参照")]
    public CanvasGroup m_endingPanel;         // エンディングパネル本体（CanvasGroupでフェード可能）
    public TMP_Text m_endingText;             // 表示するメッセージ
    public Button m_returnButton;             // タイトルへ戻るボタン
    public GameManager m_gameManager;

    [Header("フェード制御")]
    public FadeInOut m_fadeScript;          // 共通フェード処理スクリプト

    [Header("演出タイミング")]
    public float m_holdTime = 10f;            // メッセージの表示時間（秒）

    public AudioSource m_endBGM;

    public Image m_clearBackground;
    public Image m_gameOverBackground;

    private float m_timer = 0f;               // 経過時間
    private bool m_holding = false;           // 表示保持中フラグ

    /// <summary>
    /// 開始時の初期設定
    /// </summary>
    void Start()
    {
        m_returnButton.gameObject.SetActive(false); // 最初は戻るボタン非表示
        m_returnButton.onClick.AddListener(ReturnToTitle); // ボタンのクリックイベント登録
    }

    /// <summary>
    /// 毎フレーム処理：エンディング表示中の時間計測
    /// </summary>
    void Update()
    {
        if (m_holding)
        {
            m_timer += Time.deltaTime;
            if (m_timer >= m_holdTime)
            {
                m_holding = false;
                m_returnButton.gameObject.SetActive(true); // 指定秒数後にボタン表示
            }
        }
    }

    /// <summary>
    /// エンディング演出の開始（ゲームマネージャーから呼ばれる）
    /// </summary>
    /// <param name="message">表示するエンディングメッセージ</param>
    public void PlayEnding(string message, bool isClear)
    {

        // メッセージを設定してパネルを表示
        m_endingText.text = message;
        m_endingPanel.alpha = 1f;
        m_endingPanel.gameObject.SetActive(true);
        m_returnButton.gameObject.SetActive(false);

        // 画像切り替え
        m_clearBackground.gameObject.SetActive(isClear);
        m_gameOverBackground.gameObject.SetActive(!isClear);
        // フェードイン（黒→透明）
        m_fadeScript.StartFade(true, () =>
        {
            m_holding = true;
            m_timer = 0f;
        });
    }

    /// <summary>
    /// 「タイトルに戻る」ボタンの処理
    /// </summary>
    private void ReturnToTitle()
    {
        m_gameManager.m_resetFlag = true;

        // フェードアウト（透明→黒）→ パネル切り替え
        m_fadeScript.StartFade(false, () =>
        {
            m_gameManager.m_gameEnd = false;
            m_endingPanel.gameObject.SetActive(false);
            m_gameManager.m_gamePanel.SetActive(false);
            m_gameManager.m_titlePanel.SetActive(true);
            m_gameManager.m_resetFlag = false;

            // 画面が黒いままなので、再びフェードインして明るくする
            m_fadeScript.StartFade(true, null);
        });
    }
}
