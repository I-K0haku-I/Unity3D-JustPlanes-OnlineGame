using System;
using System.Collections.Generic;

namespace JustPlanes.Core.Network.Client
{
    static class ClientHandleData
    {
        private static ByteBuffer playerBuffer;
        public delegate void Packet(ByteBuffer buffer);
        public static Dictionary<int, Packet> packets = new Dictionary<int, Packet>();
        private static Dictionary<int, Requestor> packetsNew = new Dictionary<int, Requestor>();

        public static void InitializePackets()
        {

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
            DebugLog.Info("[Connection] Receiving Data!");

            buffer.WriteBytes(data);
            int packageType = buffer.ReadInteger();
            int packetID = buffer.ReadInteger();
            int entityId = buffer.ReadInteger();
            // if (packets.TryGetValue(packetID, out Packet packet))
            // {
            //     packet.Invoke(buffer);
            // }
            NetworkMagic.Receive(packageType, "", packetID, entityId, buffer);
            // if (packetsNew.TryGetValue(packetID, out Requestor requestor))
            //     requestor.Receive(buffer);
            buffer.Dispose();
        }
    }

    public interface INetworkManager
    {

    }
}