using Lovatto.Chicas.Internal;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Telepathy;

namespace Lovatto.Chicas
{
    [Serializable]
    public class GameRoom : ICustomSerializable
    {
        /// <summary>
        /// 
        /// </summary>
        public enum RoomFlags : byte
        {
            None,
            Removed,
        }

        /// <summary>
        /// Room name
        /// </summary>
        public string Name;

        /// <summary>
        /// Max players allowed in this room.
        /// </summary>
        public byte MaxPlayers;

        /// <summary>
        /// players (connection Ids) joined in this room
        /// </summary>
        public int[] PlayerList;

        /// <summary>
        /// This room custom properties
        /// </summary>
        public ChicasData Properties;

        /// <summary>
        /// 
        /// </summary>
        public RoomFlags Flags;

        /// <summary>
        /// Players inside the room count.
        /// </summary>
        /// <returns></returns>
        public int GetPlayerCount()
        {
            if (PlayerList == null || PlayerList.Length == 0) return 0;
            return PlayerList.Length;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        public void DisconnectClient(ClientRef client)
        {
            int remain = 0;
            for (int i = 0; i < PlayerList.Length; i++)
            {
                if (PlayerList[i] == client)
                {
                    // Do not remove the slot from the list since we use the index as the identifier
                    // of the players in this room, so removing it will reorder all the other players
                    // and make everything more messy, instead just set to -1 which will be interpret as free slot.
                    PlayerList[i] = -1;
                }
                else
                {
                    // if this is not an empty slot
                    if (PlayerList[i] != -1)
                    {
                        remain++;
                    }
                }
            }

            // if there is no more players in this room
            if(remain == 0)
            {
                // unlisted this room and delete it from the cache.
                GameServer.I.lobby.RemoveRoom(this);
            }
            else
            {
                // if there are remaining players in this room
                // let them know about the disconnection.

                var response = new OpResponse();
                response.AddParam(client.ConnectionId);
                WriteToAll(response.GetAsPacket(ChicasInternalEventType.PlayerLeftRoom).GetSerializedSegment());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="eventType"></param>
        public void WriteToAll(ArraySegmentX<byte> data)
        {
            ServerConsole.ServerSend(GetJoinedClients(), data);
        }

        /// <summary>
        /// Get only joined clients
        /// </summary>
        /// <returns></returns>
        public int[] GetJoinedClients()
        {
            // @TODO: Cache this list and determine when a change in the original list 
            // happen in order to do this only when a change happen.

            int[] ids = new int[0];
            for (int i = 0; i < PlayerList.Length; i++)
            {
                if (PlayerList[i] == -1) continue;

                ids.Append(PlayerList[i]);
            }
            return ids;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        public void Write(EndianBinaryWriter writer)
        {
            writer.Write(Name);
            writer.Write(MaxPlayers);
            writer.Write(PlayerList);
            writer.Write((byte)Flags);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        public void Read(EndianBinaryReader reader)
        {
            Name = reader.ReadString();
            MaxPlayers = reader.ReadByte();
            PlayerList = reader.ReadInt32Array();
            Flags = (RoomFlags)reader.ReadByte();
        }

        #region Defaults Override
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj)
        {
            if (obj is null) return false;

            if (obj is GameRoom)
            {
                GameRoom grRef = (GameRoom)obj;
                return Name == grRef.Name;
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(GameRoom a, GameRoom b)
        {
            if (a is null)
            {
                if (b is null) return true;

                return false;
            }

            if (string.IsNullOrEmpty(a.Name))
            {
                if (string.IsNullOrEmpty(b.Name)) return true;

                return false;
            }

            return a.Equals(b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(GameRoom a, GameRoom b)
        {
            if (a is null)
            {
                if (b is null) return false;

                return true;
            }

            if (string.IsNullOrEmpty(a.Name))
            {
                if (string.IsNullOrEmpty(b.Name)) return false;

                return true;
            }

            return !a.Equals(b);
        } 
        #endregion
    }
}