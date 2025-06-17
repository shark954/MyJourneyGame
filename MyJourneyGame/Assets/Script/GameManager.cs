
using UnityEngine;
using UnityEngine.UI;
using System;

/// <summary>
/// ゲーム全体の進行管理クラス。ストーリー表示・選択肢制御・戦闘管理などを含む。
/// </summary>
public class GameManager : MonoBehaviour
{
    // === UI参照 ===

    public TitleManager m_titleManager; // Inspectorで接続

    public EndingManager m_endingManager; // Inspectorで接続

    [Header("背景画像")]
    public Image m_backgroundImage; // 背景のSprite表示用

    [Header("タイトル画面")]
    public GameObject m_titlePanel;
    [Header("エンディング画面")]
    public GameObject m_endingPanel;
    [Header("ゲーム画面")]
    public GameObject m_gamePanel;


    [Header("ストーリー本文")]
    public Text m_storyText; // ストーリーテキストのUI
    [Header("ストーリーUI")]
    public GameObject m_storyPanel;

    [Header("選択肢ボタン")]
    public Button m_choiceButton1;
    public Button m_choiceButton2;

    [Header("戦闘用UI")]
    public GameObject m_battlePanel; // 戦闘パネル全体（切替用）
    public Text m_battleText; // 戦況のテキスト
    [Header("戦闘コメント表示")]
    public Text m_battleCommentText; // 攻撃・回復などのコメント表示用
    public Button m_attackButton; // 「攻撃」ボタン
    public Button m_healButton; // 「回復」ボタン

    
    // === 内部変数 ===

    private int m_currentSceneID = 0; // 現在のシーンID
    private int m_playerHP = 10; // プレイヤーのHP
    private int m_enemyHP = 8; // 敵のHP
    private System.Random rand = new System.Random(); // ダメージ用ランダム

    /// <summary>
    /// ゲーム開始時に最初のシーンを表示
    /// </summary>
    void Start()
    {
        m_titlePanel.SetActive(true);
        m_endingPanel.SetActive(false);
        m_gamePanel.SetActive(false);
    }
   

    /// <summary>
    /// 選択肢ボタンのテキストとイベント設定
    /// </summary>
    private void ConfigureChoiceButtons(string text1, UnityEngine.Events.UnityAction action1, string text2, UnityEngine.Events.UnityAction action2)
    {
        m_choiceButton1.gameObject.SetActive(true);
        m_choiceButton2.gameObject.SetActive(true);

        m_choiceButton1.GetComponentInChildren<Text>().text = text1;
        m_choiceButton2.GetComponentInChildren<Text>().text = text2;

        m_choiceButton1.onClick.RemoveAllListeners();
        m_choiceButton1.onClick.AddListener(action1);

        m_choiceButton2.onClick.RemoveAllListeners();
        m_choiceButton2.onClick.AddListener(action2);
    }

    // === 戦闘パート ===
    /// <summary>
    /// 戦闘中のコメントを設定する
    /// </summary>
    private void SetBattleComment(string comment)
    {
        m_battleCommentText.text = comment;
    }

    public void StartBattle()
    {
        m_playerHP = 10;
        m_enemyHP = 8;

        // ボタンのテキスト設定（Buttonの子にTextがある前提）
        m_attackButton.GetComponentInChildren<Text>().text = "攻撃";
        m_healButton.GetComponentInChildren<Text>().text = "回復";



        // UI切り替え
        m_storyPanel.gameObject.SetActive(false);
        m_battlePanel.SetActive(true);

        UpdateBattleText();

        m_attackButton.onClick.RemoveAllListeners();
        m_healButton.onClick.RemoveAllListeners();
        m_attackButton.onClick.AddListener(PlayerAttack);
        m_healButton.onClick.AddListener(PlayerHeal);
    }

    private void PlayerAttack()
    {

        m_enemyHP -= rand.Next(2, 5); // 敵に2~4ダメージ
        m_playerHP -= rand.Next(1, 4); // 反撃でプレイヤー1~3ダメージ
        SetBattleComment("あなたは敵に攻撃した！");
        UpdateBattleText();
        CheckBattleResult();
    }

    private void PlayerHeal()
    {

        m_playerHP += rand.Next(2, 4); // 回復2~3
        m_playerHP -= rand.Next(1, 3); // 敵の攻撃1~2
        SetBattleComment("あなたは体力を回復した！");
        UpdateBattleText();
        CheckBattleResult();
    }

    private void UpdateBattleText()
    {
        m_battleText.text = $"あなたのHP: {m_playerHP}\n敵のHP: {m_enemyHP}";
    }

    private void CheckBattleResult()
    {
        if (m_enemyHP <= 0)
        {
            EndBattle(true); // 勝利
        }
        else if (m_playerHP <= 0)
        {
            EndBattle(false); // 敗北（逃走扱い）
        }
    }

    private void EndBattle(bool victory)
    {
        m_battlePanel.SetActive(false);
        m_storyPanel.gameObject.SetActive(true);

        if (victory)
        {

        }
        else
        {

        }
        
    }
    #region 使わない
        /// <summary>
        /// 指定されたシーンIDの内容を表示
        /// </summary>choiceButton1

        /*public void ShowScene(int sceneID)
        {

            if (m_footstepClip != null)
                m_audioSource.PlayOneShot(m_footstepClip);

            m_currentSceneID = sceneID;

            // UI切り替え：通常表示ON、戦闘OFF
            m_gamePanel.gameObject.SetActive(true);
            m_battlePanel.SetActive(false);
            #region 使わない
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
                    m_endingManager.PlayEnding("足を滑らせた…\n旅はここで終わった。", false);
                    break;

                case 7:
                    SetBackgroundImage("village_background");
                    SetStoryText("敵を倒した。村の記録を見つける。");
                    ConfigureChoiceButtons("記録を読む", () => ShowScene(9), "その場を去る", () => ShowScene(4));
                    break;

                case 8:
                    SetBackgroundImage("forest_background");
                    SetStoryText("無事逃げ出したが、手がかりは失った。");
                    ConfigureChoiceButtons("旅を続ける", () => ShowScene(2),// 通常ルートに戻る
                        "旅をあきらめる", () => ShowScene(10)); // ゲームオーバーのエンディングへ
                    break;

                case 9:
                    // 勝利時
                    m_endingManager.PlayEnding("旅の向かう先へ…\nあなたはまた歩き出す。", true);
                    break;

                case 10:
                    //敗北OR撤退
                    m_endingManager.PlayEnding("希望は見えたが、旅はここで止まってしまった…", false);
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
                m_backgroundImage.sprite = sprite;
            }
        }

        /// <summary>
        /// ストーリーテキストの更新
        /// </summary>
        private void SetStoryText(string text)
        {
            m_storyText.text = text;
        }


        }*/
        #endregion
}
