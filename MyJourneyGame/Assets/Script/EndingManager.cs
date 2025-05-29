
using UnityEngine;
using TMPro;
using UnityEngine.Video;
using System.Collections;

/// <summary>
/// エンディング画面の表示とフェード演出（同一Scene構成）
/// </summary>
public class EndingManager : MonoBehaviour
{
    public CanvasGroup endingPanel;          // エンディングパネル（表示切り替え＋フェード）
    public TMP_Text endingText;              // エンディングメッセージ
    public VideoPlayer endingVideo;          // 背景動画
    public float fadeDuration = 1f;          // フェード時間

    /// <summary>
    /// ゲーム側から呼ばれてエンディングを開始する
    /// </summary>
    public void PlayEnding()
    {
        StartCoroutine(ShowEndingRoutine());
    }

    private IEnumerator ShowEndingRoutine()
    {
        endingPanel.alpha = 0;
        endingPanel.gameObject.SetActive(true);
        endingVideo.Play();
        endingText.text = "旅の向かう先へ…\nあなたはまた歩き出す。";

        // フェードイン
        yield return FadeCanvas(0, 1);
        yield return new WaitForSeconds(10f); // 表示保持時間
        // フェードアウト
        yield return FadeCanvas(1, 0);
        endingPanel.gameObject.SetActive(false);
    }

    private IEnumerator FadeCanvas(float from, float to)
    {
        float time = 0;
        while (time < fadeDuration)
        {
            endingPanel.alpha = Mathf.Lerp(from, to, time / fadeDuration);
            time += Time.deltaTime;
            yield return null;
        }
        endingPanel.alpha = to;
    }
}
