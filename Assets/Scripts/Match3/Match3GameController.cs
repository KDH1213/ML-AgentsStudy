using System.Collections.Generic;
using UnityEngine;

public class Match3GameController : MonoBehaviour
{
    private List<(int, int)> matchGridList = new List<(int, int)>();
    public Match3StateType MatchStateType { private set; get; }

    [SerializeField]
    private Match3GameBoard gameBoard;

    private void Awake()
    {
    }

    public void OnSeleteGemMove((int, int) lhs, (int,int)rhs)
    {
        if(MatchStateType == Match3StateType.FindMatches)
        {
            matchGridList.Clear();
            if (gameBoard.IsSeleteMoveable(lhs, rhs, ref matchGridList))
            {
                MatchStateType = Match3StateType.MoveFindMatches;
                gameBoard.OnSelelteGemMove(lhs, rhs);
                gameBoard.onEndMoveAction += OnMoveMatches;
            }
            else
            {
                MatchStateType = Match3StateType.FindMatches;
            }
        }        
    }

    public void OnMoveMatches()
    {
        MatchStateType = Match3StateType.MoveMatches;
        gameBoard.DestroyGem(ref matchGridList);

        gameBoard.onEndMoveAction -= OnMoveMatches;

        gameBoard.OnMoveGem();
    }

}
