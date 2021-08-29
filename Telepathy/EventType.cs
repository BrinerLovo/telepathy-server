namespace Telepathy
{
    public enum EventType : byte
    {
        Connected = 0,
        Data = 1,
        Disconnected = 2,
        CreatePlayer = 3,
        PlayerEnter = 4,
    }

    /// <summary>
    /// 
    /// </summary>
    public enum ChicasInternalEventType : byte
    {
        Data = 0,
        CreatePlayer = 1,
        PlayerConnected = 2,
    }
}
