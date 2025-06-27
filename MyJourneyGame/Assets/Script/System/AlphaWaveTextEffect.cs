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
    public TextMeshProUGUI m_textMesh; // 操作対象の TextMeshProUGUI

    [Header("色とフェードの設定,文字に適用するグラデーション")]
    public Gradient m_gradient;
    [Header("揺れの速さ（時間あたりの位相増加量）")]
    public float m_waveSpeed = 1f;
    [Header("アルファの揺れ幅（0〜1）")]
    public float m_alphaAmplitude = 0.5f;
    [Header("最小アルファ（揺れの中心値）")]
    public float m_baseAlpha = 0.5f;         


    [Header("揺れの位相差,各文字ごとの波の遅れ（位相差）")]
    public float m_phaseOffsetPerChar = 0.5f; // 

    // 内部定数
    private const byte m_MaxAlphaByte = 255;  // アルファの最大値
    private TMP_TextInfo m_textInfo;

    void Start()
    {
        // TextMeshPro の頂点情報を更新・取得
        m_textMesh.ForceMeshUpdate();
        m_textInfo = m_textMesh.textInfo;

        // フェードループ処理開始
        StartCoroutine(WaveEffectLoop());
    }

    /// <summary>
    /// アルファ値を波状に変化させて揺らめくようなループ処理
    /// </summary>
    IEnumerator WaveEffectLoop()
    {
        int charCount = m_textInfo.characterCount;

        while (true)
        {
            float globalTime = Time.time * m_waveSpeed; // 全体の経過時間（スピード調整）

            for (int i = 0; i < charCount; i++)
            {
                if (!m_textInfo.characterInfo[i].isVisible)
                    continue;

                int meshIndex = m_textInfo.characterInfo[i].materialReferenceIndex;
                int vertexIndex = m_textInfo.characterInfo[i].vertexIndex;
                Color32[] vertexColors = m_textInfo.meshInfo[meshIndex].colors32;

                // グラデーション色の取得（文字位置に応じて）
                Color gradientColor = m_gradient.Evaluate((float)i / charCount);

                // サイン波でアルファ値を揺らす（位相差を追加）
                float wave = Mathf.Sin(globalTime + i * m_phaseOffsetPerChar);
                float alphaRatio = Mathf.Clamp01(wave * m_alphaAmplitude + m_baseAlpha);
                byte alpha = (byte)(alphaRatio * m_MaxAlphaByte);

                // RGB 値を byte に変換
                byte r = (byte)(gradientColor.r * m_MaxAlphaByte);
                byte g = (byte)(gradientColor.g * m_MaxAlphaByte);
                byte b = (byte)(gradientColor.b * m_MaxAlphaByte);

                // 1文字（4頂点）に色とアルファを適用
                for (int j = 0; j < 4; j++)
                {
                    vertexColors[vertexIndex + j] = new Color32(r, g, b, alpha);
                }
            }

            // メッシュに色変更を反映
            m_textMesh.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
            yield return null; // 次のフレームまで待機
        }
    }
}
