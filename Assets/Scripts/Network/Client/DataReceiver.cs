using System;
using UnityEngine;

namespace JustPlanes.Network.Client
{

    static class DataReceiver
    {
        public static void HandleWelcomeMsg(ByteBuffer buffer)
        {
            // ByteBuffer buffer = new ByteBuffer();
            // buffer.WriteBytes(data);
            string msg = buffer.ReadString();

            Debug.Log(msg);
            DataSender.SendHelloServer();
            DataSender.SendGiveMePlayers();
        }

        public static void HandleGivePlayers(ByteBuffer buffer)
        {
            // ByteBuffer buffer = new ByteBuffer();
            // buffer.WriteBytes(data);
            // int packetID = buffer.ReadInteger();

            string name;
            int x; int y;

            int playerAmount = buffer.ReadInteger();
            for (int i = 0; i < playerAmount; i++)
            {
                name = buffer.ReadString();
                x = buffer.ReadInteger();
                y = buffer.ReadInteger();
                ClientHandleData.Manager.AddPlayer(new Player(name, x, y));
            }

        }

        internal static void HandlePlayerJoined(ByteBuffer buffer)
        {
            string name = buffer.ReadString();
            int x = buffer.ReadInteger();
            int y = buffer.ReadInteger();
            ClientHandleData.Manager.AddPlayer(new Player(name, x, y));
        }
    }
}