using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "GemObjectData", menuName = "Match3/GemObjectData", order = 0)]
public class GemObjectData : ScriptableObject
{
    [field: SerializeField]
    public List<GemObject> gemObjectPrefabList;

    [field: SerializeField]
    public GemObject GemObjectFrefab;

    [field: SerializeField]
    public SerializedDictionary<GemType, Color> gemColorTable;

    [field: SerializeField]
    public float MoveGemTime;

    public GemType GetRandomGemType()
    {
        return (GemType)Random.Range((int)GemType.Empty + 1, (int)GemType.End - 1);
    }

    public GemObject GetRamdomGem()
    {
        return gemObjectPrefabList[Random.Range(0, gemObjectPrefabList.Count)];
    }

    public Color GetColor(GemType gemType)
    {
        return gemColorTable[gemType];
    }

    [ContextMenu("InitializedGemType")]
    public void InitializedGemType()
    {
        gemColorTable.Clear();

        for (int i = (int)GemType.Empty + 1; i < (int)GemType.End; ++i)
        {
            gemColorTable.Add((GemType)i, Color.black);
        }
    }
}
