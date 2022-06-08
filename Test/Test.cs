using Lovatto.Chicas.Internal;
using System;
using System.IO;
using Telepathy;

namespace Lovatto.Chicas
{
    public class Test
    {

        public static void DoTest()
        {
            GameRoom gr = new GameRoom();
            gr.Name = "Test Room";
            gr.MaxPlayers = 8;
            gr.PlayerList = new int[1] { 1 };


            var dic = new ChicasData();
            dic.Add("TV1", 8);
            dic.Add("Hola", "Hello");

            var message = new NetworkMessage();
            message.EventCode = 7;
            message.Data = dic;

            byte[] byteArray = NetworkSerializer.SerializeStream(message);
            Log.Info($"Serialized {byteArray.Length}");

            var meesage2 = new NetworkMessage();
            NetworkSerializer.DeserializeStream(byteArray, ref meesage2);

            Log.Info($"Final message ({meesage2.EventCode})");

            foreach (var item in meesage2.Data)
            {
                Log.Info($"Key: {item.Key.ToString()} Value: {item.Value.ToString()}");
            }

            Console.ReadLine();
        }
    }
}