using System.Collections;
using System.Collections.Generic;

public class Partial : Flow
{
    protected List<Flow> flows = new List<Flow>();

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

        foreach (Flow flow in flows)
        {
            flow.Start();
        }
    }

    public override void Exit()
    {
        base.Exit();

        foreach (Flow flow in flows)
        {
            flow.Exit();
        }
    }

    public override void Update()
    {
        base.Update();

        foreach (Flow flow in flows)
        {
            flow.Update();
        }
    }

    public override bool IsComplete()
    {
        foreach (Flow flow in flows)
        {
            if (flow.IsComplete() == true)
            {
                return true;
            }
        }
        return false;
    }
}
