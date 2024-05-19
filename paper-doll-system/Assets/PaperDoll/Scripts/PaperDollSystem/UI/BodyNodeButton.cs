using Ez.Tool;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BodyNodeButton : MonoBehaviour
{
    [System.Serializable]
    private struct BodyNodeIcon
    {
        public BodyNode node;
        public Sprite image;
    }

    public Action<BodyNode> OnClickEvent;

    [SerializeField]
    private List<BodyNodeIcon> iconList = new List<BodyNodeIcon>();

    [SerializeField]
    private UICollector uiCollector;

    private BodyNode bodyNode;

    public void Init(BodyNode bodyNode)
    {
        this.bodyNode = bodyNode;

        SetIcon(bodyNode);
    }

    private void SetIcon(BodyNode bodyNode)
    {
        foreach (var icon in iconList)
        {
            if (icon.node == bodyNode)
            {
                // Set icon
                uiCollector.SetImage(UIKey.BodyNodeButton_IconImage, icon.image);
                break;
            }
        }
    }

    public void OnClick()
    {
        OnClickEvent?.Invoke(bodyNode);
    }
}