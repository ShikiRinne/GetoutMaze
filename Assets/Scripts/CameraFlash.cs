using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraFlash : MonoBehaviour
{
    [SerializeField]
    private Image Flash = null;
    [SerializeField]
    private Image Finder = null;

    [SerializeField]
    private float AttenuateTime = 0f;
    private float Alpha = 1f;

    private float FlashAlpha = 0f;


    void Start()
    {
        FlashAlpha = Flash.color.a;
        Finder.gameObject.SetActive(false);
        Flash.gameObject.SetActive(false);
    }

    void Update()
    {
        
    }

    /// <summary>
    /// カメラを構える
    /// </summary>
    public void CameraReady()
    {
        if (Input.GetMouseButton(1))
        {
            Finder.gameObject.SetActive(true);
            Debug.Log("Ready");

            if (Input.GetMouseButtonDown(0))
            {
                Flash.gameObject.SetActive(true);
                StartCoroutine(CameraShoot());
                Debug.Log("Shoot");
            }
        }
        else
        {
            Finder.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 撮影する
    /// </summary>
    IEnumerator CameraShoot()
    {
        Alpha = 1f;

        while (FlashAlpha > 0)
        {
            Alpha -= Time.deltaTime / AttenuateTime;
            Alpha = Mathf.Clamp01(Alpha);
            FlashAlpha = Alpha;

            yield return null;
        }

        //if (Flash.color.a <= 0)
        //{
        //    Flash.gameObject.SetActive(false);
        //}

        yield return null;
    }
}
