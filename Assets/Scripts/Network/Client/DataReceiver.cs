using System;

namespace JustPlanes.Network.Client
{

    static class DataReceiver
    {
        public static void HandleWelcomeMsg(ByteBuffer buffer)
        {
            // ByteBuffer buffer = new ByteBuffer();
            // buffer.WriteBytes(data);
            string msg = buffer.ReadString();

            ClientHandleData.Manager.ReceivedMsg(msg);
            DataSender.SendHelloServer();
            DataSender.SendGiveMePlayers();
            DataSender.SendGiveMeUnits();
        }

        public static void HandleGivePlayers(ByteBuffer buffer)
        {
            // ByteBuffer buffer = new ByteBuffer();
            // buffer.WriteBytes(data);
            // int packetID = buffer.ReadInteger();

            int playerAmount = buffer.ReadInteger();
            for (int i = 0; i < playerAmount; i++)
                ClientHandleData.Manager.AddPlayer(buffer.ReadPlayer());

        }

        public static void HandleGiveUnits(ByteBuffer buffer)
        {
            int playerAmount = buffer.ReadInteger();
            for (int i = 0; i < playerAmount; i++)
                ClientHandleData.Manager.AddUnit(buffer.ReadUnit());
        }

        internal static void HandleUnitDied(ByteBuffer buffer)
        {
            string id = buffer.ReadString();
            ClientHandleData.Manager.AcknowledgeUnitDied(id);
        }

        public static void HandlePlayerJoined(ByteBuffer buffer)
        {
            // string name = buffer.ReadString();
            // int x = buffer.ReadInteger();
            // int y = buffer.ReadInteger();
            Player player = buffer.ReadPlayer();
            ClientHandleData.Manager.AddPlayer(player);
        }

        public static void HandleUnitSpawned(ByteBuffer buffer)
        {
            Unit unit = buffer.ReadUnit();
            ClientHandleData.Manager.AddUnit(unit);
        }
    }
}