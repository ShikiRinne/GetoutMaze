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
    [SerializeField]
    private float SizeDefault = 0f;
    [SerializeField]
    private int SizePsylliumText = 0;
    [SerializeField]
    private float HandPosDefault = 0f;
    [SerializeField]
    private float CameraPosDefault = 0f;

    private Vector3 PsylliumTextPos;

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
        Camera
    }
    public BelongingsType BType { get; set; }

    void Start()
    {
        //メモをリストとして保存、非アクティブ化
        foreach (Transform memo in DisplayMemo.transform)
        {
            DisplayMemosList.Add(memo.gameObject);
            ExitKeyCode.Add(Random.Range(0, 10));
            memo.gameObject.SetActive(false);
        }

        foreach (Transform ui in BelongingsUI.transform)
        {
            BelongingsUIList.Add(ui.GetComponent<Image>());
        }

        PsylliumTextPos = PsylliumCountText.rectTransform.anchoredPosition;

        DialPadLock.GetComponent<DialOperation>().StartDialSetting();
        ArrowSet.GetComponent<ArrowOperation>().StertArrowSetting();

        //不要なオブジェクトを非アクティブ化
        DisplayMemo.SetActive(false);
        DialPadLock.SetActive(false);
        ArrowSet.SetActive(false);

        //Debug
        for (int i = 0; i < ExitKeyCode.Count; ++i)
        {
            Debug.Log("[" + i + "]:" + ExitKeyCode[i]);
        }
    }

    void Update()
    {
        //メモの表示・非表示切り替え
        DisplayMemo.SetActive(MemoDisplay);
        if (ControlManager.ControlManager_Instance.CanControl)
        {
            BelongingsUIOps();

            if (Input.GetKeyDown(KeyCode.Space) && HaveMemo)
            {
                MemoDisplay = !MemoDisplay;
            }
        }

        PsylliumCountText.text = "x" + PassPsylliumCount;

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
            DisplayDial(false);
        }
    }

    /// <summary>
    /// 手に持つもののUI切り替え
    /// </summary>
    private void BelongingsUIOps()
    {
        if (Input.anyKeyDown)
        {
            InputStr = Input.inputString;
            
            switch (InputStr)
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
                                    ui.rectTransform.sizeDelta = new Vector2(ui.rectTransform.sizeDelta.x + SizeExpantion,
                                                                             ui.rectTransform.sizeDelta.y + SizeExpantion);
                                    ui.rectTransform.anchoredPosition = new Vector3(HandPosDefault + (SizeExpantion / 2),
                                                                                    ui.rectTransform.anchoredPosition.y + (SizeExpantion / 2), 0);
                                    break;
                                case "Psyllium":
                                    ui.rectTransform.sizeDelta = new Vector2(SizeDefault, SizeDefault);
                                    ui.rectTransform.anchoredPosition = new Vector3(SizeExpantion / 2, 0f, 0f);
                                    PsylliumCountText.fontSize = SizePsylliumText;
                                    PsylliumCountText.rectTransform.anchoredPosition = PsylliumTextPos;
                                    break;
                                case "Camera":
                                    ui.rectTransform.sizeDelta = new Vector2(SizeDefault, SizeDefault);
                                    ui.rectTransform.anchoredPosition = new Vector3(CameraPosDefault, 0f, 0f);
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
                                    ui.rectTransform.sizeDelta = new Vector2(SizeDefault, SizeDefault);
                                    ui.rectTransform.anchoredPosition = new Vector3(HandPosDefault, 0f, 0f);
                                    break;
                                case "Psyllium":
                                    ui.rectTransform.sizeDelta = new Vector2(ui.rectTransform.sizeDelta.x + SizeExpantion,
                                                                             ui.rectTransform.sizeDelta.y + SizeExpantion);
                                    ui.rectTransform.anchoredPosition = new Vector3(0f, SizeExpantion / 2, 0f);
                                    PsylliumCountText.fontSize = (int)(SizePsylliumText + SizeExpantion);
                                    PsylliumCountText.rectTransform.anchoredPosition = new Vector3(PsylliumTextPos.x + (SizeExpantion / 2),
                                                                                                   PsylliumTextPos.y - (SizeExpantion / 2),
                                                                                                   PsylliumTextPos.z);
                                    break;
                                case "Camera":
                                    ui.rectTransform.sizeDelta = new Vector2(SizeDefault, SizeDefault);
                                    ui.rectTransform.anchoredPosition = new Vector3(CameraPosDefault, 0f, 0f);
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
                                    ui.rectTransform.sizeDelta = new Vector2(SizeDefault, SizeDefault);
                                    ui.rectTransform.anchoredPosition = new Vector3(HandPosDefault, 0f, 0f);
                                    break;
                                case "Psyllium":
                                    ui.rectTransform.sizeDelta = new Vector2(SizeDefault, SizeDefault);
                                    ui.rectTransform.anchoredPosition = new Vector3(-SizeExpantion / 2, 0f, 0f);
                                    PsylliumCountText.fontSize = SizePsylliumText;
                                    PsylliumCountText.rectTransform.anchoredPosition = PsylliumTextPos;
                                    break;
                                case "Camera":
                                    ui.rectTransform.sizeDelta = new Vector2(ui.rectTransform.sizeDelta.x + SizeExpantion,
                                                                             ui.rectTransform.sizeDelta.y + SizeExpantion);
                                    ui.rectTransform.anchoredPosition = new Vector3(CameraPosDefault - (SizeExpantion / 2),
                                                                                    ui.rectTransform.anchoredPosition.y + (SizeExpantion / 2), 0);
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
    }
}
