using UnityEngine;
using Unity.MLAgents;

public class TestController : Agent
{
    [SerializeField]    
    private float speed = 10f;
    private new Rigidbody rigidbody;

    protected override void Awake()
    {
        rigidbody = GetComponent<Rigidbody>(); 
    }
}
