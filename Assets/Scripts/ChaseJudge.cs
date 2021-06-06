using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 一定範囲内にプレイヤーが侵入すると追跡する
/// </summary>
public class ChaseJudge : MonoBehaviour
{
    private Enemy EnemyCS = null;
    private SphereCollider ChaseCollider = null;

    [SerializeField]
    private float ChaseRange = 0f;

    void Start()
    {
        EnemyCS = transform.parent.GetComponent<Enemy>();
        ChaseCollider = GetComponent<SphereCollider>();
        ChaseCollider.radius = ChaseRange;
    }

    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            EnemyCS.PlayerChase(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            EnemyCS.StopChase();
        }
    }
}
