using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class AppleAgent : Agent
{
    [SerializeField]
    private GameMap gameMap;
    public GameObject apple;
    public float moveSpeed = 3f;

    public override void OnEpisodeBegin()
    {
        transform.localPosition = gameMap.LeftTopPosition;
        apple.transform.localPosition = new Vector3(Random.Range(-4f, 4f), 0.5f, Random.Range(-4f, 4f));
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(apple.transform.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        Vector3 move = new Vector3(moveX, 0, moveZ);
        transform.localPosition += move * moveSpeed * Time.deltaTime;

        float distance = Vector3.Distance(transform.localPosition, apple.transform.localPosition);
        if (distance < 1.0f)
        {
            SetReward(1.0f);
            EndEpisode();
        }

        if (Mathf.Abs(transform.localPosition.x) > 5 || Mathf.Abs(transform.localPosition.z) > 5)
        {
            SetReward(-1.0f); // ¹üÀ§ ¹Û
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }
}
