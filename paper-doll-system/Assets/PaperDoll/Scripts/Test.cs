using Ez.Input;
using System;
using System.Collections;
using UnityEngine;

public class Test : MonoBehaviour
{
    public PaperDoll character;
    public int testLcokID = 1;
    public float moveSpeed = 5.0f;

    private PaperDollSystem paperDollSystem;

    // Start is called before the first frame update
    void Start()
    {
        PaperDollSystem.ActivePanel();
        PaperDollSystem.SetPaperDoll(character);
    }

    private void Update()
    {
        
    }

    [ContextMenu("Lock")]
    private void Lock()
    {
        PaperDollSystem.Lock(testLcokID);
    }

    [ContextMenu("UnLock")]
    private void UnLock()
    {
        PaperDollSystem.Unlock(testLcokID);
    }
}
