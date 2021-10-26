using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCastUtilities
{
    public static float RayCast(int nRays, Vector3 position,
        Vector2 direction, Vector3 offsetStart, Vector3 offsetEnd,
        List<int> collisionLayers)
    {
        float shortestDistance = 1f;
        foreach (int layer in collisionLayers)
        {
            for (int index = 0; index < nRays; index++)
            {
                float rayPosition = (float) index / (float) (nRays - 1);
                RaycastHit2D hit = Physics2D.Raycast(position +
                    offsetStart * (1 - rayPosition) + offsetEnd * rayPosition,
                    direction, 1f, layer);
                if (hit.collider != null)
                {
                    if (hit.distance < shortestDistance)
                    {
                        shortestDistance = hit.distance;
                    }
                }
            }
        }
        
        return shortestDistance;
    }

    public static float RayCastAngle(Vector3 position, 
        Vector3 direction, float gap, List<int> collisionLayers)
    {
        Vector3 positionLeft = position + Vector3.Normalize(Quaternion.Euler(0f, 0f, 90f) * direction) * gap;
        Vector3 positionRight = position + Vector3.Normalize(Quaternion.Euler(0f, 0f, -90f) * direction) * gap;
        float distanceLeft = 1.5f;
        float distanceRight = 1.5f;
        float distanceMiddle = 1.5f;
        foreach (int layer in collisionLayers)
        {
            RaycastHit2D hitLeft = Physics2D.Raycast(positionLeft,
                direction, 1f, layer);
            if (hitLeft.collider != null)
            {
                distanceLeft = hitLeft.distance;
            }

            RaycastHit2D hitRight = Physics2D.Raycast(positionRight,
                direction, 1f, layer);
            if (hitRight.collider != null)
            {
                distanceRight = hitRight.distance;
            }

            RaycastHit2D hitMiddle = Physics2D.Raycast(position,
                direction, 1f, layer);
            if (hitMiddle.collider != null)
            {
                distanceMiddle = hitMiddle.distance;
            }

            if (Mathf.Abs(distanceLeft - distanceRight) != 0) break;
        }
        if (Mathf.Abs(distanceMiddle - Mathf.Min(distanceLeft, distanceRight)) <= 0.01f)
            return 0f;
        float result = Mathf.Atan((distanceLeft - distanceRight) /
            (gap * 2)) * Mathf.Rad2Deg;
        return result;
    }
}
