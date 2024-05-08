public class FakeIdentity : INetworkIdentity
{
    public int GetNetworkID()
    {
        return -1;
    }

    public bool IsLocal()
    {
        return true;
    }

    public bool IsMaster()
    {
        return true;
    }
}
