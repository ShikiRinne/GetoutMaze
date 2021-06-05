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

    private float PtoEDistance = 0f;

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

        if (EnemyCS.NowState == Enemy.EnemyState.Chase)
        {
            if (!IsFound)
            {
                Debug.Log("Found");
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
