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

    public Action<BodyNodeButton, BodyNode> OnClickEvent;

    [SerializeField]
    private List<BodyNodeIcon> iconList = new List<BodyNodeIcon>();

    [SerializeField]
    private UICollector uiCollector;

    private Animator anim;
    private BodyNode bodyNode;

    public void Init(BodyNode bodyNode)
    {
        this.bodyNode = bodyNode;

        anim = uiCollector.GetAsset<Animator>(UIKey.BodyNodeButton_Animator);

        uiCollector.BindOnClick(UIKey.BodyNodeButton_SelectButton, OnClick);

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
        OnClickEvent?.Invoke(this, bodyNode);
    }

    public void SetSelected(bool selected)
    {
        anim.SetBool("IsSelect", selected);
    }
}