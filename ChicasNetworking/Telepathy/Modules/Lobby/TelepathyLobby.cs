using Lovatto.Chicas.Internal;
using System.Collections.Generic;
using System.IO;
using Telepathy;
using System.Linq;

namespace Lovatto.Chicas.Telepathy
{
    public class TelepathyLobby : Lobby
    {
        private Dictionary<string, GameRoom> rooms = new Dictionary<string, GameRoom>();

        /// <summary>
        /// Try to create a room.
        /// This is called when a client request to create a room.
        /// </summary>
        public override void TryCreateRoom(ClientRef connectionId, ChicasPacket packet)
        {
            Log.Info($"Create Room received, size {packet.Buffer.Length}");
            GameRoom room = new GameRoom();
            NetworkSerializer.DeserializeStream(packet.Buffer, ref room);

            // response container
            var response = new OpResponse();

            if (TryCreateRoom(connectionId, ref room))
            {
                var client = connectionId.Client;
                client.Room = room.Name;
                client.Status = ChicasNetworkStatus.InRoom;

                Log.Info($"Room {room.Name} created, players: {room.PlayerList.Length}");

                response.Code = 201;
                response.AddParam(room);

                var roomList = new GameRoom[1] { room };

                // update the rooms for all the players
                SendRoomToAll(roomList);
            }
            else
            {
                Log.Warning("Couldn't create room: " + room.Name);

                // Expectation Failed
                response.Code = 417;
            }

            packet.SetBinary(NetworkSerializer.SerializeStream(response));

            // send room create result to the requester client.
            ChicasSocket.SendData(connectionId, packet.GetSerializedPacket());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="roomName"></param>
        /// <returns></returns>
        public bool TryCreateRoom(int connectionId, ref GameRoom room)
        {
            if (rooms.ContainsKey(room.Name))
            {
                Log.Warning($"A room with the name {room.Name} already exist.");
                return false;
            }

            room.PlayerList = new int[1] { connectionId };

            rooms.Add(room.Name, room);

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionId"></param>
        /// <param name="data"></param>
        public override void JoinClientToRoom(ClientRef connectionId, ChicasPacket packet)
        {
            string roomName = NetworkSerializer.DeserializeText(packet.Buffer, 0);

            OpResponse response = new OpResponse();
            // if the room doesn't exist
            if (!rooms.ContainsKey(roomName))
            {
                response.Code = 404;
                connectionId.Writte(response.GetAsPacket(ChicasInternalEventType.JoinRoom).GetSerializedSegment());
                return;
            }

            var room = rooms[roomName];

            if (room.PlayerList.Contains(connectionId))
            {
                response.Code = 302;
                response.Error = "Player is already in this room";
                connectionId.Writte(response.GetAsPacket(ChicasInternalEventType.JoinRoom).GetSerializedSegment());
                return;
            }

            if (room.GetPlayerCount() >= room.MaxPlayers)
            {
                response.Code = 429;
                response.Error = "Room is full";
                connectionId.Writte(response.GetAsPacket(ChicasInternalEventType.JoinRoom).GetSerializedSegment());
                return;
            }

            // add the new player to the player list
            room.PlayerList = room.PlayerList.Append(connectionId);

            var client = connectionId.Client;
            client.Status = ChicasNetworkStatus.InRoom;
            client.Room = roomName;

            Log.Info($"Player {connectionId} joined to room {room.Name}, PlayersInRoom: {room.PlayerList.Length}");

            response.Code = 200;
            // attach the room in the message so the client have a copy of it.
            response.AddParam(room);

            // send the response to the client who join to the room.
            connectionId.Writte(response.GetAsPacket(ChicasInternalEventType.JoinRoom).GetSerializedSegment());

            response.Dispose();

            // send the update room info to all
            var roomList = new GameRoom[1] { room };
            var roomPacket = GetRoomListPacket(roomList);

            // update the room information for all the players in the room.
            room.WriteToAll(roomPacket.GetSerializedSegment());
        }

        /// <summary>
        /// Remove a room from the list.
        /// </summary>
        public override void RemoveRoom(GameRoom room)
        {
            string roomName = room.Name;
            if (!rooms.ContainsKey(roomName)) return;

            rooms.Remove(roomName);

            Log.Info($"Room {roomName} was removed, remaining rooms: {rooms.Count}");

            room.Flags = GameRoom.RoomFlags.Removed;
            var roomList = new GameRoom[1] { room };

            // update the updated room to all the clients
            SendRoomToAll(roomList);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override GameRoom[] GetRoomList()
        {
            return rooms.Values.ToArray();
        }

        /// <summary>
        /// Send rooms to all the clients
        /// </summary>
        public override void SendRoomToAll(GameRoom[] rooms)
        {
            // @TODO: Send only to clients connected to the lobby.

            var packet = GetRoomListPacket(rooms);
            ChicasSocket.SendData(packet.GetSerializedPacket());
        }

        /// <summary>
        /// Send all the listed rooms to a single client.
        /// </summary>
        /// <param name="connectionId"></param>
        public override void SendRoomsTo(int connectionId)
        {
            var roomList = GetRoomList();
            ChicasSocket.SendData(connectionId, GetRoomListPacket(roomList).GetSerializedPacket());

            Log.Info($"Rooms ({roomList.Length}) send to client: {connectionId}");
        }

        /// <summary>
        /// Return a network package ready to transport from the given room list.
        /// </summary>
        /// <param name="roomList"></param>
        /// <returns></returns>
        public static ChicasPacket GetRoomListPacket(GameRoom[] roomList)
        {
            var packet = ChicasPacketPool.AcquirePacket();
            packet.Code = (short)ChicasInternalEventType.RoomList;

            using (MemoryStream ms = new MemoryStream())
            {
                using (EndianBinaryWriter bw = new EndianBinaryWriter(EndianBitConverter.Big, ms))
                {
                    short length = (short)roomList.Length;
                    bw.Write(length);

                    for (int i = 0; i < length; i++)
                    {
                        bw.Write(roomList[i]);
                    }

                    packet.SetBinary(ms.ToArray());
                }
            }

            return packet;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="roomName"></param>
        /// <param name="gameRoom"></param>
        /// <returns></returns>
        public override bool TryGetRoom(string roomName, out GameRoom gameRoom)
        {
            if (rooms.ContainsKey(roomName))
            {
                gameRoom = rooms[roomName];
                return true;
            }

            gameRoom = null;
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="roomName"></param>
        /// <returns></returns>
        public bool RoomExist(string roomName)
        {
            return rooms.ContainsKey(roomName);
        }
    }
}