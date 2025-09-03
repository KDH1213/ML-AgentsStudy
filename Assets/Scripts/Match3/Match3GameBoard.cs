using System.Collections.Generic;
using UnityEngine;

public struct GridInfo
{
    public GemObject gemObject;
    public GemType gemType;
    public Vector2 position;
}

public struct MoveInfo
{
    public (int, int) startGemMovePosition;
    public int matchCount;

    public MoveInfo((int, int) startGemMovePosition, int matchCount)
    {
        this.startGemMovePosition = startGemMovePosition;
        this.matchCount = matchCount;
    }
}

public enum FindType
{
    Row,
    Col
}

public enum FindDirection
{
    Up,
    Down,
    Left,
    Right
}

public struct Point
{
    public int x, y;
    public Point(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public static Point operator +(Point a, Point b)
    {
        a.x += b.x;
        a.y += b.y;
        return a;
    }

    public static Point operator +(Point a, (int, int) b)
    {
        a.x += b.Item2;
        a.y += b.Item1;
        return a;
    }

    public static (int, int) operator +((int,int) a, Point b)
    {
        a.Item1 += b.y;
        a.Item2 += b.x;
        return a;
    }

    public static Point Up => new Point(0, 1);
    public static Point Down => new Point(0, -1);
    public static Point Left => new Point(-1, 0);
    public static Point Right => new Point(1, 0);
}


public class Match3GameBoard : MonoBehaviour
{
    [SerializeField]
    private GameObject backgroundPrefab;

    [SerializeField]
    private Match3BoadData match3BoadData;

    [SerializeField]
    private GemObjectData gemObjectData;

    private GridInfo[,] gridInfos;

    private List<(int, int)> findGridList = new List<(int, int)>();
    private Dictionary<int, MoveInfo> moveGridTable = new Dictionary<int, MoveInfo>();

    private Vector2 leftBottomPosition;
    private Vector2 scale = Vector2.one;

    private Point[] FindDirections = new Point[4] { Point.Up, Point.Down, Point.Left, Point.Right };

    private int rowCount;
    private int columnCount;

    private float moveTime;
    private float currentMoveTime;

    public System.Action onEndMoveAction;
    private int moveCount;

    private void Awake()
    {
        Initalize();
    }

    private void Initalize()
    {
        var boardObject = Instantiate(backgroundPrefab);
        boardObject.transform.localScale = new Vector3(match3BoadData.Width, match3BoadData.Height, 1f);

        rowCount = match3BoadData.Height;
        columnCount = match3BoadData.Width;

        MakeBoard();
    }

    private void MakeBoard()
    {
        gridInfos = new GridInfo[match3BoadData.Height, match3BoadData.Width];
        Vector2 startPosition = transform.position;
        startPosition += -(new Vector2(((float)match3BoadData.Width / 2f), (float)match3BoadData.Height/ 2f)) + new Vector2(0.5f, 0.5f);

        leftBottomPosition = startPosition;


        for (int j = 0; j < match3BoadData.Height; ++j)
        {
            Vector2 createPosition = startPosition + Vector2.up * j;
            for (int i = 0; i < match3BoadData.Width; ++i)
            {
                var createObject = CreateRandomGem();                
                createObject.transform.position = createPosition;

                gridInfos[j, i].gemObject = createObject;
                gridInfos[j, i].gemType = createObject.GemType;
                gridInfos[j, i].position = createPosition;

                createPosition += Vector2.right;
            }
        }
    }

    public GemObject CreateRandomGem()
    {
        var randomType = gemObjectData.GetRandomGemType();
        var createObject = Instantiate(gemObjectData.GemObjectFrefab);
        createObject.SetGemType(randomType, gemObjectData.GetColor(randomType));
        return createObject;
    }

    public bool IsSeleteMoveable((int, int) lhs, (int, int) rhs, ref List<(int, int)> matchGridList)
    {
        bool leftMoveable = IsCheckMatched(gridInfos[rhs.Item1, rhs.Item2].gemType, lhs.Item1, lhs.Item2, ref matchGridList);
        bool rightMoveable = IsCheckMatched(gridInfos[lhs.Item1, lhs.Item2].gemType, rhs.Item1, rhs.Item2, ref matchGridList);

        return leftMoveable || rightMoveable;
    }

    public bool IsCheckMatched(GemType gemType, int row, int col, ref List<(int, int)> matchGridList)
    {
        bool isMatchedColumn = false;
        bool isMatchedRow = false;

        isMatchedColumn = IsCheckColumn(gemType, col, row, ref matchGridList);
        isMatchedRow = IsCheckRow(gemType, col, row, ref matchGridList);

        return isMatchedColumn || isMatchedRow;
    }

    public bool IsCheckMatched((int, int)findGrid)
    {
        bool isMatchedColumn = false;
        bool isMatchedRow = false;



        return isMatchedColumn || isMatchedRow;
    }

    public bool IsCheckMatch((int, int) findGrid, GemType gemType, FindDirection findDirection)
    {
        int index = (int)findDirection;

        for (int i = 0; i < 2; ++i)
        {
            findGrid += FindDirections[index];

            if (gridInfos[findGrid.Item1, findGrid.Item2].gemObject == null
                || gridInfos[findGrid.Item1, findGrid.Item2].gemType != gemType)
            {
                return false;
            }
        }

        return true;
    }

    public bool IsCheckColumn(GemType gemType, int col, int row, ref List<(int, int)> matchList)
    {
        findGridList.Clear();
        findGridList.Add((row, col));

        for (int i = 1; i < 3; ++i)
        {
            int findCol = col + i;
            if (columnCount == findCol
                || gemType != gridInfos[row, findCol].gemType)
            {
                break;
            }

            findGridList.Add((row, findCol));
        }

        for (int i = 1; i < 3; ++i)
        {
            int findCol = col - i;
            if (0 > findCol
                || gemType != gridInfos[row, findCol].gemType)
            {
                break;
            }

            findGridList.Add((row, findCol));
        }

        if (findGridList.Count >= 3)
        {
            foreach (var grid in findGridList)
            {
                matchList.Add(grid);

                if(!moveGridTable.TryGetValue(grid.Item2, out var moveInfo))
                {
                    moveGridTable.Add(grid.Item2, new MoveInfo((grid), 1));
                }
                else
                {
                    moveInfo.startGemMovePosition = grid.Item1 < moveInfo.startGemMovePosition.Item1 ? grid : moveInfo.startGemMovePosition;
                    moveGridTable[grid.Item2] = moveInfo;
                }
            }

            findGridList.Clear();
            return true;
        }

        findGridList.Clear();
        return false;
    }

    public bool IsCheckRow(GemType gemType, int col, int row, ref List<(int, int)> matchList)
    {
        findGridList.Clear();
        findGridList.Add((row, col));

        for (int i = 1; i < 3; ++i)
        {
            int findrow = row + i;
            if (rowCount == findrow
                || gemType != gridInfos[findrow, col].gemType)
            {
                findGridList.Clear();
                findGridList.Add((row, col));
                break;
            }

            findGridList.Add((findrow, col));
        }

        for (int i = 1; i < 3; ++i)
        {
            int findrow = row - i;
            if (0 > findrow
                || gemType != gridInfos[findrow, col].gemType)
            {
                findGridList.Clear();
                findGridList.Add((row, col));
                break;
            }

            findGridList.Add((findrow, col));
        }

        if (findGridList.Count >= 3)
        {
            foreach (var grid in findGridList)
            {
                matchList.Add(grid);

                if (!moveGridTable.TryGetValue(grid.Item2, out var moveInfo))
                {
                    moveGridTable.Add(grid.Item2, new MoveInfo((grid), findGridList.Count));
                }
                else
                {
                    moveInfo.startGemMovePosition = grid.Item1 < moveInfo.startGemMovePosition.Item1 ? grid : moveInfo.startGemMovePosition;
                    moveInfo.matchCount = findGridList.Count;
                    moveGridTable[grid.Item2] = moveInfo;
                }
            }

            findGridList.Clear();

            return true;
        }

        findGridList.Clear();
        return false;
    }

    public void DestroyGem(ref List<(int, int)> matchGridList)
    {
        foreach(var grid in matchGridList)
        {
            // 임시로 비활성화 하기
            if (gridInfos[grid.Item1, grid.Item2].gemType != GemType.Empty)
            {
                gridInfos[grid.Item1, grid.Item2].gemObject.gameObject.SetActive(false);

                gridInfos[grid.Item1, grid.Item2].gemType = GemType.Empty;
                gridInfos[grid.Item1, grid.Item2].gemObject = null;
            }           
        }
    }

    public void OnMoveGem()
    {
        moveCount = 0;

        foreach (var moveGridInfo in moveGridTable)
        {
            var gemDestinationPosition = moveGridInfo.Value.startGemMovePosition;
            var moveTargetPosition = (gemDestinationPosition.Item1 + moveGridInfo.Value.matchCount, gemDestinationPosition.Item2);

            bool isEnd = false;
            while (!isEnd)
            {
                GemObject gemObject;

                if (moveTargetPosition.Item1 >= rowCount)
                {
                    var createObject = CreateRandomGem();
                    gemObject = createObject;

                    var createPosition = gridInfos[gemDestinationPosition.Item1, gemDestinationPosition.Item2].position;
                    createPosition.y = leftBottomPosition.y + (scale.y * moveTargetPosition.Item1);
                    createObject.transform.position = createPosition;// GetGridPosition(moveTargetPosition);
                }
                else
                {
                    gemObject = gridInfos[moveTargetPosition.Item1, moveTargetPosition.Item2].gemObject;
                }

                gridInfos[gemDestinationPosition.Item1, gemDestinationPosition.Item2].gemObject = gemObject;
                gridInfos[gemDestinationPosition.Item1, gemDestinationPosition.Item2].gemType = gemObject.GemType;

                gemObject.OnMoveGem(gridInfos[gemDestinationPosition.Item1, gemDestinationPosition.Item2].position, gemObjectData.MoveGemTime, OnEndMoveGem);
                ++moveCount;

                ++gemDestinationPosition.Item1;
                ++moveTargetPosition.Item1;

                if (gemDestinationPosition.Item1 >= rowCount)
                {
                    isEnd = true;
                }
            }
        }

        moveGridTable.Clear();
    }

    public bool GetGrid(Vector2 findPosition, out (int, int) seleteGrid)
    {
        var position = findPosition - leftBottomPosition + new Vector2(0.5f, 0.5f);
        seleteGrid.Item1 = (int)position.y;
        seleteGrid.Item2 = (int)position.x;

        Debug.Log($"y : {position.y}, x : {position.x}");
        Debug.Log($"x : {seleteGrid.Item2}, y : {seleteGrid.Item1}");

        if (position.x < 0f || position.y < 0f
            || seleteGrid.Item1 >= rowCount || seleteGrid.Item2 >= columnCount)
        {
            seleteGrid = (-1, -1);
            return false;
        }

        return true;
    }

    public Vector2 GetGridPosition((int, int) grid)
    {
        return leftBottomPosition + scale + new Vector2(grid.Item1, grid.Item2);
    }

    public void OnSelelteGemMove((int, int) lhs, (int, int) rhs)
    {
        gridInfos[lhs.Item1, lhs.Item2].gemObject.OnMoveGem(gridInfos[rhs.Item1, rhs.Item2].position, gemObjectData.MoveGemTime, OnEndMoveGem);
        gridInfos[rhs.Item1, rhs.Item2].gemObject.OnMoveGem(gridInfos[lhs.Item1, lhs.Item2].position, gemObjectData.MoveGemTime, OnEndMoveGem);

        (gridInfos[lhs.Item1, lhs.Item2].gemObject, gridInfos[rhs.Item1, rhs.Item2].gemObject) = (gridInfos[rhs.Item1, rhs.Item2].gemObject, gridInfos[lhs.Item1, lhs.Item2].gemObject);
        moveCount = 2;
    }

    public void OnEndMoveGem()
    {
        --moveCount;

        if(moveCount == 0)
        {
            onEndMoveAction?.Invoke();
        }
    }

}
