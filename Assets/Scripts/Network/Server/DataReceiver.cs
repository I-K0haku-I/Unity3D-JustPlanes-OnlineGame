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
            Game.msgQueue.Enqueue(msg);
        }

        public static void HandleGiveMePlayers(int connectionID, ByteBuffer buffer)
        {
            // ByteBuffer buffer = new ByteBuffer();
            // buffer.WriteBytes(data);
            string msg = $"{connectionID} wants to know all the players.";
            Game.msgQueue.Enqueue(msg);
            DataSender.SendGivePlayers(connectionID);
        }

        internal static void HandleUnitDamaged(int connectionID, ByteBuffer buffer)
        {
            string id = buffer.ReadString();
            int damage = buffer.ReadInteger();
            // Console.WriteLine($"{id} got damaged {damage.ToString()} from {connectionID}");
            Game.damageQueue.Enqueue(Tuple.Create(id, damage));
        }

        public static void HandleGiveMeUnits(int connectionID, ByteBuffer buffer)
        {
            string msg = $"{connectionID} wants to know all the units.";
            Game.msgQueue.Enqueue(msg);
            DataSender.SendGiveUnits(connectionID);
        }

        public static void HandleGiveMeMission(int connectionID, ByteBuffer buffer)
        {
            string msg = $"{connectionID} wants to know the mission.";
            // TOOD: opportunity to do a method here like Game.print(msg);
            Game.msgQueue.Enqueue(msg);
            DataSender.SendGiveMission(connectionID, Game.mission);
        }
    }
}