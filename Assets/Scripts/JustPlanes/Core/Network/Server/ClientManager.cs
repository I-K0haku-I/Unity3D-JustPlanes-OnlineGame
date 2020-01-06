using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace JustPlanes.Core.Network.Server
{
    static class ClientManager
    {
        public static Dictionary<string, Client> clients = new Dictionary<string, Client>();

        public static void CreateNewConnection(TcpClient tempClient)
        {
            Client newClient = new Client();
            newClient.socket = tempClient;

            // TODO: is this unique? some kind of uuid better?
            newClient.connectionID = ((IPEndPoint)tempClient.Client.RemoteEndPoint).Port.ToString();

            Console.WriteLine($"Someone connected at: {newClient.connectionID}");

            newClient.Start();
            clients.Add(newClient.connectionID.ToString(), newClient);

            // DataSender.SendWelcomeMessage(newClient.connectionID);
        }

        public static void SendDataTo(string connectionID, byte[] data)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteInteger((data.GetUpperBound(0) - data.GetLowerBound(0)) + 1);
            buffer.WriteBytes(data);
            clients[connectionID].stream.BeginWrite(buffer.ToArray(), 0, buffer.ToArray().Length, null, null);
            buffer.Dispose();
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