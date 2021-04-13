using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorProcessing : MonoBehaviour
{
    void Start()
    {
        
    }

    /// <summary>
    /// マウスでのタイトル処理
    /// カーソルが重なったテキストのサイズを変更
    /// EventTrigger_PointerEnter
    /// </summary>
    public void TitleItemOverLap()
    {
        switch (gameObject.name)
        {
            case "Text_Play":
                UIManager.UIManager_Instance.PassSelectCount = (int)UIManager.TitleTextType.Play;
                break;
            case "Text_Tutorial":
                UIManager.UIManager_Instance.PassSelectCount = (int)UIManager.TitleTextType.Tutorial;
                break;
            case "Text_Exit":
                UIManager.UIManager_Instance.PassSelectCount = (int)UIManager.TitleTextType.Exit;
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// マウスでのタイトル処理
    /// クリックした項目ごとにシーンを遷移
    /// EventTrigger_PointerClick
    /// </summary>
    public void TitleItemClick()
    {
        switch (gameObject.name)
        {
            case "Text_Play":
                GameManager.GameManager_Instance.TransitionGameState(GameManager.GameState.Play);
                break;
            case "Text_Tutorial":
                break;
            case "Text_Exit":
                GameManager.GameManager_Instance.WantQuit = true;
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// マウスでのプレイシーン処理
    /// ゲームオーバー、ゲームクリア時のテキストのサイズを変更
    /// EventTrigger_PointerEnter
    /// </summary>
    public void PlayItemOverLap()
    {
        switch (gameObject.name)
        {
            case "Text_Retry":
                UIManager.UIManager_Instance.PassSelectCount = (int)UIManager.PlayTextType.Retry;
                break;
            case "Text_Retire":
                UIManager.UIManager_Instance.PassSelectCount = (int)UIManager.PlayTextType.RetireOrToTitle;
                break;
            case "Text_ToTitle":
                UIManager.UIManager_Instance.PassSelectCount = (int)UIManager.PlayTextType.RetireOrToTitle;
                break;
        }
    }

    /// <summary>
    /// マウスでのプレイシーン処理
    /// ゲームオーバー、ゲームクリア時の項目ごとにシーン処理
    /// EventTrigger_PointerClick
    /// </summary>
    public void PlayItemClick()
    {
        switch (gameObject.name)
        {
            case "Text_Retry":
                GameManager.GameManager_Instance.TransitionGameState(GameManager.GameState.Play);
                Debug.Log("ReTry");
                break;
            case "Text_Retire":
                GameManager.GameManager_Instance.TransitionGameState(GameManager.GameState.Title);
                Debug.Log("Retire");
                break;
            case "Text_ToTitle":
                GameManager.GameManager_Instance.TransitionGameState(GameManager.GameState.Title);
                break;
        }
    }
}
