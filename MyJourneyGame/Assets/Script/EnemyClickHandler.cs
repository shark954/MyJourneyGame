using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// �G���N���b�N�����Ƃ���BattleUIManager�ɒʒm���邽�߂̃n���h���[
/// ���̃X�N���v�g�͓G��UI�v�f�iButton��Image�Ȃǁj�ɃA�^�b�`����
/// </summary>
public class EnemyClickHandler : MonoBehaviour, IPointerClickHandler
{
    public Enemy m_enemy;  // ����UI�ɑΉ�����G�L����

    /// <summary>
    /// �}�E�X�N���b�N���ɌĂ΂��C�x���g
    /// </summary>
    /// <param name="eventData">�N���b�N�Ɋւ���f�[�^</param>
    public void OnPointerClick(PointerEventData eventData)
    {
        // BattleUIManager �ɂ��̓G���N���b�N���ꂽ���Ƃ�ʒm
        BattleUIManager.m_Instance.OnEnemyClicked(m_enemy);
    }
}
