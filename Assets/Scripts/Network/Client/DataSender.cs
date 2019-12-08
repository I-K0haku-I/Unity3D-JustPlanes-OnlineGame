using System;

namespace JustPlanes.Network.Client
{

    static class DataSender
    {
        public static void SendHelloServer()
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteInteger((int)ClientPackets.CHelloServer);
            buffer.WriteString("I CONNECTED TO YOU. ALL YOUR BASE ARE BELONG TO US!");
            ClientTCP.SendData(buffer.ToArray());
            buffer.Dispose();
        }

        public static void SendGiveMePlayers()
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteInteger((int)ClientPackets.CGiveMePlayers);
            ClientTCP.SendData(buffer.ToArray());
            buffer.Dispose();
        }

        internal static void SendGiveMeUnits()
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteInteger((int)ClientPackets.CGiveMeUnits);
            ClientTCP.SendData(buffer.ToArray());
            buffer.Dispose();
        }

        public static void SendUnitDamaged(Unit unit, int damage)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteInteger((int)ClientPackets.CUnitDamaged);
            buffer.WriteString(unit.ID);
            buffer.WriteInteger(damage);
            ClientTCP.SendData(buffer.ToArray());
            buffer.Dispose();
        }

        // public static void SendHereIsMyPosition(int x, int y)
        // {
        //     ByteBuffer buffer = new ByteBuffer();
        //     buffer.WriteInteger((int)ClientPackets.CHereIsMyPosition);
        //     buffer.WriteInteger(x);
        //     buffer.WriteInteger(y);
        //     ClientTCP.SendData(buffer.ToArray());
        //     buffer.Dispose();
        // }
    }
}