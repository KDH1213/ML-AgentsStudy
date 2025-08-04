using TMPro;
using UnityEngine;

public class AppleTileObject : MonoBehaviour
{
    [SerializeField]
    private TextMeshPro nunberText;

    private Tile tile;
    
    public void SetTileInfo(Tile tile)
    {
        this.tile = tile;
        nunberText.text = tile.number.ToString();
    }

    public int Number => tile.number;
}
