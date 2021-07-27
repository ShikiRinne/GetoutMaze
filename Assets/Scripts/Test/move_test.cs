using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class move_test : MonoBehaviour
{
    private CharacterController chara;
    private GameObject MainCamera;

    private Vector3 Horizontal;
    private Vector3 Vertical;
    private Vector3 Direction;

    private float CameraRotate;

    void Start()
    {
        chara = GetComponent<CharacterController>();
        MainCamera = GameObject.Find("Main Camera");
        MainCamera.transform.parent = gameObject.transform;
        MainCamera.transform.localPosition = new Vector3(MainCamera.transform.localPosition.x, MainCamera.transform.localPosition.y, 0.1f);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        PlayerMove();
        PlayerRotate();
    }

    private void PlayerMove()
    {
        Horizontal = transform.TransformDirection(Vector3.right) * Input.GetAxisRaw("Horizontal");
        Vertical = transform.TransformDirection(Vector3.forward) * Input.GetAxisRaw("Vertical");
        Direction = Horizontal + Vertical;

        chara.Move(Time.deltaTime * Direction.normalized);
    }

    private void PlayerRotate()
    {
        CameraRotate -= Input.GetAxisRaw("Mouse Y");

        transform.Rotate(0, Input.GetAxisRaw("Mouse X"), 0);
        MainCamera.transform.localEulerAngles = new Vector3(CameraRotate, 0, 0);
    }
}
