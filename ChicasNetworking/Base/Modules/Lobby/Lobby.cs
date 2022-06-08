using Lovatto.Chicas.Internal;

namespace Lovatto.Chicas
{
    public abstract class Lobby
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionId"></param>
        /// <param name="packet"></param>
        public abstract void TryCreateRoom(ClientRef connectionId, ChicasPacket packet);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionId"></param>
        /// <param name="packet"></param>
        public abstract void JoinClientToRoom(ClientRef connectionId, ChicasPacket packet);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="room"></param>
        public abstract void RemoveRoom(GameRoom room);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="roomName"></param>
        /// <param name="gameRoom"></param>
        /// <returns></returns>
        public abstract bool TryGetRoom(string roomName, out GameRoom gameRoom);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rooms"></param>
        public abstract void SendRoomToAll(GameRoom[] rooms);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionId"></param>
        public abstract void SendRoomsTo(int connectionId);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract GameRoom[] GetRoomList();
    }
}