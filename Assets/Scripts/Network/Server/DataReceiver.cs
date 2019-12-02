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
            Console.WriteLine(msg);
        }

        public static void HandleGiveMePlayers(int connectionID, ByteBuffer buffer)
        {
            // ByteBuffer buffer = new ByteBuffer();
            // buffer.WriteBytes(data);
            Console.WriteLine($"{connectionID} wants to know all the players.");
            DataSender.SendGivePlayers(connectionID);
        }
    }
}