using System;

namespace JustPlanes.Network.Server
{

    public class DataReceiver
    {
        public static void HandleHelloServer(int connectionID, ByteBuffer buffer)
        {
            // ByteBuffer buffer = new ByteBuffer();
            // buffer.WriteBytes(data);
            // int packetID = buffer.ReadInteger();
            string msg = buffer.ReadString();
            // buffer.Dispose();
            // Console.WriteLine(msg);
            GameRunner.Game.msgQueue.Enqueue(msg);
        }

        public static void HandleGiveMePlayers(string connectionID, ByteBuffer buffer)
        {
            // ByteBuffer buffer = new ByteBuffer();
            // buffer.WriteBytes(data);
            string msg = $"{connectionID} wants to know all the players.";
            GameRunner.Game.msgQueue.Enqueue(msg);
            DataSender.SendGivePlayers(connectionID);
        }

        internal static void HandleUnitDamaged(int connectionID, ByteBuffer buffer)
        {
            string id = buffer.ReadString();
            int damage = buffer.ReadInteger();
            // Console.WriteLine($"{id} got damaged {damage.ToString()} from {connectionID}");
            GameRunner.Game.damageQueue.Enqueue(Tuple.Create(id, damage));
        }

        public static void HandleGiveMeUnits(string connectionID, ByteBuffer buffer)
        {
            string msg = $"{connectionID} wants to know all the units.";
            GameRunner.Game.msgQueue.Enqueue(msg);
            DataSender.SendGiveUnits(connectionID);
        }

        public static void HandleGiveMeMission(string connectionID, ByteBuffer buffer)
        {
            string msg = $"{connectionID} wants to know the mission.";
            // TOOD: opportunity to do a method here like GameRunner.game.print(msg);
            GameRunner.Game.msgQueue.Enqueue(msg);
            DataSender.SendGiveMission(connectionID, GameRunner.Game.mission);
        }
    }
}