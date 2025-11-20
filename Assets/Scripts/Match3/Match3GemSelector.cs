using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class Match3GemSelector : MonoBehaviour
{
    [SerializeField]
    private Match3GameBoard gameBoard;

    [SerializeField]
    private Match3GameController controller;

    private bool isSelete = false;
    private Point targetGrid;

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
                Point seleteGrid;

                if (gameBoard.GetGrid(findPosition, out seleteGrid))
                {
                    int range = math.abs(targetGrid.y - seleteGrid.y) + math.abs(targetGrid.x - seleteGrid.x);
                    if (range == 1)
                    {
                        controller.OnSeleteGemMove(targetGrid, seleteGrid);
                    }

                    isSelete = false;
                }
            }

        }        
    }


}
