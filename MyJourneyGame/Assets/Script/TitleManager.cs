
using UnityEngine;
using TMPro;
using UnityEngine.Video;
using System.Collections;

/// <summary>
/// タイトル画面の演出と開始操作を管理（同一Scene構成）
/// </summary>
public class TitleManager : MonoBehaviour
{
    public CanvasGroup titlePanel;           // タイトル用パネル（フェードに使用）
    public TMP_Text titleText;               // タイトルの文字
    public VideoPlayer titleVideoPlayer;     // 背景動画
    public float fadeDuration = 1f;          // フェード時間

    public GameObject gamePanel;             // 本編パネル（切り替え用）

    void Start()
    {
        titleText.text = "旅の向かう先";
        titleVideoPlayer.Play();
        titlePanel.alpha = 1;
    }

    /// <summary>
    /// スタートボタンを押したときに呼ばれる
    /// </summary>
    public void OnStartButtonPressed()
    {
        StartCoroutine(FadeOutAndStartGame());
    }

    private IEnumerator FadeOutAndStartGame()
    {
        float time = 0;
        while (time < fadeDuration)
        {
            titlePanel.alpha = Mathf.Lerp(1, 0, time / fadeDuration);
            time += Time.deltaTime;
            yield return null;
        }
        titlePanel.alpha = 0;
        titlePanel.gameObject.SetActive(false);
        gamePanel.SetActive(true); // 本編UI表示
    }
}
