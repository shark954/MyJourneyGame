using System.Collections;
using UnityEngine;
using TMPro;

public class TextColorChange : MonoBehaviour
{
    [Header("�e�L�X�g�Ώ�")]
    public TextMeshProUGUI textMesh; // ����Ώۂ� TextMeshProUGUI �R���|�[�l���g

    [Header("���o�ݒ�")]
    public float fadeSpeed = 2f;         // �t�F�[�h�̐i�s���x�i�傫���قǑ����j
    public float delayPerChar = 0.1f;    // �e�����̏����Ԋu�i�����珇�ɓK�p�����j
    public Gradient gradient;            // �����ɓK�p����F�̃O���f�[�V����

    [Header("�t�F�[�h����")]
    public bool fadeIn = true;           // true: �t�F�[�h�C���i�������\���j/ false: �t�F�[�h�A�E�g�i�\���������j

    // �萔�F�J���[��A���t�@�̍ő�l
    private const float maxAlpha01 = 1f;
    private const byte maxAlphaByte = 255;

    // TextMeshPro �̃��b�V�����i�[�p
    private TMP_TextInfo textInfo;

    void Start()
    {
        // �e�L�X�g�������I�Ƀ��b�V���X�V
        textMesh.ForceMeshUpdate();

        // �e�L�X�g�̒��_�E���������擾
        textInfo = textMesh.textInfo;

        // �R���[�`���ŃA�j���[�V�����J�n
        StartCoroutine(FadeWithColor());
    }

    IEnumerator FadeWithColor()
    {
        int charCount = textInfo.characterCount;

        // �����ݒ�F���ׂĂ̕����̃A���t�@�l�� 0�i�����j�܂��� 255�i�s�����j�ɂ���
        for (int meshIdx = 0; meshIdx < textInfo.meshInfo.Length; meshIdx++)
        {
            Color32[] colors = textInfo.meshInfo[meshIdx].colors32;

            for (int i = 0; i < colors.Length; i++)
            {
                colors[i].a = fadeIn ? (byte)0 : maxAlphaByte;
            }
        }
        textMesh.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

        // �������Ƃɏ��������s�i������E�ցj
        for (int i = 0; i < charCount; i++)
        {
            // ��\���̕����i�X�y�[�X�Ȃǁj�̓X�L�b�v
            if (!textInfo.characterInfo[i].isVisible)
                continue;

            // �Ώۂ̕����̃��b�V���E���_�C���f�b�N�X���擾
            int meshIndex = textInfo.characterInfo[i].materialReferenceIndex;
            int vertexIndex = textInfo.characterInfo[i].vertexIndex;
            Color32[] vertexColors = textInfo.meshInfo[meshIndex].colors32;

            // �O���f�[�V�����F�𕶎��ʒu�ɉ����Ď擾
            Color targetColor = gradient.Evaluate((float)i / charCount); // Color32 �� Color �ɕύX

            // RGB �� byte �ɕϊ��i�����I�ɕϊ��j
            byte r = (byte)(targetColor.r * maxAlphaByte);
            byte g = (byte)(targetColor.g * maxAlphaByte);
            byte b = (byte)(targetColor.b * maxAlphaByte);


            float fadeProgress = 0f;

            // ���X�ɃA���t�@��ω������ĕ\�� or ����
            while (fadeProgress < maxAlpha01)
            {
                fadeProgress += Time.deltaTime * fadeSpeed;

                // �t�F�[�h�����ɉ����ăA���t�@���ԁi0 �� 255 �܂��� 255 �� 0�j
                float alphaRatio = Mathf.Clamp01(fadeProgress);
                byte alpha = (byte)((fadeIn ? alphaRatio : 1f - alphaRatio) * maxAlphaByte);

                // ������4���_���ׂĂɐF��K�p
                for (int j = 0; j < 4; j++)
                {
                    vertexColors[vertexIndex + j] = new Color32(r, g, b, alpha);
                }

                textMesh.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
                yield return null; // ���̃t���[���܂ő҂�
            }

            // ���̕����֐i�ނ܂őҋ@
            yield return new WaitForSeconds(delayPerChar);
        }
    }
}
