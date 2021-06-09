using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class move_test : MonoBehaviour
{
    private CharacterController chara;

    void Start()
    {
        chara = GetComponent<CharacterController>();
    }

    void Update()
    {
        chara.SimpleMove(Input.GetAxis("Vertical") * transform.forward);
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.name)
        {
            case "GameObject":
                Debug.Log("object hit");
                break;
            case "GameObject (1)":
                Debug.Log("object(1) hit");
                break;
            default:
                break;
        }
    }
}
