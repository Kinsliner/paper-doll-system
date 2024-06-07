using Ez.Tool;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterPanelUI : MonoBehaviour
{
    [SerializeField]
    private UICollector uiCollector;

    private PaperDollController paperDollController;
    private BodyDirection currentDirection = BodyDirection.Front;
    private int turnCount = 0;

    // Start is called before the first frame update
    public void Init()
    {
        paperDollController = PaperDollManager.Controller;
        turnCount = (int)currentDirection;

        if (uiCollector != null)
        {
            uiCollector.BindOnClick(UIKey.Character_TurnLeftButton, OnTurnLeftClick);
            uiCollector.BindOnClick(UIKey.Character_TurnRightButton, OnTurnRightClick);
        }
    }

    public void OnTurnRightClick()
    {
        CalculateTurnCount(1);

        if (paperDollController != null)
        {
            paperDollController.Turn(currentDirection);
        }
    }

    public void OnTurnLeftClick()
    {
        CalculateTurnCount(-1);

        if (paperDollController != null)
        {
            paperDollController.Turn(currentDirection);
        }
    }

    private void CalculateTurnCount(int offset)
    {
        turnCount += offset;
        if (turnCount < 0)
        {
            turnCount = 3;
        }
        else if (turnCount > 3)
        {
            turnCount = 0;
        }

        currentDirection = (BodyDirection)turnCount;
    }
}
