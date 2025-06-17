using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    public GameObject m_battleUI; // �퓬�pUI�i�p�l���Ȃǁj

    public void StartBattle()
    {
        Debug.Log("�o�g���J�n�I");
        m_battleUI.SetActive(true);

        // ������ԂȂǃZ�b�g�A�b�v
        ShowChoices(new string[] { "�U��", "�h��", "�X�L��" });
    }

    public void ShowChoices(string[] choices)
    {
        // �{�^��UI�\�����W�b�N�i�O�q�R�[�h�𗬗p�\�j
    }

    public void OnChoiceSelected(string choice)
    {
        Debug.Log("�I�����ꂽ: " + choice);
        // �s������ & ���o �� ���̌�o�g���I����TextAdventureSystem�ɖ߂�
        EndBattle();
    }

    public void EndBattle()
    {
        m_battleUI.SetActive(false);
        FindObjectOfType<TextAdventureSystem>().ContinueFromBattle(); // �ʏ�i�s�ĊJ
    }
}
