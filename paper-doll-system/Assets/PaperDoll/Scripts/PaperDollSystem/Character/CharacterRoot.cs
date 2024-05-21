using Ez.Tool;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterRoot : MonoBehaviour
{
    [SerializeField]
    private UICollector uiCollector;

    private PaperDollController paperDollController;
    private Transform characterRoot;

    public void Init()
    {
        paperDollController = PaperDollManager.Controller;
        paperDollController.OnPaperDollSetEvent += OnPaperDollSet;

        characterRoot = uiCollector.GetAsset<Transform>(UIKey.PaperDollSystem_CharacterRoot);
    }

    private void OnPaperDollSet(PaperDoll doll)
    {
        doll.transform.SetAndFitParent(characterRoot);
    }
}
