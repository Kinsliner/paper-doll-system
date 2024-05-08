using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFixedUpdate
{
    void OnFixedUpdate();
}

public class FixedUpdateHandler : MonoBehaviour
{
    private static List<IFixedUpdate> fixedUpdates = new List<IFixedUpdate>();

    public static void Register(IFixedUpdate fixedUpdate)
    {
        if (!fixedUpdates.Contains(fixedUpdate))
        {
            fixedUpdates.Add(fixedUpdate);
        }
    }

    public static void Unregister(IFixedUpdate fixedUpdate)
    {
        if (fixedUpdates.Contains(fixedUpdate))
        {
            fixedUpdates.Remove(fixedUpdate);
        }
    }

    private void FixedUpdate()
    {
        if (fixedUpdates.IsNullOrEmpty())
            return;

        foreach (var fixedUpdate in fixedUpdates.ToArray())
        {
            fixedUpdate.OnFixedUpdate();
        }
    }
}
