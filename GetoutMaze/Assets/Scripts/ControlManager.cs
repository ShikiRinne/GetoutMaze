using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 入力周りを一括で管理する
/// </summary>
public class ControlManager : MonoBehaviour
{
    public static ControlManager ControlManager_Instance;

    public float MoveHorizontal { get; private set; }
    public float MoveVertical { get; private set; }
    public float RotateHorizontal { get; private set; }
    public float RotateVertical { get; private set; }

    public int VerticalInput { get; private set; }
    public int HorizontalInput { get; private set; }

    public bool CanControl { get; set; } = false;
    public bool CanMove { get; set; } = false;

    public enum ArrowType
    {
        Move,
        Select
    }

    public enum PressType
    {
        Push,
        Hold
    }

    void Awake()
    {
        //インスタンス生成
        ControlManager_Instance = GetComponent<ControlManager>();
    }

    void Start()
    {

    }

    /// <summary>
    /// カーソル入力
    /// </summary>
    public void InputArrow(ArrowType type)
    {
        switch (type)
        {
            //移動の際の各方向及びスティック入力
            case ArrowType.Move:
                if (CanControl && (Input.GetAxisRaw("Horizontal") != 0.0f || Input.GetAxisRaw("Vertical") != 0.0f))
                {
                    MoveHorizontal = Input.GetAxisRaw("Horizontal");
                    MoveVertical = Input.GetAxisRaw("Vertical");
                }
                else
                {
                    MoveHorizontal = 0.0f;
                    MoveVertical = 0.0f;
                }
                break;
            //項目選択時のキー入力した瞬間の判定
            case ArrowType.Select:
                if (CanControl)
                {
                    if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
                    {
                        VerticalInput = 1;
                    }
                    else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
                    {
                        VerticalInput = -1;
                    }
                    else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
                    {
                        HorizontalInput = 1;
                    }
                    else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
                    {
                        HorizontalInput = -1;
                    }
                    else
                    {
                        VerticalInput = 0;
                        HorizontalInput = 0;
                    }
                }
                else
                {
                    VerticalInput = 0;
                    HorizontalInput = 0;
                }
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// カメラ移動入力
    /// </summary>
    public void ViewAndCursor()
    {
        if (CanControl && (Input.GetAxisRaw("Mouse X") != 0.0f || Input.GetAxisRaw("Mouse Y") != 0.0f))
        {
            RotateHorizontal = Input.GetAxisRaw("Mouse X");
            RotateVertical = Input.GetAxisRaw("Mouse Y");
        }
        else
        {
            RotateHorizontal = 0.0f;
            RotateVertical = 0.0f;
        }
    }

    /// <summary>
    /// アクションを起こす
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public bool Action(PressType type)
    {
        switch(type)
        {
            //一度だけ押す判定
            case PressType.Push:
                if (Input.GetMouseButtonDown(0))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            //押し続ける判定
            case PressType.Hold:
                if (Input.GetMouseButton(0))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            default:
                return false;
        }
    }
}
