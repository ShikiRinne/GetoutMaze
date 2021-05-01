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

    public enum EnemyState
    {
        Wandering,
        Chase,
        Illuminated
    }

    void Start()
    {
        MGM = GameObject.Find("PlaySceneManager").GetComponent<MazeGenerateManager>();

        Agent = gameObject.GetComponent<NavMeshAgent>();
        transform.Rotate(0f, MGM.EnemyStartDir, 0f);
    }

    void Update()
    {
        NextTarget();
        Agent.isStopped = GameManager.GameManager_Instance.IsEnemyStop;
    }

    private void NextTarget()
    {
        if (Target == null ||
            (gameObject.transform.position.x == Target.transform.position.x &&
             gameObject.transform.position.z == Target.transform.position.z))
        {
            Target = MGM.DeadendObjectList[Random.Range(0, MGM.DeadendObjectList.Count)];
            Agent.SetDestination(Target.transform.position);
        }
    }
}
