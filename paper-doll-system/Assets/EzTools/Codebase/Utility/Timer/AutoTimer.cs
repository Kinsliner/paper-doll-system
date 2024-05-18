using System;

public class AutoTimer : IUpdate
{
    private UnityTimer unityTimer;

    public AutoTimer(UnityTimer unityTimer)
    {
        this.unityTimer = unityTimer;
        Start();
    }

    public AutoTimer(float time, TimeDirection direction, Action OnTimeUp)
    {
        unityTimer = new UnityTimer(time, direction, OnTimeUp);
        Start();
    }

    public void OnUpdate()
    {
        unityTimer.TickTime();
    }

    public void Start()
    {
        UpdateHandler.Register(this);
    }

    public void Stop()
    {
        UpdateHandler.Unregister(this);
    }
}
