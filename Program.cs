using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Threading;
using Lovatto.Chicas.Internal;
using Lovatto.Chicas;

namespace Telepathy
{
    class Program
    {
        static GameServer server;
        public const int MaxMessageSize = 16 * 1024; //16kb limit per package
        const ushort serverTick = 1000; // (100k limit to avoid deadlocks)
        const short serverFrequency = 60;

        static void Main(string[] args)
        {
            RunServerThread();
        }

        /// <summary>
        /// 
        /// </summary>
        public static void RunServerThread()
        {
            Console.WriteLine($"Local IP: {GetLocalIPAddress()}");
            Console.WriteLine($"Public IP: {GetPublicIP()}");

            server = new GameServer(MaxMessageSize);
            server.Start(1337);
            server.OnConnected += OnServerConnected;
            server.OnDisconnected += OnServerDisconnected;
            server.OnData += OnData;

            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnAppExit);

            while (true)
            {
                server.Tick(serverTick);
                Thread.Sleep(1000 / serverFrequency);
            }
        }

        public static void OnData(int connectionId, ArraySegment<byte> segment)
        {
            var eventType = (ChicasInternalEventType)segment.Array[0];

            Log.Info($"Received data ({segment.Count}) result, event type: {eventType.ToString()}");
            switch (eventType)
            {
                case ChicasInternalEventType.Data:
                    var all = server.GetAllConnectionsIds();
                    ServerSend(all, segment);
                    break;
                case ChicasInternalEventType.CreatePlayer:
                    InternalServerEventHandler.CreatePlayer(connectionId, segment);
                    break;
                default:
                    Log.Warning("Not defined event type: " + eventType.ToString());
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static bool ServerSend(List<int> connectionIds, ArraySegment<byte> segment)
        {
            Log.Info($"Send data length: {segment.Array.Length}");
            // send to all
            bool result = true;
            foreach (int connectionId in connectionIds)
                result &= server.Send(connectionId, segment);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static bool ServerSendToSingle(int connectionId, ArraySegment<byte> segment)
        {
            Log.Info($"Send data length: {segment.Array.Length} to client {connectionId}");
            // send to all
             return server.Send(connectionId, segment);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionID"></param>
        static void OnServerConnected(int connectionID)
        {
            Log.Info($"Client {connectionID} Connected, total connections: {server.ConnectionsCount()}");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionID"></param>
        static void OnServerDisconnected(int connectionID)
        {
            Log.Info(connectionID + " Disconnected");
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
        public static GameServer GetServer() => server;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string GetPublicIP()
        {
            string url = "http://checkip.dyndns.org";
            WebRequest req = WebRequest.Create(url);
            WebResponse resp = req.GetResponse();
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
