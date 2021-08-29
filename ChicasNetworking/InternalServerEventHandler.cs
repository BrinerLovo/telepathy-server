using System;
using System.Collections.Generic;
using Telepathy;
using Lovatto.Chicas.Internal;

namespace Lovatto.Chicas
{
    public class InternalServerEventHandler
    {
        /// <summary>
        /// Called after the client connect to the server and register his player presence.
        /// </summary>
        public static void CreatePlayer(int connectionID, ArraySegment<byte> message)
        {
            byte[] bytesArr = new byte[message.Count];
            Buffer.BlockCopy(message.Array, message.Offset, bytesArr, 0, message.Count);

            var nickName = NetworkSerializer.DeserializeText(bytesArr);
            var client = Program.GetServer().GetClient(connectionID);

            string echo;
            if (client != null)
            {
                client.NickName = nickName;
                echo = $"{connectionID}";
                Log.Info($"Player ({nickName.Length})({nickName}) created and link to connection {connectionID}");
            }
            else
            {
                echo = "-1";
            }

            Program.ServerSendToSingle(connectionID, new ArraySegment<byte>(NetworkSerializer.SerializeText(echo, ChicasInternalEventType.CreatePlayer)));
        }
    }
}