using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Direction {

    public const int Up = 0;
    public const int Right = 1;
    public const int Down = 2;
    public const int Left = 3;
    const int NumberFacings = 4;

    public static int GetOppositeDirection(int direction)
    {
        return (direction + 2) % NumberFacings;
    }

    public static int TurnLeft(int direction)
    {
        direction--;

        if (direction < 0)
        {
            direction = NumberFacings - 1;
        }

        return direction;
    }

    public static int TurnRight(int direction)
    {
        return (direction + 1) % NumberFacings;
    }

    public static Vector2Int GetVector(int direction)
    {
        switch(direction)
        {
            case Right:
                return Vector2Int.right;
            case Left:
                return Vector2Int.left;
            case Up:
                return Vector2Int.down;
            case Down:
                return Vector2Int.up;
            default:
                throw new System.ArgumentException("Direction not in valid range");
        }
    }
}
