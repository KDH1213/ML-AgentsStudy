using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.SocialPlatforms.Impl;

public class AppleAgent : Agent
{
    [SerializeField]
    private GameMap gameMap;
    public float moveSpeed = 3f;

    private int sizeX = 0;
    private int sizeY = 0;

    private int score = 0;

    [SerializeField]
    private GameController controller;

    [SerializeField]
    private Transform debugRect;

    [SerializeField]
    private bool isDebug;

    private Vector2 scale;
    int objectTotalCount = 0;

    private void Start()
    {
        controller.onEndGame.AddListener(EndEpisode);
    }

    public override void OnEpisodeBegin()
    {
        // ��� ��ġ/�� �ʱ�ȭ
        // selectedIndices.Clear();
        // ���� ���ġ �Ǵ� �ó����� ��ġ
        controller.OnReStartGame();

        sizeX = gameMap.CreateObjectCount.x;
        sizeY = gameMap.CreateObjectCount.y;
        score = 0;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        var appleList = gameMap.CreateObjects;

        sizeX = gameMap.CreateObjectCount.x;
        sizeY = gameMap.CreateObjectCount.y;

        foreach (var apple in appleList)
        {
            sensor.AddObservation(apple.transform.localPosition.x);
            sensor.AddObservation(apple.transform.localPosition.y);
            
            sensor.AddObservation(apple.Number); // ����ȭ
        }

        scale = appleList[0].transform.localScale;
        objectTotalCount = appleList.Count;
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        int index = actions.DiscreteActions[0];
        int direction = actions.DiscreteActions[1];

        int findRangeX = actions.DiscreteActions[2];
        int findRangeY = actions.DiscreteActions[3];

        int startPositionX = index % sizeX;
        int startPositionY = index / sizeY;

        int endPositionX = startPositionX + findRangeX;
        int endPositionY = startPositionY + findRangeY;

        if (!gameMap.CreateObjects[index].gameObject.activeSelf)
        {
            return;
        }

        Debug.Log($"index : {index}");
        Debug.Log($"findRangeX : {findRangeX}");
        Debug.Log($"findRangeY : {findRangeY}");
        Debug.Log($"startPositionX : {startPositionX}");
        Debug.Log($"startPositionY : {startPositionY}");

        if (endPositionY * sizeX + endPositionX > objectTotalCount)
        {
            SetReward(-1.0f);
            return;
        }

        int number = 0;

        for (int y = startPositionY; y < findRangeY; ++y)
        {
            for (int x = startPositionX; x < findRangeX; ++x)
            {
                int findIndex = y * sizeX + x;
                if (!gameMap.CreateObjects[findIndex].gameObject.activeSelf)
                {
                    continue;
                }

                number += gameMap.CreateObjects[findIndex].Number;
            }
        }

        Debug.Log($"number : {number}");

        if (number == 10)
        {
            float count = 0f;
            for (int y = startPositionY; y < findRangeY; ++y)
            {
                for (int x = startPositionX; x < findRangeX; ++x)
                {
                    int findIndex = y * sizeX + x;
                    if (!gameMap.CreateObjects[findIndex].gameObject.activeSelf)
                    {
                        continue;
                    }

                    gameMap.CreateObjects[findIndex].gameObject.SetActive(false);
                    count += 1f;
                }
            }

            SetReward(count);
            score += (int)count;
            controller.onChangeScore?.Invoke(score);
        }
        else
        {
            SetReward(-number);
        }

#if UNITY_EDITOR
        if(isDebug)
        {
            debugRect.gameObject.SetActive(true);
            int findRangehalfX = (findRangeX / 2);
            int findRangehalfY = (findRangeY / 2);

            int debugPositionX = startPositionX + findRangehalfX;
            int debugPositionY = startPositionY + findRangehalfY;

            var debugIndex = debugPositionY * sizeX + debugPositionX;
            debugRect.localPosition = gameMap.CreateObjects[debugIndex].transform.position;
            debugRect.localScale = new Vector3(scale.x * (float)findRangehalfX, scale.y * (float)findRangehalfY, 1f);
        }
        else
        {
            debugRect.gameObject.SetActive(false);
        }
#endif
    }

    private void EvaluateSelection()
    {
        //int sum = 0;
        
        //foreach (var index in selectedIndices)
        //{
        //    sum +=  gameMap.CreateObjects[index].Number;
        //}

        //if (sum == 10)
        //    SetReward(1.0f);
        //else
        //    SetReward(-1.0f);

        //Debug.Log("Test");

        //c();
    }
}
