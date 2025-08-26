using UnityEngine;

public class GemObject : MonoBehaviour, IGem
{
    [SerializeField]
    private GemType gemType;
    public GemType GemType => gemType;

    public void Initialized()
    {
        
    }

    public void SetGemType(GemType gemType, Color color)
    {
        this.gemType = gemType;

        GetComponent<SpriteRenderer>().color = color;
        // GetComponent<Renderer>().material.color = color;
    }
}
