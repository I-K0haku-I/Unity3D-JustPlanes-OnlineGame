using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;

namespace JustPlanes.Network.Server
{
    public class Client
    {
        public string connectionID;
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
            Console.WriteLine("Incoming packets from '{0}", socket.Client.RemoteEndPoint.ToString());

            GameRunner.Game.clientConnectedQueue.Enqueue(connectionID);
        }

        private void OnRecvData(IAsyncResult result)
        {
            byte[] newBytes = new byte[0];
            try
            {
                int length = stream.EndRead(result);
                if (length <= 0)
                {
                    CloseConnection();
                    return;
                }

                newBytes = new byte[length];
                Array.Copy(recvBuffer, newBytes, length);

                // TODO: this has to be done in the game loop!!!
                ServerHandleData.HandleData(connectionID, newBytes);

                stream.BeginRead(recvBuffer, 0, socket.ReceiveBufferSize, OnRecvData, null);
            }
            catch (System.Exception e)
            {
                Console.WriteLine($"OnRecvData something went extremely wrong at conn id {connectionID.ToString()}");
                Console.WriteLine(e);
                CloseConnection();
                return;
            }
        }

        private void CloseConnection()
        {
            string name = socket.Client.RemoteEndPoint.ToString();
            socket.Close();
            ClientManager.clients.Remove(connectionID);
            // GameRunner.Game.clients.TryRemove(connectionID, out Player p);
            // DataSender.SendPlayerLeft(player);
            Console.WriteLine("Connection from '{0}' has been terminated.", name);
            Console.WriteLine(string.Join(", ", ClientManager.clients.Values.ToList().Select(c => c.connectionID.ToString())));

            GameRunner.Game.clientDisconnectedQueue.Enqueue(connectionID);
        }
    }
}