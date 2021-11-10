using System;
using UnityEngine;

public static class TransformExtensions
{
    public static Vector3 GetCastDirection(this Transform t, CastDirection dir)
    {
        switch (dir)
        {
            case CastDirection.Up:
                return t.up;
            case CastDirection.Down:
                return -t.up;
            case CastDirection.Left:
                return -t.right;
            case CastDirection.Right:
                return t.right;
            case CastDirection.Forward:
                return t.forward;
            case CastDirection.Backward:
                return -t.forward;
            default:
                throw new NotImplementedException();
        }
    }
}
