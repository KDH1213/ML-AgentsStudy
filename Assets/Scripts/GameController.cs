using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
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
    private Color seleteColor;

    [SerializeField]
    private Color defalutColor;

    private void Awake()
    {
        mainCamera = Camera.main;

        rectSpriteRenderer = rectObject.GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if(isDrag)
        {
            var currentMousePosition = mainCamera.ScreenToWorldPoint(Mouse.current.position.value);
            var mouseCenterPosition = Vector2.Lerp(currentMousePosition, startPosition, 0.5f);

            rectObject.transform.position = mouseCenterPosition;
            rectObject.transform.localScale = new Vector3(math.abs(currentMousePosition.x - startPosition.x), math.abs(currentMousePosition.y - startPosition.y));

            if(Physics2D.OverlapBox(mouseCenterPosition, rectObject.transform.localScale, 0f, contactFilter, targetColliders) != 0)
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

            if(currentNumber == 10)
            {
                rectSpriteRenderer.color = seleteColor;
            }
            else
            {
                rectSpriteRenderer.color = defalutColor;
            }
        }
    }

    public void OnClick(InputAction.CallbackContext context)
    {
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

    private void OnDestoryTarget()
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

            if (currentNumber == 10)
            {
                foreach (var item in targetColliders)
                {
                    item.gameObject.SetActive(false);
                }
            }
        }
    }
}
