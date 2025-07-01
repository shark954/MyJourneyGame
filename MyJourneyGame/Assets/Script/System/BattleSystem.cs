using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// シナリオ内バトルを制御するメインクラス。
/// キャラ選択 → コマンド選択 → 行動処理 → 敵ターン → 勝敗判定までを担当。
/// </summary>
public class BattleSystem : MonoBehaviour
{
    public List<Enemy> m_enemies;
    public List<PlayerCharacter> m_players;
    public AudioSource m_battleBGM; //戦闘BGM

    public TextMeshProUGUI m_TurnText;
    public GameManager m_gameManager;
    public List<string> m_enemyPrefabNames; // 将来の拡張用：プレハブ名リスト
    public Transform m_enemySpawnPoint = null;

    private int m_currentTurn = 0;

    // UI関連
    public GameObject m_battleUI;
    public GameObject m_characterUI;
    public GameObject m_commandUI;
    public GameObject m_actionUI;
    public List<GameObject> m_selectionFrames; // 各キャラの選択枠UI

    // 現在の選択情報
    private string m_selectedCharacterName = "";
    private string m_selectedCommand = "";

    public bool m_escape; // 「逃げる」判定

    /// <summary>
    /// バトル開始時の初期化処理。HP/SPのリセットやUI表示の切り替え。
    /// </summary>
    public void StartBattle()
    {
        InitSelectionFrames();
        Debug.Log("バトル開始！");

        // プレイヤーと敵のステータスを初期化
        foreach (PlayerCharacter player in BattleUIManager.m_Instance.m_players)
        {
            player.ResetStatus(); // ← 確実にオーバーライドされたほうが呼ばれる
        }

        foreach (Enemy enemy in BattleUIManager.m_Instance.m_enemies) 
        { 
            enemy.ResetStatus(); // ← 確実にオーバーライドされたほうが呼ばれる
        }
        // UI切り替え
        m_battleUI.SetActive(true);
        m_characterUI.SetActive(true);
        m_commandUI.SetActive(false);
        m_actionUI.SetActive(false);

        // ★ BGM再生
        if (m_battleBGM != null && !m_battleBGM.isPlaying)
            m_battleBGM.Play();
    }

    /// <summary>
    /// キャラクター選択時に呼ばれる（ボタンイベント）
    /// </summary>
    public void OnCharacterSelected(string charaName)
    {
        m_selectedCharacterName = charaName;
        int selectedIndex = ExtractIndexFromName(charaName);
        Debug.Log(selectedIndex);

        // 全選択枠の点滅を一旦ON
        foreach (var frame in m_selectionFrames)
        {
            var blink = frame.GetComponent<BlinkEffect>();
            if (blink != null)
                blink.enabled = true;
        }

        SetSelectionFrame(selectedIndex); // 選択キャラの枠だけ点滅
        m_commandUI.SetActive(true); // コマンドUI表示
    }

    /// <summary>
    /// "Character1" → 1 のように末尾の番号を取得
    /// </summary>
    private int ExtractIndexFromName(string name)
    {
        if (name.StartsWith("Character"))
        {
            string num = name.Substring("Character".Length);
            if (int.TryParse(num, out int result))
                return result;
        }
        return 0;
    }

    /// <summary>
    /// 「攻撃」や「逃げる」などのコマンド選択時に実行
    /// </summary>
    public void OnCommandSelected(string command)
    {
        Debug.Log("選択コマンド: " + command);
        m_selectedCommand = command;

        if (command == "攻撃")
        {
            m_commandUI.SetActive(false);
            m_actionUI.SetActive(true); // 通常攻撃かスキルか選ぶ
        }

        if (command == "逃げる")
        {
            m_escape = true;
            EndBattle(true); // 勝利扱いでバトル終了
        }
    }

    // OnActionConfirmed はコメントアウトされていますが、行動確定処理の場所です。

    /// <summary>
    /// すべての選択枠の点滅・表示をオフに
    /// </summary>
    private void StopAllSelectionFrameBlink()
    {
        foreach (var frame in m_selectionFrames)
        {
            var image = frame.GetComponent<Image>();
            var blink = frame.GetComponent<BlinkEffect>();

            if (image != null) image.color = new Color(1, 1, 1, 0); // 非表示
            if (blink != null) blink.SetRender(false);             // 点滅OFF
        }
    }

    /// <summary>
    /// 指定キャラの枠のみ表示＆点滅ON
    /// </summary>
    private void SetSelectionFrame(int selectedIndex)
    {
        StopAllSelectionFrameBlink();
        var image = m_selectionFrames[selectedIndex].GetComponent<Image>();
        var blink = m_selectionFrames[selectedIndex].GetComponent<BlinkEffect>();
        if (image != null) image.color = new Color(1, 1, 1, 1); // 表示ON
        if (blink != null) blink.SetRender(true);              // 点滅ON
    }

    /// <summary>
    /// 全キャラ枠の初期化（非表示＆点滅OFF）
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

    /// <summary>
    /// プレイヤーターン終了後、敵のターン開始処理
    /// </summary>
    public void EndPlayerTurn()
    {
        StartCoroutine(EnemyTurnRoutine());
    }

    /// <summary>
    /// 敵のターン処理：スキルor攻撃 → 全滅チェック
    /// </summary>
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
                    // スキル使用確率40%、SP条件付き
                    if (enemy.m_currentSP >= 5 && Random.value < 0.4f)
                        enemy.UseSkill(target);
                    else
                        enemy.Attack(target);

                    yield return new WaitForSeconds(1.0f);

                    // 攻撃後に全滅判定
                    if (IsAllPlayersDead())
                    {
                        m_escape = false;
                        EndBattle(false); // 敗北
                        yield break;
                    }
                }
            }
        }

        m_TurnText.text = "プレイヤーのターン";
        Debug.Log("プレイヤーのターンに戻る");
    }

    /// <summary>
    /// ランダムに生きているプレイヤーを選択
    /// </summary>
    private PlayerCharacter SelectRandomPlayer()
    {
        var alivePlayers = m_players.FindAll(p => p.m_currentHP > 0);
        if (alivePlayers.Count == 0) return null;
        return alivePlayers[Random.Range(0, alivePlayers.Count)];
    }

    /// <summary>
    /// 全プレイヤーが死亡しているか確認
    /// </summary>
    private bool IsAllPlayersDead()
    {
        return m_players.TrueForAll(p => p.m_currentHP <= 0);
    }

    /// <summary>
    /// バトル終了時の処理（勝利/敗北/撤退）
    /// </summary>
    public void EndBattle(bool battleWin)
    {

        if (m_battleBGM != null && m_battleBGM.isPlaying)
            m_battleBGM.Stop();

        // 勝利（戦闘勝利）
        if (battleWin && !m_escape)
        {
            m_battleUI.SetActive(false);
            foreach (var frame in m_selectionFrames)
            {
                var blink = frame.GetComponent<BlinkEffect>();
                if (blink != null) blink.enabled = false;
            }
            m_gameManager.m_adventureSystem.ContinueFromBattle();
        }

        // 勝利（逃げる）
        if (battleWin && m_escape)
        {
            m_battleUI.SetActive(false);
            foreach (var frame in m_selectionFrames)
            {
                var blink = frame.GetComponent<BlinkEffect>();
                if (blink != null) blink.enabled = false;
            }
            m_gameManager.m_adventureSystem.m_DisplayText.text = "力及ばず勇者一行は撤退した";
            m_gameManager.m_adventureSystem.ContinueFromBattle();
        }

        // 敗北
        if (!battleWin && !m_escape)
        {
            m_battleUI.SetActive(false);
            m_gameManager.m_gameEnd = true;
            m_gameManager.m_endingManager.PlayEnding("勇者は敗北した", false);
        }
    }
}
