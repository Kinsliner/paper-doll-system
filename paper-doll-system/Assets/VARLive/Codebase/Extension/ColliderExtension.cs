using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ColliderExtension
{
    public static bool ContainsPoint(this Collider collider, Vector3 point, float tolerance)
    {
        Vector3 closestPoint = collider.ClosestPoint(point);
        float distance = Vector3.Distance(point, closestPoint);
        if (distance <= tolerance)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// ������̤j���I����
    /// </summary>
    /// <param name="Children"></param>
    /// <returns></returns>
    public static Collider GetMaxSizeCollider(this Collider[] Children)
    {
        // �䤣��I����
        if (Children.Length == 0)
            return null;

        // �p�G�u���@�ӸI����,�N�����^��
        if (Children.Length == 1)
            return Children[0];

        float maxSize = 0;

        Collider maxCollider = null;

        foreach (var collider in Children)
        {
            if (collider is BoxCollider)
            {
                BoxCollider boxCollider = collider as BoxCollider;
                if (boxCollider.size.x > maxSize)
                {
                    maxSize = boxCollider.size.x;
                    maxCollider = collider;
                }
            }
            if (collider is SphereCollider)
            {
                SphereCollider sphereCollider = collider as SphereCollider;
                if (sphereCollider.radius > maxSize)
                {
                    maxSize = sphereCollider.radius;
                    maxCollider = collider;
                }
            }
        }

        return maxCollider;
    }
}
