using System.Collections;
using System.Collections.Generic;
using System.IO.Pipes;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    private HUDManager HUDM;
    private MazeGenerateManager MGM;

    private CharacterController Chara;

    private GameObject MainCamera;
    private GameObject PsylliumClone;
    [SerializeField]
    private GameObject Psyllium = default;

    [SerializeField]
    private float SetMoveSpeed = 0f;
    [SerializeField]
    private float SetRotateSpeed = 0f;
    [SerializeField]
    private float SetHandLength = 0f;
    [SerializeField]
    private float SlowSpeed = 0f;

    private Text DefaultReticle;

    private Vector3 PlayerDirection;
    private Vector3 Direction_Horizontal;
    private Vector3 Direction_Vertical;
    private Vector3 PlayerRotation;
    private float CameraRotation;

    private Ray PlayerHands;

    void Start()
    {
        HUDM = GameObject.Find("PlaySceneManager").GetComponent<HUDManager>();
        MGM = GameObject.Find("PlaySceneManager").GetComponent<MazeGenerateManager>();
        DefaultReticle = GameObject.Find("Default").GetComponent<Text>();

        //メインカメラをプレイヤーの視点に移動
        MainCamera = GameObject.Find("Main Camera");
        MainCamera.transform.parent = gameObject.transform;
        MainCamera.transform.localPosition = new Vector3(0, transform.localScale.y, 0);

        //CharacterController取得
        Chara = GetComponent<CharacterController>();

        //壁のない方向にプレイヤーを向ける
        MainCamera.transform.localRotation = Quaternion.identity;
        CameraRotation = MGM.StartDirection;
        transform.Rotate(0f, CameraRotation, 0f);

        DefaultReticle.color = Color.gray;
    }

    void Update()
    {
        if (GameManager.GameManager_Instance.CanPlayerMove)
        {
            PlayerMove();
            CameraMove();

            switch (HUDM.BType)
            {
                case HUDManager.BelongingsType.Hand:
                    PickHands();
                    break;
                case HUDManager.BelongingsType.Psyllium:
                    SlowPsyllium();
                    break;
                default:
                    break;
            }
        }

        //ゲームオーバー遷移（後でEnemyに接触時に変更）
        if (Input.GetKeyDown(KeyCode.F1))
        {
            GameManager.GameManager_Instance.CanPlayerMove = false;
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
        CameraRotation = ControlManager.ControlManager_Instance.RotateVertical * -SetRotateSpeed;

        //回転
        //Y軸回転はプレイヤーごと回す
        transform.Rotate(0, PlayerRotation.y, 0);
        MainCamera.transform.Rotate(CameraRotation, 0, 0);
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
                    HUDM.PickupMemo();
                    hit.collider.gameObject.SetActive(false);
                }
            }
            else if (hit.collider.CompareTag("Exit"))
            {
                DefaultReticle.color = Color.red;
                if (ControlManager.ControlManager_Instance.Action(ControlManager.PressType.Push))
                {
                    HUDM.IsTouchiGoal = true;
                }
            }
            else
            {
                DefaultReticle.color = Color.gray;
            }
        }
    }

    private void SlowPsyllium()
    {
        if (ControlManager.ControlManager_Instance.Action(ControlManager.PressType.Push))
        {
            PsylliumClone = Instantiate(Psyllium, gameObject.transform.position, Quaternion.identity);
            //PsylliumClone.GetComponent<Rigidbody>().AddForce(new Vector3(0f, 0f, SlowSpeed));
        }
    }
}
