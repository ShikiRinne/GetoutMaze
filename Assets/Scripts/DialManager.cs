using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


/// <summary>
/// 画面に表示されるUIを管理する
/// </summary>
public class DialManager : MonoBehaviour
{
    private MazeGenerateManager MGM;

    [SerializeField]
    private Camera UICamera;
    [SerializeField]
    private GameObject DisplayMemo;
    [SerializeField]
    private GameObject DialPadLock;
    [SerializeField]
    private GameObject ArrowSet;
    [SerializeField]
    private GameObject Reticle_Parent;
    [SerializeField]
    private Text Reticle_Default;
    [SerializeField]
    private Text Reticle_Spray;

    private bool MemoDisplay = false;

    private List<GameObject> DisplayMemosList = new List<GameObject>();
    public List<GameObject> PassDialObject { get; set; } = new List<GameObject>();

    private List<int> ExitKeyCode = new List<int>();
    public List<int> PassDialNumberList { get; set; } = new List<int>();

    public int PassSelectDial { get; set; } = 0;
    public int GetPickMemoCount { get; set; } = 0;
    public bool IsOperateDial { get; set; }
    public bool IsTouchiGoal { get; set; }
    public bool PassCanControl { get; set; }

    public enum ReticleType
    {
        DefaultType,
        SprayType,
        DontUse
    }

    private enum DialRotateDirection
    {
        Up,
        Down
    }

    void Start()
    {
        MGM = GetComponent<MazeGenerateManager>();
        IsTouchiGoal = false;
        IsOperateDial = false;
        PassCanControl = true;

        //メモをリストとして保存、非アクティブ化
        foreach (Transform memo in DisplayMemo.transform)
        {
            DisplayMemosList.Add(memo.gameObject);
            memo.gameObject.SetActive(false);
        }

        //不要なオブジェクトを非アクティブ化
        DisplayMemo.SetActive(false);
        //DialPadLock.SetActive(false);
        //ArrowSet.SetActive(false);
    }

    void Update()
    {
        //スペースキーで取得済みメモの表示、非表示切り替え
        DisplayMemo.SetActive(MemoDisplay);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            MemoDisplay = !MemoDisplay;
        }

        if (IsTouchiGoal)
        {
            GameManager.GameManager_Instance.UseCursor(true);
            DialPadLock.SetActive(true);
            ArrowSet.SetActive(true);
        }
        else
        {
            GameManager.GameManager_Instance.UseCursor(false);
            DialPadLock.SetActive(false);
            ArrowSet.SetActive(false);
        }
    }

    /// <summary>
    /// 取得したメモの表示
    /// </summary>
    /// <param name="Pickup"></param>
    public void PickupMemo()
    {
        DisplayMemo.transform.GetChild(GetPickMemoCount).gameObject.SetActive(true);
        DisplayMemosList[GetPickMemoCount].transform.GetChild(0).GetComponent<Text>().text = ExitKeyCode[GetPickMemoCount].ToString();
        GetPickMemoCount++;
    }

    /// <summary>
    /// レティクルの種類の切り替え
    /// </summary>
    /// <param name="type"></param>
    public void ChangeReticleType(ReticleType type)
    {
        switch (type)
        {
            case ReticleType.DefaultType:
                if (!Reticle_Parent.activeSelf)
                {
                    Reticle_Parent.SetActive(true);
                }
                Reticle_Spray.gameObject.SetActive(false);
                Reticle_Default.gameObject.SetActive(true);
                break;
            case ReticleType.SprayType:
                if (!Reticle_Parent.activeSelf)
                {
                    Reticle_Parent.SetActive(true);
                }
                Reticle_Default.gameObject.SetActive(false);
                Reticle_Spray.gameObject.SetActive(true);
                break;
            case ReticleType.DontUse:
                Reticle_Parent.gameObject.SetActive(false);
                break;
            default:
                break;
        }
    }


    /// <summary>
    /// ダイヤル解除判定
    /// </summary>
    public void JudgeUnlock()
    {
        //ダイヤルの数値と脱出するキーコードが一致しているか判定
        bool isUnlock = PassDialNumberList.SequenceEqual(ExitKeyCode);
        Debug.Log("isUnlock = " + isUnlock);

        //一致ならGameClear、不一致ならそのまま非表示にして再開
        if (isUnlock)
        {
            GameManager.GameManager_Instance.TransitionGameState(GameManager.GameState.GameClear);
            Debug.Log("Unlock");
        }
        else
        {
            Debug.Log("Miss");
            IsTouchiGoal = false;
            IsOperateDial = false;
            PassCanControl = true;
        }
    }

    /// <summary>
    /// 矢印を選択されたダイヤルの位置へ移動
    /// </summary>
    /// <param name="position"></param>
    public void MoveDialArrow(int position)
    {
        switch (position)
        {
            case 0:
                ArrowSet.GetComponent<RectTransform>().localPosition = new Vector3(-44.0f, 0f, -620f);
                break;
            case 1:
                ArrowSet.GetComponent<RectTransform>().localPosition = new Vector3(-15.0f, 0f, -620f);
                break;
            case 2:
                ArrowSet.GetComponent<RectTransform>().localPosition = new Vector3(15.0f, 0f, -620f);
                break;
            case 3:
                ArrowSet.GetComponent<RectTransform>().localPosition = new Vector3(44.0f, 0f, -620f);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// キー入力でのダイヤル選択
    /// </summary>
    private void ButtonDialSelected()
    {
        ControlManager.ControlManager_Instance.InputArrow(ControlManager.ArrowType.Select);
        if (ControlManager.ControlManager_Instance.HorizontalInput != 0)
        {
            PassSelectDial += ControlManager.ControlManager_Instance.HorizontalInput;
        }

        //0未満になったら3（ダイヤル4）に、3を超えたら0に
        if (PassSelectDial < 0)
        {
            PassSelectDial = 3;
        }
        if (PassSelectDial > 3)
        {
            PassSelectDial = 0;
        }

        ButtonDialRotate(PassSelectDial);
        MoveDialArrow(PassSelectDial);
    }

    /// <summary>
    /// キー入力でのダイヤル回転
    /// </summary>
    /// <param name="dialcnt"></param>
    private void ButtonDialRotate(int dialcnt)
    {
        //数値を加減算
        PassDialNumberList[dialcnt] += ControlManager.ControlManager_Instance.VerticalInput;
        
        //入力が正
        if (ControlManager.ControlManager_Instance.VerticalInput > 0)
        {
            //回転
            PassDialObject[dialcnt].transform.Rotate(0f, 0f, -36f);
            //9を超えたら0へ
            if (PassDialNumberList[dialcnt] > 9)
            {
                PassDialNumberList[dialcnt] = 0;
            }
        }
        //入力が負
        if (ControlManager.ControlManager_Instance.VerticalInput < 0)
        {
            //回転
            PassDialObject[dialcnt].transform.Rotate(0f, 0f, 36f);
            //0を下回ったら9へ
            if (PassDialNumberList[dialcnt] < 0)
            {
                PassDialNumberList[dialcnt] = 9;
            }
        }

        //ダイヤル解除判定
        if (Input.GetKeyDown(KeyCode.Return))
        {
            JudgeUnlock();
        }
    }
}
