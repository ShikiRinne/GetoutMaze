using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoManager : MonoBehaviour
{
    private HUDManager HUDM;
    private MazeGenerateManager MGM;

    private GameObject PSM = default;
    [SerializeField]
    private GameObject MemoDisplayArea = default;
    [SerializeField]
    private GameObject MemoSet = default;
    [SerializeField]
    private GameObject Memos = default;

    [SerializeField]
    private float MemoDistance = 0f;
    private float MemoHalfSize = 0f;
    private float MemoSetPos = 0f;

    private int MemoCounts = 0;

    private bool IsMemoDisplay = false;
    private bool HaveMemo = false;

    private List<int> OrderList = new List<int>();      //重複なしランダム実現のためのリスト
    private List<int> PickList = new List<int>();       //ランダムにピックした数値を入れるリスト
    private List<int> SortList = new List<int>();       //ランダムピックした数値を並び替えるためのリスト

    void Start()
    {
        PSM = GameObject.Find("PlaySceneManager");
        HUDM = PSM.GetComponent<HUDManager>();
        MGM = PSM.GetComponent<MazeGenerateManager>();
        MemoSetPos = MemoSet.transform.position.x;
        MemoHalfSize = Memos.transform.localScale.x / 2f;

        for (int i = 0; i < MGM.PassTotalSplitMemos; ++i)
        {
            OrderList.Add(i);
        }
    }

    void Update()
    {
        MemoDisplayArea.SetActive(IsMemoDisplay);

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
    public void PickMemos(int pick)
    {
        //ランダムにピックした数値を追加
        PickList.Add(OrderList[pick]);
        //重複なしのランダムピックを行うためリストからピックした数値を削除
        OrderList.RemoveAt(pick);
        //ソートするためのリストを0で追加
        SortList.Add(0);

        SortAndDisplay(PickList);
    }

    private void SortAndDisplay(List<int> memos)
    {
        //リストをソート
        memos.Sort();

        //ソートされたリストを配列番号として参照し、それを元にキーコードを順に表示
        for (int i = 0; i < SortList.Count; ++i)
        {
            SortList[i] = HUDM.ExitKeyCode[memos[i]];
        }

        //メモの表示
        //一枚のみは中央に表示、二枚以降は少しずつずらす
        if (memos.Count <= 1)
        {
            Instantiate(Memos, new Vector3(0f, 0f, 0f), Quaternion.identity, MemoSet.transform);
        }
        else
        {
            MemoSetPos = (MemoHalfSize + MemoDistance) * MemoCounts;
            MemoSet.transform.position = new Vector3(MemoSetPos, 0f, 0f);
            Instantiate(Memos, new Vector3(-MemoSetPos, 0f, 0f), Quaternion.identity, MemoSet.transform);
        }

        //何枚追加されたかをカウント
        MemoCounts++;
    }
}
