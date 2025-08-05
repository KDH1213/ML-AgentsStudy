using System;
using System.Collections.Generic;
using UnityEngine;

public struct GridInfo
{
    public GemObject gemObject;
    public GemType gemType;
    public Vector2 position;
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

    private Vector2 leftBottomPosition;

    private int rowCount;
    private int columnCount;

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

        CreateGem();
    }

    private void CreateGem()
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
                var randomGem = gemObjectData.GetRamdomGem();
                var createObject = Instantiate(randomGem);
                createObject.transform.position = createPosition;

                gridInfos[j, i].gemObject = createObject;
                gridInfos[j, i].gemType = createObject.GemType;
                gridInfos[j, i].position = createPosition;

                createPosition += Vector2.right;
            }
        }
    }

    public bool IsMoveable((int, int) lhs, (int, int) rhs, ref List<(int, int)> matchGridList)
    {
        bool leftMoveable = IsCheckMatched(gridInfos[rhs.Item1, rhs.Item2].gemType, lhs.Item1, lhs.Item2, ref matchGridList);
        bool rightMoveable = IsCheckMatched(gridInfos[lhs.Item1, lhs.Item2].gemType, lhs.Item1, lhs.Item2, ref matchGridList);

        return leftMoveable || rightMoveable;
    }

    public bool IsCheckMatched(GemType gemType, int col, int row, ref List<(int, int)> matchGridList)
    {
        bool isMatchedColumn = false;
        bool isMatchedRow = false;

        isMatchedColumn = IsCheckColumn(gemType, col, row, ref matchGridList);
        isMatchedRow = IsCheckRow(gemType, col, row, ref matchGridList);

        return isMatchedColumn || isMatchedRow;
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
            }

            findGridList.Clear();

            return true;
        }

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
                break;
            }

            findGridList.Add((findrow, col));
        }

        if (findGridList.Count >= 3)
        {
            foreach (var grid in findGridList)
            {
                matchList.Add(grid);
            }

            findGridList.Clear();

            return true;
        }

        return false;
    }

    public void DestroyGem(ref List<(int, int)> matchGridList)
    {
        foreach(var grid in findGridList)
        {
            // 임시로 비활성화 하기
            gridInfos[grid.Item1, grid.Item2].gemObject.gameObject.SetActive(false);

            gridInfos[grid.Item1, grid.Item2].gemType = GemType.Empty;
            gridInfos[grid.Item1, grid.Item2].gemObject = null;
        }        
    }

    public void CreateGem(ref List<(int, int)> matchGridList)
    {

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
}
