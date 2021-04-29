using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

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
    private GameObject StartPoint = default;
    [SerializeField]
    private GameObject ExitPoint = default;
    [SerializeField]
    private GameObject DeadEndPoint = default;
    [SerializeField]
    private GameObject Memo = default;
    [SerializeField]
    private GameObject Enemy = default;

    private GameObject PlayerClone;
    private GameObject FarthestPoit = null;
    private float Distance = 0f;

    private List<int> MemoPositionList = new List<int>();
    private List<int> PlacementObjectList;
    private List<int> DeadendPointList;
    private List<GameObject> DeadendObjectList = new List<GameObject>();

    public Vector3 PassRestartPos { get; private set; }
    public float PlayerStartDir { get; private set; }
    public float EnemyStartDir { get; private set; }
    public int PassTotalSplitMemos { get; private set; } = 4;

    private enum MazePoint
    {
        Path = 0,
        Wall = 1,
        Start = 2,
        Exit = 3,
        DeadEnd = 4
    }

    private enum DirectionType
    {
        RIGHT = 0,
        DOWN = 1,
        LEFT = 2,
        UP = 3
    }

    void Start()
    {
        Create();
    }

    void Update()
    {
        //ゲームオーバーからのリセット
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
                    Maze[x, y] = (int)MazePoint.Wall;
                }
                else
                {
                    Maze[x, y] = (int)MazePoint.Path;
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
                Maze[x, y] = (int)MazePoint.Wall;

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
                    if (Maze[AddXWall, AddYWall] != (int)MazePoint.Wall)
                    {
                        Maze[AddXWall, AddYWall] = (int)MazePoint.Wall;
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
                if (Maze[x, y] == (int)MazePoint.Path)
                {
                    if (Maze[x + 1, y] == (int)MazePoint.Wall)
                    {
                        WallCount++;
                    }
                    if (Maze[x - 1, y] == (int)MazePoint.Wall)
                    {
                        WallCount++;
                    }
                    if (Maze[x, y + 1] == (int)MazePoint.Wall)
                    {
                        WallCount++;
                    }
                    if (Maze[x, y - 1] == (int)MazePoint.Wall)
                    {
                        WallCount++;
                    }
                }

                //三方向に壁がある位置を行き止まりとする
                if (WallCount >= 3)
                {
                    //マップ上に行き止まりとして配置しリストに保存
                    Maze[x, y] = (int)MazePoint.DeadEnd;
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

        //行き止まりの位置からランダムに選出し上書き
        int RandomPoint;
        for (int i = 0; i < PassTotalSplitMemos + 2; ++i)
        {
            RandomPoint = Random.Range(0, DeadendPointList.Count);
            
            //先にスタートとゴールの位置を選出し、残りをメモを置く位置とする
            if (i == 0)
            {
                PlacementObjectList[DeadendPointList[RandomPoint]] = (int)MazePoint.Start;
            }
            else if (i == 1)
            {
                PlacementObjectList[DeadendPointList[RandomPoint]] = (int)MazePoint.Exit;
            }
            else
            {
                MemoPositionList.Add(DeadendPointList[RandomPoint]);
            }

            DeadendPointList.RemoveAt(RandomPoint);
        }

        DeadendPointList.Clear();
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
        GameObject.Find("Floor(Clone)").GetComponent<NavMeshSurface>().BuildNavMesh();

        //オブジェクト配置リストに応じたオブジェクトを配置していく
        int Count = 0;
        for (int y = 0; y < MazeHight; ++y)
        {
            for (int x = 0; x < MazeWidth; ++x)
            {
                switch (PlacementObjectList[Count])
                {
                    case (int)MazePoint.Path:
                        break;
                    case (int)MazePoint.Wall:
                        //壁を配置
                        Instantiate(Wall, new Vector3(x, 0, y), Quaternion.identity);
                        break;
                    case (int)MazePoint.Start:
                        //プレイヤーを配置
                        //プレイヤーが出現する向きを通路側に向ける
                        PlayerStartDir = CharaDirection(x, y);
                        //プレイヤーの出現位置をプレイヤーの高さに合わせる
                        PassRestartPos = new Vector3(x, StartPoint.transform.localScale.y + StartPoint.GetComponent<CharacterController>().skinWidth, y);
                        PlayerClone = Instantiate(StartPoint, PassRestartPos, Quaternion.identity);
                        break;
                    case (int)MazePoint.Exit:
                        //出口を配置
                        Instantiate(ExitPoint, new Vector3(x, 0, y), Quaternion.identity);
                        break;
                    case (int)MazePoint.DeadEnd:
                        DeadendObjectList.Add(Instantiate(DeadEndPoint, new Vector3(x, 0, y), Quaternion.identity));
                        DeadendPointList.Add(Count);
                        if (FarthestPoit == null)
                        {
                            FarthestPoit = DeadendObjectList[0];
                        }
                        if (MemoPositionList.Contains(Count))
                        {
                            Instantiate(Memo, new Vector3(x, 0, y), Quaternion.identity);
                        }
                        break;
                    default:
                        break;
                }
                Count++;
            }
        }

        //すべてのマップ上の配置が完了した後にエネミーの位置を決定する
        if (Count == PlacementObjectList.Count)
        {
            CreateEnemy();
        }
    }

    /// <summary>
    /// エネミーの生成
    /// </summary>
    public void CreateEnemy()
    {
        //エネミーを生成する位置の決定
        int enemypoint = 0;
        for (int i = 0; i < DeadendObjectList.Count; ++i)
        {
            //行き止まりの位置とプレイヤーの位置を比較
            Distance = Mathf.Abs((PlayerClone.transform.position - DeadendObjectList[i].transform.position).sqrMagnitude);

            //現在保存している位置よりも遠い位置であれば上書き
            if (Distance > Mathf.Abs((PlayerClone.transform.position - FarthestPoit.transform.position).sqrMagnitude))
            {
                FarthestPoit = DeadendObjectList[i];
                enemypoint = i;
            }
        }

        //エネミーが出現する向きを通路側に向ける
        int Count = 0;
        for (int y = 0; y < MazeHight; ++y)
        {
            for (int x = 0; x < MazeWidth; ++x)
            {
                if (Count == DeadendPointList[enemypoint])
                {
                    EnemyStartDir = CharaDirection(x, y);
                }
                Count++;
            }
        }

        //最終的に決定した位置にエネミーを生成
        Instantiate(Enemy, new Vector3(FarthestPoit.transform.position.x, Enemy.transform.localScale.y / 2, FarthestPoit.transform.position.z), Quaternion.identity);
    }

    /// <summary>
    /// キャラクター生成時に通路側へ向かせる
    /// </summary>
    /// <param name="posX"></param>
    /// <param name="posY"></param>
    /// <returns></returns>
    private float CharaDirection(int posX, int posY)
    {
        float direction = 0;
        if (Maze[posX + 1, posY] == (int)MazePoint.Path)
        {
            direction = 90f;
        }
        if (Maze[posX, posY - 1] == (int)MazePoint.Path)
        {
            direction = 180f;
        }
        if (Maze[posX, posY + 1] == (int)MazePoint.Path)
        {
            direction = 0f;
        }
        if (Maze[posX - 1, posY] == (int)MazePoint.Path)
        {
            direction = -90f;
        }

        return direction;
    }

    /// <summary>
    /// プレイヤーの状態のリセット
    /// </summary>
    public void CharaPosReset()
    {
        //プレイヤーとの親子関係（MainCamera）を解除
        PlayerClone.transform.DetachChildren();

        //ステージ上のプレイヤーを一旦削除し再生成
        Destroy(PlayerClone);
        PlayerClone = Instantiate(StartPoint, PassRestartPos, Quaternion.identity);
    }
}
