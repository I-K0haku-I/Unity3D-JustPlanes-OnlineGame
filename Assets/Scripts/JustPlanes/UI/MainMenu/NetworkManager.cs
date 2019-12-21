using System;
using System.Collections.Generic;

namespace JustPlanes.Network
{
    public class NetworkMagic
    {
        private Dictionary<int, Requestor> reqs = new Dictionary<int, Requestor>();

        public NetworkMagic()
        {
            Requestor toSave = new LoginRequestor();
            reqs.Add(toSave.ReqId, toSave);
        }

        public void Request(ClientPackets packet, Action<ByteBuffer> buffer)
        {

        }


        public void Subscribe(ServerPackets packets)
        {

        }
    }

    public interface IRequestor
    {
    }

    public abstract class Requestor
    {
        public int ReqId;
        public int RespId;

        public virtual void Send(ByteBuffer buffer)
        {
            Network.Client.ClientTCP.SendData(buffer.ToArray());
        }
    }

    class LoginRequestor : Requestor
    {
        public new int ReqId = (int)ClientPackets.CLoginReq;
        public new int RespId = (int)ServerPackets.SLoginResp;

        public void Send(string name)
        {
            using (var buffer = new ByteBuffer())
            {
                buffer.WriteInteger(ReqId);
                base.Send(buffer);
            }
        }
    }
}