using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoManager : MonoBehaviour
{
    private GameObject MemoDisplayArea = default;
    private GameObject Memos = default;

    private float MemoHalfSize = 0f;
    private float MemoDistance = 0f;

    private int MemoCounts = 0;

    private bool IsMemoDisplay = false;
    private bool HaveMemo = false;

    private List<int> KeyCodeList = new List<int>();    //四桁キーコード
    private List<int> OrderList = new List<int>();      //重複なしランダム実現のためのリスト
    private List<int> PickList = new List<int>();       //ランダムにピックした数値を入れるリスト
    private List<int> SortList = new List<int>();       //ランダムピックした数値を並び替えるためのリスト

    void Start()
    {
        
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
}
