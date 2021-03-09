using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialOperation : MonoBehaviour
{
    [SerializeField]
    private GameObject DialPadLock;

    private List<GameObject> EachDial = new List<GameObject>();

    void Start()
    {

    }

    void Update()
    {
        
    }

    public void StartDialSetting()
    {
        foreach (Transform dial in DialPadLock.transform)
        {
            if (dial.CompareTag("Dial"))
            {
                EachDial.Add(dial.gameObject);
            }
        }
    }
}
