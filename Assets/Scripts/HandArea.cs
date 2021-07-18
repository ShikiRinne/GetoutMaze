using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandArea : MonoBehaviour
{
    private MemoManager MM;
    private MazeGenerateManager MGM;

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

        MainCamera = transform.parent.gameObject;
        transform.localScale = new Vector3(HandAreaSize, HandAreaSize, HandLength);
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, HandLength / 2f);
    }

    void Update()
    {
        HandRay = new Ray(MainCamera.transform.position, MainCamera.transform.forward);
        Debug.DrawRay(HandRay.origin, HandRay.direction, Color.red);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (GameManager.GameManager_Instance.CanPlayerMove)
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
            }
        }
    }
}
