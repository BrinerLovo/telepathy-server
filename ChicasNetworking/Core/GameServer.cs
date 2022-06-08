using Telepathy;
using System.Linq;
using Telepathy.ChicasNetworking;
using Lovatto.Chicas.Internal;

namespace Lovatto.Chicas
{
    public class GameServer : Server
    {
        /// <summary>
        /// Handle all the game room logic
        /// </summary>
        public Lobby lobby = new Lobby();

        /// <summary>
        /// 
        /// </summary>
        public ChicasPacketPool packetPool = new ChicasPacketPool();

        public GameServer(int MaxMessageSize) : base(MaxMessageSize) { }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ChicasClient GetClient(int connectionID)
        {
            ChicasClient client;
            clients.TryGetValue(connectionID, out client);
            return client;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ChicasClient GetClientByName(string clientName)
        {
            var allClients = clients.Values.ToArray();

            for (int i = 0; i < allClients.Length; i++)
            {
                if (allClients[i].NickName == clientName)
                    return allClients[i];
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ChicasClient[] GetAllClients() => clients.Values.ToArray();

        /// <summary>
        /// Current server instance
        /// </summary>
        public static GameServer I
        {
            get
            {
               return ServerConsole.GetServer();
            }
        }
    }
}
