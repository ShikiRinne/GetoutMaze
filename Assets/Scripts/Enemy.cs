using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// エネミーの処理
/// </summary>
public class Enemy : MonoBehaviour
{
    private MazeGenerateManager MGM;
    private NavMeshAgent Agent;
    private CameraFlash CF;

    private GameObject Player = null;
    private GameObject Target = null;

    private Renderer EnemyRenderer;
    
    private float SearchAngle;
    [SerializeField]
    private float LimitAngle = 0f;
    [SerializeField]
    private float AttackLength = 0f;
    [SerializeField]
    private float WanderingSpeed = 0f;
    [SerializeField]
    private float ChaseSpeed = 0f;
    [SerializeField]
    private float AttackSpeed = 0f;
    [SerializeField]
    private float DisappearTime = 0f;
    [SerializeField]
    private float WaitTime = 0f;
    private float TimeCount = 0f;
    private float EnemyAlpha = 0f;

    private int NextPoint = 0;

    private bool IsStop = false;
    public bool IsIlluminated { get; set; } = false;

    private Vector3 PlayerDirection;

    private Color EnemyColor = new Color(1f, 1f, 1f, 1f);
    
    public enum EnemyState
    {
        Wandering,
        Chase,
    }
    public EnemyState NowState { get; set; }

    void Start()
    {
        MGM = GameObject.Find("PlaySceneManager").GetComponent<MazeGenerateManager>();
        CF = GameObject.Find("Camera").GetComponent<CameraFlash>();
        Player = GameObject.FindWithTag("Player");

        Agent = gameObject.GetComponent<NavMeshAgent>();
        transform.Rotate(0f, MGM.EnemyStartDir, 0f);

        EnemyRenderer = gameObject.GetComponent<Renderer>();
        EnemyColor = EnemyRenderer.material.color;

        NextTarget();
        NowState = EnemyState.Wandering;
    }

    void Update()
    {
        EnemyStateMove(NowState);

        if (Player == null)
        {
            Player = GameObject.FindWithTag("Player");
        }

        if (IsStop)
        {
            Agent.isStopped = IsStop;
        }
        else if (GameManager.GameManager_Instance.IsEnemyStop)
        {
            Agent.isStopped = GameManager.GameManager_Instance.IsEnemyStop;
        }
        else
        {
            Agent.isStopped = false;
        }
    }

    /// <summary>
    /// エネミーの状態ごとの処理
    /// </summary>
    /// <param name="state"></param>
    private void EnemyStateMove(EnemyState state)
    {
        switch (state)
        {
            //徘徊
            case EnemyState.Wandering:
                Agent.speed = WanderingSpeed;
                //目的地到着時か目的地未設定時に再設定
                if (Agent.remainingDistance == 0f || Target == null)
                {
                    NextPoint = Random.Range(0, MGM.DeadendObjectList.Count);
                    Wait();
                }
                break;
            //追跡
            case EnemyState.Chase:
                Agent.speed = ChaseSpeed;
                //目的地（＝プレイヤー）までの距離が一定距離以下で攻撃
                if (Agent.remainingDistance <= AttackLength && !IsIlluminated)
                {
                    Attack();
                }
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 停止処理
    /// </summary>
    private void Wait()
    {
        TimeCount += Time.deltaTime;
        if (TimeCount >= WaitTime)
        {
            TimeCount = 0f;
            NextTarget();
        }
    }

    /// <summary>
    /// 次の移動地点を探索する
    /// </summary>
    private void NextTarget()
    {
        Target = MGM.DeadendObjectList[NextPoint];
        Agent.SetDestination(Target.transform.position);
    }

    /// <summary>
    /// プレイヤーを攻撃
    /// </summary>
    private void Attack()
    {
        Agent.speed = AttackSpeed;
        EnemyRenderer.material.color = Color.red;

        if (CF.IsShoot)
        {
            IsIlluminated = true;
            gameObject.GetComponent<SphereCollider>().enabled = false;
            StartCoroutine(FlashIlluminated());
        }
    }

    /// <summary>
    /// プレイヤーを追跡
    /// </summary>
    public void PlayerChase(GameObject player)
    {
        //探知する角度内であれば探知
        PlayerDirection = player.transform.position - transform.position;
        SearchAngle = Vector3.Angle(transform.forward, PlayerDirection);
        if (SearchAngle <= LimitAngle)
        {
            Agent.SetDestination(Player.transform.position);
            NowState = EnemyState.Chase;
        }
    }

    /// <summary>
    /// プレイヤーの追跡を中断
    /// </summary>
    public void StopChase()
    {
        Target = null;
        NowState = EnemyState.Wandering;
    }

    /// <summary>
    /// 被フラッシュ時
    /// </summary>
    /// <returns></returns>
    public IEnumerator FlashIlluminated()
    {
        IsStop = true;
        Target = null;
        EnemyAlpha = EnemyRenderer.material.color.a;
        yield return null;

        //アルファ値を徐々に減算
        while (EnemyColor.a > 0)
        {
            EnemyAlpha -= Time.deltaTime / DisappearTime;
            EnemyAlpha = Mathf.Clamp01(EnemyAlpha);
            EnemyColor = new Color(EnemyRenderer.material.color.r, EnemyRenderer.material.color.g, EnemyRenderer.material.color.b, EnemyAlpha);
            EnemyRenderer.material.color = EnemyColor;

            yield return null;
        }

        //エネミーを再生成
        if (EnemyRenderer.material.color.a <= 0)
        {
            MGM.CharaPosReset(MazeGenerateManager.ResetState.EnemyOnly);
        }

        yield return null;
    }

    /// <summary>
    /// プレイヤーとの衝突判定
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            IsStop = true;
            GameManager.GameManager_Instance.CanPlayerMove = false;
            GameManager.GameManager_Instance.TransitionGameState(GameManager.GameState.GameOver);
        }
    }
}
