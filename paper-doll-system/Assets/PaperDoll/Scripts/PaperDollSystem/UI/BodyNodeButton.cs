using System;
using UnityEngine;

public class BodyNodeButton : MonoBehaviour
{
    public Action<BodyNode> OnClickEvent;

    private BodyNode bodyNode;

    public void Init(BodyNode bodyNode)
    {
        this.bodyNode = bodyNode;
    }

    public void OnClick()
    {
        OnClickEvent?.Invoke(bodyNode);
    }
}