using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

namespace Telepathy
{
    class Program
    {
        static Server server;
        static ushort serverMaxReceivesPerTick = 10000;

        static void Main(string[] args)
        {
            Console.WriteLine($"Local IP: {GetLocalIPAddress()}");
            Console.WriteLine($"Public IP: {GetPublicIP()}");

            server = new Server();
            server.Start(1337);
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnAppExit);

            while (true)
            {
                CheckServerMsgLimit();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        static void CheckServerMsgLimit()
        {
            // process a maximum amount of server messages per tick
            for (int i = 0; i < serverMaxReceivesPerTick; ++i)
            {
                // stop when there is no more message
                if (!ProcessServerMessage())
                {
                    break;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static bool ProcessServerMessage()
        {
            if (server.GetNextMessage(out Telepathy.Message message))
            {
                switch (message.eventType)
                {
                    case Telepathy.EventType.Connected:
                        OnServerConnected(message.connectionId);
                        break;
                    case Telepathy.EventType.Data:
                        OnServerMessage(message.connectionId, Channels.DefaultReliable, new ArraySegment<byte>(message.data));
                        break;
                    case Telepathy.EventType.Disconnected:
                        OnServerDisconnected(message.connectionId);
                        break;
                    default:
                        // TODO handle errors from Telepathy when telepathy can report errors
                        OnServerDisconnected(message.connectionId);
                        break;
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        static void OnServerMessage(int connectionId, int channelId, ArraySegment<byte> segment)
        {
            var list = server.GetAllConnectionsIds();
            ServerSend(list, channelId, segment);
        }

        public static bool ServerSend(int connectionId, int channelId, ArraySegment<byte> segment) => ServerSend(new List<int>() { connectionId }, channelId, segment);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionIds"></param>
        /// <param name="channelId"></param>
        /// <param name="segment"></param>
        /// <returns></returns>
        public static bool ServerSend(List<int> connectionIds, int channelId, ArraySegment<byte> segment)
        {
            // telepathy doesn't support allocation-free sends yet.
            // previously we allocated in Mirror. now we do it here.
            byte[] data = new byte[segment.Count];
            Array.Copy(segment.Array, segment.Offset, data, 0, segment.Count);

            Logger.Log("Data length: " + data.Length);
            // send to all
            bool result = true;
            foreach (int connectionId in connectionIds)
                result &= server.Send(connectionId, data);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionID"></param>
        static void OnServerConnected(int connectionID)
        {
            Logger.Log(connectionID + " Connected");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionID"></param>
        static void OnServerDisconnected(int connectionID)
        {
            Logger.Log(connectionID + " Disconnected");
        }

        /// <summary>
        /// 
        /// </summary>
        static void OnAppExit(object sender, EventArgs e)
        {
            if (server != null) server.Stop();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string GetPublicIP()
        {
            string url = "http://checkip.dyndns.org";
            System.Net.WebRequest req = System.Net.WebRequest.Create(url);
            System.Net.WebResponse resp = req.GetResponse();
            System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
            string response = sr.ReadToEnd().Trim();
            string[] a = response.Split(':');
            string a2 = a[1].Substring(1);
            string[] a3 = a2.Split('<');
            string a4 = a3[0];
            return a4;
        }
    }
}
