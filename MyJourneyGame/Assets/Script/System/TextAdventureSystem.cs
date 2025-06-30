using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using TMPro;
using System.Collections;

/// <summary>
/// 外部テキストファイルからシナリオを読み取り、コマンドを実行するシステム
/// </summary>
public class TextAdventureSystem : MonoBehaviour
{
    public enum Mode
    {
        Normal,
        Battle
    }

    public Mode currentMode = Mode.Normal;

    // 全シナリオ行を保持（ジャンプ処理で使う）
    private List<string> m_AllLines = new List<string>();

    // 現在の読み込み位置（未使用でもいいが念のため）
    private int m_CurrentLineIndex = 0;

    // ラベル名とその行番号のマッピング
    private Dictionary<string, int> m_LabelLineIndex = new Dictionary<string, int>();

    [Header("背景画像（新）")]
    public Image m_backGroundImageNew; // 通常の背景表示用

    [Header("背景画像（旧）")]
    public Image m_backGroundImageOld; // フェードアウト用重ね背景

    [Header("テキストを表示するUI")]
    public Text m_DisplayText;
    [Header("画像表示用×3")]
    public List<Image> m_DisplayImage;
    [Header("背景画像")]
    public Image m_backGroundImage;
    [Header("選択肢ボタン群")]
    public List<Button> m_ChoiceButtons;
    [Header("キャラリソース【画像】")]
    public string m_CharaImageResourcePath = "Images/Character";
    [Header("背景リソース【画像】")]
    public string m_BackGroundImageResourcePath = "Images/BackGround";
    [Header("BGM用")]
    public AudioSource m_BgmSource;
    [Header("取り込んだシナリオ")]
    public Queue<string> m_Commands = new Queue<string>();
    [Header("左クリック待ちフラグ")]
    public bool m_WaitingForClick = false;
    [Header("ゲームマネージャー")]
    public GameManager m_gameManager;

    private bool m_IsFading = false;
    private float m_FadeDuration = 1.0f;
    private float m_FadeElapsed = 0f;
    private int m_SelectedChoiceIndex = -1;
    private Sprite m_PendingBackground = null;
    [Header("デバッグ変数")]
    public bool modeChange = false;

    void Start()
    {
        // キャラクター画像を初期透明にする
        foreach (Image Dummy in m_DisplayImage)
            Dummy.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);

        // StreamingAssets内のシナリオファイルをロード
        LoadScenario(Application.streamingAssetsPath + "/scenario.txt");

        // 最初のコマンドを実行
        NextCommand();
    }

    void Update()
    {
        if (m_IsFading)
        {
            m_FadeElapsed += Time.deltaTime;
            float t = Mathf.Clamp01(m_FadeElapsed / m_FadeDuration);

            m_backGroundImageOld.color = new Color(1, 1, 1, 1 - t);
            m_backGroundImageNew.color = new Color(1, 1, 1, t);

            if (t >= 1.0f)
            {
                m_IsFading = false;
                m_backGroundImageOld.color = new Color(1, 1, 1, 0);
                m_backGroundImageNew.color = new Color(1, 1, 1, 1);
                NextCommand();
            }

            return;
        }

        // 通常のクリック処理
        if (m_WaitingForClick && Input.GetMouseButtonDown(0))
        {
            NextCommand();
        }

        //デバッグ用 三項定理
       // ChangeMode(modeChange ? true : false);
    }

    //デバッグ関数 modechangeがtrueならバトルモード、falseならノーマルモード
    public void ChangeMode(bool flag)
    {
        if (flag)
        {
            currentMode = Mode.Battle;
            m_WaitingForClick = false;
            m_gameManager.m_storyPanel.SetActive(false);
            m_gameManager.m_battlePanel.SetActive(true);
        }

        if (!flag)
        {
            currentMode = Mode.Normal;
            m_WaitingForClick = true;
            m_gameManager.m_battlePanel.SetActive(false);
            m_gameManager.m_storyPanel.SetActive(true);
        }
    }

    void LoadScenario(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Debug.LogError("シナリオファイルが見つからねぇよ!: " + filePath);
            return;
        }

        string[] lines = File.ReadAllLines(filePath);
        m_AllLines = new List<string>(lines); // 全行を保持
        m_Commands.Clear();
        m_LabelLineIndex.Clear();

        for (int i = 0; i < m_AllLines.Count; i++)
        {
            string line = m_AllLines[i].Trim();

            // ラベル行（LABEL:）はジャンプ用に記録して、それ以外はコマンドとして保存
            if (line.StartsWith("LABEL:"))
            {
                string label = line.Substring(6).Trim();
                m_LabelLineIndex[label] = i + 1; // 次の行から実行再開
            }
            else if (!string.IsNullOrWhiteSpace(line))
            {
                m_Commands.Enqueue(line);
            }
        }
    }

    /// <summary>
    /// 次のコマンドをキューから取り出し、実行
    /// </summary>
    void NextCommand()
    {
        //HideChoices(); // ← ここで毎回非表示にする！

        if (m_Commands.Count == 0)
        {
            // シナリオが終了した場合
            m_DisplayText.text = "シナリオ終了";
            m_WaitingForClick = false; // 待機解除
            return;
        }

        // キューから次のコマンドを取り出す
        string command = m_Commands.Dequeue();

        // コマンドを実行
        ExecuteCommand(command);
    }

    /// <summary>
    /// コマンドの内容に応じた処理を実行
    /// </summary>
    /// <param name="command">実行するコマンド文字列</param>
    void ExecuteCommand(string command)
    {
        #region "TEXT:" で始まる場合：テキスト表示
        // "TEXT:" で始まる場合：テキスト表示
        if (command.StartsWith("TEXT:"))
        {
            //5文字削除後、Messageを表示する
            TextRender(DataPatch(command, 5));
            // 左クリック待ちにする
            m_WaitingForClick = true;
        }
        #endregion
        #region "SHOW_IMAGE_S:" で始まる場合：画像表示
        // "SHOW_IMAGE_S:" で始まる場合：画像表示
        else if (command.StartsWith("SHOW_IMAGE_S:"))
        {
            //13文字削除後、キャライメージ【中央】を表示する
            ImageCharacterRender(DataPatch(command, 13), 2);
            // 画像表示後は自動進行
            NextCommand();
        }
        #endregion
        #region "SHOW_IMAGE_R:" で始まる場合：画像表示
        // "SHOW_IMAGE_R:" で始まる場合：画像表示
        else if (command.StartsWith("SHOW_IMAGE_R:"))
        {

            //13文字削除後、キャライメージ【右】を表示する
            ImageCharacterRender(DataPatch(command, 13), 0);
            // 画像表示後は自動進行
            NextCommand();
        }
        #endregion
        #region "SHOW_IMAGE_L:" で始まる場合：画像表示
        // "SHOW_IMAGE_L:" で始まる場合：画像表示
        else if (command.StartsWith("SHOW_IMAGE_L:"))
        {
            //13文字削除後、キャライメージ【左】を表示する
            ImageCharacterRender(DataPatch(command, 13), 1);
            // 画像表示後は自動進行
            NextCommand();
        }
        #endregion
        #region "PLAY_BGM:" で始まる場合：BGM再生
            // "PLAY_BGM:" で始まる場合：BGM再生
        else if (command.StartsWith("PLAY_BGM:"))
        {
            // "PLAY_BGM:"以降の文字列
            string bgmName = command.Substring(9);
            //9文字削除後、指定したバックミュージックを流す
            BackGroundMusic(DataPatch(command, 9));
            // BGM再生後は自動進行
            NextCommand();
        }
        #endregion
        #region "SHOW_IMAGE_S_CLS:" で始まる場合：画像非表示
        // "SHOW_IMAGE_S_CLS:" で始まる場合：画像表示
        else if (command.StartsWith("SHOW_IMAGE_S_CLS:"))
        {
            // 中央イメージキャラクターパネルを消す
            ImageCharacterCLS(2);
            // 画像表示後は自動進行
            NextCommand();
        }
        #endregion
        #region "SHOW_IMAGE_R_CLS:" で始まる場合：画像非表示
        // "SHOW_IMAGE_R_CLS:" で始まる場合：画像表示
        else if (command.StartsWith("SHOW_IMAGE_R_CLS:"))
        {
            // 中央イメージキャラクターパネルを消す
            ImageCharacterCLS(0);
            // 画像表示後は自動進行
            NextCommand();
        }
        #endregion
        #region "SHOW_IMAGE_L_CLS:" で始まる場合：画像非表示
        // "SHOW_IMAGE_L_CLS:" で始まる場合：画像表示
        else if (command.StartsWith("SHOW_IMAGE_L_CLS:"))
        {
            // 中央イメージキャラクターパネルを消す
            ImageCharacterCLS(1);
            // 画像表示後は自動進行
            NextCommand();
        }
        #endregion
        #region "START_BATTLE:"で始まる場合：バトルスタート 
        else if (command.StartsWith("START_BATTLE:"))
        {
            // 戦闘システムを呼び出し
            currentMode = Mode.Battle;
            FindObjectOfType<BattleSystem>().StartBattle();
            // 今のクラスでは進行ストップ
            m_WaitingForClick = false;
            m_gameManager.m_storyPanel.SetActive(false);
        }
        #endregion
        #region "CHOICE:"で始まる場合:ボタン選択
        else if (command.StartsWith("CHOICE:"))
        {
            // 選択肢：テキストとジャンプ先ラベルを分離して格納
            string[] rawChoices = DataPatch(command, 7).Split('|');
            List<string> labels = new List<string>();
            List<string> texts = new List<string>();

            foreach (string c in rawChoices)
            {
                if (c.Contains("->"))
                {
                    string[] parts = c.Split(new string[] { "->" }, System.StringSplitOptions.None);
                    texts.Add(parts[0].Trim());   // 表示されるテキスト
                    labels.Add(parts[1].Trim());  // ジャンプ先ラベル
                }
                else
                {
                    texts.Add(c.Trim());
                    labels.Add(""); // ラベルなし
                }
            }

            ShowBranchingChoices(texts.ToArray(), labels.ToArray());
        }
        #endregion
        #region "SHOW_BGI:":背景表示
        else if (command.StartsWith("SHOW_BGI:"))
        {
            string imageName = DataPatch(command, 9);
            StartFadeBackground(imageName, 1.0f); // フェード1秒
        }
        #endregion
        #region その他
        else
        {
            // 未知のコマンド
            Debug.LogWarning("未知のコマンドにゃ?: " + command);
            // 不明なコマンドはスキップ
            NextCommand();
        }
        #endregion
    }
    /// <summary>
    /// 削除文字数文削除した後の文字を返す
    /// </summary>
    /// <param name="Message">送られた文字データ</param>
    /// <param name="Count">削除する文字数</param>
    /// <returns></returns>
    public string DataPatch(string Message, int Count)
    {
        //指定した文字列数を削除して返す
        return Message.Substring(Count);
    }
    /// <summary>
    /// Messageをメッセージウィンドゥに表示する
    /// </summary>
    /// <param name="Message"></param>
    public void TextRender(string Message)
    {
        // テキストを表示
        m_DisplayText.text = Message;
        // 左クリック待ちにする
        m_WaitingForClick = true;

    }
    /// <summary>
    /// キャラクターイメージをresourceから取り出して指定した場所に表示する
    /// </summary>
    /// <param name="Message">resourceフォルダから取り出したい画像名</param>
    /// <param name="No">表示したいパネル番号[2.中央、1.右、0.左]</param>
    public void ImageCharacterRender(string Message, int No)
    {
        // Resourcesから画像読み込み
        Sprite sprite = Resources.Load<Sprite>(m_CharaImageResourcePath + "/" + Message);
        // 表示先があるかどうか
        if (sprite != null)
        {
            //画像(Sprite)を代入
            m_DisplayImage[No].sprite = sprite;
            //画像のα値をMaxにする
            m_DisplayImage[No].color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        }
        else
            Debug.LogWarning("画像が見つからん!: " + Message);
    }

    /// <summary>
    /// 背景画像を切り替える（フェード演出付き）
    /// </summary>
    /// <param name="Message">読み込む背景画像名</param>
    public void ImageBackGroundRender(string Message)
    {
        Sprite newSprite = Resources.Load<Sprite>(m_BackGroundImageResourcePath + "/" + Message);

        if (newSprite != null)
        {
            if (m_backGroundImageNew.sprite != null)
            {
                m_backGroundImageOld.sprite = m_backGroundImageNew.sprite;
                m_backGroundImageOld.color = new Color(1, 1, 1, 1);
            }
            else
            {
                m_backGroundImageOld.color = new Color(1, 1, 1, 0);
            }

            m_backGroundImageNew.sprite = newSprite;

            // 新しい画像は透明にして、フェード完了後に表示する
            m_backGroundImageNew.color = new Color(1, 1, 1, 0);

        }
        else
        {
            Debug.LogWarning("背景画像が見つからん!: " + Message);
        }
    }



    public void BackGroundMusic(string Message)
    {
        // ResourcesからBGM読み込み
        AudioClip clip = Resources.Load<AudioClip>("Audio/" + Message);
        if (clip != null)
        {
            //読み込んだBGMを代入
            m_BgmSource.clip = clip;
            // BGM再生
            m_BgmSource.Play();
        }
        else
        {
            Debug.LogWarning("BGMが見つからん!: " + Message);
        }
    }

    /// <summary>
    /// イメージキャラクターパネルの初期化
    /// </summary>
    /// <param name="No">消したいイメージキャラクターパネル番号</param>
    public void ImageCharacterCLS(int No)
    {
        //該当画像を削除
        m_DisplayImage[No].sprite = null;
        //α値を0にする(透明化)
        m_DisplayImage[No].color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
    }

    // これが「ExecuteCommandの外」に置くラッパー関数
    public void ContinueFromBattle()
    {
        m_WaitingForClick = true;
        m_gameManager.m_storyPanel.SetActive(true);
        Debug.Log("バトル終了、ストーリー再開");
        currentMode = Mode.Normal;

        NextCommand();
    }

    public void HideChoices()
    {
        foreach (Button btn in m_ChoiceButtons)
        {
            btn.gameObject.SetActive(false);
        }
    }

    public void JumpToLabel(string label)
    {
        if (m_LabelLineIndex.ContainsKey(label))
        {
            m_Commands.Clear();
            m_CurrentLineIndex = m_LabelLineIndex[label];

            // ラベル以降のコマンドだけを再び m_Commands に入れる
            for (int i = m_CurrentLineIndex; i < m_AllLines.Count; i++)
            {
                string line = m_AllLines[i].Trim();
                if (!string.IsNullOrWhiteSpace(line) && !line.StartsWith("LABEL:"))
                    m_Commands.Enqueue(line);
            }

            // 次のコマンドへ進行
            NextCommand();
        }
        else
        {
            Debug.LogWarning("ラベルが見つからん!: " + label);
        }
    }

    public void ShowBranchingChoices(string[] texts, string[] labels)
    {
        m_WaitingForClick = false;

        if (texts == null || labels == null)
        {
            Debug.LogError("texts または labels が null です");
            return;
        }

        // 通常の分岐表示処理へ

        for (int i = 0; i < m_ChoiceButtons.Count; i++)
        {
            if (i < texts.Length)
            {
                int index = i;
                m_ChoiceButtons[i].gameObject.SetActive(true);
                var tmpText = m_ChoiceButtons[i].GetComponentInChildren<TextMeshProUGUI>();
                if (tmpText != null)
                {
                    tmpText.text = texts[i];
                }
                else
                {
                    Debug.LogError($"Button[{i}] に TextMeshProUGUI が見つかりません！");
                }

                m_ChoiceButtons[i].onClick.RemoveAllListeners();
                m_ChoiceButtons[i].onClick.AddListener(() =>
                {

                    Debug.Log("選ばれた: " + texts[index]);

                    // 全ボタン非表示
                    HideChoices();

                    // 選択されたボタンに色を残す設定（必要なら）
                    var colors = m_ChoiceButtons[index].colors;
                    colors.normalColor = colors.pressedColor;
                    m_ChoiceButtons[index].colors = colors;

                    // ラベルへジャンプまたはそのまま表示
                    if (!string.IsNullOrEmpty(labels[index]))
                    {
                        JumpToLabel(labels[index]);
                    }
                    else
                    {
                        TextRender($"あなたは「{texts[index]}」を選びました。");
                        m_WaitingForClick = true;
                    }
                });

            }
            else
            {
                m_ChoiceButtons[i].gameObject.SetActive(false);
            }
        }
    }


    /// <summary>
    /// 背景の古い画像をフェードアウトさせる
    /// </summary>
    /// <param name="duration">フェードにかける時間（秒）</param>
    private void StartFadeBackground(string imageName, float duration)
    {
        m_PendingBackground = Resources.Load<Sprite>(m_BackGroundImageResourcePath + "/" + imageName);

        if (m_PendingBackground == null)
        {
            Debug.LogWarning("背景画像が見つからん!: " + imageName);
            NextCommand();
            return;
        }

        // 前の背景がない（＝初回）なら即切り替え
        if (m_backGroundImageNew.sprite == null)
        {
            m_backGroundImageNew.sprite = m_PendingBackground;
            m_backGroundImageNew.color = new Color(1, 1, 1, 1); // 透明にせず即表示
            m_backGroundImageOld.color = new Color(1, 1, 1, 0); // 念のため非表示
            NextCommand(); // 即進行
            return;
        }

        // フェード演出あり
        m_backGroundImageOld.sprite = m_backGroundImageNew.sprite;
        m_backGroundImageOld.color = new Color(1, 1, 1, 1);

        m_backGroundImageNew.sprite = m_PendingBackground;
        m_backGroundImageNew.color = new Color(1, 1, 1, 0);

        m_IsFading = true;
        m_FadeDuration = duration;
        m_FadeElapsed = 0f;
        m_WaitingForClick = false;
    }

}

