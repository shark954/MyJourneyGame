using System.Collections.Generic;
using UnityEngine;

public class SpriteChange : MonoBehaviour
{
    /// <summary>�摜��ύX����X�v���C�g�I�u�W�F�N�g�B</summary>
    [SerializeField] private GameObject m_TargetSprite;

    /// <summary>�ύX��̉摜�����X�v���C�g�B</summary>
    [SerializeField] private Sprite NextSprite;

    [Header("�摜�v�[��"),SerializeField]
    private List<Sprite> m_spriteList;

    /// <summary>�{�^�����N���b�N�����Ƃ��ɌĂ΂�܂��B</summary>
    public void OnClick()
    {
        if (m_TargetSprite == null)
        {
            Debug.Log($"{nameof(m_TargetSprite)} �� null �ł��B");
            return;
        }
        if (NextSprite == null)
        {
            Debug.Log($"{nameof(NextSprite)} �� null �ł��B");
            return;
        }

        // �ύX�Ώۂ̃I�u�W�F�N�g������ SpriteRenderer ���擾
        var spriteRenderer = m_TargetSprite.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.Log($"{nameof(m_TargetSprite)} �� {nameof(SpriteRenderer)} �R���|�[�l���g������܂���B");
            return;
        }

        // SpriteRenderer �� sprite �ɕύX��̃X�v���C�g���Z�b�g
        spriteRenderer.sprite = NextSprite;
    }

    public void Changer(bool flag)
    {
        if (flag)
        {
            //sprite�̓���ւ�

            if (m_TargetSprite == null)
            {
                Debug.Log($"{nameof(m_TargetSprite)} �� null �ł��B");
                return;
            }
            if (NextSprite == null)
            {
                Debug.Log($"{nameof(NextSprite)} �� null �ł��B");
                return;
            }

            // �ύX�Ώۂ̃I�u�W�F�N�g������ SpriteRenderer ���擾
            var spriteRenderer = m_TargetSprite.GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                Debug.Log($"{nameof(m_TargetSprite)} �� {nameof(SpriteRenderer)} �R���|�[�l���g������܂���B");
                return;
            }

            // SpriteRenderer �� sprite �ɕύX��̃X�v���C�g���Z�b�g
            spriteRenderer.sprite = NextSprite;

        }
        else
        {
            return;
        }
    }
}