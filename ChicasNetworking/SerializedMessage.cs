using System;
using System.Collections.Generic;

namespace Lovatto.Chicas.Internal
{
    [Serializable]
    public class SerializedMessage
    {
        public SerializedMessageType SerializedMessageType = SerializedMessageType.Binary;
        public byte Code;
        public byte[] Data;

        public SerializedMessage(SerializedMessageType messageType)
        {
            SerializedMessageType = messageType;
        }
    }

    [Serializable]
    public enum SerializedMessageType
    {
        Binary = 0,
        PlainText = 1,
        NetworkMessage = 2,
        InternalMessage = 3,
    }
}