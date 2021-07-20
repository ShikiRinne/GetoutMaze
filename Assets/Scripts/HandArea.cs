using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandArea : MonoBehaviour
{
    private MemoManager MM;
    private MazeGenerateManager MGM;
    private HUDManager HUDM;

    [SerializeField]
    private GameObject Psyllium;
    private GameObject PSM;
    private GameObject MainCamera;

    private Ray HandRay;

    [SerializeField]
    private float HandAreaSize = 0;
    [SerializeField]
    private float HandLength = 0;

    private int PickMemo = 0;

    void Start()
    {
        PSM = GameObject.Find("PlaySceneManager");
        MM = PSM.GetComponent<MemoManager>();
        MGM = PSM.GetComponent<MazeGenerateManager>();
        HUDM = PSM.GetComponent<HUDManager>();

        MainCamera = transform.parent.gameObject;
        transform.localScale = new Vector3(HandAreaSize, HandAreaSize, HandLength);
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, HandLength / 2f);
    }

    void Update()
    {
        HandRay = new Ray(MainCamera.transform.position, MainCamera.transform.forward);
        Debug.DrawRay(HandRay.origin, HandRay.direction, Color.red);

        if (GameManager.GameManager_Instance.CanPlayerMove && HUDM.BType == HUDManager.BelongingsType.Psyllium)
        {
            PutPsyllium();
        }
    }

    private void PutPsyllium()
    {
        if (Physics.Raycast(HandRay, out RaycastHit hit, HandLength))
        {
            if (hit.collider.gameObject.CompareTag("Floor"))
            {
                if (ControlManager.ControlManager_Instance.Action(ControlManager.PressType.Push) && HUDM.PassPsylliumCount > 0)
                {
                    //サイリウムをプレイヤーに向いている方向に倒して生成
                    Instantiate(Psyllium, new Vector3(hit.point.x, Psyllium.transform.localScale.z, hit.point.z), Quaternion.Euler(90f, transform.eulerAngles.y, 0f));
                    HUDM.PassPsylliumCount--;
                }
            }
        }
    }

    /// <summary>
    /// 取得・接触処理
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (GameManager.GameManager_Instance.CanPlayerMove && HUDM.BType == HUDManager.BelongingsType.Hand)
        {
            switch (other.tag)
            {
                case "Notes":
                    if (ControlManager.ControlManager_Instance.Action(ControlManager.PressType.Push))
                    {
                        PickMemo = Random.Range(0, MGM.PassTotalSplitMemos);
                        MM.PickMemos(PickMemo);
                        other.gameObject.SetActive(false);
                    }
                    break;
                case "Exit":
                    if (ControlManager.ControlManager_Instance.Action(ControlManager.PressType.Push))
                    {
                        HUDM.IsTouchiGoal = true;
                    }
                    break;
                case "Psyllium":
                    if (ControlManager.ControlManager_Instance.Action(ControlManager.PressType.Push))
                    {
                        Destroy(other.gameObject);
                        HUDM.PassPsylliumCount++;
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
