using Lovatto.Chicas.Internal;
using System.Collections.Generic;
using Lovatto.Chicas;
using System.Linq;

namespace Telepathy.ChicasNetworking
{
    public class Lobby
    {
        private Dictionary<string, GameRoom> rooms = new Dictionary<string, GameRoom>();

        /// <summary>
        /// Try to create a room.
        /// This is called when a client request to create a room.
        /// </summary>
        public void TryCreateRoom(ClientRef connectionId, ArraySegmentX<byte> data)
        {
            GameRoom room = NetworkSerializer.Deserialize<GameRoom>(data.Array);

            // response container
            var response = new OpResponse();

            if (TryCreateRoom(connectionId, ref room))
            {
                connectionId.Player.Room = room.Name;
                connectionId.Player.Status = ChicasNetworkStatus.InRoom;

                Log.Info($"Room {room.Name} created.");

                response.Code = 201;
                response.AddParam(room);

                // update the rooms for all the players
                SendRoomsToAll();
            }
            else
            {
                Log.Warning("Couldn't create room: " + room.Name);

                // Expectation Failed
                response.Code = 417;
            }

            // send room create result to the requester client.
            ServerConsole.ServerSendToSingle(connectionId, new ArraySegmentX<byte>(NetworkSerializer.Serialize(response, ChicasInternalEventType.CreateRoom)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionId"></param>
        /// <param name="data"></param>
        public void JoinClientToRoom(ClientRef connectionId, ArraySegmentX<byte> data)
        {
            var message = NetworkSerializer.DeserializeChicasData(data.Array);

            OpResponse response = new OpResponse();
            // if the room doesn't exist
            if (!rooms.ContainsKey((string)message.DataTable["room"]))
            {
                response.Code = 404;
                connectionId.SerializeAndWritte(response, ChicasInternalEventType.JoinRoom);
                return;
            }

            var room = rooms[(string)message.DataTable["room"]];

            if (room.PlayerList.Contains(connectionId))
            {
                response.Code = 302;
                response.Error = "Player is already in this room.";
                connectionId.SerializeAndWritte(response, ChicasInternalEventType.JoinRoom);
                return;
            }

            // add the new player to the player list
            room.PlayerList = room.PlayerList.Append(connectionId);

            Log.Info($"Player {connectionId} joined to room {room.Name}, PlayersInRoom: {room.PlayerList.Length}");

            response.Code = 200;
            // attach the room in the message so the client have a copy of it.
            response.Params = new object[1]
            {
                room
            };

            // send the response to the client who join to the room.
            connectionId.SerializeAndWritte(response, ChicasInternalEventType.JoinRoom);

            // update the room information for all the players in the room.
            room.SerializedAndWriteToAll(room, ChicasInternalEventType.RoomUpdate);
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
        /// Remove a room from the list.
        /// </summary>
        public void RemoveRoom(GameRoom room)
        {
            string roomName = room.Name;
            if (!rooms.ContainsKey(roomName)) return;

            rooms.Remove(roomName);

            Log.Info($"Room {roomName} was removed, remaining rooms: {rooms.Count}");

            // update the rooms for all the players
            SendRoomsToAll();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public GameRoom[] GetRoomList()
        {
            return rooms.Values.ToArray();
        }

        /// <summary>
        /// Send rooms to all the clients
        /// </summary>
        public void SendRoomsToAll()
        {
            // @TODO: Make so only send the information of the changed room
            // @TODO: Send only to the clients in lobby.

            var roomList = new GameRoomList(GetRoomList());
            foreach (var room in roomList.rooms)
            {
                //Log.Warning($"Sending room: {room.Name} players: {room.PlayerList.Count} max: {room.MaxPlayers}");
            }
            ServerConsole.ServerSendToAll(new ArraySegmentX<byte>(NetworkSerializer.Serialize(roomList, ChicasInternalEventType.RoomList)));
        }

        /// <summary>
        /// Send room list to a single client
        /// </summary>
        /// <param name="connectionId"></param>
        public void SendRoomTo(int connectionId)
        {
            var roomList = new GameRoomList(GetRoomList());
            ServerConsole.ServerSendToSingle(connectionId, new ArraySegmentX<byte>(NetworkSerializer.Serialize(roomList, ChicasInternalEventType.RoomList)));
            Log.Info($"Rooms send to {connectionId}");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="roomName"></param>
        /// <param name="gameRoom"></param>
        /// <returns></returns>
        public bool TryGetRoom(string roomName, out GameRoom gameRoom)
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
