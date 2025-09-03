using UnityEngine;

public class GemObject : MonoBehaviour, IGem
{
    [SerializeField]
    private GemType gemType;
    public GemType GemType => gemType;

    private Vector2 startPosition;
    private Vector2 movePosition;

    private float startMoveTime = 0f;
    private float moveTime = 0f;

    private System.Action endMoveAction;

    public void Initialized()
    {
        
    }

    private void Awake()
    {
        enabled = false;
    }

    private void Update()
    {
        float ratio = (Time.time - startMoveTime) / moveTime;
        transform.position = Vector2.Lerp(startPosition, movePosition, ratio);

        if (ratio >= 1f)
        {
            enabled = false;
            endMoveAction?.Invoke();
        }
    }

    public void SetGemType(GemType gemType, Color color)
    {
        this.gemType = gemType;

        GetComponent<SpriteRenderer>().color = color;
        // GetComponent<Renderer>().material.color = color;
    }

    public void OnMoveGem(Vector2 position, float moveTime, System.Action endMoveAction)
    {
        startPosition = transform.position;
        movePosition = position;

        this.startMoveTime = Time.time;
        this.moveTime = moveTime;
        enabled = true;

        this.endMoveAction = endMoveAction;
    }
}
