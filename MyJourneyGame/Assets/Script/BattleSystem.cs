using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// シナリオ内バトルの簡易処理を行う管理クラス。
/// キャラ選択 → コマンド選択 → アクション → 終了までを制御。
/// </summary>
public class BattleSystem : MonoBehaviour
{
    public List<Enemy> m_enemies;
    public List<PlayerCharacter> m_players;

    public TextMeshProUGUI m_TurnText;

    private int m_currentTurn = 0;
    // UI要素
    public GameObject m_battleUI;         // 戦闘全体UI（ON/OFFでバトルの表示制御）
    public GameObject m_characterUI;      // キャラクター選択UI（複数キャラボタンなど）
    public GameObject m_commandUI;        // コマンド選択UI（攻撃、防御など）
    public GameObject m_actionUI;         // 行動選択UI（通常攻撃、スキルなど）
    public List<GameObject> m_selectionFrames; // 各キャラの白背景（選択枠、点滅用）

    // 選択中情報
    private string m_selectedCharacterName = ""; // 現在選択されているキャラ名
    private string m_selectedCommand = "";       // 現在選択中のコマンド内容

    /// <summary>
    /// バトル開始時に実行。UI初期化や枠リセットなど。
    /// </summary>
    public void StartBattle()
    {
        InitSelectionFrames(); // 枠全て透明＆点滅OFF
        Debug.Log("バトル開始！");

        // UI表示制御
        m_battleUI.SetActive(true);
        m_characterUI.SetActive(true);
        m_commandUI.SetActive(false);
        m_actionUI.SetActive(false);
    }

    /// <summary>
    /// キャラが選択されたときに実行（ボタンから文字列で渡される）
    /// </summary>
    public void OnCharacterSelected(string charaName)
    {
        m_selectedCharacterName = charaName;
        int selectedIndex = ExtractIndexFromName(charaName); // "Character2" → 2
        Debug.Log(selectedIndex);
        foreach (var frame in m_selectionFrames)
        {
            var blink = frame.GetComponent<BlinkEffect>();
            if (blink != null)
                blink.enabled = true;
        }
        SetSelectionFrame(selectedIndex); // 選ばれたキャラだけ枠を点滅させる

        m_commandUI.SetActive(true); // 次にコマンド選択UIを表示
    }

    /// <summary>
    /// キャラ名からインデックスを取得（末尾の数字）
    /// </summary>
    private int ExtractIndexFromName(string name)
    {
        if (name.StartsWith("Character"))
        {
            string num = name.Substring("Character".Length);
            if (int.TryParse(num, out int result))
                return result;
        }
        return 0; // デフォルトは先頭キャラ
    }

    /// <summary>
    /// コマンド（攻撃など）が選択されたときに実行
    /// </summary>
    public void OnCommandSelected(string command)
    {
        Debug.Log("選択コマンド: " + command);
        m_selectedCommand = command;

        if (command == "攻撃")
        {
            m_commandUI.SetActive(false);
            m_actionUI.SetActive(true);
        }
    }

    /// <summary>
    /// 実際の行動（通常攻撃・スキルなど）が選ばれたときの処理
    /// </summary>
    public void OnActionConfirmed(string action)
    {
        Debug.Log($"{m_selectedCharacterName} の {action} 実行！");

        // 実際の行動処理などを挿入（敵HPを減らすなど）

        StopAllSelectionFrameBlink(); // 選択キャラの枠点滅を解除（ターン終了）
    }

    /// <summary>
    /// すべての選択枠を透明＋点滅OFFにする
    /// </summary>
    private void StopAllSelectionFrameBlink()
    {
        foreach (var frame in m_selectionFrames)
        {
            var image = frame.GetComponent<Image>();
            var blink = frame.GetComponent<BlinkEffect>();

            if (image != null) image.color = new Color(1, 1, 1, 0);
            if (blink != null) blink.SetRender(false);
        }
    }

    /// <summary>
    /// 指定されたキャラの枠のみ表示＆点滅、それ以外は非表示＆非点滅
    /// </summary>
    private void SetSelectionFrame(int selectedIndex)
    {
        StopAllSelectionFrameBlink();
        var image = m_selectionFrames[selectedIndex].GetComponent<Image>();
        var blink = m_selectionFrames[selectedIndex].GetComponent<BlinkEffect>();
        if (image != null) image.color = new Color(1, 1, 1, 1); // 表示
        if (blink != null) blink.SetRender(true);              // 点滅ON

    }

    /// <summary>
    /// 最初にすべてのキャラ枠を透明＆点滅OFFに初期化
    /// </summary>
    private void InitSelectionFrames()
    {
        foreach (var frame in m_selectionFrames)
        {
            var img = frame.GetComponent<Image>();
            if (img != null) img.color = new Color(1, 1, 1, 0);
            var blink = frame.GetComponent<BlinkEffect>();
            if (blink != null) blink.enabled = false;
        }
    }


    public void EndPlayerTurn()
    {
        StartCoroutine(EnemyTurnRoutine());
    }

    private IEnumerator EnemyTurnRoutine()
    {
        m_TurnText.text = "敵のターン";

        foreach (var enemy in m_enemies)
        {
            if (enemy.gameObject.activeSelf)
            {
                var target = SelectRandomPlayer();
                if (target != null)
                {
                    enemy.Attack(target);
                    yield return new WaitForSeconds(1.0f); // 少し間を置く
                }
            }
        }


        Debug.Log("プレイヤーのターンに戻る");
        // プレイヤーのターン開始（必要な処理をここに）
        m_TurnText.text = "プレイヤーのターン";
    }

    private PlayerCharacter SelectRandomPlayer()
    {
        var alivePlayers = m_players.FindAll(p => p.m_currentHP > 0);
        if (alivePlayers.Count == 0) return null;
        return alivePlayers[Random.Range(0, alivePlayers.Count)];
    }



    /// <summary>
    /// バトル終了時にUIを非表示＆点滅解除。ストーリーに戻る。
    /// </summary>
    public void EndBattle()
    {
        m_battleUI.SetActive(false);

        foreach (var frame in m_selectionFrames)
        {
            var blink = frame.GetComponent<BlinkEffect>();
            if (blink != null)
                blink.enabled = false;
        }

        FindObjectOfType<TextAdventureSystem>().ContinueFromBattle(); // ストーリー進行再開
    }
}
