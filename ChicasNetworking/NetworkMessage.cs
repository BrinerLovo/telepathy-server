using System.Collections;
using System.Collections.Generic;

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

        public void ParseData(ChicasData hashtable)
        {
            NetworkMessageType = (NetworkMessageTarget)hashtable["msgt"];
            DataTable.Remove("msgt");
            DataTable = hashtable;
        }
    }

    public enum NetworkMessageTarget
    {
        All = 0,
        Others = 1,
        ServerOnly = 2,
    }
}