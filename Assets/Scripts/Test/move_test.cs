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
    [SerializeField]
    private float HandSize = 0f;
    [SerializeField]
    private float HandLength = 0f;

    RaycastHit hit;

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

    private void OnDrawGizmos()
    {
        bool isHit = Physics.BoxCast(MainCamera.transform.position, Vector3.one * (HandSize / 2f), MainCamera.transform.forward, out hit, Quaternion.identity, HandLength);

        if (isHit)
        {
            Gizmos.DrawRay(MainCamera.transform.position, MainCamera.transform.forward * hit.distance);
            Gizmos.DrawWireCube(MainCamera.transform.position + MainCamera.transform.forward * hit.distance, Vector3.one * (HandSize));

            Debug.Log(hit.collider.tag);
        }
        else
        {
            Gizmos.DrawRay(MainCamera.transform.position, MainCamera.transform.forward * HandLength);
            Gizmos.DrawWireCube(MainCamera.transform.position + MainCamera.transform.forward * HandLength, Vector3.one * (HandSize));
        }
    }
}
