using UnityEngine;

public class GameMap : MonoBehaviour
{
    [SerializeField]
    private GameObject applePrefab;

    [SerializeField]
    private Vector2 mapLenght;

    private void Awake()
    {
        CreateMap();
    }

    private void CreateMap()
    {

    }
}
