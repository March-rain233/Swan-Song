using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public enum Direction
{
    Left,
    Right,
    Up,
    Down,
}
public static class DirectionExtensions
{
    public static Direction ToDirection(this Vector2Int vect)
    {
        var angle = Mathf.Atan2(vect.y, vect.x);
        if(angle >= 0 && angle < Mathf.PI / 4 || angle >= Mathf.PI * 7 / 4 && angle <= Mathf.PI * 2)
        {
            return Direction.Right;
        }
        else if(angle >= Mathf.PI / 4 && angle < Mathf.PI * 3 / 4)
        {
            return Direction.Up;
        }
        else if(angle >= Mathf .PI * 3 / 4 && angle < Mathf.PI * 5 / 4)
        {
            return Direction.Left;
        }
        else
        {
            return Direction.Down;
        }
    }

    public static Vector2Int ToVector2Int(this Direction direction)
    {
        switch (direction)
        {
            case Direction.Left:
                return Vector2Int.left;
            case Direction.Right:
                return Vector2Int.right;
            case Direction.Up:
                return Vector2Int.up;
            case Direction.Down:
                return Vector2Int.down;
            default:
                return Vector2Int.zero;
        }
    }
}