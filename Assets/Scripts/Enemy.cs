using System.Collections;
using System.Collections.Generic;
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

    private GameObject Player;
    private GameObject Target = null;
    [SerializeField]
    private GameObject RecognitionArea = null;
    private SphereCollider AreaCollider = null;
    
    private float SearchAngle;
    [SerializeField]
    private float LimitAngle = 0f;
    [SerializeField]
    private float SearchLength = 0f;
    [SerializeField]
    private float ChaseLength = 0f;
    [SerializeField]
    private float WaitTime = 0f;
    private float TimeCount = 0f;

    private int NextPoint = 0;

    private Vector3 PlayerDirection;

    public enum EnemyState
    {
        Wandering,
        Chase,
        Illuminated
    }
    public EnemyState NowState;

    void Start()
    {
        MGM = GameObject.Find("PlaySceneManager").GetComponent<MazeGenerateManager>();
        Player = GameObject.FindWithTag("Player");
        AreaCollider = RecognitionArea.GetComponent<SphereCollider>();
        AreaCollider.radius = SearchLength;

        Agent = gameObject.GetComponent<NavMeshAgent>();
        transform.Rotate(0f, MGM.EnemyStartDir, 0f);

        NextTarget();
        NowState = EnemyState.Wandering;
    }

    void Update()
    {
        Agent.isStopped = GameManager.GameManager_Instance.IsEnemyStop;

        EnemyStateMove(NowState);
    }

    /// <summary>
    /// エネミーの状態ごとの処理
    /// </summary>
    /// <param name="state"></param>
    private void EnemyStateMove(EnemyState state)
    {
        switch (state)
        {
            case EnemyState.Wandering:
                if (Agent.remainingDistance == 0f)
                {
                    Target = null;
                    NextPoint = Random.Range(0, MGM.DeadendObjectList.Count);
                    Wait();
                }
                break;
            case EnemyState.Chase:
                Agent.SetDestination(Player.transform.position);
                break;
            case EnemyState.Illuminated:
                break;
        }
    }

    /// <summary>
    /// 停止処理
    /// </summary>
    private void Wait()
    {
        Agent.isStopped = true;
        TimeCount += Time.deltaTime;
        if (TimeCount >= WaitTime)
        {
            TimeCount = 0f;
            if (Agent.isStopped)
            {
                NextTarget();
                Agent.isStopped = false;
            }
        }
    }

    /// <summary>
    /// 次の移動地点を探索する
    /// </summary>
    private void NextTarget()
    {
        Target = MGM.DeadendObjectList[NextPoint];
        Agent.SetDestination(Target.transform.position);
        Debug.Log("nextpos:(" + Target.transform.position.x + ", " + Target.transform.position.z + ")");
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
            NowState = EnemyState.Wandering;
        }
    }
}
