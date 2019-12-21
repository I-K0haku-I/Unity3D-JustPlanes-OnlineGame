using System;
using System.Linq;
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

        internal static void SendPlayerLeft(Player player)
        {
            using (var buffer = new ByteBuffer())
            {
                buffer.WriteInteger((int)ServerPackets.SPlayerLeft);
                buffer.WritePlayer(player);
                ClientManager.SendDataToAll(buffer.ToArray());
            }
        }

        internal static void SendUnitDied(Unit unit)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteInteger((int)ServerPackets.SUnitDied);
            buffer.WriteString(unit.ID);

            ClientManager.SendDataToAll(buffer.ToArray());
            buffer.Dispose();
        }

        internal static void SendUnitsDamage(List<Tuple<string, int>> damageToSend)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteInteger((int)ServerPackets.SUnitsDamaged);

            buffer.WriteInteger(damageToSend.Count);
            damageToSend.ForEach(damageItem => {
                buffer.WriteString(damageItem.Item1);
                buffer.WriteInteger(damageItem.Item2);
            });

            ClientManager.SendDataToAll(buffer.ToArray());
            buffer.Dispose();
        }

        internal static void SendUnitsDied(List<Unit> unitDeathToSend)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteInteger((int)ServerPackets.SUnitsDied);

            buffer.WriteInteger(unitDeathToSend.Count);
            unitDeathToSend.ForEach(unit => {
                buffer.WriteString(unit.ID);
            });

            ClientManager.SendDataToAll(buffer.ToArray());
            buffer.Dispose();
        }

        internal static void SendMissionComplete()
        {
            using (var buffer = new ByteBuffer())
            {
                buffer.WriteInteger((int)ServerPackets.SCompleteMission);
                ClientManager.SendDataToAll(buffer.ToArray());
            }
        }

        internal static void SendGiveMission(MissionHandler mission)
        {
            using (var buffer = new ByteBuffer())
            {
                buffer.WriteInteger((int)ServerPackets.SGiveMission);
                buffer.WriteInteger((int)MissionTypes.MTKILLRats);
                buffer.WriteInteger(mission.enemiesToKill);
                buffer.WriteInteger(mission.enemiesKilled);
                ClientManager.SendDataToAll(buffer.ToArray());
            }
        }

        internal static void SendGiveMission(int connectionID, MissionHandler mission)
        {
            using (var buffer = new ByteBuffer())
            {
                buffer.WriteInteger((int)ServerPackets.SGiveMission);
                buffer.WriteInteger((int)MissionTypes.MTKILLRats);
                buffer.WriteInteger(mission.enemiesToKill);
                buffer.WriteInteger(mission.enemiesKilled);
                ClientManager.SendDataTo(connectionID, buffer.ToArray());
            }
        }

        internal static void SendMissionUpdate(List<int> missionToSend)
        {
            using (var buffer = new ByteBuffer())
            {
                buffer.WriteInteger((int)ServerPackets.SUpdateMission);
                buffer.WriteInteger(missionToSend.Sum());
                ClientManager.SendDataToAll(buffer.ToArray());
            }
        }
    }
}