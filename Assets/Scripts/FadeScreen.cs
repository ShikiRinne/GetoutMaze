using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.UI;

public class FadeScreen : MonoBehaviour
{
    public static FadeScreen FadeScreen_Instance;
    private Color color;

    [SerializeField]
    private Image FadeImage = null;
    [SerializeField]
    private float FadeTime;
    private float Alpha;
    private float Monochrome;
    private float FadeDirection;

    /// <summary>
    /// フェードインかフェードアウトか
    /// </summary>
    public enum FadeType
    {
        IN,
        OUT
    }

    /// <summary>
    /// フェードさせる画像の色
    /// </summary>
    public enum FadeColor
    {
        Black,
        White
    }

    /// <summary>
    /// フェードしている時間を渡す
    /// </summary>
    public float PassFadeTime
    {
        get { return FadeTime; }
        private set { FadeTime = value; }
    }

    /// <summary>
    /// FadeImageを渡す
    /// </summary>
    public GameObject PassFadeObject { get; private set; }

    void Awake()
    {
        //インスタンス生成と同時に既に存在する場合は重複しないよう削除
        if (FadeScreen_Instance == null)
        {
            FadeScreen_Instance = GetComponent<FadeScreen>();
            PassFadeObject = gameObject;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        Alpha = FadeImage.color.a;
        Monochrome = 0.0f;
        color = new Color(Monochrome, Monochrome, Monochrome, Alpha);
    }

    /// <summary>
    /// フェードの実行
    /// </summary>
    /// <param name="type"></param>
    public IEnumerator FadeExecution(FadeType type, float fadetime, FadeColor fadeColor)
    {
        //フェードタイプから＋方向か－方向かを決定する
        switch (type)
        {
            case FadeType.IN:
                FadeDirection = -1;
                if (Alpha != 1)
                {
                    Alpha = 1;
                }
                while (FadeImage.color.a > 0)
                {
                    CalcAlpha(FadeDirection, fadetime, (float)fadeColor);
                    yield return null;
                }
                break;
            case FadeType.OUT:
                FadeDirection = 1;
                if (Alpha != 0)
                {
                    Alpha = 0;
                }
                while (FadeImage.color.a < 1)
                {
                    CalcAlpha(FadeDirection, fadetime, (float)fadeColor);
                    yield return null;
                }
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// アルファ値の加減算
    /// </summary>
    /// 正負方向 <param name="direction"></param>
    /// フェードさせる時間 <param name="fadetime"></param>
    /// 文字の色 <param name="monochrome"></param>
    public void CalcAlpha(float direction, float fadetime, float fadecolor)
    {
        Alpha += direction * (Time.deltaTime / fadetime);
        Alpha = Mathf.Clamp01(Alpha);
        color = new Color(fadecolor, fadecolor, fadecolor, Alpha);
        FadeImage.color = color;
    }
}
