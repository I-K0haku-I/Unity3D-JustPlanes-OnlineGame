using System;
using System.Collections.Generic;

namespace JustPlanes.Network.Client
{
    static class ClientHandleData
    {
        private static ByteBuffer playerBuffer;
        public delegate void Packet(byte[] data);
        public static Dictionary<int, Packet> packets = new Dictionary<int, Packet>();

        public static void InitializePackets()
        {
            packets.Add((int)ServerPackets.SWelcomeMsg, DataReceiver.HandleWelcomeMsg);
        }

        public static void HandleData(byte[] data)
        {
            byte[] incomingBuffer = (byte[])data.Clone();
            int packetLength = 0;

            if (playerBuffer == null)
                playerBuffer = new ByteBuffer();

            playerBuffer.WriteBytes(incomingBuffer);
            if (playerBuffer.Count() == 0)
            {
                playerBuffer.Clear();
                return;
            }

            if (playerBuffer.Length() >= 4)
            {
                packetLength = playerBuffer.ReadInteger(false);
                if (packetLength <= 0) // TODO: wtf, how is this check even possible? I mean why is it used like this?
                {
                    playerBuffer.Clear();
                    return;
                }
            }

            // TODO: understand this better, very confusing code style imo...

            while (packetLength > 0 & packetLength <= playerBuffer.Length() - 4)
            {
                if (packetLength <= playerBuffer.Length() - 4)
                {
                    playerBuffer.ReadInteger();
                    data = playerBuffer.ReadBytes(packetLength);
                    HandleDataPackets(data);
                }

                packetLength = 0;
                if (playerBuffer.Length() >= 4)
                {
                    packetLength = playerBuffer.ReadInteger(false);
                    if (packetLength <= 0)
                    {
                        playerBuffer.Clear();
                        return;
                    }
                }
            }

            if (packetLength <= 1)
            {
                playerBuffer.Clear();
            }

        }

        private static void HandleDataPackets(byte[] data)
        {
            ByteBuffer buffer = new ByteBuffer();

            // Debug.Log(string.Join(" ", data));

            buffer.WriteBytes(data);
            int packetID = buffer.ReadInteger();
            buffer.Dispose();
            if (packets.TryGetValue(packetID, out Packet packet))
            {
                packet.Invoke(data);
            }
        }
    }
}