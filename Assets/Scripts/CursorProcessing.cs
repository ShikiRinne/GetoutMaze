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
    /// 項目にカーソルが重なったとき
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
    /// 項目をクリックしたとき
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

    public void PlayItemClick()
    {
        switch (gameObject.name)
        {
            case "Text_Retry":
                Debug.Log("ReTry");
                break;
            case "Text_Retire":
                Debug.Log("Retire");
                break;
            case "Text_ToTitle":
                GameManager.GameManager_Instance.TransitionGameState(GameManager.GameState.Title);
                break;
        }
    }
}
