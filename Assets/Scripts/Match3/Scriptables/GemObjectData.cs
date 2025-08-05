using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GemObjectData", menuName = "Match3/GemObjectData", order = 0)]
public class GemObjectData : ScriptableObject
{
    [field: SerializeField]
    public List<GemObject> gemObjectPrefabList;

    public GemObject GetRamdomGem()
    {
        return gemObjectPrefabList[Random.Range(0, gemObjectPrefabList.Count)];
    }
}
