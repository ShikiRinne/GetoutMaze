using System.Collections;
using System.Collections.Generic;
using System.IO.Pipes;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    private HUDManager HUDM;
    private MazeGenerateManager MGM;
    private CameraFlash CF;

    private CharacterController Chara;

    private GameObject MainCamera;
    private GameObject Camera = default;
    [SerializeField]
    private GameObject Psyllium = default;

    [SerializeField]
    private float SetMoveSpeed = 0f;
    [SerializeField]
    private float SetRotateSpeed = 0f;
    [SerializeField]
    private float SetHandLength = 0f;

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
        Camera = GameObject.Find("Camera");
        CF = Camera.GetComponent<CameraFlash>();

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

        Camera.SetActive(false);

        DefaultReticle.color = Color.gray;
    }

    void Update()
    {
        if (GameManager.GameManager_Instance.CanPlayerMove)
        {
            //レイの射出
            PlayerHands = new Ray(MainCamera.transform.position, MainCamera.transform.forward);
            Debug.DrawRay(PlayerHands.origin, PlayerHands.direction, Color.red);

            //移動
            PlayerMove();
            CameraMove();

            //持ち物に対応した操作
            switch (HUDM.BType)
            {
                case HUDManager.BelongingsType.Hand:
                    PickHands();
                    break;
                case HUDManager.BelongingsType.Psyllium:
                    PutPsyllium();
                    break;
                case HUDManager.BelongingsType.Camera:
                    ReadyCamera();
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
        if (Physics.Raycast(PlayerHands, out RaycastHit hit, SetHandLength))
        {
            switch (hit.collider.tag)
            {
                case "Notes":
                    DefaultReticle.color = Color.red;
                    if (ControlManager.ControlManager_Instance.Action(ControlManager.PressType.Push))
                    {
                        HUDM.PickupMemo();
                        hit.collider.gameObject.SetActive(false);
                    }
                    break;
                case "Exit":
                    DefaultReticle.color = Color.red;
                    if (ControlManager.ControlManager_Instance.Action(ControlManager.PressType.Push))
                    {
                        HUDM.IsTouchiGoal = true;
                    }
                    break;
                case "Psyllium":
                    DefaultReticle.color = Color.red;
                    if (ControlManager.ControlManager_Instance.Action(ControlManager.PressType.Push))
                    {
                        Destroy(hit.collider.gameObject);
                        HUDM.PassPsylliumCount++;
                    }
                    break;
                default:
                    DefaultReticle.color = Color.gray;
                    break;
            }
        }
        else
        {
            DefaultReticle.color = Color.gray;
        }
    }

    /// <summary>
    /// サイリウムを置く
    /// </summary>
    private void PutPsyllium()
    {
        if (Physics.Raycast(PlayerHands, out RaycastHit hit, SetHandLength))
        {
            if (hit.collider.name == "Floor(Clone)")
            {
                DefaultReticle.color = Color.green;
                if (ControlManager.ControlManager_Instance.Action(ControlManager.PressType.Push) && HUDM.PassPsylliumCount > 0)
                {
                    //サイリウムをプレイヤーに向いている方向に倒して生成
                    Instantiate(Psyllium, new Vector3(hit.point.x, Psyllium.transform.localScale.z, hit.point.z), Quaternion.Euler(90f, transform.eulerAngles.y, 0f));
                    HUDM.PassPsylliumCount--;                    
                }
            }
            else
            {
                DefaultReticle.color = Color.gray;
            }
        }
        else
        {
            DefaultReticle.color = Color.gray;
        }
    }

    /// <summary>
    /// カメラ操作
    /// 右クリックで構えて左クリックで撮影
    /// </summary>
    private void ReadyCamera()
    {
        //フラッシュ焚いてる間はカメラを下げない
        if (Input.GetMouseButton(1) || CF.IsFlash)
        {
            Camera.SetActive(true);
            Debug.Log("Ready");

            //連打不可
            if (ControlManager.ControlManager_Instance.Action(ControlManager.PressType.Push) && !CF.IsFlash)
            {
                StartCoroutine(CF.CameraShoot());
            }
        }
        else
        {
            Camera.SetActive(false);
        }
    }
}
