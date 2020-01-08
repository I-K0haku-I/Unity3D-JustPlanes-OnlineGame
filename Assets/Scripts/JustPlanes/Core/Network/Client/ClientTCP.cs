using System;
using System.Net.Sockets;

namespace JustPlanes.Core.Network.Client
{
    static class ClientTCP
    {
        private static TcpClient clientSocket;
        private static NetworkStream myStream;
        private static byte[] recvBuffer;
        public static string ServerAddress = "127.0.0.1";
        public static int ServerPort = 5569;
        private static IUnityThread unityThread;

        public static bool IsConnected { get { return (clientSocket != null ? clientSocket.Connected : false); } }

        public static void InitializingNetworking(IUnityThread thread)
        {
            unityThread = thread;
            clientSocket = new TcpClient();
            clientSocket.ReceiveBufferSize = 4096;
            clientSocket.SendBufferSize = 4096;
            recvBuffer = new byte[4096 * 2];
            clientSocket.BeginConnect(ServerAddress, ServerPort, new AsyncCallback(ClientConnectCallback), clientSocket);
        }

        private static void ClientConnectCallback(IAsyncResult result)
        {
            clientSocket.EndConnect(result);
            if (clientSocket.Connected == false)
                return;
            else
            {
                clientSocket.NoDelay = true;
                myStream = clientSocket.GetStream();
                myStream.BeginRead(recvBuffer, 0, 4096 * 2, ReceiveCallback, null);
            }
        }

        private static void ReceiveCallback(IAsyncResult result)
        {
            try
            {
                int length = myStream.EndRead(result);
                if (length <= 0)
                    return;

                byte[] newBytes = new byte[length];
                Array.Copy(recvBuffer, newBytes, length);
                unityThread.executeInFixedUpdateClean(() =>
                {
                    ClientHandleData.HandleData(newBytes);
                });
                myStream.BeginRead(recvBuffer, 0, 4096 * 2, ReceiveCallback, null);
            }
            catch (System.Exception)
            {
                return;
            }
        }

        public static void SendData(byte[] data)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteInteger((data.GetUpperBound(0) - data.GetLowerBound(0)) + 1);
            buffer.WriteBytes(data);
            myStream.BeginWrite(buffer.ToArray(), 0, buffer.ToArray().Length, null, null);
            buffer.Dispose();
        }

        public static void Disconnect()
        {
            clientSocket.Close();
        }
    }

    internal interface IUnityThread
    {
        void executeInFixedUpdateClean(Action p);
    }
}