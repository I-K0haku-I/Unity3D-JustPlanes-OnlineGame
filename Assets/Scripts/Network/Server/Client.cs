using System;
using System.Net.Sockets;

namespace JustPlanes.Network.Server
{
    public class Client
    {
        public int connectionID;
        public TcpClient socket;
        public NetworkStream stream;
        private byte[] recvBuffer;
        public ByteBuffer buffer;

        public void Start()
        {
            socket.SendBufferSize = 4096;
            socket.ReceiveBufferSize = 4096;
            stream = socket.GetStream();
            recvBuffer = new byte[4096 * 2];
            stream.BeginRead(recvBuffer, 0, recvBuffer.Length, OnRecvData, null);
            Console.WriteLine("Incoming connection from '{0}", socket.Client.RemoteEndPoint.ToString());
        }

        private void OnRecvData(IAsyncResult result)
        {
            try
            {
                int length = stream.EndRead(result);
                if (length <= 0)
                {
                    CloseConnection();
                    return;
                }

                byte[] newBytes = new byte[length];
                Array.Copy(recvBuffer, newBytes, length);

                ServerHandleData.HandleData(connectionID, newBytes);

                stream.BeginRead(recvBuffer, 0, socket.ReceiveBufferSize, OnRecvData, null);
            }
            catch (System.Exception)
            {
                CloseConnection();
                return;
            }
        }

        private void CloseConnection()
        {
            Console.WriteLine("Connection from '{0}' has been terminated.", socket.Client.RemoteEndPoint.ToString());
            socket.Close();
        }
    }
}