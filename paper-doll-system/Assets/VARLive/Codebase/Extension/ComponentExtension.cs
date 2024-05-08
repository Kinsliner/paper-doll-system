using UnityEngine;

public static class ComponentExtension
{
    /// <summary>
    /// Get the component of the specified type, if it exists. Otherwise, add it.
    /// </summary>
    public static T GetOrAddComponent<T>(this GameObject go) where T : Component
    {
        T component = go.GetComponent<T>();
        if (component == null)
        {
            component = go.AddComponent<T>();
        }
        return component;
    }

    /// <summary>
    /// Get the component of the specified type, if it exists. Otherwise, add it.
    /// </summary>
    public static T GetOrAddComponent<T>(this Component component) where T : Component
    {
        //check null
        if (component == null)
        {
            return null;
        }

        return component.gameObject.GetOrAddComponent<T>();
    }
}
