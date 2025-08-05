using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Match3BoadData", menuName = "Match3/Match3BoadData", order = 0)]
public class Match3BoadData : ScriptableObject
{
    [field: SerializeField]
    public List<GemObject> GemList { private set; get; }

    [field: SerializeField]
    public int Width { private set; get; }
    [field: SerializeField]
    public int Height { private set; get; }

}
