using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILateUpdate
{
    void OnLateUpdate();
}

public class LateUpdateHandler : MonoBehaviour
{
    private static List<ILateUpdate> lateUpdates = new List<ILateUpdate>();

    public static void Register(ILateUpdate lateUpdate)
    {
        if (lateUpdates.Contains(lateUpdate) == false)
        {
            lateUpdates.Add(lateUpdate);
        }
    }

    public static void Unregister(ILateUpdate lateUpdate)
    {
        if (lateUpdates.Contains(lateUpdate))
        {
            lateUpdates.Remove(lateUpdate);
        }
    }

    private void LateUpdate()
    {
        if (lateUpdates.IsNullOrEmpty())
            return;

        foreach (var lateUpdate in lateUpdates.ToArray())
        {
            lateUpdate.OnLateUpdate();
        }
    }
}
