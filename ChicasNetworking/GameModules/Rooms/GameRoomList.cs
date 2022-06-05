using System;

namespace Lovatto.Chicas
{
    [Serializable]
    public class GameRoomList
    {
        public GameRoom[] rooms;

        public GameRoomList() { }

        public GameRoomList(GameRoom[] roomList)
        {
            rooms = roomList;
        }
    }
}