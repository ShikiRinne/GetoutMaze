using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// 画面に表示されるUIを管理する
/// </summary>
public class HUDManager : MonoBehaviour
{
    private CameraFlash CF;

    [SerializeField]
    private GameObject DisplayMemo = default;
    [SerializeField]
    private GameObject DialPadLock = default;
    [SerializeField]
    private GameObject ArrowSet = default;
    [SerializeField]
    private GameObject Reticle_Parent = default;
    [SerializeField]
    private GameObject BelongingsUI = default;

    [SerializeField]
    private Text PsylliumCountText = null;
    [SerializeField]
    private Text Reticle_Default = null;
    [SerializeField]
    private Text Reticle_Spray = null;

    [SerializeField]
    private float SizeExpantion = 0f;
    private float ExpandedUIPos = 0f;
    private float ShiftedUIPos = 0f;
    private float HandDefaultPos = 0f;
    private float PsylliumDefaultPos = 0f;
    private float CameraDefaultPos = 0f;

    private string InputStr = null;

    private bool MemoDisplay = false;
    private bool HaveMemo = false;

    private List<GameObject> DisplayMemosList = new List<GameObject>();
    private List<Image> BelongingsUIList = new List<Image>();
    private List<int> ExitKeyCode = new List<int>();

    public int GetPickMemoCount { get; set; } = 0;
    public int PassPsylliumCount { get; set; } = 5;
    public bool IsTouchiGoal { get; set; } = false;

    public enum ReticleType
    {
        DefaultType,
        SprayType,
        DontUse
    }

    public enum BelongingsType
    {
        Hand,
        Psyllium,
        Camera,
        None
    }
    public BelongingsType BType { get; set; } = BelongingsType.None;

    private enum ShiftDirection
    {
        RShift,
        LShift,
        None
    }

    void Start()
    {
        //メモをリストとして保存、非アクティブ化
        foreach (Transform memo in DisplayMemo.transform)
        {
            DisplayMemosList.Add(memo.gameObject);
            ExitKeyCode.Add(Random.Range(0, 10));
            memo.gameObject.SetActive(false);
        }

        //所持品UIをリストとして保存、各UIのデフォルト位置を各変数に保存
        foreach (Transform ui in BelongingsUI.transform)
        {
            BelongingsUIList.Add(ui.GetComponent<Image>());

            switch (ui.name)
            {
                case "Hand":
                    HandDefaultPos = ui.GetComponent<Image>().rectTransform.anchoredPosition.x;
                    break;
                case "Psyllium":
                    PsylliumDefaultPos = ui.GetComponent<Image>().rectTransform.anchoredPosition.x;
                    break;
                case "Camera":
                    CameraDefaultPos = ui.GetComponent<Image>().rectTransform.anchoredPosition.x;
                    break;
                default:
                    break;
            }
        }

        //初期値を1（Hand）に設定
        InputStr = "1";
        BelongingsUIOps(InputStr);

        CF = GameObject.Find("Camera").GetComponent<CameraFlash>();
        DialPadLock.GetComponent<DialOperation>().StartDialSetting();
        ArrowSet.GetComponent<ArrowOperation>().StertArrowSetting();

        //不要なオブジェクトを非アクティブ化
        DisplayMemo.SetActive(false);
        DialPadLock.SetActive(false);
        ArrowSet.SetActive(false);

        //debug
        Debug.Log("KeyCode:" + ExitKeyCode[0] + ExitKeyCode[1] + ExitKeyCode[2] + ExitKeyCode[3]);
    }

    void Update()
    {
        DisplayMemo.SetActive(MemoDisplay);
        if (ControlManager.ControlManager_Instance.CanControl)
        {
            //所持品の切り替え
            if (Input.anyKeyDown && !CF.IsReady)
            {
                InputStr = Input.inputString;
                BelongingsUIOps(InputStr);
            }

            //メモの表示非表示切り替え
            //メモを持っていなければ処理しない
            if (Input.GetKeyDown(KeyCode.Space) && HaveMemo)
            {
                MemoDisplay = !MemoDisplay;
            }
        }

        //サイリウムの所持本数表示
        PsylliumCountText.text = "x" + PassPsylliumCount;

        //ゴール到達時
        if (IsTouchiGoal)
        {
            DisplayDial(true);
        }
    }

    /// <summary>
    /// ダイヤル表示処理
    /// </summary>
    /// <param name="isdisplay"></param>
    private void DisplayDial(bool isdisplay)
    {
        GameManager.GameManager_Instance.UseCursor(isdisplay);
        GameManager.GameManager_Instance.CanPlayerMove = !isdisplay;
        GameManager.GameManager_Instance.IsEnemyStop = isdisplay;
        DialPadLock.SetActive(isdisplay);
    }

    /// <summary>
    /// 取得したメモの表示
    /// </summary>
    /// <param name="Pickup"></param>
    public void PickupMemo()
    {
        if (!HaveMemo)
        {
            HaveMemo = true;
        }

        //メモ用紙のアクティブ化
        DisplayMemo.transform.GetChild(GetPickMemoCount).gameObject.SetActive(true);
        //キーコードをメモに反映
        DisplayMemosList[GetPickMemoCount].transform.GetChild(0).GetComponent<Text>().text = ExitKeyCode[GetPickMemoCount].ToString();
        //カウントの追加
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
                Reticle_Parent.SetActive(false);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// ダイヤル解除判定
    /// </summary>
    public void JudgeUnlock(List<int> dialnum)
    {
        //ダイヤルの数値と脱出するキーコードが一致しているか判定
        bool isUnlock = dialnum.SequenceEqual(ExitKeyCode);

        //一致ならGameClear、不一致ならそのまま非表示にして再開
        if (isUnlock)
        {
            DialPadLock.GetComponent<DialOperation>().PlayShackleSound();
            GameManager.GameManager_Instance.TransitionGameState(GameManager.GameState.GameClear);
        }
        else
        {
            IsTouchiGoal = false;
            DisplayDial(false);
        }
    }

    /// <summary>
    /// 手に持つもののUI切り替え
    /// </summary>
    private void BelongingsUIOps(string str)
    {
        switch (str)
        {
            //手のUIを拡大し、他UIを縮小してずらす
            case "1":
                if (BType != BelongingsType.Hand)
                {
                    foreach (Image ui in BelongingsUIList)
                    {
                        switch (ui.name)
                        {
                            case "Hand":
                                UITransform(ui, true, ShiftDirection.RShift, HandDefaultPos);
                                break;
                            case "Psyllium":
                                UITransform(ui, false, ShiftDirection.RShift, PsylliumDefaultPos);
                                break;
                            case "Camera":
                                UITransform(ui, false, ShiftDirection.None, CameraDefaultPos);
                                break;
                            default:
                                break;
                        }
                    }
                    BType = BelongingsType.Hand;
                }
                break;
            //サイリウムのUIを拡大し、他UIを縮小してずらす
            case "2":
                if (BType != BelongingsType.Psyllium)
                {
                    foreach (Image ui in BelongingsUIList)
                    {
                        switch (ui.name)
                        {
                            case "Hand":
                                UITransform(ui, false, ShiftDirection.None, HandDefaultPos);
                                break;
                            case "Psyllium":
                                UITransform(ui, true, ShiftDirection.None, PsylliumDefaultPos);
                                break;
                            case "Camera":
                                UITransform(ui, false, ShiftDirection.None, CameraDefaultPos);
                                break;
                            default:
                                break;
                        }
                    }
                    BType = BelongingsType.Psyllium;
                }
                break;
            //カメラのUIを拡大し、他UIを縮小してずらす
            case "3":
                if (BType != BelongingsType.Camera)
                {
                    foreach (Image ui in BelongingsUIList)
                    {
                        switch (ui.name)
                        {
                            case "Hand":
                                UITransform(ui, false, ShiftDirection.None, HandDefaultPos);
                                break;
                            case "Psyllium":
                                UITransform(ui, false, ShiftDirection.LShift, PsylliumDefaultPos);
                                break;
                            case "Camera":
                                UITransform(ui, true, ShiftDirection.LShift, CameraDefaultPos);
                                break;
                            default:
                                break;
                        }
                    }
                    BType = BelongingsType.Camera;
                }
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 各UIの変形
    /// </summary>
    /// <param name="ui"></param>
    /// <param name="expantion"></param>
    /// <param name="shift"></param>
    private void UITransform(Image ui, bool expantion, ShiftDirection shift, float basepos)
    {
        //拡大し、拡大した分だけ上にずらしてY軸の変数に保存
        if (expantion)
        {
            ui.rectTransform.localScale = new Vector3(1f + SizeExpantion, 1f + SizeExpantion, 1f);
            ExpandedUIPos = ui.rectTransform.anchoredPosition.y + (ui.rectTransform.sizeDelta.y * SizeExpantion / 2);
        }
        else
        {
            ui.rectTransform.localScale = new Vector3(1f, 1f, 1f);
            ExpandedUIPos = 0;
        }

        //左右どちらかにずらしてX軸の変数に保存
        switch (shift)
        {
            case ShiftDirection.RShift:
                ShiftedUIPos = basepos + (ui.rectTransform.sizeDelta.x * SizeExpantion / 2);
                break;
            case ShiftDirection.LShift:
                ShiftedUIPos = basepos - (ui.rectTransform.sizeDelta.x * SizeExpantion / 2);
                break;
            case ShiftDirection.None:
                ShiftedUIPos = basepos;
                break;
            default:
                break;
        }

        //保存したX軸とY軸の値をUIの位置に反映
        ui.rectTransform.anchoredPosition = new Vector2(ShiftedUIPos, ExpandedUIPos);
    }
}
