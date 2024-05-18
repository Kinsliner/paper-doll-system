using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUpdate
{
    void OnUpdate();
}

public class UpdateHandler : MonoBehaviour
{
    private static List<IUpdate> updates = new List<IUpdate>();

    public static void Register(IUpdate update)
    {
        if (updates.Contains(update) == false)
        {
            updates.Add(update);
        }
    }

    public static void Unregister(IUpdate update)
    {
        if (updates.Contains(update))
        {
            updates.Remove(update);
        }
    }

    private void Update()
    {
        if (updates.IsNullOrEmpty())
            return;

        foreach (var update in updates.ToArray())
        {
            update.OnUpdate();
        }
    }
}
