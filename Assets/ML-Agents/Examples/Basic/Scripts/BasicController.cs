using System;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using UnityEngine;

public class BasicController : Agent
{
    [SerializeField]
    private float speed = 10f;

    [SerializeField]
    private new Rigidbody rigidbody;

    //private void FixedUpdate()
    //{
    //    if(Input.GetKey(KeyCode.A))
    //    {
    //        rigidbody.AddForce(speed * Vector3.left);
    //    }

    //    if (Input.GetKey(KeyCode.A))
    //    {
    //        rigidbody.AddForce(speed * Vector3.right);
    //    }
    //}

    public override void OnActionReceived(ActionBuffers actions)
    {
        int Discrete = actions.DiscreteActions[0];

        Debug.Log($"index : {Discrete.ToString()}");

        int Discrete1 = actions.DiscreteActions[1];

        Debug.Log($"Discrete1 : {Discrete1.ToString()}");

        int Discrete2 = actions.DiscreteActions[2];

        Debug.Log($"Discrete2 : {Discrete2.ToString()}");

        switch (Discrete)
        {
            case 0:
                {
                    rigidbody.AddForce(speed * Vector3.left);
                    break;
                }
              
            case 1:
                {
                    rigidbody.AddForce(speed * Vector3.right);
                    break;
                }
            default:
                break;
        }
    }
}
