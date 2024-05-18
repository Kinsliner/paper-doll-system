using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathExtension
{
    //return the near float of grid
    public static float RoundToGrid(this float value, float grid)
    {
        return Mathf.Round(value / grid) * grid;
    }

    //convet point on grid
    public static Vector3 RoundToGrid(this Vector3 value, float grid)
    {
        return new Vector3(value.x.RoundToGrid(grid), value.y.RoundToGrid(grid), value.z.RoundToGrid(grid));
    }

    public static bool IsZero(this float value)
    {
        return Mathf.Approximately(value, 0.0f) ||
               Mathf.Approximately(value, -1.1920928955078126e-7f)||
               Mathf.Approximately(value, 1.1920928955078126e-7f);
    }

    public static bool IsInfinity(this float value)
    {
        return float.IsInfinity(value);
    }

    public static bool IsZeroOrInfinity(this float value)
    {
        return value.IsZero() || value.IsInfinity();
    }

    public static bool Between(this float value, float max, float min)
    {
        return value <= max && value >= min;
    }

    public static bool Between(this Vector3 position, Vector3 max, Vector3 min)
    {
        if (position.x.Between(max.x, min.x) &&
            position.y.Between(max.y, min.y) &&
            position.z.Between(max.z, min.z))
        {
            return true;
        }
        return false;
    }

    public static float MaxComponent(this Vector3 position)
    {
        return Mathf.Max(position.x, position.y, position.z);
    }
}
