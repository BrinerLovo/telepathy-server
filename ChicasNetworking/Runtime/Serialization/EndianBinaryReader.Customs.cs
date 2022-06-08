using System;
using System.Collections;
using System.Collections.Generic;

namespace Lovatto.Chicas.Internal
{
    public partial class EndianBinaryReader : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public object ReadObject()
        {
            var type = (SerializeObjectType)ReadByte();

            switch (type)
            {
                case SerializeObjectType.Byte:
                    return ReadByte();

                case SerializeObjectType.Short:
                    return ReadInt16();

                case SerializeObjectType.Integer:
                    return ReadInt32();

                case SerializeObjectType.Float:
                    return ReadSingle();

                case SerializeObjectType.String:
                    return ReadString();

                case SerializeObjectType.GameRoom:
                    return ReadGameRoom();

                case SerializeObjectType.ByteArray:
                    short length = ReadInt16();
                    return ReadBytes(length);

                default:
                    throw new ArgumentNullException($"No implemented serialization type: {type.ToString()}");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public object[] ReadObjectArray()
        {
            short length = ReadInt16();
            var array = new object[length];

            for (int i = 0; i < length; i++)
            {
                array[i] = ReadObject();
            }
            return array;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IDictionary<object, object> ReadDictionary()
        {
            short length = ReadInt16();

            var dictionary = new Dictionary<object, object>();

            while (length > 0)
            {
                var key = ReadObject();
                var value = ReadObject();

                dictionary.Add(key, value);
                length--;
            }

            return dictionary;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public GameRoom ReadGameRoom()
        {
            short length = ReadInt16();
            byte[] data = new byte[length];

            ReadInternal(data, length);

            var room = new GameRoom();
            NetworkSerializer.DeserializeStream(data, ref room);
            return room;
        }
    }
}