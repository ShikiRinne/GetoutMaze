using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private MazeGenerateManager MGM;

    private NavMeshAgent Agent;

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
        
    }
}
