using System;

namespace JustPlanes.Network.Server
{
    public enum ServerPackets
    {
        SWelcomeMsg = 1,
    }

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
    }
}