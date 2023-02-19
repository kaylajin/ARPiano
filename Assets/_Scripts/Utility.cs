using System.Linq;
using UnityEngine;

public class Utility
{
    public static bool TryGetInputPosition(out Vector2[] positions)
    {
        if (Input.touchCount == 0)
        {
            positions = new Vector2[0];
            return false;
        }

        positions = Input.touches
                         .Where(touch => touch.phase == TouchPhase.Began)
                         .Select(touch => touch.position)
                         .ToArray();

        return true;
    }
}
