using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.AI;

public class DialOperation : MonoBehaviour
{
    private DialManager DM;
    private ArrowOperation AO;

    public List<GameObject> EachDial { get; set; } = new List<GameObject>();
    public List<MeshRenderer> RendererList { get; private set; } = new List<MeshRenderer>();
    public List<int> DialNumberList { get; set; } = new List<int>();
    public int PassSelectDial { get; set; } = 0;
    public int PassSelectCount { get; set; } = 0;

    private bool canRotate = false;
    private bool isActiveself = false;

    [ColorUsage(false, false)]
    private Color32 DefaultColor;
    [ColorUsage(false, true)]
    private Color32 SelectedColor;

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
        if (gameObject.activeSelf && !isActiveself)
        {
            DialLuminescent(PassSelectDial);
            isActiveself = true;
        }

        ControlManager.ControlManager_Instance.InputArrow(ControlManager.ArrowType.Select);

        DialSelect(ControlManager.ControlManager_Instance.HorizontalInput);

        if (canRotate)
        {
            DialChangeNum(ControlManager.ControlManager_Instance.VerticalInput);
        }
        //AO.MoveArrow(PassSelectDial);
    }

    /// <summary>
    /// 外部で開始時のみ呼び出す
    /// </summary>
    public void StartDialSetting()
    {
        AO = GameObject.Find("ArrowSet").GetComponent<ArrowOperation>();

        foreach (Transform dial in gameObject.transform)
        {
            RendererList.Add(dial.GetComponent<MeshRenderer>());
            if (dial.CompareTag("Dial"))
            {
                EachDial.Add(dial.gameObject);
                DialNumberList.Add(0);
            }
        }
        DefaultColor = new Color32(0, 0, 0, 255);
        SelectedColor = new Color32(100, 100, 100, 255);
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
            if (PassSelectDial < 0)
            {
                PassSelectDial = 4;
            }
            if (PassSelectDial > 4)
            {
                PassSelectDial = 0;
            }
            DialLuminescent(PassSelectDial);
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (PassSelectDial != 4)
            {
                switch (PassSelectCount)
                {
                    case 0:
                        AO.gameObject.SetActive(true);
                        AO.MoveArrow(PassSelectDial);
                        canRotate = true;
                        PassSelectCount++;
                        break;
                    case 1:
                        AO.gameObject.SetActive(false);
                        canRotate = false;
                        PassSelectCount--;
                        break;
                }
            }
            else
            {
                //DialManagerのIsTouchGoalをfalseにしないとダメ
                gameObject.SetActive(false);
            }
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

    /// <summary>
    /// 選択されているオブジェクトをEmissionで発光させる
    /// </summary>
    /// <param name="select"></param>
    public void DialLuminescent(int select)
    {
        foreach (MeshRenderer num in RendererList)
        {
            if (num == RendererList[select])
            {
                num.material.EnableKeyword("_EMISSION");
                num.material.SetColor("_EmissionColor", SelectedColor);
            }
            else
            {
                num.material.SetColor("_EmissionColor", DefaultColor);
            }
        }
    }
}
