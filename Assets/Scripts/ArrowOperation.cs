using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEditorInternal.VR;
using UnityEngine;

public class ArrowOperation : MonoBehaviour
{
    private RectTransform ArrowRect;

    [SerializeField]
    private Vector3 Dial1Pos = new Vector3();
    [SerializeField]
    private Vector3 Dial2Pos = new Vector3();
    [SerializeField]
    private Vector3 Dial3Pos = new Vector3();
    [SerializeField]
    private Vector3 Dial4Pos = new Vector3();

    void Start()
    {
        ArrowRect = gameObject.GetComponent<RectTransform>();
        ArrowRect.anchoredPosition = Dial1Pos;

        gameObject.SetActive(false);
    }

    void Update()
    {
        
    }

    /// <summary>
    /// 外部で開始時のみ呼び出す
    /// </summary>
    public void StertArrowSetting()
    {
        ArrowRect = gameObject.GetComponent<RectTransform>();
        ArrowRect.anchoredPosition = Dial1Pos;
    }
    
    /// <summary>
    /// 矢印の移動
    /// </summary>
    /// <param name="position"></param>
    public void MoveArrow(int position)
    {
        switch (position)
        {
            case 0:
                ArrowRect.anchoredPosition = Dial1Pos;
                break;
            case 1:
                ArrowRect.anchoredPosition = Dial2Pos;
                break;
            case 2:
                ArrowRect.anchoredPosition = Dial3Pos;
                break;
            case 3:
                ArrowRect.anchoredPosition = Dial4Pos;
                break;
        }
    }
}
