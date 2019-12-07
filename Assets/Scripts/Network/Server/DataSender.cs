using System;
using System.Collections.Generic;

namespace JustPlanes.Network.Server
{
    static class DataSender
    {
        public static void SendWelcomeMessage(int connectionID)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteInteger((int)ServerPackets.SWelcomeMsg);
            buffer.WriteString("WELCOME TO THIS STUPID SERVER");
            // TODO: this is fine but feels like you could do this differently and somehow not use ByteBuffer twice in the process 
            ClientManager.SendDataTo(connectionID, buffer.ToArray());
            buffer.Dispose();
        }

        public static void SendGivePlayers(int connectionID)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteInteger((int)ServerPackets.SGivePlayers);

            List<Player> players = ClientManager.GetPlayers();
            buffer.WriteInteger(players.Count);

            foreach (var p in players)
                buffer.WritePlayer(p);

            ClientManager.SendDataTo(connectionID, buffer.ToArray());
            buffer.Dispose();
        }

        public static void SendGiveUnits(int connectionID)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteInteger((int)ServerPackets.SGiveUnits);

            List<Unit> units = Game.GetUnits();
            buffer.WriteInteger(units.Count);

            foreach (var u in units)
                buffer.WriteUnit(u);
            
            ClientManager.SendDataTo(connectionID, buffer.ToArray());
            buffer.Dispose();
        }

        public static void SendPlayerJoined(Player player)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteInteger((int)ServerPackets.SPlayerJoined);
            buffer.WritePlayer(player);

            ClientManager.SendDataToAll(buffer.ToArray());
            buffer.Dispose();
        }

        internal static void SendUnitSpawned(Unit unit)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteInteger((int)ServerPackets.SUnitSpawned);
            buffer.WriteUnit(unit);

            ClientManager.SendDataToAll(buffer.ToArray());
            buffer.Dispose();
        }
    }
}