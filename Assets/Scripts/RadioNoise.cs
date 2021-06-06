using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class RadioNoise : MonoBehaviour
{
    private Enemy EnemyCS;

    [SerializeField]
    private AudioMixer NoiseMixer;
    [SerializeField]
    private AudioSource ChaseSource;
    [SerializeField]
    private AudioSource FoundSource;
    [SerializeField]
    private AudioClip FoundSE;

    [SerializeField]
    private float MinVolume = 0f;
    [SerializeField]
    private float MaxVolume = 0f;

    private bool IsFound = false;

    void Start()
    {
        EnemyCS = GameObject.FindWithTag("Enemy").GetComponent<Enemy>();
    }

    void Update()
    {
        if (EnemyCS == null)
        {
            EnemyCS = GameObject.FindWithTag("Enemy").GetComponent<Enemy>();
        }
    }

    /// <summary>
    /// エネミーがプレイヤーを発見したときに鳴らす
    /// </summary>
    public void PlayChaseSound()
    {
        if (EnemyCS.IsPlayerFind)
        {
            if (!IsFound)
            {
                Debug.Log("Noise");
                IsFound = true;
                FoundSource.PlayOneShot(FoundSE);
                ChaseSource.Play();
            }
        }
        else
        {
            IsFound = false;
            ChaseSource.Stop();
        }
    }
}
