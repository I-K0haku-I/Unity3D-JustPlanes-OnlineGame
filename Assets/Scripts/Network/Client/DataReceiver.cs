using System;
using UnityEngine;

namespace JustPlanes.Network.Client
{
    
    public enum ServerPackets
    {
        SWelcomeMsg = 1,
    }
    static class DataReceiver
    {
        public static void HandleWelcomeMsg(byte[] data)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteBytes(data);
            int packetID = buffer.ReadInteger();
            string msg = buffer.ReadString();
            buffer.Dispose();

            Debug.Log(msg);
            DataSender.SendHelloServer();
        }
    }
}