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
        /// <returns></returns>
        public static byte[] SerializeMessage(SerializedMessage networkMessage, ChicasInternalEventType eventType = ChicasInternalEventType.Data)
        {
            if (networkMessage == null)
                return null;

            byte[] data;
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, networkMessage);
                data = ms.ToArray();
            }

            return ChicasNetworkingUtility.InsertInitialByteInArray((byte)eventType, data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static SerializedMessage DeserializeMessage(byte[] arrBytes)
        {
            using (MemoryStream memStream = new MemoryStream())
            {
                BinaryFormatter binForm = new BinaryFormatter();
                memStream.Write(arrBytes, 0, arrBytes.Length);

                //byte 0 = EventType, so start seeking from byte index 1
                memStream.Seek(1, SeekOrigin.Begin);
                try
                {
                    return (SerializedMessage)binForm.Deserialize(memStream);
                }
                catch (SerializationException ex) { Log.Error("Serialization Error: " + ex.Message); return null; }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static byte[] SerializeChicasData(NetworkMessage networkMessage)
        {
            if (networkMessage == null)
                return null;

            if (networkMessage.DataTable == null)
                networkMessage.DataTable = new ChicasData();

            networkMessage.DataTable.Add("msgt", networkMessage.NetworkMessageType);

            byte[] data;
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, networkMessage.DataTable);
                data = ms.ToArray();
            }

            return data;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static NetworkMessage DeserializeChicasData(byte[] arrBytes)
        {
            ChicasData obj;
            using (MemoryStream memStream = new MemoryStream())
            {
                BinaryFormatter binForm = new BinaryFormatter();
                memStream.Write(arrBytes, 0, arrBytes.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                try
                {
                    obj = (ChicasData)binForm.Deserialize(memStream);
                    NetworkMessage nm = new NetworkMessage();
                    nm.ParseData(obj);
                    return nm;
                }
                catch (SerializationException ex) { Log.Error("Serialization Error: " + ex.Message); return null; }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static byte[] SerializeText(string text, ChicasInternalEventType eventType = ChicasInternalEventType.Data)
        {
            var byteArray = Encoding.UTF8.GetBytes(text);
            return ChicasNetworkingUtility.InsertInitialByteInArray((byte)eventType, byteArray);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string DeserializeText(byte[] arrBytes)
        {
            byte[] desArr = new byte[arrBytes.Length - 1];
            Array.Copy(arrBytes, 1, desArr, 0, desArr.Length);
            return Encoding.UTF8.GetString(desArr);
        }
    }
}