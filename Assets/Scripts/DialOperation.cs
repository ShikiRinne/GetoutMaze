using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialOperation : MonoBehaviour
{
    private ArrowOperation AO;

    private List<GameObject> EachDial = new List<GameObject>();
    private List<int> DialNumberList = new List<int>();

    public int PassSelectDial { get; set; } = 0;
    public int PassDialNumber { get; set; } = 0;

    public enum RotateDirType
    {
        Up,
        Down
    }
    public RotateDirType DirType { get; set; }

    void Start()
    {

    }

    void Update()
    {
        ControlManager.ControlManager_Instance.InputArrow(ControlManager.ArrowType.Select);

        DialSelect(ControlManager.ControlManager_Instance.HorizontalInput);
        DialSetting(ControlManager.ControlManager_Instance.VerticalInput);
        DialNumChange(PassDialNumber);
        AO.MoveArrow(PassSelectDial);
    }

    /// <summary>
    /// 外部で開始時のみ呼び出す
    /// </summary>
    public void StartDialSetting()
    {
        AO = GameObject.Find("ArrowSet").GetComponent<ArrowOperation>();

        foreach (Transform dial in gameObject.transform)
        {
            if (dial.CompareTag("Dial"))
            {
                EachDial.Add(dial.gameObject);
                DialNumberList.Add(0);
            }
        }
    }

    /// <summary>
    /// ダイヤルの選択
    /// </summary>
    /// <param name="select"></param>
    private void DialSelect(int select)
    {
        //操作されている場合のみ代入
        if (select != 0)
        {
            PassSelectDial += select;
        }

        //0を下回ったら3(Dial4)に戻す
        if (PassSelectDial < 0)
        {
            PassSelectDial = 3;
        }
        //3を上回ったら0(Dial1)に戻す
        if (PassSelectDial > 3)
        {
            PassSelectDial = 0;
        }
    }

    private void DialSetting(int rotate)
    {
        if (rotate != 0)
        {
            PassDialNumber += rotate;
            if (PassDialNumber > 9)
            {
                PassDialNumber = 0;
            }
            if (PassDialNumber < 0)
            {
                PassDialNumber = 9;
            }

            if (rotate > 0)
            {
                DialRotation(RotateDirType.Up);
            }
            if (rotate < 0)
            {
                DialRotation(RotateDirType.Down);
            }
        }
    }

    public void DialNumChange(int dialnum)
    {
        DialNumberList[PassSelectDial] = dialnum;

        if (DialNumberList[PassSelectDial] > 9)
        {
            DialNumberList[PassSelectDial] = 0;
        }
        if (DialNumberList[PassSelectDial] < 0)
        {
            DialNumberList[PassSelectDial] = 9;
        }
    }

    public void DialRotation(RotateDirType direction)
    {
        switch (direction)
        {
            case RotateDirType.Up:
                EachDial[PassSelectDial].transform.Rotate(0f, 0f, -36f);
                break;
            case RotateDirType.Down:
                EachDial[PassSelectDial].transform.Rotate(0f, 0f, 36f);
                break;
            default:
                break;
        }
    }
}
