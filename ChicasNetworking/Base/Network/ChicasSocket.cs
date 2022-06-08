using Lovatto.Chicas.Internal;

namespace Lovatto.Chicas
{
    public abstract class ChicasSocket
    {
        public static ChicasSocket Active;
        public ChicasPacketPool packetPool = new ChicasPacketPool();

        /// <summary>
        /// 
        /// </summary>
        public abstract bool IsConnected
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        public abstract bool Connect();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract void Stop();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public abstract void Send(byte[] data);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionId"></param>
        /// <param name="data"></param>
        public abstract void Send(int connectionId, byte[] data);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionGroup"></param>
        /// <param name="data"></param>
        public abstract void Send(int[] connectionGroup, byte[] data);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract ChicasClient[] GetAllClients();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientName"></param>
        /// <returns></returns>
        public abstract ChicasClient GetClientByName(string clientName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionID"></param>
        /// <returns></returns>
        public abstract ChicasClient GetClient(int connectionID);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionId"></param>
        /// <returns></returns>
        public abstract string GetClientIpAddress(int connectionId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lobby"></param>
        /// <returns></returns>
        public abstract Lobby GetLobby(string lobby = "");

        #region Static Functions
        /// <summary>
        /// Send data to all clients.
        /// </summary>
        /// <param name="data"></param>
        public static void SendData(byte[] data)
        {
            Active.Send(data);
        }

        /// <summary>
        /// Send data to the given client.
        /// </summary>
        /// <param name="data"></param>
        public static void SendData(int connectionId, byte[] data)
        {
            Active.Send(connectionId, data);
        }

        /// <summary>
        /// Send data to the given client group.
        /// </summary>
        /// <param name="data"></param>
        public static void SendData(int[] connectionGroup, byte[] data)
        {
            Active.Send(connectionGroup, data);
        } 
        #endregion
    }
}