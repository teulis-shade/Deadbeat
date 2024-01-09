using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveRoutine : Routine
{
    private enum move
    {
        UP,
        DOWN,
        LEFT,
        RIGHT,
        WAIT
    }

    private int currentPosition = 0;

    [SerializeField] List<move> moves;
    // Start is called before the first frame update
    void Awake()
    {
        int verticalMove = 0;
        int horizontalMove = 0;
        foreach (move m in moves)
        {
            if (move.UP == m)
            {
                verticalMove++;
            }
            if (move.DOWN == m)
            {
                verticalMove--;
            }
            if (move.RIGHT == m)
            {
                horizontalMove++;
            }
            if (move.LEFT == m)
            {
                horizontalMove--;
            }
        }
        if (horizontalMove != 0 || verticalMove != 0)
        {
            Debug.LogError("HEY, your movement for " + gameObject.name + " does not return to the starting point. Fix it, or the character won't move");
            moves.Clear();
        }
    }

    public override void PerformRoutine()
    {
        switch (moves[currentPosition++])
        {
            case move.UP:
                GetComponent<Entity>().Move(0, 1);
                break;
            case move.RIGHT:
                GetComponent<Entity>().Move(1, 0);
                break;
            case move.LEFT:
                GetComponent<Entity>().Move(-1, 0);
                break;
            case move.DOWN:
                GetComponent<Entity>().Move(0, -1);
                break;
            default:
                break;
        }
        if (currentPosition >= moves.Count)
        {
            currentPosition = 0;
        }
    }
}
