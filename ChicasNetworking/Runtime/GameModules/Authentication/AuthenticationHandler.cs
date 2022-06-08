using Lovatto.Chicas.Internal;
using Telepathy;

namespace Lovatto.Chicas
{
    public class AuthenticationHandler
    {
        /// <summary>
        /// Called after the client connect to the server and register his player presence.
        /// </summary>
        public static void CreatePlayer(ClientRef connectionID, ChicasPacket package)
        {
            var text = NetworkSerializer.DeserializeText(package.Buffer, 0);
            var client = connectionID.Client;

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
                client.Status = ChicasNetworkStatus.ConnectedToLobby;

                echo = $"{(int)connectionID}";
                Log.Info($"Player: {nickName}#{id} created and link to connection: {(int)connectionID}");
            }
            else
            {
                echo = "-1";
            }

            // re-use the same packet to send the response.
            package.SetBinary(NetworkSerializer.SerializeText(echo));

            ChicasSocket.SendData(connectionID, package.GetSerializedPacket());

            // after create the player, sent the current room list so he have a copy of it locally.
            ChicasSocket.Active.GetLobby(string.Empty).SendRoomsTo(connectionID);
        }
    }
}