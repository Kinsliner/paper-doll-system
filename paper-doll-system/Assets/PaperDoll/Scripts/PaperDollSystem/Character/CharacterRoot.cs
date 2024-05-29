using Ez.Tool;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterRoot : MonoBehaviour
{
    public bool IsActive { get; set; }

    [SerializeField]
    private Transform root;

    private PaperDollController paperDollController;

    /// <summary>
    /// 初始化
    /// </summary>
    public void Init()
    {
        paperDollController = PaperDollManager.Controller;
        paperDollController.OnPaperDollSetEvent += OnPaperDollSet;

        if (root == null)
        {
            root = transform;
        }
    }

    private void OnPaperDollSet(PaperDoll doll)
    {
        if (doll == null)
        {
            return;
        }
        if (IsActive == false)
        {
            return;
        }
        if (root == null)
        {
            root = transform;
        }
        doll.transform.SetAndFitParent(root);
    }
}
