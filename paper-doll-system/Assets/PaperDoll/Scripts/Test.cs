using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public ClosetPanelUI closetPanelUI;

    // Start is called before the first frame update
    void Start()
    {
        ModelAssetManager.Init();
        PaperDollManager.Init();

        if(closetPanelUI != null)
        {
            closetPanelUI.Init();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
