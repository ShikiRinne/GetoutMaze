using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateNotes_test : MonoBehaviour
{
    [SerializeField]
    private GameObject NotesUI;
    [SerializeField]
    private GameObject NotesSet;

    private List<int> OrderList = new List<int>();
    private List<int> NumberList = new List<int>(); //元のリスト
    private List<int> PickList = new List<int>();   //ランダムにピックした数値を入れるリスト
    private List<int> SortList = new List<int>();   //ソートするためのリスト

    private int Length = 4;
    private int Pick = 0;
    private int MemoCounts = 0;

    private float MemoSetPos = 0f;
    private float MemoHalfSize = 0f;
    [SerializeField]
    private float MemoDistance = 0f;

    void Start()
    {
        MemoSetPos = NotesSet.transform.position.x;
        MemoHalfSize = NotesUI.transform.localScale.x / 2f;

        for (int i = 0; i < Length; ++i)
        {
            OrderList.Add(i);

            NumberList.Add(Random.Range(0, 10));
            Debug.Log("NumberList[" + i + "] = " + NumberList[i]);
        }
    }

    void Update()
    {
        
    }

    public void  PickNotes()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //ランダムにピックした数値を追加
            Pick = Random.Range(0, OrderList.Count);
            PickList.Add(OrderList[Pick]);
            OrderList.RemoveAt(Pick);
            //0で要素を仮追加
            SortList.Add(0);

            Sort(PickList);
        }
    }

    private void Sort(List<int> items)
    {
        //リストをソート
        if (items.Count < 2)
        {
            items.Sort();
        }

        //ソートされたリストを配列番号として参照し、NumberListを順に表示
        for (int i = 0; i < SortList.Count; ++i)
        {
            SortList[i] = NumberList[PickList[i]];
        }

        if (items.Count <= 1)
        {
            GameObject notes = Instantiate(NotesUI, NotesSet.transform);
        }
        else
        {
            MemoSetPos = MemoDistance * MemoCounts;
            NotesSet.GetComponent<RectTransform>().anchoredPosition = new Vector3(-MemoSetPos / 2f, 0f, 0f);
            GameObject notes = Instantiate(NotesUI, NotesSet.transform);
            notes.transform.localPosition = new Vector3(MemoSetPos, notes.transform.localPosition.y, notes.transform.localPosition.z);
        }

        //何枚追加されたかをカウント
        MemoCounts++;
    }
}
