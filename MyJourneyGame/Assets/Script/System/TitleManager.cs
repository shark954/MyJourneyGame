using UnityEngine;

/// <summary>
/// タイトル画面の演出と開始操作を管理（FadeInOut スクリプト使用版）
/// </summary>
public class TitleManager : MonoBehaviour
{
    public CanvasGroup m_titlePanel;         // タイトル用パネル（UI制御）
    public FadeInOut m_fadeScript;           // フェードスクリプト（Imageにアタッチ）
    public GameManager m_gameManager;        // ゲームマネージャー 
    public AudioSource m_titleBGM;

    private bool m_gameStarted = false;      // 遷移完了フラグ
    private float m_waitTime = 1f;           // フェードアウト待ち時間
    private float m_timer = 0f;

    void Update()
    {
        if (m_gameStarted)
        {
            m_timer += Time.deltaTime;

            if (m_timer >= m_waitTime)
            {
                m_titlePanel.gameObject.SetActive(false);
                //m_gamePanel.SetActive(true);
                
            }
        }
    }

    /// <summary>
    /// スタートボタンから呼び出される（フェード開始）
    /// </summary>
    public void OnStartButtonPressed()
    {
        m_fadeScript.StartFade(false, () =>
        {
            // フェードアウト完了後にゲーム開始
            m_titlePanel.gameObject.SetActive(false);
            m_gameManager.m_gamePanel.SetActive(true);

            m_gameManager.m_storyPanel.SetActive(true);
            m_gameManager.m_adventureSystem.enabled = true;
           

            // フェードインで黒を消す
            m_fadeScript.StartFade(true);
            enabled = false; // TitleManagerを無効化
        });
    }
}
