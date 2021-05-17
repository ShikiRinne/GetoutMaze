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
    [SerializeField]
    private GameObject RecognitionArea = null;
    [SerializeField]
    private SphereCollider AreaCollider = null;

    private Renderer EnemyRenderer;
    
    private float SearchAngle;
    [SerializeField]
    private float LimitAngle = 0f;
    [SerializeField]
    private float SearchLength = 0f;
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

    private Vector3 PlayerDirection;

    private Color EnemyColor = new Color(1f, 1f, 1f, 1f);
    
    public bool IsAttack { get; set; } = false;
    public bool IsReGeneration { get; set; } = false;

    public enum EnemyState
    {
        Wandering,
        Chase,
        Illuminated
    }
    public EnemyState NowState { get; set; }

    void Start()
    {
        MGM = GameObject.Find("PlaySceneManager").GetComponent<MazeGenerateManager>();
        CF = GameObject.Find("Camera").GetComponent<CameraFlash>();
        Player = GameObject.FindWithTag("Player");
        AreaCollider = RecognitionArea.GetComponent<SphereCollider>();
        AreaCollider.radius = SearchLength;

        Agent = gameObject.GetComponent<NavMeshAgent>();
        transform.Rotate(0f, MGM.EnemyStartDir, 0f);

        EnemyRenderer = gameObject.GetComponent<Renderer>();

        Wait();
        NowState = EnemyState.Wandering;
        IsReGeneration = false;
    }

    void Update()
    {
        EnemyStateMove(NowState);

        Debug.Log(EnemyRenderer.material.color.a);

        if (IsReGeneration)
        {
            Destroy(gameObject);
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
                if (Agent.remainingDistance <= AttackLength)
                {
                    Attack();
                }
                break;
            case EnemyState.Illuminated:
                StartCoroutine(FlashIlluminated());
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
        IsAttack = true;
        Agent.speed = AttackSpeed;
        EnemyRenderer.material.color = Color.red;

        if (CF.IsShoot)
        {
            NowState = EnemyState.Illuminated;
        }
    }

    /// <summary>
    /// プレイヤー探知
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            //探知する角度内であれば探知
            PlayerDirection = other.transform.position - transform.position;
            SearchAngle = Vector3.Angle(transform.forward, PlayerDirection);
            if (SearchAngle <= LimitAngle)
            {
                Agent.SetDestination(Player.transform.position);
                NowState = EnemyState.Chase;
            }
        }
    }

    /// <summary>
    /// プレイヤー非探知
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Target = null;
            NowState = EnemyState.Wandering;
        }
    }

    /// <summary>
    /// 被フラッシュ時
    /// </summary>
    /// <returns></returns>
    public IEnumerator FlashIlluminated()
    {
        Debug.Log("Illuminated");
        Agent.isStopped = true;
        Target = null;
        EnemyAlpha = EnemyRenderer.material.color.a;
        yield return null;

        while (EnemyColor.a > 0)
        {
            EnemyAlpha -= Time.deltaTime / DisappearTime;
            EnemyAlpha = Mathf.Clamp01(EnemyAlpha);
            EnemyColor = new Color(EnemyRenderer.material.color.r, EnemyRenderer.material.color.g, EnemyRenderer.material.color.b, EnemyAlpha);
            EnemyRenderer.material.color = EnemyColor;

            yield return null;
        }

        //たぶんIsAttackで赤くする処理が止まってないからアルファ値が0にならなくてここに入らない
        if (EnemyRenderer.material.color.a <= 0)
        {
            IsReGeneration = true;
        }

        yield return null;
    }

    //private void OnDrawGizmos()
    //{
    //    Handles.color = Color.red;
    //    Handles.DrawSolidArc(transform.position, Vector3.up, Quaternion.Euler(0f, -LimitAngle, 0f) * transform.forward, LimitAngle * 2f, AreaCollider.radius);
    //}
}
