using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class RadioNoise : MonoBehaviour
{
    [SerializeField]
    private AudioMixer NoiseMixer;
    [SerializeField]
    private AudioSource WarningSource;
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

    void Start()
    {
        
    }

    void Update()
    {
        //シーン遷移のフェード時にオーディオを停止する
        if (GameManager.GameManager_Instance.IsAudioStop)
        {
            WarningSource.Stop();
            ChaseSource.Stop();
        }
    }

    /// <summary>
    /// プレイヤーとエネミーの距離が近い場合に鳴らす
    /// </summary>
    /// <param name="isWarning"></param>
    public void PlayWarningSound(bool isWarning)
    {
        if (isWarning)
        {
            WarningSource.Play();
        }
        else
        {
            WarningSource.Stop();
        }
    }

    /// <summary>
    /// エネミーがプレイヤーを発見したときに鳴らす
    /// </summary>
    public void PlayChaseSound(bool isChase)
    {
        if (isChase)
        {
            //警告音を停止して追跡音のみを再生させる
            WarningSource.Stop();
            ChaseSource.Play();
            FoundSource.PlayOneShot(FoundSE);
        }
        else
        {
            ChaseSource.Stop();
        }
    }
}
