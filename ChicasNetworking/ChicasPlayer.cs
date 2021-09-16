using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using Telepathy;

namespace Lovatto.Chicas
{
    public class ChicasPlayer : ConnectionState
    {
        public int AuthID;
        public int OwnerID;
        public string NickName;
        public string ProvidedAddress;

        /// <summary>
        /// 
        /// </summary>
        public ChicasPlayer(TcpClient client, int MaxMessageSize) : base(client, MaxMessageSize)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetAdress()
        {
            return Program.GetServer().GetClientAddress(OwnerID);
        }
    }
}
