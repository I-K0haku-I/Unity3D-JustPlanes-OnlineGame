using System;
using System.Collections.Generic;
using System.Linq;
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
        public Player player;

        public void Start()
        {
            // TODO: this needs to be gone
            // maybe do an event in here and subscribe with game
            Game.players.TryAdd(connectionID, player);
            DataSender.SendPlayerJoined(player);
    
            socket.SendBufferSize = 4096;
            socket.ReceiveBufferSize = 4096;
            stream = socket.GetStream();
            recvBuffer = new byte[4096 * 2];
            stream.BeginRead(recvBuffer, 0, recvBuffer.Length, OnRecvData, null);
            Console.WriteLine("Incoming packets from '{0}", socket.Client.RemoteEndPoint.ToString());
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

                ServerHandleData.HandleData(connectionID, newBytes);

                stream.BeginRead(recvBuffer, 0, socket.ReceiveBufferSize, OnRecvData, null);
            }
            catch (System.Exception e)
            {
                Console.WriteLine($"OnRecvData something went extremely wrong!: {newBytes.ToString()} from {connectionID.ToString()}");
                Console.WriteLine(e);
                CloseConnection();
                return;
            }
        }

        private void CloseConnection()
        {
            socket.Close();
            ClientManager.clients.Remove(connectionID);
            Game.players.TryRemove(connectionID, out Player p);
            DataSender.SendPlayerLeft(player);
            Console.WriteLine("Connection from '{0}' has been terminated.", socket.Client.RemoteEndPoint.ToString());
            Console.WriteLine(string.Join(", ", ClientManager.clients.Values.ToList().Select(c => c.connectionID.ToString())));
        }
    }
}