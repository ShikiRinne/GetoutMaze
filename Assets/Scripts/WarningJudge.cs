using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarningJudge : MonoBehaviour
{
    private RadioNoise RN = null;
    private SphereCollider WarningCollider = null;

    [SerializeField]
    private float WarningRange = 0f;

    void Start()
    {
        RN = transform.parent.GetChild(0).GetComponent<RadioNoise>();
        WarningCollider = GetComponent<SphereCollider>();
        WarningCollider.radius = WarningRange;
    }

    void Update()
    {
        
    }

    /// <summary>
    /// エネミーが一定範囲内に侵入したとき警告音を鳴らす
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            RN.PlayWarningSound(true);
            Debug.Log("NearEnemy");
        }
    }

    /// <summary>
    /// 範囲外にエネミーが出たとき警告音を停止する
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            RN.PlayWarningSound(false);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawSphere(transform.position, WarningRange);
    }
}
