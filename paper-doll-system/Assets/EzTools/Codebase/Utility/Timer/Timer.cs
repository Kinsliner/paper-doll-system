using System.Collections;
using System.Collections.Generic;

public class Timer
{
    public float Time { get; set; }
    public float StartTime { get; }

    public Timer()
    {
        Time = 0;
        StartTime = 0;
    }

    public Timer(float time)
    {
        Time = time;
        StartTime = time;
    }

    public void Reset()
    {
        Time = StartTime;
    }

    public void UpdateTickDown(float deltaTime)
    {
        Time -= deltaTime;
    }

    public void UpdateTickUp(float deltaTime)
    {
        Time += deltaTime;
    }

    public bool IsOverTime(float over)
    {
        return Time >= over;
    }

    public bool IsLessTime(float less)
    {
        return Time <= less;
    }
}
