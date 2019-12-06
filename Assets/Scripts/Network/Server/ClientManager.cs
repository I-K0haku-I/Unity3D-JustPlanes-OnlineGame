using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace JustPlanes.Network.Server
{
    static class ClientManager
    {
        public static Dictionary<int, Client> clients = new Dictionary<int, Client>();

        public static void CreateNewConnection(TcpClient tempClient)
        {
            Client newClient = new Client();
            newClient.socket = tempClient;

            // TODO: is this unique? some kind of uuid better?
            newClient.connectionID = ((IPEndPoint)tempClient.Client.RemoteEndPoint).Port;
            newClient.player = new Player(newClient.connectionID.ToString(), 0, 0);
            Game.players.TryAdd(newClient.connectionID, newClient.player);
            DataSender.SendPlayerJoined(newClient.player);
            newClient.Start();
            clients.Add(newClient.connectionID, newClient);

            DataSender.SendWelcomeMessage(newClient.connectionID);
        }

        public static void SendDataTo(int connectionID, byte[] data)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteInteger((data.GetUpperBound(0) - data.GetLowerBound(0)) + 1);
            buffer.WriteBytes(data);
            clients[connectionID].stream.BeginWrite(buffer.ToArray(), 0, buffer.ToArray().Length, null, null);
            buffer.Dispose();
        }

        public static List<Player> GetPlayers()
        {
            return clients.Values.ToList().Select(c => c.player).ToList();
        }

        public static void SendDataToAll(byte[] data)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteInteger((data.GetUpperBound(0) - data.GetLowerBound(0)) + 1);
            buffer.WriteBytes(data);
            foreach (var client in clients.Values)
            {
                client.stream.BeginWrite(buffer.ToArray(), 0, buffer.ToArray().Length, null, null);
            }
            buffer.Dispose();
        }
    }
}