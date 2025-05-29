using System.Collections;
using UnityEngine;
using TMPro;

/// <summary>
/// TextMeshPro のテキストに炎のような揺らぎを付けるエフェクト。
/// アルファ（透明度）と位置をノイズベースで変化させ、リアルタイムで更新する。
/// </summary>
public class FlameTextEffect : MonoBehaviour
{
    [Header("テキスト対象")]
    public TextMeshProUGUI textMesh; // 操作対象の TextMeshPro テキスト

    [Header("炎の色と動き")]
    public Gradient flameGradient;          // 炎の色（赤〜黄など）グラデーション
    public float noiseSpeed = 2f;           // ノイズ進行速度（時間に対する動きの速さ）
    public float alphaAmplitude = 0.3f;     // アルファの変動幅（ちらつきの強さ）
    public float baseAlpha = 0.7f;          // 基本の透明度（炎の濃さ）
    public float verticalShift = 2f;        // 文字の上下揺れ幅（炎の高さ）

    [Header("ノイズの設定")]
    public float noiseOffsetScale = 0.3f;   // 文字ごとのノイズ座標ズレ（炎の個別ゆらぎ）

    // 内部定数
    private const byte MaxAlphaByte = 255;

    // テキストメッシュ情報格納用
    private TMP_TextInfo textInfo;

    void Start()
    {
        // テキストのメッシュ情報を強制更新し、情報取得
        textMesh.ForceMeshUpdate();
        textInfo = textMesh.textInfo;

        // エフェクトループ開始
        StartCoroutine(FlameLoop());
    }

    /// <summary>
    /// 各フレームで文字の色・アルファ・位置を更新し、炎のようなゆらぎを演出。
    /// </summary>
    IEnumerator FlameLoop()
    {
        int charCount = textInfo.characterCount;

        while (true)
        {
            // 時間による進行（ノイズの時間軸）
            float timeSeed = Time.time * noiseSpeed;

            for (int i = 0; i < charCount; i++)
            {
                if (!textInfo.characterInfo[i].isVisible)
                    continue;

                // 頂点・色のインデックスを取得
                int meshIndex = textInfo.characterInfo[i].materialReferenceIndex;
                int vertexIndex = textInfo.characterInfo[i].vertexIndex;
                Vector3[] vertices = textInfo.meshInfo[meshIndex].vertices;
                Color32[] colors = textInfo.meshInfo[meshIndex].colors32;

                // ノイズによるゆらぎ生成（0.0〜1.0）
                float noiseValue = Mathf.PerlinNoise(i * noiseOffsetScale, timeSeed);

                // アルファ計算（ベース値＋ノイズのゆらぎ）
                float alphaRatio = Mathf.Clamp01(baseAlpha + (noiseValue - 0.5f) * alphaAmplitude * 2f);
                byte alpha = (byte)(alphaRatio * MaxAlphaByte);

                // グラデーション色をノイズ値で選ぶ
                Color color = flameGradient.Evaluate(noiseValue);
                byte r = (byte)(color.r * MaxAlphaByte);
                byte g = (byte)(color.g * MaxAlphaByte);
                byte b = (byte)(color.b * MaxAlphaByte);

                // 縦方向の揺れ（炎が立ち上る印象を出す）
                float yOffset = (noiseValue - 0.5f) * verticalShift;

                // 4頂点すべてに色と位置を適用
                for (int j = 0; j < 4; j++)
                {
                    vertices[vertexIndex + j].y += yOffset;
                    colors[vertexIndex + j] = new Color32(r, g, b, alpha);
                }
            }

            // メッシュに反映（位置＋カラー両方）
            textMesh.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32 | TMP_VertexDataUpdateFlags.Vertices);

            yield return null; // 毎フレーム更新
        }
    }
}
