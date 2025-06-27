using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 敵をクリックしたときにBattleUIManagerに通知するためのハンドラー
/// このスクリプトは敵のUI要素（ButtonやImageなど）にアタッチする
/// </summary>
public class EnemyClickHandler : MonoBehaviour, IPointerClickHandler
{
    public Enemy m_enemy;  // このUIに対応する敵キャラ

    /// <summary>
    /// マウスクリック時に呼ばれるイベント
    /// </summary>
    /// <param name="eventData">クリックに関するデータ</param>
    public void OnPointerClick(PointerEventData eventData)
    {
        // BattleUIManager にこの敵がクリックされたことを通知
        BattleUIManager.m_Instance.OnEnemyClicked(m_enemy);
    }
}
