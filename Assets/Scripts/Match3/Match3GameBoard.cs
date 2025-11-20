using System.Collections.Generic;
using UnityEngine;

public struct GridInfo
{
    public GemObject gemObject;
    public GemType gemType;
    public Vector2 position;

    public Point point;
}

public struct MoveInfo
{
    public Point startGemMovePosition;
    public int matchCount;

    public MoveInfo(Point startGemMovePosition, int matchCount)
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

    private List<Point> findGridList = new List<Point>();
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

    [SerializeField]
    private bool isDebug = true;

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
                gridInfos[j, i].point = new Point(j, i);

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

    public bool IsSeleteMoveable(Point lhs, Point rhs, ref List<Point> matchGridList)
    {
        OnChangeGridInfo(lhs, rhs);
        bool leftMoveable = IsCheckMatched(lhs, ref matchGridList);
        bool rightMoveable = IsCheckMatched(rhs, ref matchGridList);
        OnChangeGridInfo(lhs, rhs);
        return leftMoveable || rightMoveable;
    }

    public bool IsCheckMatched(Point coordinate, ref List<Point> matchGridList)
    {
        bool isMatchedColumn = false;
        bool isMatchedRow = false;

        isMatchedColumn = IsCheckColumn(gridInfos[coordinate.y, coordinate.x].gemType, coordinate, ref matchGridList);
        isMatchedRow = IsCheckRow(gridInfos[coordinate.y, coordinate.x].gemType, coordinate, ref matchGridList);

        return isMatchedColumn || isMatchedRow;
    }

    public bool IsCheckMatched(GemType gemType, Point coordinate, ref List<Point> matchGridList)
    {
        bool isMatchedColumn = false;
        bool isMatchedRow = false;

        isMatchedColumn = IsCheckColumn(gemType, coordinate, ref matchGridList);
        isMatchedRow = IsCheckRow(gemType, coordinate, ref matchGridList);

        return isMatchedColumn || isMatchedRow;
    }

    public bool IsCheckMatched(Point findGrid)
    {
        bool isMatchedColumn = false;
        bool isMatchedRow = false;



        return isMatchedColumn || isMatchedRow;
    }

    public bool IsCheckMatch(Point findGrid, GemType gemType, FindDirection findDirection)
    {
        int index = (int)findDirection;

        for (int i = 0; i < 2; ++i)
        {
            findGrid += FindDirections[index];

            if (gridInfos[findGrid.y, findGrid.x].gemObject == null
                || gridInfos[findGrid.y, findGrid.x].gemType != gemType)
            {
                return false;
            }
        }

        return true;
    }

    public bool IsCheckColumn(GemType gemType, Point coordinate, ref List<Point> matchList)
    {
        findGridList.Clear();
        findGridList.Add(coordinate);

        for (int i = 1; i < 3; ++i)
        {
            int findCol = coordinate.x + i;
            if (columnCount == findCol
                || gemType != gridInfos[coordinate.y, findCol].gemType)
            {
                break;
            }

            findGridList.Add(new (findCol, coordinate.y));
        }

        for (int i = 1; i < 3; ++i)
        {
            int findCol = coordinate.x - i;
            if (0 > findCol
                || gemType != gridInfos[coordinate.y, findCol].gemType)
            {
                break;
            }

            findGridList.Add(new(findCol, coordinate.y));
        }

        if (findGridList.Count >= 3)
        {
            foreach (var grid in findGridList)
            {
                matchList.Add(grid);

                if(!moveGridTable.TryGetValue(grid.x, out var moveInfo))
                {
                    moveGridTable.Add(grid.x, new MoveInfo(grid, 1));
                }
                else
                {
                    moveInfo.startGemMovePosition = grid.y < moveInfo.startGemMovePosition.y ? grid : moveInfo.startGemMovePosition;
                    moveGridTable[grid.x] = moveInfo;
                }
            }

            findGridList.Clear();
            return true;
        }

        findGridList.Clear();
        return false;
    }

    public bool IsCheckRow(GemType gemType, Point coordinate, ref List<Point> matchList)
    {
        findGridList.Clear();
        findGridList.Add(coordinate);

        for (int i = 1; i < 3; ++i)
        {
            int findrow = coordinate.y + i;
            if (rowCount == findrow
                || gemType != gridInfos[findrow, coordinate.x].gemType)
            {
                break;
            }

            findGridList.Add(new (coordinate.x, findrow));
        }

        for (int i = 1; i < 3; ++i)
        {
            int findrow = coordinate.y - i;
            if (0 > findrow
                || gemType != gridInfos[findrow, coordinate.x].gemType)
            {
                break;
            }

            findGridList.Add(new (coordinate.x, findrow));
        }

        if (findGridList.Count >= 3)
        {
            foreach (var grid in findGridList)
            {
                matchList.Add(grid);

                if (!moveGridTable.TryGetValue(grid.x, out var moveInfo))
                {
                    moveGridTable.Add(grid.x, new MoveInfo(grid, findGridList.Count));
                }
                else
                {
                    moveInfo.startGemMovePosition = grid.y < moveInfo.startGemMovePosition.y ? grid : moveInfo.startGemMovePosition;
                    moveInfo.matchCount = findGridList.Count;
                    moveGridTable[grid.x] = moveInfo;
                }
            }

            findGridList.Clear();

            return true;
        }

        findGridList.Clear();
        return false;
    }

    private void OnChangeGridInfo(Point lhs ,Point rhs)
    {
        (gridInfos[lhs.y, lhs.x].gemType, gridInfos[rhs.y, rhs.x].gemType) =  (gridInfos[rhs.y, rhs.x].gemType, gridInfos[lhs.y, lhs.x].gemType);
        (gridInfos[lhs.y, lhs.x].gemObject, gridInfos[rhs.y, rhs.x].gemObject) = (gridInfos[rhs.y, rhs.x].gemObject, gridInfos[lhs.y, lhs.x].gemObject);
    }

    public void DestroyGem(ref List<Point> matchGridList)
    {
        foreach(var grid in matchGridList)
        {
            // 임시로 비활성화 하기
            if (gridInfos[grid.y, grid.x].gemType != GemType.Empty)
            {
                gridInfos[grid.y, grid.x].gemObject.gameObject.SetActive(false);

                gridInfos[grid.y, grid.x].gemType = GemType.Empty;
                gridInfos[grid.y, grid.x].gemObject = null;
            }           
        }
    }

    public void OnMoveGem()
    {
        moveCount = 0;

        foreach (var moveGridInfo in moveGridTable)
        {
            var gemDestinationPosition = moveGridInfo.Value.startGemMovePosition;
            var moveTargetPosition = new Point(gemDestinationPosition.x, gemDestinationPosition.y + moveGridInfo.Value.matchCount);

            //bool isEnd = false;
            while (gemDestinationPosition.y < rowCount)
            {
                GemObject gemObject;

                if (moveTargetPosition.y >= rowCount)
                {
                    var createObject = CreateRandomGem();
                    gemObject = createObject;

                    var createPosition = gridInfos[gemDestinationPosition.y, gemDestinationPosition.x].position;
                    createPosition.y = leftBottomPosition.y + (scale.y * moveTargetPosition.y);
                    createObject.transform.position = createPosition;// GetGridPosition(moveTargetPosition);
                }
                else
                {
                    if(gridInfos[moveTargetPosition.y, moveTargetPosition.x].gemObject == null)
                    {
                        ++moveTargetPosition.y;
                        continue;
                    }
                    else
                    {
                        gemObject = gridInfos[moveTargetPosition.y, moveTargetPosition.x].gemObject;
                    }
                }

                if (gemObject == null)
                {
                    Debug.LogError($"Index : {gemDestinationPosition.y} , {gemDestinationPosition.x}");
                }

                if(gemDestinationPosition.y > rowCount
                    || gemDestinationPosition.y < 0
                    || gemDestinationPosition.x > rowCount
                    || gemDestinationPosition.x < 0)
                {
                    Debug.LogError($"Index : {gemDestinationPosition.y} , {gemDestinationPosition.x}");
                }

                gridInfos[gemDestinationPosition.y, gemDestinationPosition.x].gemObject = gemObject;
                gridInfos[gemDestinationPosition.y, gemDestinationPosition.x].gemType = gemObject.GemType;

                gemObject.OnMoveGem(gridInfos[gemDestinationPosition.y, gemDestinationPosition.x].position, gemObjectData.MoveGemTime, OnEndMoveGem);
                ++moveCount;

                ++gemDestinationPosition.y;
                ++moveTargetPosition.y;
            }
        }

        moveGridTable.Clear();
    }

    public bool GetGrid(Vector2 findPosition, out Point seleteGrid)
    {
        var position = findPosition - leftBottomPosition + new Vector2(0.5f, 0.5f);
        seleteGrid.y = (int)position.y;
        seleteGrid.x = (int)position.x;

        Debug.Log($"y : {position.y}, x : {position.x}");
        Debug.Log($"x : {seleteGrid.x}, y : {seleteGrid.y}");

        if (position.x < 0f || position.y < 0f
            || seleteGrid.y >= rowCount || seleteGrid.x >= columnCount)
        {
            seleteGrid = new (-1, -1);
            return false;
        }

        return true;
    }

    public Vector2 GetGridPosition(Point grid)
    {
        return leftBottomPosition + scale + new Vector2(grid.y, grid.x);
    }

    public void OnSelelteGemMove(Point lhs, Point rhs)
    {
        gridInfos[lhs.y, lhs.x].gemObject.OnMoveGem(gridInfos[rhs.y, rhs.x].position, gemObjectData.MoveGemTime, OnEndMoveGem);
        gridInfos[rhs.y, rhs.x].gemObject.OnMoveGem(gridInfos[lhs.y, lhs.x].position, gemObjectData.MoveGemTime, OnEndMoveGem);

        OnChangeGridInfo(lhs, rhs);
        moveCount = 2;
    }

    public void OnEndMoveGem()
    {
        --moveCount;

        if (moveCount == 0)
        {
            onEndMoveAction?.Invoke();
        }
    }

    private void OnDrawGizmos()
    {
        if(isDebug)
        {
            Vector2 pos = new (0.5f, 0f);
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.black;   // 글자 색상 변경
            style.fontSize = 14;                   // 글자 크기
            style.alignment = TextAnchor.MiddleCenter;

            if(gridInfos.Length == 0)
            {
                return;
            }

            foreach (var item in gridInfos)
            {
#if UNITY_EDITOR
                UnityEditor.Handles.Label(item.position , $"{item.gemType.ToString()}\n{item.point.y}, {item.point.x}", style);
#endif
                //Gizmos.DrawIcon(item.position, $"{item.gemType.ToString()}\n{item.point.y}, {item.point.x}", false, Color.black);
            }
        }
    }

}
