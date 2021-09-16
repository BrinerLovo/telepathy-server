using System;
using System.Collections.Generic;
using Telepathy;
using System.Linq;

namespace Lovatto.Chicas
{
    public class GameServer : Server
    {
        public GameServer(int MaxMessageSize) : base(MaxMessageSize) { }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ChicasPlayer GetClient(int connectionID)
        {
            ChicasPlayer client;
            clients.TryGetValue(connectionID, out client);
            return client;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ChicasPlayer GetClientByName(string clientName)
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
        public ChicasPlayer[] GetAllClients() => clients.Values.ToArray();
    }
}
