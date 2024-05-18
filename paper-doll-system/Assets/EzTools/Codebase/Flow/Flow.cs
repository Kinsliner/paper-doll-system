public abstract class Flow
{
    public virtual void Start()
    {
    }

    public virtual void Update()
    {
    }

    public virtual void Exit()
    {
    }

    public abstract bool IsComplete();
}