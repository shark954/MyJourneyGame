using System.Collections.Generic;
using UnityEngine;

public class SpriteChange : MonoBehaviour
{
    /// <summary>画像を変更するスプライトオブジェクト。</summary>
    [SerializeField] private GameObject m_TargetSprite;

    /// <summary>変更後の画像を持つスプライト。</summary>
    [SerializeField] private Sprite NextSprite;

    [Header("画像プール"),SerializeField]
    private List<Sprite> m_spriteList;

    /// <summary>ボタンをクリックしたときに呼ばれます。</summary>
    public void OnClick()
    {
        if (m_TargetSprite == null)
        {
            Debug.Log($"{nameof(m_TargetSprite)} が null です。");
            return;
        }
        if (NextSprite == null)
        {
            Debug.Log($"{nameof(NextSprite)} が null です。");
            return;
        }

        // 変更対象のオブジェクトが持つ SpriteRenderer を取得
        var spriteRenderer = m_TargetSprite.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.Log($"{nameof(m_TargetSprite)} に {nameof(SpriteRenderer)} コンポーネントがありません。");
            return;
        }

        // SpriteRenderer の sprite に変更後のスプライトをセット
        spriteRenderer.sprite = NextSprite;
    }

    public void Changer(bool flag)
    {
        if (flag)
        {
            //spriteの入れ替え

            if (m_TargetSprite == null)
            {
                Debug.Log($"{nameof(m_TargetSprite)} が null です。");
                return;
            }
            if (NextSprite == null)
            {
                Debug.Log($"{nameof(NextSprite)} が null です。");
                return;
            }

            // 変更対象のオブジェクトが持つ SpriteRenderer を取得
            var spriteRenderer = m_TargetSprite.GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                Debug.Log($"{nameof(m_TargetSprite)} に {nameof(SpriteRenderer)} コンポーネントがありません。");
                return;
            }

            // SpriteRenderer の sprite に変更後のスプライトをセット
            spriteRenderer.sprite = NextSprite;

        }
        else
        {
            return;
        }
    }
}