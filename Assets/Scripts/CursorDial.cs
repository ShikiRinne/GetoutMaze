using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CursorDial : MonoBehaviour
{
    private DialOperation DO;

    private bool isClick = false;

    void Start()
    {
        DO = GameObject.Find("DialPadLock").GetComponent<DialOperation>();
    }

    public void ClickSelect()
    {
        if (gameObject.CompareTag("Dial"))
        {
            switch (gameObject.name)
            {
                case "Dial1":
                    DO.PassSelectDial = 0;
                    break;
                case "Dial2":
                    DO.PassSelectDial = 1;
                    break;
                case "Dial3":
                    DO.PassSelectDial = 2;
                    break;
                case "Dial4":
                    DO.PassSelectDial = 3;
                    break;
                default:
                    break;
            }
        }
    }

    public void ClickRotate()
    {
        switch (gameObject.name)
        {
            case "UpArrow":
                DO.PassDialNumber += 1;
                if (DO.PassDialNumber > 9)
                {
                    DO.PassDialNumber = 0;
                }
                DO.DialRotation(DialOperation.RotateDirType.Up);
                break;
            case "DownArrow":
                DO.PassDialNumber -= 1;
                if (DO.PassDialNumber < 0)
                {
                    DO.PassDialNumber = 9;
                }
                DO.DialRotation(DialOperation.RotateDirType.Down);
                break;
            default:
                break;
        }
    }
}
