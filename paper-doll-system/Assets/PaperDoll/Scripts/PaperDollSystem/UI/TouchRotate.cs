using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchRotate : MonoBehaviour
{
    [SerializeField]
    private LeanEventAdapter leftSwipe;
    [SerializeField]
    private LeanEventAdapter rightSwipe;

    private void OnEnable()
    {
        if (leftSwipe != null)
        {
            leftSwipe.OnSwipeEvent += TuneLeft;
        }
        if (rightSwipe != null)
        {
            rightSwipe.OnSwipeEvent += TuneRight;
        }
    }

    private void OnDisable()
    {
        if (leftSwipe != null)
        {
            leftSwipe.OnSwipeEvent -= TuneLeft;
        }
        if (rightSwipe != null)
        {
            rightSwipe.OnSwipeEvent -= TuneRight;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    private void TuneRight()
    {
        PaperDollSystem.TurnRightByPanel();
    }

    private void TuneLeft()
    {
        PaperDollSystem.TurnLeftByPanel();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
