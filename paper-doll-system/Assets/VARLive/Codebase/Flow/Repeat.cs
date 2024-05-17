using System;
using System.Collections.Generic;

public class Repeat : Flow
{
    private Func<bool> condition;
    private List<Flow> flows = new List<Flow>();
    private Flow currentFlow = null;
    private int currentIndex = 0;

    public Repeat(Func<bool> condition)
    {
        this.condition = condition;
    }

    public void AddFlow(Flow flow)
    {
        flows.Add(flow);
    }

    public void RemoveFlow(Flow flow)
    {
        flows.Remove(flow);
    }
    
    public override void Start()
    {
        base.Start();

        currentIndex = 0;
        currentFlow = null;
        StartFlow();
    }

    private void StartFlow()
    {
        if (currentIndex < flows.Count)
        {
            currentFlow = flows[currentIndex];
            currentFlow.Start();
        }
        else
        {
            currentFlow = null;
        }
    }

    public override void Update()
    {
        base.Update();

        if (currentFlow != null)
        {
            currentFlow.Update();
            if (currentFlow.IsComplete())
            {
                NextFlow();
            }
        }
    }

    public void NextFlow()
    {
        if (currentFlow != null)
        {
            currentFlow.Exit();
        }
        
        currentIndex += 1;
        if (currentIndex >= flows.Count)
        {
            currentIndex = 0;
        }
        
        StartFlow();
    }

    public override void Exit()
    {
        base.Exit();

        foreach (Flow flow in flows)
        {
            flow.Exit();
        }
    }

    public override bool IsComplete()
    {
        if (condition != null)
        {
            return condition.Invoke();
        }
        return false;
    }
}
