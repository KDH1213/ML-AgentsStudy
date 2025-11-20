using System.Collections.Generic;
using UnityEngine;

public class Match3GameController : MonoBehaviour
{
    private List<Point> matchGridList = new List<Point>();
    public Match3StateType MatchStateType { private set; get; }

    [SerializeField]
    private Match3GameBoard gameBoard;

    private void Awake()
    {
    }

    public void OnSeleteGemMove(Point lhs, Point rhs)
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

        gameBoard.onEndMoveAction += OnEndMoveMatches;
        gameBoard.OnMoveGem();
        MatchStateType = Match3StateType.FindMatches;
    }

    public void OnEndMoveMatches()
    {
        gameBoard.onEndMoveAction -= OnEndMoveMatches;
        MatchStateType = Match3StateType.FindMatches;
    }
}
