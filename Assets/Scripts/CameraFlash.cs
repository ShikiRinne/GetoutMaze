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
    private Image Flash = null;
    private Image Finder = null;

    [SerializeField]
    private float AttenuateTime = 0f;
    private float Alpha = 0f;

    private Color FlashColor = new Color(1f, 1f, 1f, 0f);

    public bool IsShoot { get; set; } = false;
    public bool IsFlash { get; set; } = false;


    void Start()
    {
        foreach (Transform camera in gameObject.transform)
        {
            switch (camera.name)
            {
                case "Flash":
                    Flash = camera.GetComponent<Image>();
                    break;
                case "Finder":
                    Finder = camera.GetComponent<Image>();
                    break;
                default:
                    break;
            }
            camera.gameObject.SetActive(false);
        }

        Flash.color = FlashColor;
    }

    void Update()
    {
        
    }

    /// <summary>
    /// 撮影する
    /// </summary>
    public void CameraShoot()
    {
        //フラッシュ焚いてる間はカメラを下げない
        if (Input.GetMouseButton(1) || IsFlash)
        {
            Finder.gameObject.SetActive(true);
            Flash.gameObject.SetActive(true);

            //連打不可
            if (ControlManager.ControlManager_Instance.Action(ControlManager.PressType.Push) && !IsFlash)
            {
                IsShoot = true;
                StartCoroutine(FlashAttenuation());
            }
            else
            {
                IsShoot = false;
            }
        }
        else
        {
            Finder.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// フラッシュの減衰
    /// </summary>
    public IEnumerator FlashAttenuation()
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
            Flash.gameObject.SetActive(false);
        }
        yield return null;
    }
}
