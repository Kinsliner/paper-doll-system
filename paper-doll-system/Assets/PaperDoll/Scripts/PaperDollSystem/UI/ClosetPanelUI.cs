using Ez.Tool;
using System.Collections;
using UnityEngine;

public class ClosetPanelUI : MonoBehaviour
{
    [SerializeField]
    private UICollector uiCollector;

    private PaperDollController paperDollController;

    public void Init()
    {
        paperDollController = PaperDollManager.Controller; ;
    }
}