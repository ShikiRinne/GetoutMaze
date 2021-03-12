using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialOperation : MonoBehaviour
{
    private ArrowOperation AO;

    public List<GameObject> EachDial { get; set; } = new List<GameObject>();
    public List<Renderer> DialRendererList { get; private set; } = new List<Renderer>();
    public List<int> DialNumberList { get; set; } = new List<int>();
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
        DialChangeNum(ControlManager.ControlManager_Instance.VerticalInput);
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
                DialRendererList.Add(dial.GetComponent<Renderer>());
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

    /// <summary>
    /// ダイヤルの数値を変更する
    /// </summary>
    /// <param name="rotate"></param>
    private void DialChangeNum(int rotate)
    {
        if (rotate != 0)
        {
            DialNumberList[PassSelectDial] += rotate;

            if (rotate > 0)
            {
                DialRotation(RotateDirType.Up);
            }
            if (rotate < 0)
            {
                DialRotation(RotateDirType.Down);
            }
        }

        if (DialNumberList[PassSelectDial] > 9)
        {
            DialNumberList[PassSelectDial] = 0;
        }
        if (DialNumberList[PassSelectDial] < 0)
        {
            DialNumberList[PassSelectDial] = 9;
        }
    }

    /// <summary>
    /// ダイヤルを回転させる
    /// </summary>
    /// <param name="direction"></param>
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
