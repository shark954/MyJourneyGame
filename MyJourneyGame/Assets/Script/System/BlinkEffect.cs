using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UIのImageをアルファ値で点滅させる効果を付与するスクリプト。
/// 主に選択中のキャラ枠などに使用される。
/// </summary>
public class BlinkEffect : MonoBehaviour
{
    public float m_speed = 2f;  // 点滅の速さ（数値が大きいと速く点滅する）

    private Image m_image;        // 対象のImageコンポーネント
    private Color m_baseColor = new Color(1, 1, 1,1);    // 元のカラー情報（色味を保持してαのみ変更）
    private bool m_initialized = false; // 初期化済みかどうかのフラグ

    public List<Image> m_TwoImage;
    public bool m_Flag = false;

    public float m_alpha;
    private bool m_fadeIn = false;
    private float m_fadeSpeed = 1f;

    /// <summary>
    /// コンポーネント初期化時にImageを取得
    /// </summary>
    void Awake()
    {
        m_image = GetComponent<Image>();
    }
    public void SetRender(bool Flag)
    {
        m_Flag = Flag;
        Color newColor = m_baseColor;
        newColor.a = 0.0f;
        m_image.color = newColor;
    }

    /// <summary>
    /// 毎フレーム、アルファ値を0〜1で変化させて点滅させる
    /// </summary>
    void Update()
    {
        // 無効・未初期化・透明状態の場合は点滅処理をスキップ
        //if (!enabled || m_image == null || m_image.color.a == 0f) return;

        Color newColor = m_baseColor;
        if (m_Flag)
        {
            DoFade();
            // α値を0〜1の間で繰り返す
            float alpha = m_alpha;

            // 元の色にαだけ反映
            newColor.a = alpha;
        }
        else
        {
            // 元の色にαだけ反映
            newColor.a = 0.0f;
        }
        m_image.color = newColor;
    }

    public void DoFade()
    {
        if (m_fadeIn)
        {
            m_alpha -= Time.deltaTime * m_fadeSpeed;
            if (m_alpha <= 0f)
            {
                m_alpha = 0f;
                m_fadeIn = !m_fadeIn;
            }
        }
        else
        {
            m_alpha += Time.deltaTime * m_fadeSpeed;
            if (m_alpha >= 1f)
            {
                m_alpha = 1f;
                m_fadeIn = !m_fadeIn;
            }
        }
    }
}
