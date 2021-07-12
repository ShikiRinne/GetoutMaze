using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BucketSorting : MonoBehaviour
{
    private List<int> NumberList = new List<int>(); //元のリスト
    private List<int> PickList = new List<int>();   //ランダムにピックした数値を入れるリスト
    private List<int> SortList = new List<int>();   //ソートするためのリスト

    private int Pick = 0;

    void Start()
    {
        for (int i = 0; i < 10; ++i)
        {
            NumberList.Add(Random.Range(0, 10));
            Debug.Log("NumberList[" + i + "] = " + NumberList[i]);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            BucketSort(NumberList);
        }
    }

    private void BucketSort(List<int> items)
    {
        foreach (int item in items)
        {
            SortList.Add(item);
        }

        int index = 0;
        for (int i = 0; i < SortList.Count; ++i)
        {
            for (int j = 0; j < SortList[i]; ++j)
            {
                SortList[index] = i;
                Debug.Log("Sort[" + i + "] = " + SortList[index]);

                index++;
            }
        }
    }
}
