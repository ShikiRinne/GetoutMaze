using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private NavMeshAgent Agent;

    public enum EnemyState
    {
        Wandering,
        Chase,
        Illuminated
    }

    void Start()
    {
        Agent = gameObject.GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        
    }
}
