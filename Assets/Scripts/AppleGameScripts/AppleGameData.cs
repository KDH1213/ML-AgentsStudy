using UnityEngine;

[CreateAssetMenu(fileName = "AppleGameData", menuName = "AppleGameData", order = 3)]
public class AppleGameData : ScriptableObject
{
    [field: SerializeField]
    public Color SeleteColor { private set; get; }

    [field: SerializeField]
    public Color DefalutColor { private set; get; }

    [field: SerializeField]
    public int NumberOfSuccess { private set; get; }

    [field: SerializeField]
    public float GameTimeLimit { private set; get; }
}
