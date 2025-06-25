using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeInOut : MonoBehaviour
{
    [Header("アルファ値、色はスクリプトで設定")]
    public float m_alpha;

    [Header("フェード用のImage")]
    public Image m_fade;


    private bool m_fadeIn = false;
    private float m_fadeSpeed = 1f;


    [Header("true:フェードイン,false:フェードアウト")]
    public bool m_InOut;

    private bool isFading = false; // フェード中か

    private Action onFadeComplete; // フェードが終わったら呼び出す
    // Start is called before the first frame update
    void Start()
    {
        m_alpha = 0f;
        ApplyAlpha();

    }

    // Update is called once per frame
    void Update()
    {
        if (isFading)
        {
            DoFade();
        }
    }

    public void StartFade(bool fadeInFlag, Action onComplete = null)
    {
        m_fadeIn = fadeInFlag;
        isFading = true;
        onFadeComplete = onComplete;
    }

    private void DoFade()
    {
        if (m_fadeIn)
        {
            m_alpha -= Time.deltaTime * m_fadeSpeed;
            if (m_alpha <= 0f)
            {
                m_alpha = 0f;
                EndFade();
            }
        }
        else
        {
            m_alpha += Time.deltaTime * m_fadeSpeed;
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