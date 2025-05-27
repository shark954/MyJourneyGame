using System.Collections;
using UnityEngine;
using TMPro;

/// <summary>
/// TextMeshPro の文字に対して、グラデーションカラーと揺らめくような
/// アルファフェードをループでかける演出スクリプト。
/// </summary>
public class AlphaWaveTextEffect : MonoBehaviour
{
    [Header("テキスト対象")]
    public TextMeshProUGUI textMesh; // 操作対象の TextMeshProUGUI

    [Header("色とフェードの設定,文字に適用するグラデーション")]
    public Gradient gradient;
    [Header("揺れの速さ（時間あたりの位相増加量）")]
    public float waveSpeed = 1f;
    [Header("アルファの揺れ幅（0〜1）")]
    public float alphaAmplitude = 0.5f;
    [Header("最小アルファ（揺れの中心値）")]
    public float baseAlpha = 0.5f;         


    [Header("揺れの位相差,各文字ごとの波の遅れ（位相差）")]
    public float phaseOffsetPerChar = 0.5f; // 

    // 内部定数
    private const byte MaxAlphaByte = 255;  // アルファの最大値
    private TMP_TextInfo textInfo;

    void Start()
    {
        // TextMeshPro の頂点情報を更新・取得
        textMesh.ForceMeshUpdate();
        textInfo = textMesh.textInfo;

        // フェードループ処理開始
        StartCoroutine(WaveEffectLoop());
    }

    /// <summary>
    /// アルファ値を波状に変化させて揺らめくようなループ処理
    /// </summary>
    IEnumerator WaveEffectLoop()
    {
        int charCount = textInfo.characterCount;

        while (true)
        {
            float globalTime = Time.time * waveSpeed; // 全体の経過時間（スピード調整）

            for (int i = 0; i < charCount; i++)
            {
                if (!textInfo.characterInfo[i].isVisible)
                    continue;

                int meshIndex = textInfo.characterInfo[i].materialReferenceIndex;
                int vertexIndex = textInfo.characterInfo[i].vertexIndex;
                Color32[] vertexColors = textInfo.meshInfo[meshIndex].colors32;

                // グラデーション色の取得（文字位置に応じて）
                Color gradientColor = gradient.Evaluate((float)i / charCount);

                // サイン波でアルファ値を揺らす（位相差を追加）
                float wave = Mathf.Sin(globalTime + i * phaseOffsetPerChar);
                float alphaRatio = Mathf.Clamp01(wave * alphaAmplitude + baseAlpha);
                byte alpha = (byte)(alphaRatio * MaxAlphaByte);

                // RGB 値を byte に変換
                byte r = (byte)(gradientColor.r * MaxAlphaByte);
                byte g = (byte)(gradientColor.g * MaxAlphaByte);
                byte b = (byte)(gradientColor.b * MaxAlphaByte);

                // 1文字（4頂点）に色とアルファを適用
                for (int j = 0; j < 4; j++)
                {
                    vertexColors[vertexIndex + j] = new Color32(r, g, b, alpha);
                }
            }

            // メッシュに色変更を反映
            textMesh.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
            yield return null; // 次のフレームまで待機
        }
    }
}
