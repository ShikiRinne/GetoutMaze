using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

/// <summary>
/// マップの生成と同時にマップ上にメモをばら撒く
/// </summary>
public class MazeGenerateManager: MonoBehaviour
{
    private int[,] Maze;

    [SerializeField]
    private int MazeHight = default;
    [SerializeField]
    private int MazeWidth = default;

    [SerializeField]
    private GameObject Floor = default;
    [SerializeField]
    private GameObject Wall = default;
    [SerializeField]
    private GameObject Player = default;
    [SerializeField]
    private GameObject Exit = default;
    [SerializeField]
    private GameObject Memo = default;

    private int MemoCount;

    private List<int> PlacementObjectList;
    private List<int> DeadendPointList;
    private List<GameObject> MemoStorageList = new List<GameObject>();

    public Vector3 PassRestartPos { get; private set; }

    private enum ObjectType
    {
        Path = 0,
        Wall = 1,
        Start = 2,
        Exit = 3
    }

    private enum DirectionType
    {
        RIGHT = 0,
        DOWN = 1,
        LEFT = 2,
        UP = 3
    }

    public float StartDirection { get; private set; }

    public int PassTotalSplitMemos { get; private set; } = 4;

    //配置した後メモを管理するKeyCodeMemoで使用するためプロパティで受け渡しできるようにする
    public List<int> NotePositionList { get; set; }
    public List<GameObject> NotesList { get; set; }

    void Start()
    {
        Create();
    }

    void Update()
    {
        if (GameManager.GameManager_Instance.WantReset)
        {
            CharaPosReset();
            GameManager.GameManager_Instance.WantReset = false;
        }
    }

    /// <summary>
    /// マップ構成を決める
    /// </summary>
    private void Design()
    {
        //mazesizeが偶数の場合奇数に変更
        if (MazeHight % 2 == 0)
        {
            MazeHight++;
        }
        if(MazeWidth % 2 == 0)
        {
            MazeWidth++;
        }

        Maze = new int[MazeHight, MazeWidth];

        //外壁生成
        for (int y = 0; y < MazeHight; ++y)
        {
            for (int x = 0; x < MazeWidth; ++x)
            {
                if (y == 0 || y == MazeHight - 1 ||
                    x == 0 || x == MazeWidth - 1)
                {
                    Maze[x, y] = (int)ObjectType.Wall;
                }
                else
                {
                    Maze[x, y] = (int)ObjectType.Path;
                }
            }
        }

        //内壁生成
        int RandomDirection;
        int AddXWall;
        int AddYWall;
        for (int y = 2; y < MazeHight - 1; y += 2)
        {
            for (int x = 2; x < MazeWidth - 1; x += 2)
            {
                //棒倒し法の基準となる壁の生成
                Maze[x, y] = (int)ObjectType.Wall;

                //棒倒し法でマップ内が迷路状になるよう壁を生成
                while (true)
                {
                    //内壁の最上段のみ上側に壁が生成できる
                    if(y == 2)
                    {
                        RandomDirection = Random.Range(0, 4);
                    }
                    else
                    {
                        RandomDirection = Random.Range(0, 3);
                    }

                    //四方のどこか一方をランダムに選出
                    AddXWall = x;
                    AddYWall = y;
                    switch (RandomDirection)
                    {
                        case (int)DirectionType.RIGHT:
                            AddXWall++;
                            break;
                        case (int)DirectionType.DOWN:
                            AddYWall++;
                            break;
                        case (int)DirectionType.LEFT:
                            AddXWall--;
                            break;
                        case (int)DirectionType.UP:
                            AddYWall--;
                            break;
                    }
                    //選出された方向に壁がなければ生成して抜ける、あれば繰り返す
                    if (Maze[AddXWall, AddYWall] != (int)ObjectType.Wall)
                    {
                        Maze[AddXWall, AddYWall] = (int)ObjectType.Wall;
                        break;
                    }
                }
            }
        }

        //行き止まりになっているポイントをリストに保存
        //同時にマップ全体を番号付けしてリストに保存
        DeadendPointList = new List<int>();
        int MapNumber = 0;
        int WallCount = 0;
        for (int y = 0; y < MazeHight; ++y)
        {
            for (int x = 0; x < MazeWidth; ++x)
            {
                //四方のどこに壁があるかチェック、いくつの方向に壁があるかカウント
                if (Maze[x, y] == (int)ObjectType.Path)
                {
                    if (Maze[x + 1, y] == (int)ObjectType.Wall)
                    {
                        WallCount++;
                    }
                    if (Maze[x - 1, y] == (int)ObjectType.Wall)
                    {
                        WallCount++;
                    }
                    if (Maze[x, y + 1] == (int)ObjectType.Wall)
                    {
                        WallCount++;
                    }
                    if (Maze[x, y - 1] == (int)ObjectType.Wall)
                    {
                        WallCount++;
                    }
                }

                //三方向に壁がある位置を行き止まりのリストに追加
                if (WallCount >= 3)
                {
                    DeadendPointList.Add(MapNumber);
                    WallCount = 0;
                }
                else
                {
                    WallCount = 0;
                }

                MapNumber++;
            }
        }

        //配置するオブジェクトのリストを作成
        PlacementObjectList = new List<int>();
        for (int y = 0; y < MazeHight; ++y)
        {
            for (int x = 0; x < MazeWidth; ++x)
            {
                PlacementObjectList.Add(Maze[x, y]);
            }
        }

        //行き止まりの位置からランダムに選出
        MemoCount = PassTotalSplitMemos;
        NotePositionList = new List<int>();
        int RandomPoint;
        for (int i = 0; i < MemoCount + 2; ++i)
        {
            RandomPoint = Random.Range(0, DeadendPointList.Count);
            
            //先にスタートとゴールの位置を選出し、残りをメモを置く位置とする
            if (i == 0)
            {
                PlacementObjectList[DeadendPointList[RandomPoint]] = (int)ObjectType.Start;
            }
            else if (i == 1)
            {
                PlacementObjectList[DeadendPointList[RandomPoint]] = (int)ObjectType.Exit;
            }
            else
            {
                NotePositionList.Add(DeadendPointList[RandomPoint]);
            }

            DeadendPointList.RemoveAt(RandomPoint);
        }
    }

    /// <summary>
    /// マップの生成
    /// </summary>
    public void Create()
    {
        Design();

        //床をマップの大きさをもとにサイズと位置を調整して生成
        Floor.GetComponent<Transform>().localScale = new Vector3(MazeWidth * 0.1f, 1, MazeHight * 0.1f);
        Instantiate(Floor, new Vector3(Mathf.Floor(MazeWidth / 2.0f), 0, Mathf.Floor(MazeHight / 2.0f)), Quaternion.identity);

        //オブジェクト配置リストに応じたオブジェクトを配置していく
        int Count = 0;
        for (int y = 0; y < MazeHight; ++y)
        {
            for (int x = 0; x < MazeWidth; ++x)
            {
                
                switch (PlacementObjectList[Count])
                {
                    case (int)ObjectType.Path:
                        //メモを置く位置であればメモを置く
                        if (NotePositionList.Contains(Count))
                        {
                            MemoStorageList.Add(Instantiate(Memo, new Vector3(x, 0, y), Quaternion.identity));
                        }                        
                        break;
                    case (int)ObjectType.Wall:
                        //壁を配置
                        Instantiate(Wall, new Vector3(x, 0, y), Quaternion.identity);
                        break;
                    case (int)ObjectType.Start:
                        //プレイヤーを配置
                        //プレイヤーが出現する向きを通路側に向ける
                        if (Maze[x + 1, y] == (int)ObjectType.Path)
                        {
                            StartDirection = 90f;
                        }
                        if (Maze[x, y - 1] == (int)ObjectType.Path)
                        {
                            StartDirection = 180f;
                        }
                        if (Maze[x, y + 1] == (int)ObjectType.Path)
                        {
                            StartDirection = 0f;
                        }
                        if (Maze[x - 1, y] == (int)ObjectType.Path)
                        {
                            StartDirection = -90f;
                        }
                        //プレイヤーの出現位置をプレイヤーの高さに合わせる
                        PassRestartPos = new Vector3(x, Player.transform.localScale.y, y);
                        Instantiate(Player, PassRestartPos, Quaternion.identity);
                        break;
                    case (int)ObjectType.Exit:
                        //出口を配置
                        Instantiate(Exit, new Vector3(x, 0, y), Quaternion.identity);
                        break;
                }

                Count++;
            }
        }
    }

    public void CharaPosReset()
    {
        Player.transform.position = PassRestartPos;
        Debug.Log("PositionReset");
    }
}
