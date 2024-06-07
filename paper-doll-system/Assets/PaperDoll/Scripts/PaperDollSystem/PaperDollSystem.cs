using Ez.SystemModule;
using System.Collections.Generic;
using UnityEngine;

public class PaperDollSystem : ISystem
{
    private static PaperDollCanvas canvas;
    private string canvasPath = "PaperDoll/PaperDollCanvas";

    /// <summary>
    /// 初始化
    /// </summary>
    public void Init()
    {
        // 初始化紙娃娃系統資料
        ModelAssetManager.Init();
        PaperDollManager.Init();

        // 建立Canvas
        var canvas = Resources.Load<GameObject>(canvasPath);
        if (canvas != null)
        {
            var canvasObj = GameObject.Instantiate(canvas);
            PaperDollSystem.canvas = canvasObj.GetComponent<PaperDollCanvas>();
            PaperDollSystem.canvas.Init();
        }

        // 關閉紙娃娃面板
        DeactivePanel();
    }

    public void Uninit()
    {
        // 釋放紙娃娃系統資料
        ModelAssetManager.Uninit();
        PaperDollManager.Uninit();
    }

    public void Update()
    {
    }

    /// <summary>
    /// 開啟紙娃娃面板
    /// </summary>
    public static void ActivePanel()
    {
        if (canvas != null)
        {
            canvas.Active();
        }
    }

    /// <summary>
    /// 關閉紙娃娃面板
    /// </summary>
    public static void DeactivePanel()
    {
        if (canvas != null)
        {
            canvas.Deactive();
        }
    }

    /// <summary>
    /// 控制紙娃娃朝左轉
    /// </summary>
    public static void TurnLeftByPanel()
    {
        canvas.CharacterPanelUI.OnTurnLeftClick();
    }

    /// <summary>
    /// 控制紙娃娃朝右轉
    /// </summary>
    public static void TurnRightByPanel()
    {
        canvas.CharacterPanelUI.OnTurnRightClick();
    }

    /// <summary>
    /// 鎖定紙娃娃部位
    /// </summary>
    /// <param name="id">紙娃娃資料ID</param>
    public static void Lock(int id)
    {
        PaperDollManager.Controller.Lock(id);
    }

    /// <summary>
    /// 解鎖紙娃娃部位
    /// </summary>
    /// <param name="id">紙娃娃資料ID</param>
    public static void Unlock(int id)
    {
        PaperDollManager.Controller.Unlock(id);
    }

    /// <summary>
    /// 設定紙娃娃
    /// </summary>
    public static void SetPaperDoll(PaperDoll paperDoll)
    {
        PaperDollManager.Controller.SetupPaperDoll(paperDoll);
    }


    /// <summary>
    /// 設定紙娃娃和方向
    /// </summary>
    public static void SetPaperDoll(PaperDoll paperDoll, BodyDirection direction)
    {
        PaperDollManager.Controller.SetupPaperDoll(paperDoll);
        PaperDollManager.Controller.Turn(direction);
    }

    /// <summary>
    /// 設定紙娃娃，並附加部位
    /// </summary>
    /// <param name="ids">要附加的紙娃娃資料ID</param>
    public static void SetPaperDoll(PaperDoll paperDoll, params int[] ids)
    {
        PaperDollManager.Controller.SetupPaperDoll(paperDoll);
        foreach (var id in ids)
        {
            PaperDollManager.Controller.Attach(id);
        }
    }

    /// <summary>
    /// 取得紙娃娃使用中的部位
    /// </summary>
    public static List<PaperDollController.PaperDollCache> GetCachesOnPaperDoll()
    {
        return PaperDollManager.Controller.GetCachesOnPaperDoll();
    }

    /// <summary>
    /// 設定紙娃娃使用中的部位
    /// </summary>
    public static void SetCachesToPaperDoll(List<PaperDollController.PaperDollCache> caches)
    {
        foreach (var cache in caches)
        {
            PaperDollManager.Controller.Attach(cache);
        }
    }
}
