using UnityEngine;
using TMPro; // TextMeshPro ���g���ꍇ�͂�����

/// <summary>
/// �f�o�b�O�p�{�^��
/// </summary>
public class ModeChangeButton : MonoBehaviour
{
    public TextAdventureSystem textSystem;
    public TextMeshProUGUI labelText; // �{�^���̕\���p�e�L�X�g

    public void ToggleMode()
    {
        if (textSystem != null)
        {
            textSystem.modeChange = !textSystem.modeChange;

            if (labelText != null)
            {
                labelText.text = textSystem.modeChange ? "�o�g�����[�h" : "�m�[�}�����[�h";
            }
        }
    }

    private void Start()
    {
        // �����\��
        if (textSystem != null && labelText != null)
        {
            labelText.text = textSystem.modeChange ? "�o�g�����[�h" : "�m�[�}�����[�h";
        }
    }
}
