using Ez.Tool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaperDollCanvas : MonoBehaviour
{
    [SerializeField]
    private UICollector uiCollector;

    /// <summary>
    /// 衣櫃面板
    /// </summary>
    public ClosetPanelUI ClosetPanelUI => uiCollector.GetAsset<ClosetPanelUI>(UIKey.PaperDollCanvas_ClosetPanel);

    /// <summary>
    /// 角色面板
    /// </summary>
    public CharacterPanelUI CharacterPanelUI => uiCollector.GetAsset<CharacterPanelUI>(UIKey.PaperDollCanvas_CharacterPanel);

    /// <summary>
    /// 角色根節點
    /// </summary>
    public CharacterRoot CharacterRoot => uiCollector.GetAsset<CharacterRoot>(UIKey.PaperDollCanvas_CharacterRoot);

    /// <summary>
    /// 初始化
    /// </summary>
    public void Init()
    {
        ClosetPanelUI.Init();
        CharacterPanelUI.Init();
        CharacterRoot.Init();
    }

    /// <summary>
    /// 開啟紙娃娃面板
    /// </summary>
    public void Active()
    {
        if (uiCollector != null)
        {
            uiCollector.Active(UIKey.PaperDollCanvas_Root);
        }
        CharacterRoot.IsActive = true;
    }

    /// <summary>
    /// 關閉紙娃娃面板
    /// </summary>
    public void Deactive()
    {
        if(uiCollector != null)
        {
            uiCollector.Deactive(UIKey.PaperDollCanvas_Root);
        }
        CharacterRoot.IsActive = false;
    }
}