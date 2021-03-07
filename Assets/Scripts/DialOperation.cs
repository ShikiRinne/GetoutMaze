using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialOperation : MonoBehaviour
{
    private List<GameObject> EachDialList = new List<GameObject>();
    private List<int> DialNumberList = new List<int>();
    private List<int> ExitKeyCode = new List<int>();

    [SerializeField]
    private GameObject ArrowSet;

    [SerializeField]
    private Vector3 Dial1Pos;
    [SerializeField]
    private Vector3 Dial2Pos;
    [SerializeField]
    private Vector3 Dial3Pos;
    [SerializeField]
    private Vector3 Dial4Pos;

    public int PassSelectDial { get; set; } = 0;

    void Start()
    {
        //各ダイヤルオブジェクトをリストに保存し数値を初期化
        //同時に解除コードを設定
        foreach (Transform dial in gameObject.transform)
        {
            if (dial.CompareTag("Dial"))
            {
                EachDialList.Add(dial.gameObject);
                DialNumberList.Add(0);
                ExitKeyCode.Add(Random.Range(1, 10));
            }
        }

        ArrowSet.GetComponent<RectTransform>().localPosition = Dial1Pos;
    }

    void Update()
    {
        DialSelect();
        
    }

    /// <summary>
    /// ダイヤルを選択する
    /// </summary>
    private void DialSelect()
    {
        ControlManager.ControlManager_Instance.InputArrow(ControlManager.ArrowType.Select);
        if (ControlManager.ControlManager_Instance.HorizontalInput != 0)
        {
            PassSelectDial += ControlManager.ControlManager_Instance.HorizontalInput;
        }

        //0未満になったら3（ダイヤル4）に、3を超えたら0に
        if (PassSelectDial < 0)
        {
            PassSelectDial = 3;
        }
        if (PassSelectDial > 3)
        {
            PassSelectDial = 0;
        }

        DialRotate(PassSelectDial);
    }

    /// <summary>
    /// 選択されているダイヤルを回転させる
    /// </summary>
    /// <param name="dial"></param>
    private void DialRotate(int dial)
    {
        //数値を加減算
        DialNumberList[dial] += ControlManager.ControlManager_Instance.VerticalInput;

        //入力が正
        if (ControlManager.ControlManager_Instance.VerticalInput > 0)
        {
            //回転
            EachDialList[dial].transform.Rotate(0f, 0f, -36f);
            //9を超えたら0へ
            if (DialNumberList[dial] > 9)
            {
                DialNumberList[dial] = 0;
            }
        }
        //入力が負
        if (ControlManager.ControlManager_Instance.VerticalInput < 0)
        {
            //回転
            EachDialList[dial].transform.Rotate(0f, 0f, 36f);
            //0を下回ったら9へ
            if (DialNumberList[dial] < 0)
            {
                DialNumberList[dial] = 9;
            }
        }
    }

    private void MoveArrow(int position)
    {

    }
}
