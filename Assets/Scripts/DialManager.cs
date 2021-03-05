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
    private GameObject DisplayList;
    [SerializeField]
    private GameObject DialPadLock;
    [SerializeField]
    private GameObject ArrowSet;
    [SerializeField]
    private GameObject UpArrow;
    [SerializeField]
    private GameObject DownArrow;
    [SerializeField]
    private GameObject Reticle_Parent;
    [SerializeField]
    private Text Reticle_Default;
    [SerializeField]
    private Text Reticle_Spray;
    [SerializeField]
    private LayerMask MaskLayer;

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

        //ダイヤルを初期化
        for (int i = 0; i < MGM.PassTotalSplitMemos; ++i)
        {
            PassDialNumberList.Add(0);
        }

        //メモをリストとして保存、非アクティブ化
        for (int i = 0; i < MGM.PassTotalSplitMemos; ++i)
        {
            DisplayMemosList.Add(DisplayList.transform.GetChild(i).gameObject);
            DisplayMemosList[i].SetActive(false);
        }

        //各ダイヤルをリストとして保存、非アクティブ化
        for (int i = 0; i < DialPadLock.transform.childCount; ++i)
        {
            if (DialPadLock.transform.GetChild(i).CompareTag("Dial"))
            {
                PassDialObject.Add(DialPadLock.transform.GetChild(i).gameObject);
            }
        }

        //脱出するキーの設定
        SetExitKeyCode();
        Debug.Log("Code = " + ExitKeyCode[0] + ExitKeyCode[1] + ExitKeyCode[2] + ExitKeyCode[3]);

        //ダイヤルを操作する矢印の初期位置の設定
        ArrowSet.GetComponent<RectTransform>().localPosition = new Vector3(-44.0f, 0f, -620f);

        //不要なオブジェクトを非アクティブ化
        DisplayList.SetActive(false);
        DialPadLock.SetActive(false);
        ArrowSet.SetActive(false);
    }

    void Update()
    {
        Debug.Log("IsTouchGoal:" + IsOperateDial);

        //スペースキーで取得済みメモの表示、非表示切り替え
        DisplayList.SetActive(MemoDisplay);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            MemoDisplay = !MemoDisplay;
        }

        //ゴールに触れたとき、UIの操作を行っていないとき
        //if (IsTouchiGoal/* && !UIManager.UIManager_Instance.PassCanUIOperation*/)
        //{
        //    PassCanControl = false;
        //    GameManager.GameManager_Instance.UseCursor(true);
        //    ChangeReticleType(ReticleType.DontUse);
        //    DialPadLock.SetActive(true);
        //    ArrowSet.SetActive(true);
        //    MouseDialSelected();
        //    ButtonDialSelected();
        //    MoveArrow(SelectDial);
        //}
        //else
        //{
        //    GameManager.GameManager_Instance.UseCursor(false);
        //    ChangeReticleType(ReticleType.DefaultType);
        //    DialPadLock.SetActive(false);
        //    ArrowSet.SetActive(false);
        //}
        DialSetActive(IsOperateDial);
    }

    /// <summary>
    /// 取得したメモの表示
    /// </summary>
    /// <param name="Pickup"></param>
    public void PickupMemo()
    {
        DisplayList.transform.GetChild(GetPickMemoCount).gameObject.SetActive(true);
        DisplayMemosList[GetPickMemoCount].transform.GetChild(0).GetComponent<Text>().text = ExitKeyCode[GetPickMemoCount].ToString();
        if (GetPickMemoCount < MGM.PassTotalSplitMemos)
        {
            GetPickMemoCount++;
        }
    }

    /// <summary>
    /// 脱出するキーコードの設定
    /// </summary>
    private void SetExitKeyCode()
    {
        for (int i = 0; i < MGM.PassTotalSplitMemos; ++i)
        {
            ExitKeyCode.Add(Random.Range(1, 10));
        }
    }

    /// <summary>
    /// ダイヤルの操作
    /// </summary>
    /// <param name="active"></param>
    public void DialSetActive(bool active)
    {
        PassCanControl = !active;
        GameManager.GameManager_Instance.UseCursor(active);
        DialPadLock.SetActive(active);
        ArrowSet.SetActive(active);

        if (active)
        {
            ChangeReticleType(ReticleType.DontUse);
            MouseDialSelected();
            ButtonDialSelected();
            MoveDialArrow(PassSelectDial);
        }
        else
        {
            ChangeReticleType(ReticleType.DefaultType);
        }
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
    /// マウスでのダイヤル選択
    /// </summary>
    private void MouseDialSelected()
    {
        GameObject select;
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = UICamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit3D;
            RaycastHit2D hit2D = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, MaskLayer);

            //クリックされた位置にあるダイヤルを参照
            if (Physics.Raycast(ray, out hit3D))
            {
                if (hit3D.transform.gameObject.CompareTag("Dial"))
                {
                    select = hit3D.transform.gameObject;

                    //各ダイヤルの処理
                    switch (select.name)
                    {
                        case "Dial1":
                            PassSelectDial = 0;
                            if (hit2D)
                            {
                                PassDialNumberList[0] = MouseDialRotate(hit2D.transform.gameObject, select, PassDialNumberList[0]);
                            }
                            break;
                        case "Dial2":
                            PassSelectDial = 1;
                            if (hit2D)
                            {
                                PassDialNumberList[1] = MouseDialRotate(hit2D.transform.gameObject, select, PassDialNumberList[1]);
                            }
                            break;
                        case "Dial3":
                            PassSelectDial = 2;
                            if (hit2D)
                            {
                                PassDialNumberList[2] = MouseDialRotate(hit2D.transform.gameObject, select, PassDialNumberList[2]);
                            }
                            break;
                        case "Dial4":
                            PassSelectDial = 3;
                            if (hit2D)
                            {
                                PassDialNumberList[3] = MouseDialRotate(hit2D.transform.gameObject, select, PassDialNumberList[3]);
                            }
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    JudgeUnlock();
                }
            }
        }
    }

    /// <summary>
    /// マウスでのダイヤル回転
    /// </summary>
    /// <param name="hit2D"></param>
    /// <param name="select"></param>
    private int MouseDialRotate(GameObject arrow, GameObject select, int dialnum)
    {
        //上下どちらのダイヤルをクリックしているか
        switch (arrow.name)
        {
            case "UpArrow":
                //回転
                select.transform.Rotate(0f, 0f, -36f);

                dialnum += 1;
                if (dialnum > 9)
                {
                    dialnum = 0;
                }
                break;
            case "DownArrow":
                //回転
                select.transform.Rotate(0f, 0f, 36f);

                dialnum -= 1;
                if (dialnum < 0)
                {
                    dialnum = 9;
                }
                break;
            default:
                break;
        }
        return dialnum;
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
            //DialSetActive(false);
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
