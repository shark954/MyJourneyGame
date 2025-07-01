using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UIのImageを点滅させる効果を与えるスクリプト。
/// 主にキャラクター選択時の枠（白い選択フレームなど）に使用される。
/// </summary>
public class BlinkEffect : MonoBehaviour
{
    public float m_speed = 2f;  // 点滅速度（この変数は現在未使用）

    private Image m_image;      // 対象のImageコンポーネント
    private Color m_baseColor = new Color(1, 1, 1, 1); // ベース色（色味は固定、アルファだけ変化）
    private bool m_initialized = false; // 初期化済みフラグ（現在未使用）

    public List<Image> m_TwoImage; // 予備：複数イメージ対応用（現在未使用）
    public bool m_Flag = false;    // 点滅ON/OFF制御用フラグ

    public float m_alpha;         // 現在のアルファ値（0～1）
    private bool m_fadeIn = false; // フェード方向フラグ（trueならフェードアウト）
    private float m_fadeSpeed = 1f; // フェード速度（1秒で切り替え）

    /// <summary>
    /// 起動時にImageコンポーネントを取得
    /// </summary>
    void Awake()
    {
        m_image = GetComponent<Image>();
    }

    /// <summary>
    /// 点滅のON/OFFを切り替える関数
    /// 引数 Flag が true なら点滅開始、false なら非表示
    /// </summary>
    public void SetRender(bool Flag)
    {
        m_Flag = Flag;

        // 点滅OFF時は透明にする
        Color newColor = m_baseColor;
        newColor.a = 0.0f;
        m_image.color = newColor;
    }

    /// <summary>
    /// 毎フレーム実行：アルファ値を変化させて点滅効果を表現
    /// </summary>
    void Update()
    {
        Color newColor = m_baseColor;

        if (m_Flag)
        {
            DoFade();            // アルファ値の増減計算
            newColor.a = m_alpha;
        }
        else
        {
            newColor.a = 0.0f;   // OFF時は完全透明
        }

        m_image.color = newColor;
    }

    /// <summary>
    /// アルファ値を0〜1の範囲で増減させる（フェードイン・アウト）
    /// </summary>
    public void DoFade()
    {
        if (m_fadeIn)
        {
            m_alpha -= Time.deltaTime * m_fadeSpeed;
            if (m_alpha <= 0f)
            {
                m_alpha = 0f;
                m_fadeIn = false;
            }
        }
        else
        {
            m_alpha += Time.deltaTime * m_fadeSpeed;
            if (m_alpha >= 1f)
            {
                m_alpha = 1f;
                m_fadeIn = true;
            }
        }
    }
}
