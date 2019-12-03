using System;
using System.Linq;
using System.Collections.Generic;

namespace JustPlanes.Network.Server
{
    static class ServerHandleData
    {
        public delegate void Packet(int connectionID, ByteBuffer data);
        public static Dictionary<int, Packet> packets = new Dictionary<int, Packet>();

        public static void InitializePackets()
        {
            packets.Add((int)ClientPackets.CHelloServer, DataReceiver.HandleHelloServer);
            packets.Add((int)ClientPackets.CGiveMePlayers, DataReceiver.HandleGiveMePlayers);
        }

        public static void HandleData(int connectionID, byte[] data)
        {
            byte[] incomingBuffer = (byte[])data.Clone();
            int packetLength = 0;

            Client client = ClientManager.clients[connectionID];
            if (client.buffer == null)
                client.buffer = new ByteBuffer();

            client.buffer.WriteBytes(incomingBuffer);
            if (client.buffer.Count() == 0)
            {
                client.buffer.Clear();
                return;
            }

            if (client.buffer.Length() >= 4)
            {
                packetLength = client.buffer.ReadInteger(false);
                if (packetLength <= 0) // TODO: wtf, how is this check even possible? I mean why is it used like this?
                {
                    client.buffer.Clear();
                    return;
                }
            }

            // TODO: understand this better, very confusing code style imo...

            while (packetLength > 0 & packetLength <= client.buffer.Length() - 4)
            {
                if (packetLength <= client.buffer.Length() - 4)
                {
                    client.buffer.ReadInteger();
                    data = client.buffer.ReadBytes(packetLength);
                    HandleDataPackets(connectionID, data);
                }

                packetLength = 0;
                if (client.buffer.Length() >= 4)
                {
                    packetLength = client.buffer.ReadInteger(false);
                    if (packetLength <= 0)
                    {
                        client.buffer.Clear();
                        return;
                    }
                }
            }

            if (packetLength <= 1)
            {
                client.buffer.Clear();
            }

        }

        private static void HandleDataPackets(int connectionID, byte[] data)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteBytes(data);
            // Console.WriteLine(string.Join(",", data.ToList().Select(b => b.ToString())));
            int packetID = buffer.ReadInteger();
            if (packets.TryGetValue(packetID, out Packet packet))
            {
                packet.Invoke(connectionID, buffer);
            }
            buffer.Dispose();
        }
    }
}