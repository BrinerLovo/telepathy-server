using Lovatto.Chicas.Internal;
using System;

namespace Lovatto.Chicas
{
    [Serializable]
    public class NetworkMessage : ICustomSerializable
    {
        public short EventCode;
        public ChicasData Data;

        public NetworkMessage() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventCode"></param>
        public NetworkMessage(short eventCode)
        {
            EventCode = eventCode;
        }

        /// <summary>
        /// Add data to the container.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(string key, object value)
        {
            if (Data == null) Data = new ChicasData();

            Data.Add(key, value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object GetParam(object key)
        {
            if (Data == null || !Data.ContainsKey(key)) return null;

            return Data[key];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        public void Write(EndianBinaryWriter writer)
        {
            writer.Write(EventCode);
            writer.Write(Data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        public void Read(EndianBinaryReader reader)
        {
            EventCode = reader.ReadInt16();
            Data = new ChicasData(reader.ReadDictionary());
        }
    }
}