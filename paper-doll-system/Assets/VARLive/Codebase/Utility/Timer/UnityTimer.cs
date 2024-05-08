using System;
using UnityEngine;

public enum TimeDirection
{
    Positive,
    Negative
}

public class UnityTimer
{
    public float Percent 
    {
        get
        {
            float t = Mathf.InverseLerp(timer.StartTime, reachTime, timer.Time);
            return t;
        }
    }

    public float CurrentTime
    {
        get
        {
            return timer.Time;
        }
        set
        {
            timer.Time = value;
        }
    }

    private Action OnTimeUp;
    private Timer timer;
    private TimeDirection timeDirection;
    private float reachTime;

    public UnityTimer()
    {
        timer = new Timer();
        timeDirection = TimeDirection.Positive;
    }

    public UnityTimer(float time, TimeDirection direction)
    {
        this.timeDirection = direction;
        SetupTimer(time);
    }

    public UnityTimer(float time, TimeDirection direction, Action OnTimeUp)
    {
        this.timeDirection = direction;
        this.OnTimeUp = OnTimeUp;
        SetupTimer(time);
    }

    private void SetupTimer(float time)
    {
        if (timeDirection == TimeDirection.Positive)
        {
            reachTime = time;
            timer = new Timer();
        }
        if (timeDirection == TimeDirection.Negative)
        {
            reachTime = 0;
            timer = new Timer(time);
        }
    }

    public void TickTime()
    {
        IsTickOrTimeUp();
    }

    public void TickTime(float deltaTime)
    {
        IsTickOrTimeUp(deltaTime);
    }

    public bool IsTickOrTimeUp()
    {
        return IsTickOrTimeUp(Time.deltaTime);
    }

    public bool IsTickOrTimeUp(float deltaTime)
    {
        UpdateTimeByDirection(deltaTime);
        if (IsTimeUp())
        {
            OnTimeUp?.Invoke();
            Reset();
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool IsTickOrTimeUpOnce()
    {
        return IsTickOrTimeUpOnce(Time.deltaTime);
    }

    public bool IsTickOrTimeUpOnce(float deltaTime)
    {
        UpdateTimeByDirection(deltaTime);
        if (IsTimeUp())
        {
            OnTimeUp?.Invoke();
            return true;
        }
        else
        {
            return false;
        }
    }

    public void UpdateTimeByDirection(float deltaTime)
    {
        if (timeDirection == TimeDirection.Positive)
        {
            timer.UpdateTickUp(deltaTime);
        }
        if (timeDirection == TimeDirection.Negative)
        {
            timer.UpdateTickDown(deltaTime);
        }
    }

    public bool IsTimeUp()
    {
        return IsTimeUp(reachTime);
    }

    public bool IsTimeUp(float reachTime)
    {
        if (timeDirection == TimeDirection.Positive)
        {
            return timer.IsOverTime(reachTime);
        }
        if (timeDirection == TimeDirection.Negative)
        {
            return timer.IsLessTime(reachTime);
        }
        return false;
    }

    public void Reset()
    {
        timer.Reset();
    }
}
