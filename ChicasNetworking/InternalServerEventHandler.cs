using System;
using System.Collections.Generic;
using Telepathy;
using Lovatto.Chicas.Internal;

namespace Lovatto.Chicas
{
    public class InternalServerEventHandler
    {
        /// <summary>
        /// Called after the client connect to the server and register his player presence.
        /// </summary>
        public static void CreatePlayer(int connectionID, ArraySegmentX<byte> message)
        {
            byte[] bytesArr = new byte[message.Count];
            Buffer.BlockCopy(message.Array, message.Offset, bytesArr, 0, message.Count);

            var text = NetworkSerializer.DeserializeText(bytesArr);
            var client = ServerConsole.GetServer().GetClient(connectionID);

            // Example text Username|10 
            string[] split = text.Split('|');
            string nickName = split[0];
            int id;
            int.TryParse(split[1], out id);

            string echo;
            if (client != null)
            {
                client.NickName = nickName;
                client.AuthID = id;
                echo = $"{connectionID}";
                Log.Info($"Player: {nickName}#{id} created and link to connection: {connectionID}");
            }
            else
            {
                echo = "-1";
            }

            ServerConsole.ServerSendToSingle(connectionID, new ArraySegmentX<byte>(NetworkSerializer.SerializeText(echo, ChicasInternalEventType.CreatePlayer)));

            // after create the player, sent the current room list so he have a copy of it locally.
            ServerConsole.GetServer().lobby.SendRoomTo(connectionID);
        }

        public struct FriendData
        {
            public bool Found;
            public int AuthID;
            public int ConnectionID;
            public string NickName;
            public bool IsOnline;
            public bool InGroup;

            public FriendData(int authID)
            {
                Found = true;
                AuthID = authID;
                ConnectionID = -1;
                NickName = "";
                IsOnline = false;
                InGroup = false;
            }

            public int IsOnlineBinary() => IsOnline ? 1 : 0;
        }

        /// <summary>
        /// 
        /// </summary>
        public static void FetchFriends(int connectionID, ArraySegmentX<byte> message)
        {
            byte[] bytesArr = new byte[message.Count];
            Buffer.BlockCopy(message.Array, message.Offset, bytesArr, 0, message.Count);

            // Get the text from byte array
            var line = NetworkSerializer.DeserializeText(bytesArr);

            // decompile the data from the text line
            var split = line.Split(',');
            var ids = new int[split.Length];

            // get the friends ids to verify
            for (int i = 0; i < split.Length; i++)
            {
                if (string.IsNullOrEmpty(split[i])) continue;

                ids[i] = int.Parse(split[i]);
            }

            var cliens = ServerConsole.GetServer().GetAllClients();
            var friends = new FriendData[ids.Length];

            // check if the friends are connected to the server
            for (int i = 0; i < cliens.Length; i++)
            {
                for (int e = 0; e < ids.Length; e++)
                {
                    // if a friend is connected
                    if(ids[e] == cliens[i].AuthID)
                    {
                        // collect their information
                        friends[e] = new FriendData(ids[e]);
                        friends[e].ConnectionID = cliens[i].OwnerID;
                        friends[e].IsOnline = true;
                    }
                }
            }

            // echo the information of the connected friends to the client who request the information
            string echo = "";
            for (int i = 0; i < friends.Length; i++)
            {
                if (friends[i].Found == false) continue;

                var f = friends[i];
                echo += $"{f.AuthID}|{f.ConnectionID}|{f.IsOnlineBinary()}&";
            }

            //Log.Info($"Fetched friends: {echo}");

            ServerConsole.ServerSendToSingle(connectionID, new ArraySegmentX<byte>(NetworkSerializer.SerializeText(echo, ChicasInternalEventType.FetchFriends)));
        }

        /// <summary>
        /// 
        /// </summary>
        public static void SendInvitation(int connectionID, ArraySegmentX<byte> message)
        {
            var text = GetTextFromSegment(message);

            string[] split = text.Split('|');
            int friendConnectionID = int.Parse(split[0]);
            var friend = ServerConsole.GetServer().GetClient(friendConnectionID);

            if(friend == null)
            {
                Log.Warning($"Couldn't found client with conID {friendConnectionID}");
                return;
            }

            var sender = ServerConsole.GetServer().GetClient(connectionID);
            if (sender == null)
            {
                Log.Warning($"Couldn't found client sender with conID {connectionID}");
                return;
            }
            sender.ProvidedAddress = split[1];

            string data = $"{sender.NickName}|{connectionID}|{sender.AuthID}|{sender.ProvidedAddress}|{split[2]}";
            ServerConsole.ServerSendToSingle(friendConnectionID, new ArraySegmentX<byte>(NetworkSerializer.SerializeText(data, ChicasInternalEventType.ReceiveInvitation)));
            Log.Info($"Invitation send to {friend.NickName} from {sender.NickName} to join in {sender.ProvidedAddress}");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static string GetTextFromSegment(ArraySegmentX<byte> message)
        {
            return NetworkSerializer.DeserializeText(GetBytesFromSegment(message));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static byte[] GetBytesFromSegment(ArraySegmentX<byte> message)
        {
            byte[] bytesArr = new byte[message.Count];
            Buffer.BlockCopy(message.Array, message.Offset, bytesArr, 0, message.Count);
            return bytesArr;
        }
    }
}