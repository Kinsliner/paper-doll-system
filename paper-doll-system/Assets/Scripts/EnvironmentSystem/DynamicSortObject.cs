using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicSortObject : MonoBehaviour
{
    public Transform Anchor
    {
        get
        {
            return anchor;
        }
    }

    public int SortingOrderBackground
    {
        get
        {
            return sortingOrderBackground;
        }
    }

    public int SortingOrderForeground
    {
        get
        {
            return sortingOrderForeground;
        }
    }

    [SerializeField]
    private Transform anchor;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private int sortingOrderBackground = -90;

    [SerializeField]
    private int sortingOrderForeground = 90;

    private void Reset()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        anchor = transform;
    }

    private void OnEnable()
    {
        DynamicSorting.AddDynamicSortObject(this);
    }

    private void OnDisable()
    {
        DynamicSorting.RemoveDynamicSortObject(this);
    }

    public void SetToBackground()
    {
        SetSortingOrder(sortingOrderBackground);
    }

    public void SetToForeground()
    {
        SetSortingOrder(sortingOrderForeground);
    }

    private void SetSortingOrder(int sortingOrder)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = sortingOrder;
        }
    }
}
