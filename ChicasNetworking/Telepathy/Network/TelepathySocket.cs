using Lovatto.Chicas.Telepathy;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Telepathy;

namespace Lovatto.Chicas
{
    public class TelepathySocket : ChicasSocket
    {
        static TelepathyServer server;
        static TelepathyLobby lobby = new TelepathyLobby();

        public const int port = 1337;
        public const int MaxMessageSize = 16 * 1024; //16kb limit per package
        const ushort serverTick = 1000; // (100k limit to avoid deadlocks)
        const short serverFrequency = 60;

        public override bool IsConnected { get => server != null; }

        #region Socket Implementation
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool Connect()
        {
            Console.WriteLine($"Local IP: {GetLocalIPAddress()}");
            Console.WriteLine($"Public IP: {GetPublicIP()}");

            server = new TelepathyServer(MaxMessageSize);
            server.Start(port);
            server.OnConnected += OnServerConnected;
            server.OnDisconnected += OnServerDisconnected;
            server.OnData += Receive;

            while (true)
            {
                server.Tick(serverTick);
                Thread.Sleep(1000 / serverFrequency);
            }
        }

        public override void Send(byte[] data)
        {
            Log.Info($"Send data length: {data.Length}");
            var connectionIds = server.GetAllConnectionsIds();
            // send to all
            bool result = true;
            foreach (int connectionId in connectionIds)
            {
                result &= server.Send(connectionId, new ArraySegmentX<byte>(data));
            }
        }

        public override void Send(int connectionId, byte[] data)
        {
            Log.Info($"Send data length: {data.Length} to client {connectionId}");
            server.Send(connectionId, new ArraySegmentX<byte>(data));
        }

        public override void Send(int[] connectionGroup, byte[] data)
        {
            Log.Info($"Send data to Group length: {data.Length}");
            bool result = true;
            var segment = new ArraySegmentX<byte>(data);

            foreach (int connectionId in connectionGroup)
                result &= server.Send(connectionId, segment);
        }

        public override void Stop()
        {
            if (server != null) server.Stop();
        }

        public override Lobby GetLobby(string lobbyName)
        {
            return lobby;
        }

        public override ChicasClient[] GetAllClients()
        {
            return server.clients.Values.ToArray();
        }

        public override ChicasClient GetClientByName(string clientName)
        {
            var allClients = GetAllClients();

            for (int i = 0; i < allClients.Length; i++)
            {
                if (allClients[i].NickName == clientName)
                    return allClients[i];
            }
            return null;
        }

        public override ChicasClient GetClient(int connectionID)
        {
            ChicasClient client;
            server.clients.TryGetValue(connectionID, out client);
            return client;
        }

        public override string GetClientIpAddress(int connectionId)
        {
            return server.GetClientAddress(connectionId);
        }
        #endregion

        #region Socket Events
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
        /// <param name="connectionId"></param>
        /// <param name="data"></param>
        public void Receive(int connectionId, byte[] data)
        {
            var packet = packetPool.Acquire(data, 0);
            var eventType = (ChicasInternalEventType)packet.Code;

            Log.Info($"Received data ({data.Length}) result, event type: {eventType.ToString()}");
            switch (eventType)
            {
                case ChicasInternalEventType.Data:
                    // just transmit the data to all other clients
                    var all = server.GetAllConnectionsIds();
                    Send(all, data);
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
                    lobby.TryCreateRoom(connectionId, packet);
                    break;
                case ChicasInternalEventType.JoinRoom:
                    lobby.JoinClientToRoom(connectionId, packet);
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
        /// <param name="connectionID"></param>
        static void OnServerDisconnected(int connectionID)
        {
            var client = (ClientRef)connectionID;
            if (client != null)
            {
                // if the disconnected client was in a room
                if (client.Client.Status == ChicasNetworkStatus.InRoom)
                {
                    GameRoom room;
                    if (lobby.TryGetRoom(client.Client.Room, out room))
                    {
                        room.DisconnectClient(client);
                    }
                }
            }

            Log.Info($"{connectionID} Disconnected, remaining: {server.ConnectionsCount() - 1}");
        } 
        #endregion

        #region Helper Functions

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
        #endregion
    }
}