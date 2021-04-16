using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 全シーンUI処理
/// </summary>
public class UIManager : MonoBehaviour
{
    public static UIManager UIManager_Instance;

    [SerializeField]
    private Image BackGround = null;
    [SerializeField]
    private GameObject TitleText = null;
    [SerializeField]
    private GameObject GameClearText = null;
    [SerializeField]
    private GameObject GameOverText = null;

    [SerializeField]
    private GameObject TitleSet = null;
    [SerializeField]
    private GameObject PlaySet = null;
    [SerializeField]
    private GameObject SelectArrow = null;

    [SerializeField]
    private Vector3 ArrowPos_Play = default;
    [SerializeField]
    private Vector3 ArrowPos_Tutorial = default;
    [SerializeField]
    private Vector3 ArrowPos_Exit = default;
    [SerializeField]
    private Vector3 ArrowPos_ReTry = default;
    [SerializeField]
    private Vector3 ArrowPos_RetireOrToTitle = default;

    [SerializeField]
    private int FontSize_Default;
    [SerializeField]
    private int FontSize_Selected;

    private List<Text> TitleTextList = new List<Text>();
    private List<Text> PlayTextSetList = new List<Text>();

    public enum DisplayColor
    {
        Black,
        White
    }

    public enum DisplayText
    {
        Title,
        Clear,
        Over,
        None
    }
    public DisplayText NowDisplayText { get; set; }

    public enum TitleTextType
    {
        Play = 0,
        Tutorial = -1,
        Exit = -2
    }

    public enum PlayTextType
    {
        Retry = 0,
        RetireOrToTitle = -1
    }

    public int PassSelectCount { get; set; } = 0;

    public bool PassCanUIOperation { get; set; }

    /// <summary>
    /// 通常フォントサイズを渡す
    /// </summary>
    public int Pass_FontSize_Default
    {
        get { return FontSize_Default; }
        private set { FontSize_Default = value; }
    }

    /// <summary>
    /// 選択時フォントサイズを渡す
    /// </summary>
    public int Pass_FontSize_Selected
    {
        get { return FontSize_Selected; }
        private set { FontSize_Selected = value; }
    }

    void Awake()
    {
        //インスタンス生成と同時に既に存在する場合は重複しないよう削除
        if (UIManager_Instance == null)
        {
            UIManager_Instance = GetComponent<UIManager>();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        Text text;

        //タイトルのテキストをリストに格納
        foreach (Transform titletext in TitleSet.transform)
        {
            text = titletext.GetComponent<Text>();
            TitleTextList.Add(text);
        }

        //プレイシーンのテキストをリストに格納
        foreach (Transform playtext in PlaySet.transform)
        {
            text = playtext.GetComponent<Text>();
            PlayTextSetList.Add(text);
        }

        BackGround.gameObject.SetActive(false);
        GameClearText.SetActive(false);
        GameOverText.SetActive(false);
        PlaySet.SetActive(false);
    }

    void Update()
    {
        if (GameManager.GameManager_Instance.PassNowState == GameManager.GameState.Play)
        {
            PassCanUIOperation = false;
        }
        else
        {
            PassCanUIOperation = true;
        }

        //UIを操作するタイミングでのみ操作可
        if (PassCanUIOperation)
        {
            ControlManager.ControlManager_Instance.InputArrow(ControlManager.ArrowType.Select);

            switch (GameManager.GameManager_Instance.PassNowState)
            {
                case GameManager.GameState.Title:
                    NowDisplayText = DisplayText.Title;
                    SelectMenu(-2);
                    TitleItemSelect();
                    break;
                case GameManager.GameState.GameClear:
                    NowDisplayText = DisplayText.Clear;
                    SelectMenu(-1);
                    PlayItemSelect();
                    break;
                case GameManager.GameState.GameOver:
                    NowDisplayText = DisplayText.Over;
                    SelectMenu(-1);
                    PlayItemSelect();
                    break;
                default:
                    break;
            }
        }
    }

    /// <summary>
    /// キー入力でのタイトル処理
    /// </summary>
    private void SelectMenu(int count)
    {
        //入力をカウント
        PassSelectCount += ControlManager.ControlManager_Instance.VerticalInput;

        //最上位へ移動したら最下位へ、最下位へ移動したら最上位へ移動させる
        if (PassSelectCount < count)
        {
            PassSelectCount = 0;
        }
        if (PassSelectCount > 0)
        {
            PassSelectCount = count;
        }
    }

    /// <summary>
    /// タイトルのテキストの表示非表示
    /// </summary>
    /// <param name="display"></param>
    public void TitleItemDisplay(bool display)
    {
        PassSelectCount = 0;
        BackGround.gameObject.SetActive(false);
        TitleText.SetActive(display);
        TitleSet.SetActive(display);
        SelectArrow.SetActive(display);
    }

    /// <summary>
    /// ゲームプレイ時（ゲームクリア、ゲームオーバー）のテキストの表示非表示
    /// </summary>
    /// <param name="display"></param>
    /// <param name="textset"></param>
    public void PlayItemDisplay(DisplayText textset)
    {
        PassSelectCount = 0;

        switch (textset)
        {
            case DisplayText.Clear:
                PlayItemCollection(true);
                BackGround.color = new Color((float)DisplayColor.White, (float)DisplayColor.White, (float)DisplayColor.White);
                GameClearText.SetActive(true);
                GameOverText.SetActive(false);
                //ゲームクリア画面で使用するのはテキスト「ToTitle」なので「Retire」を非表示
                foreach (Text text in PlayTextSetList)
                {
                    text.gameObject.SetActive(true);
                    if (text.transform.name == "Text_Retry")
                    {
                        text.color = new Color((float)DisplayColor.Black, (float)DisplayColor.Black, (float)DisplayColor.Black);
                    }
                    if (text.transform.name == "Text_Retire")
                    {
                        text.gameObject.SetActive(false);
                    }
                }
                break;
            case DisplayText.Over:
                PlayItemCollection(true);
                BackGround.color = new Color((float)DisplayColor.Black, (float)DisplayColor.Black, (float)DisplayColor.Black);
                GameOverText.SetActive(true);
                GameClearText.SetActive(false);
                //ゲームオーバー画面で使用するのはテキスト「Retire」なので「ToTitle」を非表示
                foreach (Text text in PlayTextSetList)
                {
                    text.gameObject.SetActive(true);
                    if (text.transform.name == "Text_Retry")
                    {
                        text.color = new Color((float)DisplayColor.White, (float)DisplayColor.White, (float)DisplayColor.White);
                    }
                    if (text.transform.name == "Text_ToTitle")
                    {
                        text.gameObject.SetActive(false);
                    }
                }
                break;
            case DisplayText.None:
                PlayItemCollection(false);
                GameOverText.SetActive(false);
                GameClearText.SetActive(false);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// ゲームプレイ時のテキストを一括で操作するための塊
    /// </summary>
    /// <param name="display"></param>
    private void PlayItemCollection(bool display)
    {
        BackGround.gameObject.SetActive(display);
        SelectArrow.SetActive(display);
        PlaySet.SetActive(display);
    }

    /// <summary>
    /// タイトル時UIの各項目ごとに選択された場合の処理
    /// </summary>
    private void TitleItemSelect()
    {
        switch (PassSelectCount)
        {
            case (int)TitleTextType.Play:
                SelectItem("Text_Play", ArrowPos_Play, GameManager.GameState.Play);
                break;
            case (int)TitleTextType.Tutorial:
                SelectItem("Text_Tutorial", ArrowPos_Tutorial, GameManager.GameState.Tutorial);
                break;
            case (int)TitleTextType.Exit:
                SelectItem("Text_Exit", ArrowPos_Exit, GameManager.GameState.Exit);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// ゲームプレイ時UIの各項目ごとに選択された場合の処理
    /// </summary>
    private void PlayItemSelect()
    {
        switch (PassSelectCount)
        {
            case (int)PlayTextType.Retry:
                SelectItem("Text_Retry", ArrowPos_ReTry, GameManager.GameState.Play);
                break;
            case (int)PlayTextType.RetireOrToTitle:
                //ゲームクリアとゲームオーバーで表示させるテキストを変える
                string type = null;
                switch (NowDisplayText)
                {
                    case DisplayText.Clear:
                        type = "Text_ToTitle";
                        break;
                    case DisplayText.Over:
                        type = "Text_Retire";
                        break;
                }
                SelectItem(type, ArrowPos_RetireOrToTitle, GameManager.GameState.Title);
                break;
        }
    }

    /// <summary>
    /// 選択されているテキスト処理
    /// </summary>
    /// <param name="textname"></param>
    /// <param name="arrowposition"></param>
    /// <param name="state"></param>
    private void SelectItem(string textname, Vector3 arrowposition, GameManager.GameState state)
    {
        //現在表示されているテキストの種類を取得
        List<Text> nowtext = new List<Text>();
        switch (NowDisplayText)
        {
            case DisplayText.Title:
                nowtext = TitleTextList;
                break;
            case DisplayText.Clear:
                nowtext = PlayTextSetList;
                break;
            case DisplayText.Over:
                nowtext = PlayTextSetList;
                break;
            default:
                break;
        }

        //選択されているテキストのサイズを大きくし、それ以外をデフォルトサイズに戻す
        foreach (Text text in nowtext)
        {
            if (text.name == textname)
            {
                text.fontSize = FontSize_Selected;
            }
            else
            {
                text.fontSize = FontSize_Default;
            }
        }

        //項目の隣に矢印を移動
        SelectArrow.GetComponent<RectTransform>().anchoredPosition = arrowposition;

        //項目選択時の処理
        //Exitが選択された場合のみそのまま終了フラグを送る
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (state == GameManager.GameState.Exit)
            {
                GameManager.GameManager_Instance.WantQuit = true;
            }
            else
            {
                GameManager.GameManager_Instance.TransitionGameState(state);
            }
        }
    }
}
