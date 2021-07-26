using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hand_test : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        Debug.Log(other.gameObject.tag);
    }
}
