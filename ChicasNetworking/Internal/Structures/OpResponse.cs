using Lovatto.Chicas.Internal;
using System;
using Telepathy;

namespace Lovatto.Chicas
{
    [Serializable]
    public class OpResponse : ICustomSerializable
    {
        public short Code;
        public string Message = "";
        public string Error = "";
        public object[] Params;

        /// <summary>
        /// Add parameter
        /// </summary>
        /// <param name="obj"></param>
        public void AddParam(object obj)
        {
            if (Params == null)
            {
                Params = new object[1];
                Params[0] = obj;
                return;
            }

            Params.Append(obj);
        }

        /// <summary>
        /// Get parameter from list
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public object GetParameter(int index)
        {
            if (index < 0 || Params == null) return null;
            if (index > Params.Length - 1)
            {
                Log.Error("Parameter index out of range.");
                return null;
            }
            return Params[index];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        public void Write(EndianBinaryWriter writer)
        {
            writer.Write(Code);
            writer.Write(Message);
            writer.Write(Error);
            writer.Write(Params);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        public void Read(EndianBinaryReader reader)
        {
            Code = reader.ReadInt16();
            Message = reader.ReadString();
            Error = reader.ReadString();
            Params = reader.ReadObjectArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ChicasPacket GetAsPacket(ChicasInternalEventType eventType)
        {
            var packet = ChicasPacketPool.AcquirePacket();
            packet
                .SetCode(eventType)
                .SetBinary(NetworkSerializer.SerializeStream(this));

            return packet;
        }
    }
}