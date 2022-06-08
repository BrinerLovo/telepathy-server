using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Telepathy;
using System.Text;

namespace Lovatto.Chicas.Internal
{
    public static class NetworkSerializer
    {

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="packet"></param>
        /// <returns></returns>
        public static byte[] SerializeStream<T>(T packet) where T : ICustomSerializable
        {
            if (packet == null) return null;

            byte[] data;
            using (MemoryStream ms = new MemoryStream())
            {
                using(EndianBinaryWriter bw = new EndianBinaryWriter(EndianBitConverter.Big, ms))
                {
                    packet.Write(bw);
                    data = ms.ToArray();
                }
            }
            return data;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static void DeserializeStream<T>(byte[] arrBytes, ref T result) where T : ICustomSerializable
        {
            using (MemoryStream memStream = new MemoryStream(arrBytes))
            {
                using(EndianBinaryReader reader = new EndianBinaryReader(EndianBitConverter.Big, memStream))
                {
                    result.Read(reader);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static byte[] SerializeText(string text)
        {
            return Encoding.UTF8.GetBytes(text);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static byte[] SerializeText(string text, ChicasInternalEventType eventType)
        {
            var byteArray = Encoding.UTF8.GetBytes(text);
            return ChicasNetworkingUtility.InsertInitialByteInArray((byte)eventType, byteArray);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string DeserializeText(byte[] arrBytes, int offset = 2)
        {
            byte[] desArr = new byte[arrBytes.Length - offset];
            Array.Copy(arrBytes, offset, desArr, 0, desArr.Length);
            return Encoding.UTF8.GetString(desArr);
        }
    }
}