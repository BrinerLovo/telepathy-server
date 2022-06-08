using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Threading;
using Lovatto.Chicas;
using Lovatto.Chicas.Internal;

namespace Telepathy
{
    class ServerConsole
    {
        static GameServer server;
        public const int port = 1337;
        public const int MaxMessageSize = 16 * 1024; //16kb limit per package
        const ushort serverTick = 1000; // (100k limit to avoid deadlocks)
        const short serverFrequency = 60;

        static void Main(string[] args)
        {
            //Test.DoTest();

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
            server.Start(port);
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

        /// <summary>
        /// Called when the server receive data from a client
        /// </summary>
        public static void OnData(int connectionId, ArraySegmentX<byte> segment)
        {
            // the first byte of each package is the message type
            // var eventType = (ChicasInternalEventType)segment.Array[0];

            var packet = server.packetPool.Acquire(segment.Array, 0);
            var eventType = (ChicasInternalEventType)packet.Code;

            Log.Info($"Received data ({segment.Count}) result, event type: {eventType.ToString()}");
            switch (eventType)
            {
                case ChicasInternalEventType.Data:
                    // just transmit the data to all other clients
                    var all = server.GetAllConnectionsIds();
                    ServerSend(all, segment);
                    break;
                case ChicasInternalEventType.CreatePlayer:
                    AuthenticationHandler.CreatePlayer(connectionId, packet);
                    break;
                case ChicasInternalEventType.FetchFriends:
                    FriendsHandler.FetchFriends(connectionId, packet);
                    break;
                case ChicasInternalEventType.SendInvitation:
                    InvitationHandler.HandleSendInvitation(connectionId, packet);
                    break;
                case ChicasInternalEventType.CreateRoom:
                    server.lobby.TryCreateRoom(connectionId, packet);
                    break;
                case ChicasInternalEventType.JoinRoom:
                    server.lobby.JoinClientToRoom(connectionId, packet);
                    break;
                default:
                    Log.Warning("Not defined event type: " + eventType.ToString());
                    break;
            }

            // once processed, we don't need it anymore.
            packet.Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static bool ServerSend(int[] connectionIds, ArraySegment<byte> segment)
        {
            Log.Info($"Send data to Group length: {segment.Count}");
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
        public static bool ServerSendToAll(ArraySegment<byte> segment)
        {
            Log.Info($"Send data length: {segment.Count}");
            var connectionIds = server.GetAllConnectionsIds();
            // send to all
            bool result = true;
            foreach (int connectionId in connectionIds)
            {
                result &= server.Send(connectionId, segment);
            }

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
            var client = (ClientRef)connectionID;
            if(client != null)
            {
                // if the disconnected client was in a room
                if(client.Player.Status == ChicasNetworkStatus.InRoom)
                {
                    GameRoom room;
                    if(GetServer().lobby.TryGetRoom(client.Player.Room, out room))
                    {
                        room.DisconnectClient(client);
                    }
                }
            }

            Log.Info($"{connectionID} Disconnected, remaining: {server.ConnectionsCount() - 1}");
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
            string url = "http://api.ipify.org/";
            WebRequest req = WebRequest.Create(url);
            WebResponse resp = req.GetResponse();
            System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
            string response = sr.ReadToEnd().Trim();
            return response;
        }
    }
}
