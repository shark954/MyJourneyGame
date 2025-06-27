using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// マウスカーソルのホバーでImageの透明度を変える処理
/// 初期状態は透明、カーソルが乗ると表示、離れるとまた透明になる
/// </summary>
public class FadeOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Image m_image;  // 操作対象のImage

    // 初期化処理（Imageコンポーネント取得）
    void Awake()
    {
        m_image = GetComponent<Image>();
        SetAlpha(0f); // 初期状態は完全に透明
    }

    /// <summary>
    /// マウスカーソルがUIに乗ったとき呼ばれる（IPointerEnterHandler）
    /// </summary>
    public void OnPointerEnter(PointerEventData eventData)
    {
        SetAlpha(1f); // 不透明に（画像が見えるように）
    }

    /// <summary>
    /// マウスカーソルがUIから離れたとき呼ばれる（IPointerExitHandler）
    /// </summary>
    public void OnPointerExit(PointerEventData eventData)
    {
        SetAlpha(0f); // 再び透明に戻す
    }

    /// <summary>
    /// Imageのアルファ値（透明度）を設定する
    /// </summary>
    void SetAlpha(float alpha)
    {
        if (m_image == null) return;
        Color color = m_image.color;
        color.a = alpha;
        m_image.color = color;
    }
}
