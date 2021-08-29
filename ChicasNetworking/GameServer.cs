using System;
using System.Collections.Generic;
using Telepathy;

namespace Lovatto.Chicas
{
    public class GameServer : Server
    {
        public GameServer(int MaxMessageSize) : base(MaxMessageSize) { }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ConnectionState GetClient(int connectionID)
        {
            ConnectionState client;
            clients.TryGetValue(connectionID, out client);
            return client;
        }
    }
}
