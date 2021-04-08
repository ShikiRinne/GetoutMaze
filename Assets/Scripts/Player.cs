using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    private DialManager DM;

    private GameObject MainCamera;

    private CharacterController Chara;

    [SerializeField]
    private float SetMoveSpeed;
    [SerializeField]
    private float SetRotateSpeed;
    [SerializeField]
    private float SetHandLength;

    private Text DefaultReticle;

    private Vector3 PlayerDirection;
    private Vector3 Direction_Horizontal;
    private Vector3 Direction_Vertical;
    private Vector3 PlayerRotation;
    private Vector3 CameraRotation;

    private Ray PlayerHands;

    void Start()
    {
        DM = GameObject.Find("PlaySceneManager").GetComponent<DialManager>();
        DefaultReticle = GameObject.Find("Default").GetComponent<Text>();

        //メインカメラをプレイヤーの視点に移動
        MainCamera = GameObject.Find("Main Camera");
        MainCamera.transform.parent = gameObject.transform;
        MainCamera.transform.localPosition = new Vector3(0, transform.localScale.y, 0);

        //CharacterController取得
        Chara = GetComponent<CharacterController>();

        //壁のない方向にプレイヤーを向ける
        CameraRotation.y = GameObject.Find("PlaySceneManager").GetComponent<MazeGenerateManager>().StartDirection;
        transform.rotation = Quaternion.Euler(CameraRotation);
    }

    void Update()
    {
        PlayerMove();
        CameraMove();
        PickHands();

        if (Input.GetKeyDown(KeyCode.F1))
        {
            ControlManager.ControlManager_Instance.CanPlayerMove = false;
            GameManager.GameManager_Instance.TransitionGameState(GameManager.GameState.GameOver);
        }
    }

    /// <summary>
    /// プレイヤーの移動
    /// </summary>
    private void PlayerMove()
    {
        //プレイヤー移動のインスタンス取得
        ControlManager.ControlManager_Instance.InputArrow(ControlManager.ArrowType.Move);

        //縦軸と横軸の移動量を算出してプレイヤーの移動に代入
        Direction_Horizontal = transform.TransformDirection(Vector3.right) * ControlManager.ControlManager_Instance.MoveHorizontal;
        Direction_Vertical = transform.TransformDirection(Vector3.forward) * ControlManager.ControlManager_Instance.MoveVertical;
        PlayerDirection = Direction_Horizontal + Direction_Vertical;

        //移動
        Chara.Move(PlayerDirection.normalized * SetMoveSpeed * Time.deltaTime);
    }

    /// <summary>
    /// カメラの移動
    /// </summary>
    private void CameraMove()
    {
        //マウス移動のインスタンス取得
        ControlManager.ControlManager_Instance.ViewAndCursor();

        //算出した回転量をVector3に代入
        PlayerRotation.y = ControlManager.ControlManager_Instance.RotateHorizontal * SetRotateSpeed;
        CameraRotation.x = ControlManager.ControlManager_Instance.RotateVertical * -SetRotateSpeed;

        //回転
        //Y軸回転はプレイヤーごと回す
        transform.Rotate(0, PlayerRotation.y, 0);
        MainCamera.transform.Rotate(CameraRotation.x, 0, 0);
    }

    /// <summary>
    /// 拾得処理
    /// </summary>
    private void PickHands()
    {
        PlayerHands = new Ray(MainCamera.transform.position, MainCamera.transform.forward);
        Debug.DrawRay(PlayerHands.origin, PlayerHands.direction, Color.red);
        if (Physics.Raycast(PlayerHands, out RaycastHit hit, SetHandLength))
        {
            if (hit.collider.CompareTag("Notes"))
            {
                DefaultReticle.color = Color.red;
                if (ControlManager.ControlManager_Instance.Action(ControlManager.PressType.Push))
                {
                    DM.PickupMemo();
                    hit.collider.gameObject.SetActive(false);
                }
            }
            else if (hit.collider.CompareTag("Exit"))
            {
                DefaultReticle.color = Color.red;
                if (ControlManager.ControlManager_Instance.Action(ControlManager.PressType.Push))
                {
                    DM.IsTouchiGoal = true;
                }
            }
            else
            {
                DefaultReticle.color = Color.gray;
            }
        }
    }
}
