using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// カメラ処理
/// </summary>
public class CameraFlash : MonoBehaviour
{
    [SerializeField]
    private Image Flash = null;

    [SerializeField]
    private float AttenuateTime = 0f;
    private float Alpha = 0f;

    private Color FlashColor = new Color(1f, 1f, 1f, 0f);

    public bool IsFlash { get; set; } = false;


    void Start()
    {
        Flash.color = FlashColor;
    }

    void Update()
    {
        
    }

    /// <summary>
    /// 撮影する
    /// </summary>
    public IEnumerator CameraShoot()
    {
        //最初に1を入れる（真っ白にする）
        IsFlash = true;
        Alpha = 1f;
        FlashColor = new Color(1f, 1f, 1f, Alpha);
        Flash.color = FlashColor;

        //徐々に透明にする（減衰させる）
        while (Alpha > 0)
        {
            Alpha -= Time.deltaTime / AttenuateTime;
            Alpha = Mathf.Clamp01(Alpha);
            FlashColor = new Color(1f, 1f, 1f, Alpha);
            Flash.color = FlashColor;

            yield return null;
        }

        if (Alpha <= 0)
        {
            IsFlash = false;
        }
        yield return null;
    }
}
