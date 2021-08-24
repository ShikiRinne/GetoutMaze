using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    private List<GameObject> MemosList = new List<GameObject>(); //メモのリスト

    private int Length = 4;
    private int Pick = 0;
    private int MemoCounts = 0;

    private float MemoSetPos = 0f;
    [SerializeField]
    private float MemoDistance = 0f;

    void Start()
    {
        MemoSetPos = NotesSet.transform.position.x;

        for (int i = 0; i < Length; ++i)
        {
            OrderList.Add(i);

            NumberList.Add(Random.Range(0, 10));
            Debug.Log("NumberList[" + i + "] = " + NumberList[i]);
        }
        Debug.Log(OrderList.Count);

        foreach (Transform memos in NotesSet.transform)
        {
            MemosList.Add(memos.gameObject);
            memos.gameObject.SetActive(false);
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
            MemosList[MemoCounts].SetActive(true);
            MemoSetPos = -MemoDistance / 2f * MemoCounts;
            NotesSet.GetComponent<RectTransform>().anchoredPosition = new Vector3(MemoSetPos, 0f, 0f);
            OrderList.RemoveAt(Pick);
            //0で要素を仮追加
            SortList.Add(0);

            Sort(PickList);

            MemoCounts++;
        }
    }

    private void Sort(List<int> items)
    {
        //リストをソート
        if (items.Count > 1)
        {
            items.Sort();
        }

        //ソートされたリストを配列番号として参照し、NumberListを順に表示
        for (int i = 0; i < SortList.Count; ++i)
        {
            SortList[i] = NumberList[items[i]];
            MemosList[i].transform.GetChild(0).GetComponent<Text>().text = (items[i] + 1).ToString() + "/4";
            MemosList[i].transform.GetChild(1).GetComponent<Text>().text = SortList[i].ToString();
        }
    }
}
