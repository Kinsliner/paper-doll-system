using Lean.Touch;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LeanEventAdapter : MonoBehaviour
{
    public Action OnSwipeEvent;

    /// <summary>Ignore fingers with StartedOverGui?</summary>
    [SerializeField]
    private bool ignoreStartedOverGui = true;

    /// <summary>Ignore fingers with OverGui?</summary>
    [SerializeField]
    private bool ignoreIsOverGui;

    /// <summary>The required angle of the swipe in degrees.
    /// 0 = Up.
    /// 90 = Right.
    /// 180 = Down.
    /// 270 = Left.</summary>
    [SerializeField]
    private float requiredAngle;

    /// <summary>The angle of the arc in degrees that the swipe must be inside.
    /// -1 = No requirement.
    /// 90 = Quarter circle (+- 45 degrees).
    /// 180 = Semicircle (+- 90 degrees).</summary>
    [SerializeField]
    private float requiredArc = -1.0f;

    [SerializeField]
    private float swipeThreshold = 300;

    private float swipeDis;
    private LeanFinger currentSwipeFinger;
    private List<LeanFinger> candidateSwipeFingers = new List<LeanFinger>();

    private void OnEnable()
    {
        LeanTouch.OnFingerDown += HandleFingerDown;
        LeanTouch.OnFingerUp += HandleFingerUp;
        LeanTouch.OnFingerUpdate += HandleFingerUpdate;
    }

    private void OnDisable()
    {
        LeanTouch.OnFingerDown -= HandleFingerDown;
        LeanTouch.OnFingerUp -= HandleFingerUp;
        LeanTouch.OnFingerUpdate -= HandleFingerUpdate;
    }

    private void HandleFingerDown(LeanFinger finger)
    {
        // 如果目前沒有滑動中的手指，則加入候選手指
        if (currentSwipeFinger == null)
        {
            candidateSwipeFingers.TryAdd(finger);
        }
    }

    private void HandleFingerUp(LeanFinger finger)
    {
        // 如果目前滑動手指是這隻手指，則清除目前滑動手指
        if (currentSwipeFinger == finger)
        {
            currentSwipeFinger = null;
        }

        // 清除該候選手指
        candidateSwipeFingers.Remove(finger);
    }

    private void HandleFingerUpdate(LeanFinger finger)
    {
        // 如果目前沒有滑動中的手指，則檢查候選手指是否符合滑動條件
        if (currentSwipeFinger == null)
        {
            if (candidateSwipeFingers.Contains(finger))
            {
                foreach (var candidateSwipeFinger in candidateSwipeFingers)
                {
                    if (HandleFingerSwipe(candidateSwipeFinger))
                    {
                        // 找到滑動手指，設定為目前滑動手指
                        currentSwipeFinger = finger;
                    }
                }
            }
        }
        // 如果目前有滑動中的手指，則檢查是否繼續滑動
        if (currentSwipeFinger != null)
        {
            if (IsDistanceValid(finger) && IsAngleValid(finger.ScreenDelta))
            {
                // 滑動距離超過閾值，觸發滑動事件
                float delta = finger.ScreenDelta.magnitude;
                swipeDis += delta;
                if (swipeDis > swipeThreshold)
                {
                    // 重置滑動距離
                    swipeDis = 0;
                    OnSwipeEvent?.Invoke();
                }
            }
        }
    }

    /// <summary>
    /// 檢查手指是否符合滑動條件
    /// </summary>
    private bool HandleFingerSwipe(LeanFinger finger)
    {
        if (ignoreStartedOverGui == true && finger.StartedOverGui == true)
        {
            return false;
        }

        if (ignoreIsOverGui == true && finger.IsOverGui == true)
        {
            return false;
        }

        return IsDistanceValid(finger) && IsAngleValid(finger.ScreenDelta);
    }

    /// <summary>
    /// 角度是否符合條件
    /// </summary>
    private bool IsAngleValid(Vector2 vector)
    {
        if (requiredArc >= 0.0f)
        {
            var angle = Mathf.Atan2(vector.x, vector.y) * Mathf.Rad2Deg;
            var angleDelta = Mathf.DeltaAngle(angle, requiredAngle);

            if (angleDelta < requiredArc * -0.5f || angleDelta >= requiredArc * 0.5f)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// 距離是否符合條件
    /// </summary>
    private bool IsDistanceValid(LeanFinger finger)
    {
        float swipeDis = finger.SwipeScreenDelta.magnitude;
        if (swipeDis > swipeThreshold)
        {
            return true;
        }
        return false;
    }
}
