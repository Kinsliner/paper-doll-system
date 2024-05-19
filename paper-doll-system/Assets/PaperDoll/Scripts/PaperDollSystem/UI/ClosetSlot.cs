using Ez.Tool;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosetSlot : MonoBehaviour
{
    [System.Serializable]
    private struct SlotView
    {
        public BodyNode node;
        [UIKey("ClosetSlot")]
        public string rootKey;
        [UIKey("ClosetSlot")]
        public string iconKey;
    }

    public bool IsEmpty { get; private set; }

    [SerializeField]
    private UICollector uiCollector;

    [SerializeField]
    private List<SlotView> slotViews = new List<SlotView>();

    public void Init()
    {
    }

    public void Clear()
    {
    }

    public void Display(PaperDollController.PaperDollCache paperDollCache)
    {

    }
}
