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
    private Color m_baseColor;    // 元のカラー情報（色味を保持してαのみ変更）
    private bool m_initialized = false; // 初期化済みかどうかのフラグ

    public List<Image> m_TwoImage;
    public bool m_Flag = false;

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
    /// 有効化時に元の色を保存（αを最大にして点滅準備）
    /// </summary>
    void OnEnable()
    {
        /*
        if (m_image != null)
        {
            m_baseColor = m_image.color;
            m_baseColor.a = 1f;
            m_initialized = true;
        }
        */
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
            // α値を0〜1の間で繰り返す
            float alpha = (Mathf.Sin(Time.time * m_speed) + 1f) / 2f;

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

    /// <summary>
    /// 無効化されたときにαを最大に戻す（次回有効化時の見た目リセット用）
    /// </summary>
    void OnDisable()
    {
        /*
        if (m_image != null)
        {
            Color reset = m_image.color;
            reset.a = 1f;
            m_image.color = reset;
        }
        */
    }
}
