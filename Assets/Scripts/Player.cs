using System.Collections;
using System.Collections.Generic;
using System.IO.Pipes;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    private HUDManager HUDM;
    private MazeGenerateManager MGM;
    private MemoManager MM;
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
    [SerializeField]
    private float SetHandSize = 0f;

    private Image DefaultReticle;

    private Vector3 PlayerDirection;
    private Vector3 Direction_Horizontal;
    private Vector3 Direction_Vertical;
    private Vector3 PlayerRotation;
    private float CameraRotation;

    private Ray HandRay;
    private RaycastHit Touch;

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

    private bool BoxCast = false;
    public bool IsShoot { get; set; } = false;

    void Start()
    {
        GameObject psm = GameObject.Find("PlaySceneManager");
        HUDM = psm.GetComponent<HUDManager>();
        MGM = psm.GetComponent<MazeGenerateManager>();
        MM = psm.GetComponent<MemoManager>();
        DefaultReticle = GameObject.Find("Default").GetComponent<Image>();
        CF = GameObject.Find("Camera").GetComponent<CameraFlash>();

        //メインカメラをプレイヤーの視点に移動
        MainCamera = GameObject.Find("Main Camera");
        MainCamera.transform.parent = gameObject.transform;
        MainCamera.transform.localPosition = new Vector3(0, transform.localScale.y, 0);

        //CharacterController取得
        Chara = GetComponent<CharacterController>();

        //壁のない方向にプレイヤーを向ける
        MainCamera.transform.localRotation = Quaternion.identity;
        transform.Rotate(0f, MGM.PlayerStartDir, 0f);

        //レティクルの色をグレーに設定
        DefaultReticle.color = Color.gray;
    }

    void Update()
    {
        if (GameManager.GameManager_Instance.CanPlayerMove)
        {
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
        //ボックスレイの射出
        BoxCast = Physics.BoxCast(MainCamera.transform.position,
                                  Vector3.one * (SetHandSize / 2f),
                                  MainCamera.transform.forward,
                                  out Touch,
                                  Quaternion.identity,
                                  SetHandLength);

        //触れた物に応じた処理
        if (BoxCast)
        {
            HUDM.ChangeReticleType(HUDManager.ReticleType.Hand);
            switch (Touch.collider.tag)
            {
                case "Notes":
                    HUDM.ChangeReticleType(HUDManager.ReticleType.Hand);
                    if (ControlManager.ControlManager_Instance.Action(ControlManager.PressType.Push))
                    {
                        MM.PickMemos();
                        Touch.collider.gameObject.SetActive(false);
                        PickMemoSource.PlayOneShot(PickMemoClip);
                    }
                    break;
                case "Exit":
                    HUDM.ChangeReticleType(HUDManager.ReticleType.Hand);
                    if (ControlManager.ControlManager_Instance.Action(ControlManager.PressType.Push))
                    {
                        HUDM.IsTouchiGoal = true;
                    }
                    break;
                case "Psyllium":
                    HUDM.ChangeReticleType(HUDManager.ReticleType.Hand);
                    if (ControlManager.ControlManager_Instance.Action(ControlManager.PressType.Push))
                    {
                        Destroy(Touch.collider.gameObject);
                        HUDM.PassPsylliumCount++;
                        PsylliumSource.PlayOneShot(PsylliumClip);
                    }
                    break;
                default:
                    HUDM.ChangeReticleType(HUDManager.ReticleType.Default);
                    break;
            }
        }
        else
        {
            HUDM.ChangeReticleType(HUDManager.ReticleType.Default);
        }
    }

    /// <summary>
    /// サイリウムを置く
    /// </summary>
    private void PutPsyllium()
    {
        //レイの射出
        HandRay = new Ray(MainCamera.transform.position, MainCamera.transform.forward);
        Debug.DrawRay(HandRay.origin, HandRay.direction, Color.red);

        if (Physics.Raycast(HandRay, out RaycastHit hit, SetHandLength))
        {
            if (hit.collider.gameObject.CompareTag("Floor"))
            {
                HUDM.ChangeReticleType(HUDManager.ReticleType.Psyllium);
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
                HUDM.ChangeReticleType(HUDManager.ReticleType.Default);
            }
        }
        else
        {
            HUDM.ChangeReticleType(HUDManager.ReticleType.Default);
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

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        if (BoxCast)
        {
            Gizmos.DrawRay(MainCamera.transform.position, MainCamera.transform.forward * Touch.distance);
            Gizmos.DrawWireCube(MainCamera.transform.position + MainCamera.transform.forward * Touch.distance, Vector3.one * (SetHandSize / 2f));
        }
        else
        {
            Gizmos.DrawRay(MainCamera.transform.position, MainCamera.transform.forward * SetHandLength);
            Gizmos.DrawWireCube(MainCamera.transform.position + MainCamera.transform.forward * SetHandLength, Vector3.one * (SetHandSize / 2f));
        }
    }
}
