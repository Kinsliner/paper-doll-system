using Lean.Touch;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LeanEventAdapter : MonoBehaviour
{
    public Action OnSwipeEvent;

    [SerializeField]
    private LeanFingerSwipe leanFingerSwipe;

    private void OnEnable()
    {
        if (leanFingerSwipe != null)
        {
            leanFingerSwipe.OnFinger.AddListener(OnSwipe);
        }
    }

    private void OnDisable()
    {
        if (leanFingerSwipe != null)
        {
            leanFingerSwipe.OnFinger.RemoveListener(OnSwipe);
        }
    }

    public void OnSwipe(LeanFinger finger)
    {
        OnSwipeEvent?.Invoke();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
