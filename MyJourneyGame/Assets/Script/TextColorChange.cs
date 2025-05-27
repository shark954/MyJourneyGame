using System.Collections;
using UnityEngine;
using TMPro;

public class TextColorChange : MonoBehaviour
{
    [Header("テキスト対象")]
    public TextMeshProUGUI textMesh; // 操作対象の TextMeshProUGUI コンポーネント

    [Header("演出設定")]
    public float fadeSpeed = 2f;         // フェードの進行速度（大きいほど速い）
    public float delayPerChar = 0.1f;    // 各文字の処理間隔（左から順に適用される）
    public Gradient gradient;            // 文字に適用する色のグラデーション

    [Header("フェード方向")]
    public bool fadeIn = true;           // true: フェードイン（透明→表示）/ false: フェードアウト（表示→透明）

    // 定数：カラーやアルファの最大値
    private const float maxAlpha01 = 1f;
    private const byte maxAlphaByte = 255;

    // TextMeshPro のメッシュ情報格納用
    private TMP_TextInfo textInfo;

    void Start()
    {
        // テキストを強制的にメッシュ更新
        textMesh.ForceMeshUpdate();

        // テキストの頂点・文字情報を取得
        textInfo = textMesh.textInfo;

        // コルーチンでアニメーション開始
        StartCoroutine(FadeWithColor());
    }

    IEnumerator FadeWithColor()
    {
        int charCount = textInfo.characterCount;

        // 初期設定：すべての文字のアルファ値を 0（透明）または 255（不透明）にする
        for (int meshIdx = 0; meshIdx < textInfo.meshInfo.Length; meshIdx++)
        {
            Color32[] colors = textInfo.meshInfo[meshIdx].colors32;

            for (int i = 0; i < colors.Length; i++)
            {
                colors[i].a = fadeIn ? (byte)0 : maxAlphaByte;
            }
        }
        textMesh.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

        // 文字ごとに処理を実行（左から右へ）
        for (int i = 0; i < charCount; i++)
        {
            // 非表示の文字（スペースなど）はスキップ
            if (!textInfo.characterInfo[i].isVisible)
                continue;

            // 対象の文字のメッシュ・頂点インデックスを取得
            int meshIndex = textInfo.characterInfo[i].materialReferenceIndex;
            int vertexIndex = textInfo.characterInfo[i].vertexIndex;
            Color32[] vertexColors = textInfo.meshInfo[meshIndex].colors32;

            // グラデーション色を文字位置に応じて取得
            Color targetColor = gradient.Evaluate((float)i / charCount); // Color32 → Color に変更

            // RGB を byte に変換（明示的に変換）
            byte r = (byte)(targetColor.r * maxAlphaByte);
            byte g = (byte)(targetColor.g * maxAlphaByte);
            byte b = (byte)(targetColor.b * maxAlphaByte);


            float fadeProgress = 0f;

            // 徐々にアルファを変化させて表示 or 消去
            while (fadeProgress < maxAlpha01)
            {
                fadeProgress += Time.deltaTime * fadeSpeed;

                // フェード方向に応じてアルファを補間（0 → 255 または 255 → 0）
                float alphaRatio = Mathf.Clamp01(fadeProgress);
                byte alpha = (byte)((fadeIn ? alphaRatio : 1f - alphaRatio) * maxAlphaByte);

                // 文字の4頂点すべてに色を適用
                for (int j = 0; j < 4; j++)
                {
                    vertexColors[vertexIndex + j] = new Color32(r, g, b, alpha);
                }

                textMesh.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
                yield return null; // 次のフレームまで待つ
            }

            // 次の文字へ進むまで待機
            yield return new WaitForSeconds(delayPerChar);
        }
    }
}
