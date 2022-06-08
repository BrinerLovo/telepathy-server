using Lovatto.Chicas.Internal;
using Telepathy;

namespace Lovatto.Chicas
{
    public class FriendsHandler
    {
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

        public static void FetchFriends(int connectionID, ChicasPacket package)
        {

            // Get the text from byte array
            var line = NetworkSerializer.DeserializeText(package.Buffer, 0);

            // decompile the data from the text line
            var split = line.Split(',');
            var ids = new int[split.Length];

            // get the friends ids to verify
            for (int i = 0; i < split.Length; i++)
            {
                if (string.IsNullOrEmpty(split[i])) continue;

                ids[i] = int.Parse(split[i]);
            }

            var cliens = ChicasSocket.Active.GetAllClients();
            var friends = new FriendData[ids.Length];

            // check if the friends are connected to the server
            for (int i = 0; i < cliens.Length; i++)
            {
                for (int e = 0; e < ids.Length; e++)
                {
                    // if a friend is connected
                    if (ids[e] == cliens[i].AuthID)
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

            package.SetBinary(NetworkSerializer.SerializeText(echo));

            Log.Info($"Fetched friends response: {echo}, size: {package.Buffer.Length}");

            ChicasSocket.SendData(connectionID, package.GetSerializedPacket());
        }
    }
}