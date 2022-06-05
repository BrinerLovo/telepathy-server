namespace Lovatto.Chicas
{
    public class NetworkMessage
    {
        public NetworkMessageTarget NetworkMessageType = NetworkMessageTarget.All;
        public ChicasData DataTable;

        public NetworkMessage(NetworkMessageTarget networkMessageType = NetworkMessageTarget.All)
        {
            NetworkMessageType = networkMessageType;
            DataTable = new ChicasData();
        }

        /// <summary>
        /// Add data to the container.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(string key, object value)
        {
            if (DataTable == null) DataTable = new ChicasData();

            DataTable.Add(key, value);
        }

        /// <summary>
        /// Parse a network data to local only data
        /// </summary>
        /// <param name="hashtable"></param>
        public void ParseData(ChicasData hashtable)
        {
            NetworkMessageType = (NetworkMessageTarget)hashtable[ChicasNetworkingConstants.NETWORK_MESSAGE_TYPE_NAME];
            DataTable.Remove(ChicasNetworkingConstants.NETWORK_MESSAGE_TYPE_NAME);
            DataTable = hashtable;
        }
    }

    public enum NetworkMessageTarget
    {
        All = 0,
        Others = 1,
        ServerOnly = 2,
        ClientOnly = 3,
    }
}