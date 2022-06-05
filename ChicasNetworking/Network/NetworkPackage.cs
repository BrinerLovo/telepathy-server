using System;

namespace Lovatto.Chicas.Internal
{
    [Serializable]
    public class NetworkPackage
    {
        public int OwnerID;
        public byte Code;
        public byte[] Data;
    }
}