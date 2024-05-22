using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public ClosetPanelUI closetPanelUI;
    public PaperDoll character;
    public CharacterRoot characterRoot;

    // Start is called before the first frame update
    void Start()
    {
        ModelAssetManager.Init();
        PaperDollManager.Init();

        if (closetPanelUI != null)
        {
            closetPanelUI.Init();
        }
        if (characterRoot != null)
        {
            characterRoot.Init();
        }

        PaperDollManager.Controller.SetupPaperDoll(character, BodyNode.Head, BodyNode.Body);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
