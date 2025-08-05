using UnityEngine;

public class GemObject : MonoBehaviour, IGem
{
    [SerializeField]
    private GemType gemType;
    public GemType GemType => gemType;
}
