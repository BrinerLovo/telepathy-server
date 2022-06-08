using Lovatto.Chicas.Internal;
using Telepathy;

namespace Lovatto.Chicas
{
    public class AuthenticationHandler
    {
        /// <summary>
        /// Called after the client connect to the server and register his player presence.
        /// </summary>
        public static void CreatePlayer(int connectionID, ChicasPacket package)
        {
            var text = NetworkSerializer.DeserializeText(package.Buffer, 0);
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

            // re-use the same packet to send the response.
            package.SetBinary(NetworkSerializer.SerializeText(echo));

            ServerConsole.ServerSendToSingle(connectionID, package.GetSerializedSegment());

            // after create the player, sent the current room list so he have a copy of it locally.
            ServerConsole.GetServer().lobby.SendRoomsTo(connectionID);
        }
    }
}