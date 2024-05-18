using System.Collections.Generic;

public class Sequence : Flow
{
    private List<Flow> flows = new List<Flow>();

    private Flow currentFlow = null;
    private int currentIndex = 0;

    public void AddFlow(Flow flow)
    {
        flows.Add(flow);
    }

    public void RemoveFlow(Flow flow)
    {
        flows.Remove(flow);
    }

    public void ClearFlows()
    {
        flows.Clear();
    }

    public override void Start()
    {
        base.Start();

        currentIndex = 0;
        currentFlow = null;
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
        StartFlow();
    }

    public void PrevFlow()
    {
        if (currentFlow != null)
        {
            currentFlow.Exit();
        }
        
        currentIndex -= 1;
        if (currentIndex < 0)
        {
            currentIndex = 0;
        }
        
        StartFlow();
    }

    public override bool IsComplete()
    {
        return currentIndex >= flows.Count;
    }
}