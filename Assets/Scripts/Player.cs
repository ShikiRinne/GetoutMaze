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

    [SerializeField]
    private AudioSource PlayerAudio;
    [SerializeField]
    private AudioSource PickMemoSource;
    [SerializeField]
    private AudioSource PsylliumSource;
    [SerializeField]
    private AudioClip Walk;
    [SerializeField]
    private AudioClip PickMemoClip;
    [SerializeField]
    private AudioClip PsylliumClip;

    public bool IsShoot { get; set; } = false;

    void Start()
    {
        HUDM = GameObject.Find("PlaySceneManager").GetComponent<HUDManager>();
        MGM = GameObject.Find("PlaySceneManager").GetComponent<MazeGenerateManager>();
        DefaultReticle = GameObject.Find("Default").GetComponent<Text>();
        CF = GameObject.Find("Camera").GetComponent<CameraFlash>();

        //メインカメラをプレイヤーの視点に移動
        MainCamera = GameObject.Find("Main Camera");
        MainCamera.transform.parent = gameObject.transform;
        MainCamera.transform.localPosition = new Vector3(0, transform.localScale.y, 0);

        //CharacterController取得
        Chara = GetComponent<CharacterController>();

        //壁のない方向にプレイヤーを向ける
        MainCamera.transform.localRotation = Quaternion.identity;
        //CameraRotation = MGM.PlayerStartDir;
        transform.Rotate(0f, MGM.PlayerStartDir, 0f);

        //レティクルの色をグレーに設定
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
                    CF.CameraShoot();
                    break;
                case HUDManager.BelongingsType.None:
                    Debug.LogError("不正な操作");
                    break;
                default:
                    break;
            }
        }
        else
        {
            PlayWalkSound(false);
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
        Chara.Move(SetMoveSpeed * Time.deltaTime * PlayerDirection.normalized);

        //歩行音を鳴らす
        //操作不可時（ゲームオーバー時）に停止
        if (Chara.velocity != Vector3.zero)
        {
            if (!PlayerAudio.isPlaying)
            {
                PlayWalkSound(true);
            }
        }
        else
        {
            PlayWalkSound(false);
        }
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
        CameraRotation += ControlManager.ControlManager_Instance.RotateVertical * -SetRotateSpeed;
        //カメラの上下方向の制限
        if (CameraRotation > 90f)
        {
            CameraRotation = 90f;
        }
        if (CameraRotation < -90f)
        {
            CameraRotation = -90f;
        }

        //回転
        //Y軸回転はプレイヤーごと回す
        transform.Rotate(0, PlayerRotation.y, 0);
        MainCamera.transform.localEulerAngles = new Vector3(CameraRotation, 0f, 0f);
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
                        PickMemoSource.PlayOneShot(PickMemoClip);
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
                        PsylliumSource.PlayOneShot(PsylliumClip);
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
            if (hit.collider.gameObject.CompareTag("Floor"))
            {
                DefaultReticle.color = Color.green;
                if (ControlManager.ControlManager_Instance.Action(ControlManager.PressType.Push) && HUDM.PassPsylliumCount > 0)
                {
                    //サイリウムをプレイヤーに向いている方向に倒して生成
                    Instantiate(Psyllium, new Vector3(hit.point.x, Psyllium.transform.localScale.z, hit.point.z), Quaternion.Euler(90f, transform.eulerAngles.y, 0f));
                    PsylliumSource.PlayOneShot(PsylliumClip);
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
    /// 歩行音の再生
    /// </summary>
    /// <param name="isplay"></param>
    private void PlayWalkSound(bool isplay)
    {
        if (isplay)
        {
            PlayerAudio.Play();
        }
        else
        {
            PlayerAudio.Stop();
        }
    }
}
