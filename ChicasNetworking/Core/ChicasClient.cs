using System.Net.Sockets;
using Telepathy;

namespace Lovatto.Chicas
{
    public class ChicasClient : ConnectionState
    {
        public int AuthID;
        public int OwnerID;
        public string NickName;
        public ChicasNetworkStatus Status = ChicasNetworkStatus.ConnectedToMaster;
        public string ProvidedAddress;

        /// <summary>
        /// Room where this player is joined (if any)
        /// </summary>
        public string Room;

        /// <summary>
        /// 
        /// </summary>
        public ChicasClient(TcpClient client, int MaxMessageSize) : base(client, MaxMessageSize)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetAdress()
        {
            return ServerConsole.GetServer().GetClientAddress(OwnerID);
        }
    }
}
