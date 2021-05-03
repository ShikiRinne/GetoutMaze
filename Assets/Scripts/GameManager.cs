using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// ゲームの進行を管理する
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager GameManager_Instance;

    private Vector2Int WindowSize = new Vector2Int(1920, 1080);
    private int FrameRate = 60;

    public bool WantQuit { get; set; } = false;
    public bool WantReset { get; set; } = false;
    public bool CanPlayerMove { get; set; } = false;
    public bool IsEnemyStop { get; set; } = true;

    public enum GameState
    {
        Title,
        Play,
        Tutorial,
        GameClear,
        GameOver,
        Exit
    }
    public GameState PassNowState { get; private set; } //ゲームの現在の状態
    public GameState PassFromState { get; set; }        //どこから呼び出すかの分岐

    void Awake()
    {
        //インスタンス生成と同時に既に存在する場合は重複しないよう削除
        if (GameManager_Instance == null)
        {
            GameManager_Instance = GetComponent<GameManager>();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        //画面サイズとフレームレートを固定
        Screen.SetResolution(WindowSize.x, WindowSize.y, true, FrameRate);
    }

    void Start()
    {
        StartCoroutine(PlayStart());
    }

    void Update()
    {
        if (WantQuit || Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Application.Quit");
            Application.Quit();
        }
    }

    /// <summary>
    /// ゲーム開始時の最初のフェードイン
    /// </summary>
    /// <returns></returns>
    private IEnumerator PlayStart()
    {
        StartCoroutine(FadeScreen.FadeScreen_Instance.FadeExecution(FadeScreen.FadeType.IN, 2.0f, FadeScreen.FadeColor.Black));
        yield return new WaitForSeconds(FadeScreen.FadeScreen_Instance.PassFadeTime);
        FadeScreen.FadeScreen_Instance.PassFadeObject.SetActive(false);
        ControlManager.ControlManager_Instance.CanControl = true;
    }

    /// <summary>
    /// 次のシーンを決定
    /// </summary>
    /// <param name="next"></param>
    public void TransitionGameState(GameState next)
    {
        PassNowState = next;
        switch (next)
        {
            case GameState.Title:
                UIManager.UIManager_Instance.PassCanUIOperation = true;
                StartCoroutine(ChangeGameScene("Title", FadeScreen.FadeColor.Black));
                break;
            case GameState.Play:
                UIManager.UIManager_Instance.PassCanUIOperation = false;
                StartCoroutine(ChangeGameScene("Play", FadeScreen.FadeColor.Black));
                break;
            case GameState.Tutorial:
                break;
            case GameState.GameClear:
                UIManager.UIManager_Instance.PassCanUIOperation = true;
                StartCoroutine(ChangeGameScene("GameClear", FadeScreen.FadeColor.White));
                break;
            case GameState.GameOver:
                UIManager.UIManager_Instance.PassCanUIOperation = true;
                StartCoroutine(ChangeGameScene("GameOver", FadeScreen.FadeColor.Black));
                break;
            case GameState.Exit:
                Debug.LogError("不正な操作");
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// シーンの切り替えとフェード
    /// </summary>
    /// <param name="state"></param>
    /// <returns></returns>
    public IEnumerator ChangeGameScene(string state, FadeScreen.FadeColor color)
    {
        //フェード画像をアクティブ化し、フェードを実行
        FadeScreen.FadeScreen_Instance.PassFadeObject.SetActive(true);
        StartCoroutine(FadeScreen.FadeScreen_Instance.FadeExecution(FadeScreen.FadeType.OUT, 2.0f, color));
        //フェード中操作不可
        ControlManager.ControlManager_Instance.CanControl = false;
        //フェードしている時間待機
        yield return new WaitForSeconds(FadeScreen.FadeScreen_Instance.PassFadeTime);

        switch (state)
        {
            case "Title":
                UIManager.UIManager_Instance.PlayItemDisplay(UIManager.DisplayText.None);
                UIManager.UIManager_Instance.TitleItemDisplay(true);
                PassFromState = GameState.Title;
                SceneManager.LoadScene(state);
                break;
            case "Play":
                UseCursor(false);
                CanPlayerMove = true;
                IsEnemyStop = false;
                //どこからPlayシーンが呼び出されたかで処理を変更する
                switch (PassFromState)
                {
                    //タイトルからのスタート
                    case GameState.Title:
                        UIManager.UIManager_Instance.TitleItemDisplay(false);
                        SceneManager.LoadScene(state);
                        break;
                    //ゲームクリアからのリトライ
                    //シーンを再度呼び出して新たなステージで再開
                    case GameState.GameClear:
                        UIManager.UIManager_Instance.PlayItemDisplay(UIManager.DisplayText.None);
                        SceneManager.LoadScene(state);
                        break;
                    //ゲームオーバーからのリトライ
                    //シーンを呼び出さず進行状況をそのままに再開
                    case GameState.GameOver:
                        UIManager.UIManager_Instance.PlayItemDisplay(UIManager.DisplayText.None);
                        WantReset = true;
                        break;
                    default:
                        break;
                }
                break;
            case "Tutorial":
                break;
            case "GameClear":
                UseCursor(true);
                PassFromState = GameState.GameClear;
                UIManager.UIManager_Instance.PlayItemDisplay(UIManager.DisplayText.Clear);
                break;
            case "GameOver":
                UseCursor(true);
                PassFromState = GameState.GameOver;
                UIManager.UIManager_Instance.PlayItemDisplay(UIManager.DisplayText.Over);
                break;
            case "Exit":
                Debug.LogError("不正な操作");
                break;
            default:
                break;
        }

        StartCoroutine(FadeScreen.FadeScreen_Instance.FadeExecution(FadeScreen.FadeType.IN, 2.0f, color));
        yield return new WaitForSeconds(FadeScreen.FadeScreen_Instance.PassFadeTime);

        ControlManager.ControlManager_Instance.CanControl = true;
        FadeScreen.FadeScreen_Instance.PassFadeObject.SetActive(false);
    }

    /// <summary>
    /// マウスカーソルの使用
    /// </summary>
    /// <param name="isUse"></param>
    public void UseCursor(bool isUse)
    {
        Cursor.visible = isUse;
        if (isUse)
        {
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
