namespace Lovatto.Chicas
{
    /// <summary>
    /// 
    /// </summary>
    public enum ChicasInternalEventType : short
    {
        Data = 0,
        CreatePlayer = 1,
        PlayerConnected = 2,
        FetchFriends = 3,
        SendInvitation = 4,
        ReceiveInvitation = 5,
        RoomList = 6,
        CreateRoom,
        JoinRoom,
        RoomUpdate,
        PlayerLeftRoom,
    }
}