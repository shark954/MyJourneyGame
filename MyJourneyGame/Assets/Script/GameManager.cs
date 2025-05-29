
using UnityEngine;
using UnityEngine.UI;
using System;

/// <summary>
/// ゲーム全体の進行管理クラス。ストーリー表示・選択肢制御・戦闘管理などを含む。
/// </summary>
public class GameManager : MonoBehaviour
{
    // === UI参照 ===

    public TitleManager titleManager; // Inspectorで接続

    public EndingManager endingManager; // Inspectorで接続

    [Header("背景画像")]
    public Image backgroundImage; // 背景のSprite表示用

    [Header("ストーリー本文")]
    public Text storyText; // ストーリーテキストのUI

    [Header("選択肢ボタン")]
    public Button choiceButton1;
    public Button choiceButton2;

    [Header("戦闘用UI")]
    public GameObject battlePanel; // 戦闘パネル全体（切替用）
    public Text battleText; // 戦況のテキスト
    public Button attackButton; // 「攻撃」ボタン
    public Button healButton; // 「回復」ボタン

    // === 内部変数 ===

    private int currentSceneID = 0; // 現在のシーンID
    private int playerHP = 10; // プレイヤーのHP
    private int enemyHP = 8; // 敵のHP
    private System.Random rand = new System.Random(); // ダメージ用ランダム

    /// <summary>
    /// ゲーム開始時に最初のシーンを表示
    /// </summary>
    void Start()
    {
        ShowScene(0);
    }

    /// <summary>
    /// 指定されたシーンIDの内容を表示
    /// </summary>
    public void ShowScene(int sceneID)
    {
        currentSceneID = sceneID;

        // UI切り替え：通常表示ON、戦闘OFF
        storyText.transform.parent.gameObject.SetActive(true);
        battlePanel.SetActive(false);

        switch (sceneID)
        {
            case 0:
                SetBackgroundImage("forest_background");
                SetStoryText("森の中で分かれ道に立っている。");
                ConfigureChoiceButtons("左の道へ", () => ShowScene(1), "右の道へ", () => ShowScene(2));
                break;

            case 1:
                SetBackgroundImage("village_background");
                SetStoryText("村の廃墟にたどり着いた。物音が聞こえる。");
                ConfigureChoiceButtons("調べる", () => ShowScene(3), "通り過ぎる", () => ShowScene(4));
                break;

            case 2:
                SetBackgroundImage("mountain_background");
                SetStoryText("山道を歩いていると、老人と出会った。");
                ConfigureChoiceButtons("話しかける", () => ShowScene(5), "無視して通る", () => ShowScene(6));
                break;

            case 3:
                StartBattle(); // 戦闘開始
                break;

            case 4:
                SetBackgroundImage("forest_background");
                SetStoryText("森に戻り、再び道を選ぶ。");
                ConfigureChoiceButtons("山道へ行く", () => ShowScene(2), "村に戻る", () => ShowScene(1));
                break;

            case 5:
                SetBackgroundImage("hut_background");
                SetStoryText("老人が地図を渡そうとする。");
                ConfigureChoiceButtons("地図を受け取る", () => ShowScene(9), "拒否する", () => ShowScene(6));
                break;

            case 6:
                ShowEnding("足を滑らせた…\n旅はここで終わった。", "dark_background");
                break;

            case 7:
                SetBackgroundImage("village_background");
                SetStoryText("敵を倒した。村の記録を見つける。");
                ConfigureChoiceButtons("記録を読む", () => ShowScene(9), "その場を去る", () => ShowScene(4));
                break;

            case 8:
                SetBackgroundImage("forest_background");
                SetStoryText("無事逃げ出したが、手がかりは失った。");
                ConfigureChoiceButtons("旅を続ける", () => ShowScene(2), "村に戻る", () => ShowScene(1));
                break;

            case 9:
                endingManager.PlayEnding(); // これで演出が開始される
                break;
        }
    }

    /// <summary>
    /// 背景画像を読み込み表示
    /// </summary>
    private void SetBackgroundImage(string imageName)
    {
        Sprite sprite = Resources.Load<Sprite>(imageName);
        if (sprite != null)
        {
            backgroundImage.sprite = sprite;
        }
    }

    /// <summary>
    /// ストーリーテキストの更新
    /// </summary>
    private void SetStoryText(string text)
    {
        storyText.text = text;
    }

    /// <summary>
    /// 選択肢ボタンのテキストとイベント設定
    /// </summary>
    private void ConfigureChoiceButtons(string text1, UnityEngine.Events.UnityAction action1, string text2, UnityEngine.Events.UnityAction action2)
    {
        choiceButton1.gameObject.SetActive(true);
        choiceButton2.gameObject.SetActive(true);

        choiceButton1.GetComponentInChildren<Text>().text = text1;
        choiceButton2.GetComponentInChildren<Text>().text = text2;

        choiceButton1.onClick.RemoveAllListeners();
        choiceButton1.onClick.AddListener(action1);

        choiceButton2.onClick.RemoveAllListeners();
        choiceButton2.onClick.AddListener(action2);
    }

    // === 戦闘パート ===

    public void StartBattle()
    {
        playerHP = 10;
        enemyHP = 8;

        // UI切り替え
        storyText.transform.parent.gameObject.SetActive(false);
        battlePanel.SetActive(true);

        UpdateBattleText();

        attackButton.onClick.RemoveAllListeners();
        healButton.onClick.RemoveAllListeners();
        attackButton.onClick.AddListener(PlayerAttack);
        healButton.onClick.AddListener(PlayerHeal);
    }

    private void PlayerAttack()
    {
        enemyHP -= rand.Next(2, 5); // 敵に2~4ダメージ
        playerHP -= rand.Next(1, 4); // 反撃でプレイヤー1~3ダメージ
        UpdateBattleText();
        CheckBattleResult();
    }

    private void PlayerHeal()
    {
        playerHP += rand.Next(2, 4); // 回復2~3
        playerHP -= rand.Next(1, 3); // 敵の攻撃1~2
        UpdateBattleText();
        CheckBattleResult();
    }

    private void UpdateBattleText()
    {
        battleText.text = $"あなたのHP: {playerHP}\n敵のHP: {enemyHP}";
    }

    private void CheckBattleResult()
    {
        if (enemyHP <= 0)
        {
            EndBattle(true); // 勝利
        }
        else if (playerHP <= 0)
        {
            EndBattle(false); // 敗北（逃走扱い）
        }
    }

    private void EndBattle(bool victory)
    {
        battlePanel.SetActive(false);
        storyText.transform.parent.gameObject.SetActive(true);

        if (victory)
        {
            ShowScene(7);
        }
        else
        {
            ShowScene(8);
        }
    }

    // === エンディング処理 ===

    private void ShowEnding(string message, string background)
    {
        SetBackgroundImage(background);
        SetStoryText(message);
        choiceButton1.gameObject.SetActive(false);
        choiceButton2.gameObject.SetActive(false);
    }
}
