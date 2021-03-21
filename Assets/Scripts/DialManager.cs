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

    private List<int> ExitKeyCode = new List<int>();
    public List<int> PassDialNumberList { get; set; } = new List<int>();

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

    void Start()
    {
        IsTouchiGoal = false;
        IsOperateDial = false;
        PassCanControl = true;

        //メモをリストとして保存、非アクティブ化
        foreach (Transform memo in DisplayMemo.transform)
        {
            if (memo.CompareTag("Memo"))
            {
                DisplayMemosList.Add(memo.gameObject);
                ExitKeyCode.Add(Random.Range(0, 10));
                memo.gameObject.SetActive(false);
            }
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

        if (IsTouchiGoal)
        {
            GameManager.GameManager_Instance.UseCursor(true);
            DialPadLock.SetActive(true);
            //ArrowSet.SetActive(true);
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
            //DialSetActive(false);
            Debug.Log("Miss");
            IsTouchiGoal = false;
            IsOperateDial = false;
            PassCanControl = true;
        }
    }
}
