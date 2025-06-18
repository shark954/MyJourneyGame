using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using TMPro;

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
        // m_WaitingForClickがtrueの時、左クリックを検出
        if (m_WaitingForClick && Input.GetMouseButtonDown(0))
        {
            // 次のコマンドを実行
            NextCommand();
        }
    }

    /// <summary>
    /// 外部テキストファイル（シナリオ）を読み込む処理
    /// </summary>
    /// <param name="filePath">ファイルパス</param>
    /// 
   /* void LoadScenario(string filePath)
    {
        if (!File.Exists(filePath))
        {
            // ファイルが存在しない場合エラーを表示
            Debug.LogError("シナリオファイルが見つからねぇよ!: " + filePath);
            return;
        }

        // ファイルを1行ずつ読み込む
        string[] lines = File.ReadAllLines(filePath);
        foreach (string line in lines)
        {
            // 空行を除いてキューに追加
            if (!string.IsNullOrWhiteSpace(line))
                m_Commands.Enqueue(line.Trim());
        }
    }*/

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
        #region "SHOW_BGI:":背景表示
        else if (command.StartsWith("SHOW_BGI:"))
        {
            // 9文字削除後、背景を表示する
            ImageBackGroundRender(DataPatch(command, 9));
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
        #region "START_BATTLE"で始まる場合：バトルスタート 
        else if (command.StartsWith("START_BATTLE"))
        {
            // 戦闘システムを呼び出し
            currentMode = Mode.Battle;
            FindObjectOfType<BattleSystem>().StartBattle();
            // 今のクラスでは進行ストップ
            m_WaitingForClick = false;
        }
        #endregion
        #region "ボタン選択"
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

    public void ImageBackGroundRender(string Message)
    {
        Sprite sprite = Resources.Load<Sprite>(m_BackGroundImageResourcePath + "/" + Message);
        if (sprite != null)
        {
            m_backGroundImage.sprite = sprite;
            m_backGroundImage.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        }
        else
        {
            Debug.LogWarning("画像がない:" + Message);
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

    public void ShowChoices(string[] choices)
    {
        m_WaitingForClick = false;

        for (int i = 0; i < m_ChoiceButtons.Count; i++)
        {
            if (i < choices.Length)
            {
                int index = i;
                m_ChoiceButtons[i].gameObject.SetActive(true);
                m_ChoiceButtons[i].GetComponentInChildren<Text>().text = choices[i];
                m_ChoiceButtons[i].onClick.RemoveAllListeners();
                m_ChoiceButtons[i].onClick.AddListener(() =>
                {
                    Debug.Log("選ばれた: " + choices[index]);
                    HideChoices();

                    if (currentMode == Mode.Battle)
                    {
                        FindObjectOfType<BattleSystem>().OnChoiceSelected(choices[index]);
                    }
                    else
                    {
                        // 通常モードの処理
                        TextRender($"あなたは「{choices[index]}」を選びました。");
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
                    HideChoices(); // 他の選択肢を非表示
                    UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(m_ChoiceButtons[index].gameObject);

                    if (!string.IsNullOrEmpty(labels[index]))
                    {
                        // ラベルが設定されていればジャンプ
                        JumpToLabel(labels[index]);
                    }
                    else
                    {
                        // ラベルがない場合はそのまま通常進行
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

}

