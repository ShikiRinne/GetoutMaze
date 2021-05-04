using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private MazeGenerateManager MGM;
    private NavMeshAgent Agent;

    private GameObject Target = null;
    [SerializeField]
    private GameObject RecognitionArea = null;

    [SerializeField]
    private float SearchLength = 0f;
    [SerializeField]
    private float ChaseLength = 0f;
    [SerializeField]
    private float WaitTime = 0f;

    private bool IsSearchNext = false;

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

    private void EnemyStateMove(EnemyState state)
    {
        switch (state)
        {
            case EnemyState.Wandering:
                if (Agent.remainingDistance == 0f)
                {
                    Target = null;
                    NextTarget();
                }
                break;
            case EnemyState.Chase:
                break;
            case EnemyState.Illuminated:
                break;
        }
    }

    private IEnumerator ArriveTarget()
    {
        Agent.isStopped = true;
        yield return new WaitForSeconds(WaitTime);

        Agent.isStopped = false;
        yield return null;
    }

    private void NextTarget()
    {
        StartCoroutine(ArriveTarget());

        if (Target == null)
        {
            int random = Random.Range(0, MGM.DeadendObjectList.Count);
            Target = MGM.DeadendObjectList[random];
            Agent.SetDestination(Target.transform.position);
        }
        Debug.Log("nextpos:(" + Target.transform.position.x + ", " + Target.transform.position.z + ")");
    }
}
