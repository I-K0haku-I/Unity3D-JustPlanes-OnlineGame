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
            DataSender.SendGiveMeMission();
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

        internal static void HandleUnitsDamaged(ByteBuffer buffer)
        {
            int damageItemAmount = buffer.ReadInteger();
            for (int i = 0; i < damageItemAmount; i++)
                ClientHandleData.Manager.AcknowledgeUnitDamaged(buffer.ReadString(), buffer.ReadInteger());
        }

        internal static void HandleCompleteMission(ByteBuffer buffer)
        {
            ClientHandleData.Manager.AcknowledgeMissionComplete();
        }

        internal static void HandleUpdateMission(ByteBuffer buffer)
        {
            int enemiesKilledDelta = buffer.ReadInteger();
            ClientHandleData.Manager.UpdateMission(enemiesKilledDelta);
        }

        internal static void HandleGiveMission(ByteBuffer buffer)
        {
            // TODO: could add buffer method that reads an integer and returns the enum instance directly
            int type = buffer.ReadInteger();
            int enemiesToKill = buffer.ReadInteger();
            int enemiesKilled = buffer.ReadInteger();
            ClientHandleData.Manager.AddMission((MissionTypes)type, enemiesToKill, enemiesKilled);
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

        public static void HandleUnitsDied(ByteBuffer buffer)
        {
            int unitAmount = buffer.ReadInteger();
            for (int i = 0; i < unitAmount; i++)
                ClientHandleData.Manager.AcknowledgeUnitDied(buffer.ReadString());
        }
    }
}