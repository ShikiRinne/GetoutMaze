using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 一定範囲内にプレイヤーが侵入すると追跡する
/// </summary>
public class ChaseJudge : MonoBehaviour
{
    private RadioNoise RN = null;
    private Enemy EnemyCS = null;
    private SphereCollider ChaseCollider = null;

    [SerializeField]
    private float ChaseRange = 0f;

    void Start()
    {
        RN = GameObject.FindWithTag("Player").transform.GetChild(0).GetComponent<RadioNoise>();
        EnemyCS = transform.parent.GetComponent<Enemy>();
        ChaseCollider = GetComponent<SphereCollider>();
        ChaseCollider.radius = ChaseRange;
    }

    void Update()
    {
        if (RN == null)
        {
            RN = GameObject.FindWithTag("Player").transform.GetChild(0).GetComponent<RadioNoise>();
        }
    }

    /// <summary>
    /// プレイヤーが範囲内に存在する場合エネミーに追跡させる
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            EnemyCS.PlayerChase(other.gameObject);

            //エネミーがフラッシュを受けた場合追跡音を停止する
            if (EnemyCS.IsntPlayAudio)
            {
                RN.PlayChaseSound(false);
            }
        }
    }

    /// <summary>
    /// プレイヤーが範囲内に侵入したとき追跡音を再生する
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            RN.PlayChaseSound(true);
        }
    }

    /// <summary>
    /// 範囲外にプレイヤーが出た場合エネミーに追跡をやめさせる
    /// 追跡音を停止する
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            EnemyCS.StopChase();
            RN.PlayChaseSound(false);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, ChaseRange);
    }
}
