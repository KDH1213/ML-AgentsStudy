using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class GameController : MonoBehaviour
{
    [SerializeField]
    private GameObject rectObject;

    private SpriteRenderer rectSpriteRenderer;

    private bool isDrag;
    private Vector2 startPosition;

    private Camera mainCamera;

    [SerializeField]
    private List<Collider2D> targetColliders;

    [SerializeField]
    private ContactFilter2D contactFilter;

    private int currentNumber = 0;

    [SerializeField]
    private AppleGameData appleGameData;

    public UnityEvent onRestartGame;
    public UnityEvent<int> onChangeScore;
    public UnityEvent<float,float> onChangeTimer;
    public UnityEvent onEndGame;


    private int score = 0;
    private float currentTime;

    private bool isPlaying = true;


    private void Awake()
    {
        mainCamera = Camera.main;

        rectSpriteRenderer = rectObject.GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        currentTime = appleGameData.GameTimeLimit;
    }

    private void Update()
    {
        if(isPlaying)
        {
            if (isDrag)
            {
                var currentMousePosition = mainCamera.ScreenToWorldPoint(Mouse.current.position.value);
                var mouseCenterPosition = Vector2.Lerp(currentMousePosition, startPosition, 0.5f);

                rectObject.transform.position = mouseCenterPosition;
                rectObject.transform.localScale = new Vector3(math.abs(currentMousePosition.x - startPosition.x), math.abs(currentMousePosition.y - startPosition.y));

                if (Physics2D.OverlapBox(mouseCenterPosition, rectObject.transform.localScale, 0f, contactFilter, targetColliders) != 0)
                {
                    currentNumber = 0;

                    foreach (var item in targetColliders)
                    {
                        currentNumber += item.GetComponent<AppleTileObject>().Number;
                    }
                }
                else
                {
                    currentNumber = 0;
                }

                if (currentNumber == appleGameData.NumberOfSuccess)
                {
                    rectSpriteRenderer.color = appleGameData.SeleteColor;
                }
                else
                {
                    rectSpriteRenderer.color = appleGameData.DefalutColor;
                }
            }


            currentTime -= Time.deltaTime;
            onChangeTimer?.Invoke(currentTime, appleGameData.GameTimeLimit);


            if (currentTime <= 0f)
            {
                isPlaying = false;
                onEndGame?.Invoke();
            }
        }

       
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        if(!isPlaying)
        {
            return;
        }

        if(context.started)
        {
            isDrag = true;
            rectObject.SetActive(true);
            startPosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.value);
        }

        if (context.canceled)
        {
            rectObject.SetActive(false);
            isDrag = false;
            OnDestoryTarget();
        }
    }
    
    public void OnReStartGame()
    {
        onRestartGame?.Invoke();
        isPlaying = true;

        score = 0;
        currentTime = appleGameData.GameTimeLimit;
        onChangeTimer?.Invoke(currentTime, appleGameData.GameTimeLimit);
    }


    private void OnDestoryTarget()
    {
        var currentMousePosition = mainCamera.ScreenToWorldPoint(Mouse.current.position.value);
        var mouseCenterPosition = Vector2.Lerp(currentMousePosition, startPosition, 0.5f);

        rectObject.transform.position = mouseCenterPosition;
        rectObject.transform.localScale = new Vector3(math.abs(currentMousePosition.x - startPosition.x), math.abs(currentMousePosition.y - startPosition.y));

        int colliderCount = Physics2D.OverlapBox(mouseCenterPosition, rectObject.transform.localScale, 0f, contactFilter, targetColliders);
        if (colliderCount != 0)
        {
            currentNumber = 0;

            foreach (var item in targetColliders)
            {
                currentNumber += item.GetComponent<AppleTileObject>().Number;
            }

            if (currentNumber == appleGameData.NumberOfSuccess)
            {
                foreach (var item in targetColliders)
                {
                    item.gameObject.SetActive(false);
                }
                score += colliderCount;
                onChangeScore?.Invoke(score);
            }
        }
    }
}
