using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CursorLocker
{
    public static bool IsLocked { get; private set; } = true;

    private static bool isVisable = false;

    public static void Lock()
    {
        isVisable = false;
        IsLocked = true;
        UpdateCursor();
    }

    public static void Unlock()
    {
        isVisable = true;
        IsLocked = false;
        UpdateCursor();
    }

    public static void Switch()
    {
        isVisable = !isVisable;
        IsLocked = !IsLocked;
        UpdateCursor();
    }

    private static void UpdateCursor()
    {
        Cursor.visible = isVisable;
        if (IsLocked)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
