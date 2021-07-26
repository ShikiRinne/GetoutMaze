using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class move_test : MonoBehaviour
{
    private CharacterController chara;

    private Vector3 Horizontal;
    private Vector3 Vertical;
    private Vector3 Direction;

    void Start()
    {
        chara = GetComponent<CharacterController>();
    }

    void Update()
    {
        PlayerMove();
    }

    private void PlayerMove()
    {
        Horizontal = transform.TransformDirection(Vector3.right) * Input.GetAxisRaw("Horizontal");
        Vertical = transform.TransformDirection(Vector3.forward) * Input.GetAxisRaw("Vertical");
        Direction = Horizontal + Vertical;

        chara.Move(Time.deltaTime * Direction.normalized);
    }
}
