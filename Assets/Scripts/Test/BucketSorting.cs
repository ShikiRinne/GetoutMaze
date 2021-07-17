using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BucketSorting : MonoBehaviour
{
    private List<int> OrderList = new List<int>();
    private List<int> NumberList = new List<int>(); //元のリスト
    private List<int> PickList = new List<int>();   //ランダムにピックした数値を入れるリスト
    private List<int> SortList = new List<int>();   //ソートするためのリスト
    private List<GameObject> NumberCards = new List<GameObject>();

    [SerializeField]
    private GameObject Canvas = default;

    private int Length = 4;
    private int Pick = 0;
    private int Count = 0;

    void Start()
    {
        for (int i = 0; i < Length; ++i)
        {
            OrderList.Add(i);

            NumberList.Add(Random.Range(0, 10));
            Debug.Log("NumberList[" + i + "] = " + NumberList[i]);
        }

        foreach (Transform card in Canvas.transform)
        {
            NumberCards.Add(card.gameObject);
            card.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            //ランダムにピックした数値を追加
            Pick = Random.Range(0, OrderList.Count);
            PickList.Add(OrderList[Pick]);
            NumberCards[Count].SetActive(true);
            OrderList.RemoveAt(Pick);
            //0で要素を仮追加
            SortList.Add(0);

            Sort(PickList);

            Count++;
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
            NumberCards[i].GetComponent<Text>().text = NumberList[PickList[i]].ToString();
        }
    }
}
