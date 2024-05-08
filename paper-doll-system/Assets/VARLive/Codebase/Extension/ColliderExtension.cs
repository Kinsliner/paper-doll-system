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
    /// 取物件最大的碰撞器
    /// </summary>
    /// <param name="Children"></param>
    /// <returns></returns>
    public static Collider GetMaxSizeCollider(this Collider[] Children)
    {
        // 找不到碰撞器
        if (Children.Length == 0)
            return null;

        // 如果只有一個碰撞器,就直接回傳
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
