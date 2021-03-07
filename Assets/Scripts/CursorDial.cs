using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CursorDial : MonoBehaviour
{
    private DialOperation DP;

    void Start()
    {
        DP = GameObject.Find("DialPadLock").GetComponent<DialOperation>();
    }

    public void ClickSelect()
    {
        if (gameObject.CompareTag("Dial"))
        {
            switch (gameObject.name)
            {
                case "Dial1":
                    DP.PassSelectDial = 0;
                    break;
                case "Dial2":
                    DP.PassSelectDial = 1;
                    break;
                case "Dial3":
                    DP.PassSelectDial = 2;
                    break;
                case "Dial4":
                    DP.PassSelectDial = 3;
                    break;
                default:
                    break;
            }
        }
    }

    public void ClickRotate()
    {
        //switch (gameObject.name)
        //{
        //    case "UpArrow":
        //        DP.PassDialNumberList[DM.PassSelectDial] += 1;
        //        if (DM.PassDialNumberList[DM.PassSelectDial] > 9)
        //        {
        //            DM.PassDialNumberList[DM.PassSelectDial] = 0;
        //        }
        //        break;
        //    case "DownArrow":
        //        Debug.Log("down");
        //        DM.PassDialObject[DM.PassSelectDial].transform.Rotate(0f, 0f, 36f);
        //        DM.PassDialNumberList[DM.PassSelectDial] -= 1;
        //        if (DM.PassDialNumberList[DM.PassSelectDial] < 0)
        //        {
        //            DM.PassDialNumberList[DM.PassSelectDial] = 9;
        //        }
        //        break;
        //    default:
        //        break;
        //}
    }
}
