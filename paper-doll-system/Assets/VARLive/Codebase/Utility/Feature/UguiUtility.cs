using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UguiUtility
{
    //check if the mouse is over the UI
    public static bool IsMouseHover(RectTransform rectTransform)
    {
        if (rectTransform == null)
            return false;

        Vector2 localMousePosition = Vector2.zero;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, Input.mousePosition, null, out localMousePosition))
        {
            if (rectTransform.rect.Contains(localMousePosition))
            {
                // 滑鼠在UI物體的範圍內
                return true;
            }
        }
        return false;
    }
}
