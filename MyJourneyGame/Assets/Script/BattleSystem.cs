using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    public GameObject m_battleUI; // 戦闘用UI（パネルなど）

    public void StartBattle()
    {
        Debug.Log("バトル開始！");
        m_battleUI.SetActive(true);

        // 初期状態などセットアップ
        ShowChoices(new string[] { "攻撃", "防御", "スキル" });
    }

    public void ShowChoices(string[] choices)
    {
        // ボタンUI表示ロジック（前述コードを流用可能）
    }

    public void OnChoiceSelected(string choice)
    {
        Debug.Log("選択された: " + choice);
        // 行動処理 & 演出 → その後バトル終了でTextAdventureSystemに戻す
        EndBattle();
    }

    public void EndBattle()
    {
        m_battleUI.SetActive(false);
        FindObjectOfType<TextAdventureSystem>().ContinueFromBattle(); // 通常進行再開
    }
}
