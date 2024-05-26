using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public ClosetPanelUI closetPanelUI;
    public CharacterPanelUI characterPanelUI;
    public PaperDoll character;
    public CharacterRoot characterRoot;
    public int testLcokID = 1;

    // Start is called before the first frame update
    void Start()
    {
        ModelAssetManager.Init();
        PaperDollManager.Init();

        if (closetPanelUI != null)
        {
            closetPanelUI.Init();
        }
        if (characterPanelUI != null)
        {
            characterPanelUI.Init();
        }
        if (characterRoot != null)
        {
            characterRoot.Init();
        }

        PaperDollManager.Controller.SetupPaperDoll(character, BodyNode.Head, BodyNode.Body);
    }

    [ContextMenu("Lock")]
    private void Lock()
    {
        PaperDollManager.Controller.Lock(testLcokID);
    }

    [ContextMenu("UnLock")]
    private void UnLock()
    {
        PaperDollManager.Controller.Unlock(testLcokID);
    }
}
