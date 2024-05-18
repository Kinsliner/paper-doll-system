using System;
using UnityEngine;

public class GizmosDebugger : MonoBehaviour
{
    private static Action OnDrawGizmosEvent;

    public static void DrawGizmos(Action action)
    {
        OnDrawGizmosEvent = action;
    }

    public static void DrawColorGizmos(Action action, Color color)
    {
        OnDrawGizmosEvent = () =>
        {
            Color originColor = Gizmos.color;

            Gizmos.color = color;

            action?.Invoke();

            Gizmos.color = originColor;
        };
    }

    private void OnDrawGizmos()
    {
        OnDrawGizmosEvent?.Invoke();
    }
}
