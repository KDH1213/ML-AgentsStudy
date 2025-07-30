using System.Collections.Generic;
using Unity.Burst;
using UnityEngine;

public class GameMap : MonoBehaviour
{
    [SerializeField]
    private GameObject applePrefab;

    [SerializeField]
    private Vector2Int createObjectCount;

    // [SerializeField]
    // private float spacing;

    [SerializeField]
    private float padding;

    [SerializeField]
    private GameObject mapObject;

    [SerializeField]
    private List<GameObject> createObjects = new List<GameObject>();

    private Vector2 resolution;
    private void Awake()
    {
        resolution.x = Screen.width;
        resolution.y = Screen.height;

        if(createObjects.Count == 0)
        {
            CreateMap();
        }
        else
        {
            ResetTileInfo();
        }
    }

    [ContextMenu("DestoryMap")]
    private void DestroyTiles()
    {
        if (createObjects.Count != 0)
        {
            for (int i = 0; i < createObjects.Count; ++i)
            {
                DestroyImmediate(createObjects[i]);
            }

            createObjects.Clear();
        }        
    }

    [ContextMenu("CreateMap")]
    private void CreateMap()
    {
        if(createObjects.Count != 0)
        {
            for(int i = 0; i < createObjects.Count; ++i)
            {
                Destroy(createObjects[i]);
            }

            createObjects.Clear();
        }

        var camera = Camera.main;
        var orthographicSize = camera.orthographicSize;

        var height = (orthographicSize - padding) * 2;
        var width = ((orthographicSize - padding) * 2) * (1920f / 1080f);

        mapObject.transform.localScale = new Vector2(width, height);

        var totalSize = new Vector2(createObjectCount.x, createObjectCount.y);
        var createSize = new Vector2(width / totalSize.x, height / totalSize.y);

        createSize = createSize.x < createSize.y ? new Vector2(createSize.x, createSize.x) : new Vector2(createSize.y, createSize.y);

        float startCreatePositionX = -(camera.transform.position.x + (createSize.x * (float)(createObjectCount.x / 2)));
        float startCreatePositionY = camera.transform.position.y + (createSize.y * (float)(createObjectCount.y / 2));

        var creatStartPoint = new Vector2(startCreatePositionX, startCreatePositionY);
        var offsetPosition = new Vector3(createSize.x, -createSize.y) * 0.5f;

        Tile[] tiles = new Tile[createObjectCount.x * createObjectCount.y];

        CreateTileInfos(ref tiles);
        SuffleTiles(tiles);

        for (int j = 0; j < createObjectCount.y; ++j)
        {
            var creatPoint = creatStartPoint + (Vector2.down * j * createSize.y);

            for (int i = 0; i < createObjectCount.x; ++i)
            {
                Vector3 position = new Vector3(creatPoint.x, creatPoint.y) + offsetPosition;
                var createObject = Instantiate(applePrefab, position, Quaternion.identity);
                createObject.transform.localScale = createSize;

                createObjects.Add(createObject);
                creatPoint.x += createSize.x;
                createObject.GetComponent<AppleTileObject>().SetTileInfo(tiles[j * createObjectCount.x + i]);
            }
        }

    }

    [BurstCompile]
    private void CreateTileInfos(ref Tile[] tiles)
    {
        int number = 1;
        int numberCount = (createObjectCount.x * createObjectCount.y) / 9;
        int setCount = 0;

        int lenght = tiles.Length;
        for (int i = 0; i < lenght; ++i)
        {
            tiles[i].number = number;
            ++setCount;

            if (setCount == numberCount)
            {
                ++number;
                setCount = 0;
            }
        }
    }

    [BurstCompile]
    private void SuffleTiles(Tile[] tiles)
    {
        int lenght = tiles.Length;
        for (int i = lenght - 1; i >= 0; --i)
        {
            int rand = Random.Range(0, i + 1);

            Tile tile = tiles[i];
            tiles[i] = tiles[rand];
            tiles[rand] = tile;
        }
    }

    private void ResetTileInfo()
    {
        Tile[] tiles = new Tile[createObjectCount.x * createObjectCount.y];

        CreateTileInfos(ref tiles);
        SuffleTiles(tiles);

        int count = createObjects.Count;

        for (int i = 0; i < count; ++i)
        {
            createObjects[i].SetActive(true);
            createObjects[i].GetComponent<AppleTileObject>().SetTileInfo(tiles[i]);
        }
    }
}
