using System;
using UnityEngine;
using UnityEngine.UI;

public class Fade2 : MonoBehaviour
{
    [Header("フェード用のImage")]
    public Image m_fade;

    private float m_alpha = 0f;
    private bool isFading = false;
    private bool fadeIn = false;
    private float fadeSpeed = 1f;

    private Action onFadeComplete; // フェード完了時に呼ぶ処理

    void Start()
    {
        m_alpha = 0f;
        ApplyAlpha();
    }

    void Update()
    {
        if (isFading)
        {
            DoFade();
        }
    }

    public void StartFade(bool fadeInFlag, Action onComplete = null)
    {
        fadeIn = fadeInFlag;
        isFading = true;
        onFadeComplete = onComplete;
    }

    private void DoFade()
    {
        if (fadeIn)
        {
            m_alpha -= Time.deltaTime * fadeSpeed;
            if (m_alpha <= 0f)
            {
                m_alpha = 0f;
                EndFade();
            }
        }
        else
        {
            m_alpha += Time.deltaTime * fadeSpeed;
            if (m_alpha >= 1f)
            {
                m_alpha = 1f;
                EndFade();
            }
        }

        ApplyAlpha();
    }

    private void ApplyAlpha()
    {
        if (m_fade != null)
            m_fade.color = new Color(0f, 0f, 0f, m_alpha);
    }

    private void EndFade()
    {
        isFading = false;
        onFadeComplete?.Invoke();
    }
}
