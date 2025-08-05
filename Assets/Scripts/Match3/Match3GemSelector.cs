using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Match3GemSelector : MonoBehaviour
{
    [SerializeField]
    private Match3GameBoard gameBoard;

    [SerializeField]
    private Match3GameController controller;

    private bool isSelete = false;
    private (int, int) targetGrid;

    public void OnClick(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            if (!isSelete)
            {
                var findPosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.value);

                if (gameBoard.GetGrid(findPosition, out targetGrid))
                {
                    isSelete = true;
                }
            }
            else
            {
                var findPosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.value);
                (int, int) seleteGrid;

                if (gameBoard.GetGrid(findPosition, out seleteGrid))
                {
                    int range = math.abs(targetGrid.Item1 - seleteGrid.Item1) + math.abs(targetGrid.Item2 - seleteGrid.Item2);
                    if (range == 1)
                    {
                        controller.SeleteGemMove(targetGrid, seleteGrid);
                    }

                    isSelete = false;
                }
            }

        }        
    }


}
