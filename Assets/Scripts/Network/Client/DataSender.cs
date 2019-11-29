using System;

namespace JustPlanes.Network.Client
{
    public enum ClientPackets
    {
        CHelloServer = 1,
    }

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
    }
}