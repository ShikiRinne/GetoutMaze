using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialOperation : MonoBehaviour
{
    private HUDManager HUDM;
    private ArrowOperation AO;

    [SerializeField]
    private AudioSource DialAudio;
    [SerializeField]
    private AudioSource ShackleAudio;
    [SerializeField]
    private AudioClip DialClip;
    [SerializeField]
    private AudioClip ShackleClip;

    public List<GameObject> EachDial { get; set; } = new List<GameObject>();
    public List<MeshRenderer> RendererList { get; private set; } = new List<MeshRenderer>();
    public List<int> DialNumberList { get; set; } = new List<int>();
    public int PassSelectDial { get; set; } = 0;
    public int PassSelectCount { get; set; } = 0;
    public bool PassCanRotate { get; set; } = false;

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

        if (PassCanRotate)
        {
            DialChangeNum(ControlManager.ControlManager_Instance.VerticalInput);
        }
    }

    /// <summary>
    /// 外部で開始時のみ呼び出す
    /// </summary>
    public void StartDialSetting()
    {
        HUDM = GameObject.Find("PlaySceneManager").GetComponent<HUDManager>();
        AO = GameObject.Find("ArrowSet").GetComponent<ArrowOperation>();

        //リストに挿入
        foreach (Transform dial in gameObject.transform)
        {
            //各ダイヤルとシャックルのMeshRendererをリストに挿入
            RendererList.Add(dial.GetComponent<MeshRenderer>());
            //各ダイヤルをダイヤルリストに挿入、ダイヤルの番号を0で挿入
            if (dial.CompareTag("Dial"))
            {
                EachDial.Add(dial.gameObject);
                DialNumberList.Add(0);
            }
        }

        //通常時と選択時のオブジェクトのEmissyonColorを設定
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
        if (!PassCanRotate && select != 0)
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

            //選択されているダイヤルを発光させる
            DialLuminescent(PassSelectDial);
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            DialDicision();
        }
    }

    /// <summary>
    /// 選択され決定されたダイヤルの操作
    /// </summary>
    public void DialDicision()
    {
        if (PassSelectDial != 4)
        {
            //一度押したらダイヤルを回転、もう一度押したら矢印を非表示にしてダイヤル選択
            switch (PassSelectCount)
            {
                case 0:
                    AO.gameObject.SetActive(true);
                    AO.MoveArrow(PassSelectDial);
                    PassCanRotate = true;
                    PassSelectCount++;
                    break;
                case 1:
                    AO.gameObject.SetActive(false);
                    PassCanRotate = false;
                    PassSelectCount--;
                    break;
            }
        }
        else
        {
            HUDM.JudgeUnlock(DialNumberList);
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
        DialAudio.PlayOneShot(DialClip);
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

    /// <summary>
    /// ロック解除音を外部で鳴らす
    /// </summary>
    public void PlayShackleSound()
    {
        ShackleAudio.PlayOneShot(ShackleClip);
    }
}
