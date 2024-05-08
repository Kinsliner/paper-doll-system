using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Installer
{
    public static T Install<T>() where T : Component
    {
        T comp = Object.FindObjectOfType<T>(true);
        if (comp == null)
        {
            string name = typeof(T).Name;
            GameObject go = new GameObject($"[{name}]");
            comp = go.AddComponent<T>();
        }
        return comp;
    }
}
