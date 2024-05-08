using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TransformExtension
{
    public static void SetAndFitParentPose(this Transform child, Transform parent)
    {
        child.SetParent(parent);
        child.FitParentPose();
    }

    public static void SetAndFitParent(this Transform child, Transform parent)
    {
        child.SetParent(parent);
        child.FitParent();
    }

    public static void SetAndFitParentScale(this Transform child, Transform parent)
    {
        child.SetParent(parent);
        child.FitParentScale();
    }

    public static void FitParent(this Transform child)
    {
        child.localPosition = Vector3.zero;
        child.localRotation = Quaternion.identity;
        child.localScale = Vector3.one;
    }

    public static void FitParentPose(this Transform child)
    {
        child.localPosition = Vector3.zero;
        child.localRotation = Quaternion.identity;
    }

    public static void FitParentScale(this Transform child)
    {
        child.localScale = Vector3.one;
    }

    public static Transform GetChild(this Transform root, string childName)
    {
        if (string.IsNullOrEmpty(childName))
            return null;
        if (root.childCount <= 0)
            return null;

        Transform result = null;
        foreach (Transform child in root)
        {
            if (child.name == childName)
            {
                result = child;
                break;
            }
            if (child.childCount > 0)
            {
                result = GetChild(child, childName);
                if (result != null)
                {
                    break;
                }
            }
        }
        return result;
    }

    public static List<Transform> GetChildren(this Transform root)
    {
        List<Transform> result = new List<Transform>();
        foreach (Transform child in root)
        {
            result.Add(child);

            if (child.childCount > 0)
            {
                result.AddRange(GetChildren(child));
            }
        }
        return result;

    }

    //find root parent transform
    public static Transform GetRootParent(this Transform child)
    {
        if (child.parent == null)
            return child;
        else
            return GetRootParent(child.parent);
    }

    /// <summary>
    /// Find root parent transform with predicate, thiis will return null if no match
    /// </summary>
    public static Transform GetRootParent(this Transform child, System.Predicate<Transform> predicate)
    {
        if (child.parent == null)
        {
            if (predicate(child))
            {
                return child;
            }
            else
            {
                return null;
            }
        }
        else
        {
            if (predicate(child))
                return child;
            else
                return GetRootParent(child.parent, predicate);
        }
    }

    /// <summary>
    /// Is parent of the child
    /// </summary>
    public static bool IsParentOf(this Transform parent, Transform child)
    {
        if (child == null)
            return false;
        if (parent == null)
            return false;
        if (parent == child)
            return false;
        if (parent.childCount <= 0)
            return false;

        bool result = false;
        foreach (Transform t in parent)
        {
            if (t == child)
            {
                result = true;
                break;
            }
            if (t.childCount > 0)
            {
                result = IsParentOf(t, child);
                if (result)
                {
                    break;
                }
            }
        }
        return result;
    }

    public static Pose GetWorldPose(this Transform transform)
    {
        return new Pose(transform.position, transform.rotation);
    }

    public static Pose GetLocalPose(this Transform transform)
    {
        return new Pose(transform.localPosition, transform.localRotation);
    }

    public static void SetWorldPose(this Transform transform, Pose pose)
    {
        transform.position = pose.position;
        transform.rotation = pose.rotation;
    }

    public static void SetLocalPose(this Transform transform, Pose pose)
    {
        transform.localPosition = pose.position;
        transform.localRotation = pose.rotation;
    }
}
