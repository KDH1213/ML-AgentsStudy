using System.Collections.Generic;
using UnityEngine;

public class Match3GameController : MonoBehaviour
{
    private List<(int, int)> matchGridList = new List<(int, int)>();
    public Match3StateType MatchStateType { private set; get; }

    [SerializeField]
    private Match3GameBoard gameBoard;

    public void SeleteGemMove((int, int) lhs, (int,int)rhs)
    {
        MatchStateType = Match3StateType.FindMatches;

        matchGridList.Clear();
        if(gameBoard.IsMoveable(lhs, rhs, ref matchGridList))
        {
            MatchStateType = Match3StateType.MoveFindMatches;
        }
        else
        {
            MatchStateType = Match3StateType.FindMatches;
        }
    }

    public void OnMoveMatches()
    {
        MatchStateType = Match3StateType.MoveMatches;
        gameBoard.DestroyGem(ref matchGridList);
    }

}
