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
    private Text Reticle_Default = null;
    [SerializeField]
    private Text Reticle_Spray = null;

    [SerializeField]
    private float SizeExpantion = 0f;
    [SerializeField]
    private float SizeDefault = 0f;
    [SerializeField]
    private float PosDefault = 0f;

    private string InputStr = null;

    private bool MemoDisplay = false;

    private List<GameObject> DisplayMemosList = new List<GameObject>();

    private List<int> ExitKeyCode = new List<int>();

    public int GetPickMemoCount { get; set; } = 0;
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

        DialPadLock.GetComponent<DialOperation>().StartDialSetting();
        ArrowSet.GetComponent<ArrowOperation>().StertArrowSetting();

        //不要なオブジェクトを非アクティブ化
        DisplayMemo.SetActive(false);
        DialPadLock.SetActive(false);
        ArrowSet.SetActive(false);
    }

    void Update()
    {
        //スペースキーで取得済みメモの表示、非表示切り替え
        DisplayMemo.SetActive(MemoDisplay);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            MemoDisplay = !MemoDisplay;
        }

        BelongingsUIOps();

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
        DialPadLock.SetActive(isdisplay);
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
                case "1":
                    foreach (Image ui in BelongingsUI.transform)
                    {
                        switch (ui.name)
                        {
                            case "Hand":
                                ui.rectTransform.sizeDelta = new Vector2(ui.rectTransform.sizeDelta.x + SizeExpantion,
                                                                         ui.rectTransform.sizeDelta.y + SizeExpantion);
                                ui.rectTransform.anchoredPosition = new Vector3(ui.rectTransform.anchoredPosition.x - (SizeExpantion / 2),
                                                                                ui.rectTransform.anchoredPosition.y + (SizeExpantion / 2), 0);
                                break;
                            case "Psyllium":
                                ui.rectTransform.sizeDelta = new Vector2(SizeDefault, SizeDefault);
                                ui.rectTransform.anchoredPosition = new Vector3(0 + (SizeExpantion / 4), 0f, 0f);
                                break;
                            case "Camera":
                                ui.rectTransform.sizeDelta = new Vector2(SizeDefault, SizeDefault);
                                ui.rectTransform.anchoredPosition = new Vector3(PosDefault, 0f, 0f);
                                break;
                            default:
                                break;
                        }
                    }
                    Debug.Log(InputStr);
                    break;
                case "2":
                    foreach (Image ui in BelongingsUI.transform)
                    {
                        switch (ui.name)
                        {
                            case "Hand":
                                ui.rectTransform.sizeDelta = new Vector2(SizeDefault, SizeDefault);
                                ui.rectTransform.anchoredPosition = new Vector3(ui.rectTransform.anchoredPosition.x - (SizeExpantion / 2), 0, 0);
                                break;
                            case "Psyllium":
                                ui.rectTransform.sizeDelta = new Vector2(SizeDefault, SizeDefault);
                                ui.rectTransform.anchoredPosition = new Vector3(0f, 0f, 0f);
                                break;
                            case "Camera":
                                ui.rectTransform.sizeDelta = new Vector2(SizeDefault, SizeDefault);
                                ui.rectTransform.anchoredPosition = new Vector3(PosDefault, 0f, 0f);
                                break;
                            default:
                                break;
                        }
                    }
                    Debug.Log(InputStr);
                    break;
                case "3":
                    Debug.Log(InputStr);
                    break;
                default:
                    break;
            }
        }
    }
}
