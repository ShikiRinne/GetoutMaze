using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CursorDial : MonoBehaviour
{
    private DialOperation DO;

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
                DO.DirType = DialOperation.RotateDirType.Up;
                break;
            case "DownArrow":
                DO.DirType = DialOperation.RotateDirType.Down;
                break;
            default:
                break;
        }
    }
}
