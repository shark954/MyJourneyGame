using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// �}�E�X�J�[�\���̃z�o�[��Image�̓����x��ς��鏈��
/// ������Ԃ͓����A�J�[�\�������ƕ\���A�����Ƃ܂������ɂȂ�
/// </summary>
public class FadeOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Image m_image;  // ����Ώۂ�Image

    // �����������iImage�R���|�[�l���g�擾�j
    void Awake()
    {
        m_image = GetComponent<Image>();
        SetAlpha(0f); // ������Ԃ͊��S�ɓ���
    }

    /// <summary>
    /// �}�E�X�J�[�\����UI�ɏ�����Ƃ��Ă΂��iIPointerEnterHandler�j
    /// </summary>
    public void OnPointerEnter(PointerEventData eventData)
    {
        SetAlpha(1f); // �s�����Ɂi�摜��������悤�Ɂj
    }

    /// <summary>
    /// �}�E�X�J�[�\����UI���痣�ꂽ�Ƃ��Ă΂��iIPointerExitHandler�j
    /// </summary>
    public void OnPointerExit(PointerEventData eventData)
    {
        SetAlpha(0f); // �Ăѓ����ɖ߂�
    }

    /// <summary>
    /// Image�̃A���t�@�l�i�����x�j��ݒ肷��
    /// </summary>
    void SetAlpha(float alpha)
    {
        if (m_image == null) return;
        Color color = m_image.color;
        color.a = alpha;
        m_image.color = color;
    }
}
