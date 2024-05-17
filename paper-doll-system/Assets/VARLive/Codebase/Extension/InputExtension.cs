using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InputExtension
{
    public static Vector3 GetMouseWorldXZ()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        worldPos.y = 0f;
        return worldPos;
    }
}
