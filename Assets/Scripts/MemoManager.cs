using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MemoManager : MonoBehaviour
{
    private HUDManager HUDM;
    private MazeGenerateManager MGM;

    private RectTransform MemosPos;

    private GameObject PSM = default;
    [SerializeField]
    private GameObject DisplayMemos = default;

    [SerializeField]
    private float MemoDistance = 0f;

    private int MemoCounts = 0;

    private bool IsMemoDisplay = false;
    private bool HaveMemo = false;

    private List<GameObject> MemosList = new List<GameObject>();
    private List<int> OrderList = new List<int>();      //重複なしランダム実現のためのリスト
    private List<int> PickList = new List<int>();       //ランダムにピックした数値を入れるリスト

    void Start()
    {
        PSM = GameObject.Find("PlaySceneManager");
        HUDM = PSM.GetComponent<HUDManager>();
        MGM = PSM.GetComponent<MazeGenerateManager>();
        MemosPos = DisplayMemos.GetComponent<RectTransform>();

        for (int i = 0; i < MGM.PassTotalSplitMemos; ++i)
        {
            OrderList.Add(i);
        }

        foreach (Transform memos in DisplayMemos.transform)
        {
            MemosList.Add(memos.gameObject);
            memos.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        DisplayMemos.SetActive(IsMemoDisplay);

        //メモの表示非表示切り替え
        //メモ未所持なら処理しない
        if (ControlManager.ControlManager_Instance.CanControl)
        {
            if (Input.GetKeyDown(KeyCode.Space) && HaveMemo)
            {
                IsMemoDisplay = !IsMemoDisplay;
            }
        }
    }

    /// <summary>
    /// メモを拾う
    /// </summary>
    /// <param name="pick"></param>
    public void PickMemos()
    {
        //ランダムにピックした数値を追加
        int pick = Random.Range(0, OrderList.Count);
        PickList.Add(OrderList[pick]);

        //拾ったメモを表示して中央になるようずらす
        MemosList[MemoCounts].SetActive(true);
        MemosPos.anchoredPosition = new Vector2(-MemoDistance / 2f * MemoCounts, MemosPos.anchoredPosition.y);

        //重複なしのランダムピックを行うためリストからピックした数値を削除
        OrderList.RemoveAt(pick);

        SortAndDisplay(PickList);
    }

    /// <summary>
    /// 拾ったメモをソートして並べる
    /// </summary>
    /// <param name="memos"></param>
    private void SortAndDisplay(List<int> memos)
    {
        MemoCounts++;

        //メモを所持状態にする
        if (!HaveMemo)
        {
            HaveMemo = true;
        }

        //リストをソート
        if (memos.Count > 1)
        {
            memos.Sort();
        }

        //ソートされたリストを配列番号として参照し、NumberListを順に表示
        for (int i = 0; i < MemoCounts; ++i)
        {
            MemosList[i].transform.GetChild(0).GetComponent<Text>().text = (memos[i] + 1).ToString() + "/4";
            MemosList[i].transform.GetChild(1).GetComponent<Text>().text = HUDM.ExitKeyCode[memos[i]].ToString();
        }
    }
}
