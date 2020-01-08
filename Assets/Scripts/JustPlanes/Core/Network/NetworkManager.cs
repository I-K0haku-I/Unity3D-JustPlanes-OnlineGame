using System;
using System.Collections.Generic;

namespace JustPlanes.Core.Network
{
    public static class NetworkMagic
    {
        private static Dictionary<int, Requestor> reqs = new Dictionary<int, Requestor>();
        private static Dictionary<(int, int), IHandleCommand> commandHandlers = new Dictionary<(int, int), IHandleCommand>();
        private static Dictionary<(int, int), IHandleCommand> targetHandlers = new Dictionary<(int, int), IHandleCommand>();
        private static Dictionary<(int, int), IHandleCommand> broadcastHandlers = new Dictionary<(int, int), IHandleCommand>();

        public static bool IsServer;
        internal static bool IsClient;
        // private static HashSet<string> registeredActions = new HashSet<string>();
        private static Dictionary<string, object> registeredActions = new Dictionary<string, object>();

        public static bool IsConnected { get { return Client.ClientTCP.IsConnected; } }

        internal static Action<T> GenericRegister<T>(int id, Action<T> action, int entityId, PackageTypes packType, Dictionary<(int, int), IHandleCommand> handleDict) where T : NetworkData
        {
            if (registeredActions.ContainsKey(action.Method.Name))
            {
                DebugLog.Warning($"CONTAINS THIS ACTION ALREADY: {action.Method.Name}");
                return (Action<T>)registeredActions[action.Method.Name];
            }

            var key = (id, entityId);
            if (handleDict.ContainsKey(key))
                throw new Exception($"There already is a command with the id \"{id}, {entityId}\", choose another one");

            NetworkMagicCommand<T> operation = new NetworkMagicCommand<T>(id, packType, entityId, action);
            handleDict.Add(key, (IHandleCommand)operation);
            registeredActions.Add(action.Method.Name, (Action<T>)operation.HandleData);
            return operation.HandleData;
        }

        public static Action<T> RegisterCommand<T>(int id, Action<T> action, int entityId = 1) where T : NetworkData
        {
            return NetworkMagic.GenericRegister<T>(id, action, entityId, PackageTypes.Command, commandHandlers);
        }

        public static Action<T> RegisterTargeted<T>(int id, Action<T> action, int entityId = 1) where T : NetworkData
        {
            return NetworkMagic.GenericRegister<T>(id, action, entityId, PackageTypes.Targeted, targetHandlers);
        }

        public static Action<T> RegisterBroadcasted<T>(int id, Action<T> action, int entityId = 1) where T : NetworkData
        {
            return NetworkMagic.GenericRegister<T>(id, action, entityId, PackageTypes.Broadcast, broadcastHandlers);
        }

        // internal static Action<T> RegisterTargeted<T>(int id, Action<T> action, int entityId = 1) where T : NetworkData
        // {
        //     if (targetHandlers.ContainsKey(id))
        //         throw new Exception($"There already is a targeted RPC with the id \"{id}\", choose another one");

        //     // NetowrkMagicTargetedRPC<T> targetedRPC = new NetowrkMagicTargetedRPC<T>(id, action);
        //     NetworkMagicCommand<T> targetedRPC = new NetworkMagicCommand<T>(id, PackageTypes.Targeted, entityId, action);
        //     targetHandlers.Add(id, (IHandleCommand)targetedRPC);
        //     return targetedRPC.HandleData;
        // }

        // internal static Action<T> RegisterBroadcasted<T>(int id, Action<T> action, int entityId = 1) where T : NetworkData
        // {
        //     if (registeredActions.ContainsKey(action.Method.Name))
        //     {
        //         DebugLog.Warning($"CONTAINS THIS ACTION ALREADY: {action.Method.Name}");
        //         return (Action<T>)registeredActions[action.Method.Name];
        //     }

        //     if (broadcastHandlers.ContainsKey(id))
        //         throw new Exception($"There already is a broadcast RPC with the id \"{id}\", choose another one");

        //     NetworkMagicCommand<T> broadcast = new NetworkMagicCommand<T>(id, PackageTypes.Broadcast, entityId, action);
        //     broadcastHandlers.Add(id, (IHandleCommand)broadcast);
        //     registeredActions.Add(action.Method.Name, (Action<T>)broadcast.HandleData);
        //     return broadcast.HandleData;
        // }

        internal static void Receive(int packageType, string connectionID, int packetID, int entityId, ByteBuffer buffer)
        {
            var key = (packetID, entityId);
            if (packageType == (int)PackageTypes.Command)
            {
                if (commandHandlers.TryGetValue(key, out IHandleCommand command))
                    command.HandleCommand(connectionID, buffer);
                else
                    DebugLog.Warning($"Trying to handle command {packetID}, but it's not registered.");
            }
            else if (packageType == (int)PackageTypes.Targeted)
            {
                if (targetHandlers.TryGetValue(key, out IHandleCommand targetedRPC))
                    targetedRPC.HandleCommand(connectionID, buffer);
                else
                    DebugLog.Warning($"Trying to handle targeted {packetID}, but it's not registered.");
            }
            else if (packageType == (int)PackageTypes.Broadcast)
            {
                if (broadcastHandlers.TryGetValue(key, out IHandleCommand broadcastRPC))
                    broadcastRPC.HandleCommand(connectionID, buffer);
                else
                    DebugLog.Warning($"Trying to handle broadcasted {packetID}, but it's not registered.");
            }
        }
    }

    public class NetworkData
    {
        public string connId;
    }

    internal interface IHandleCommand
    {
        bool HasAction(object action);
        void HandleCommand(string connectionID, ByteBuffer buffer);
        void HandleData(NetworkData obj);
    }

    public enum PackageTypes
    {
        Command,
        Targeted,
        Broadcast
    }

    internal class NetworkMagicCommand<T> : IHandleCommand where T : NetworkData
    {
        private Action<T> action;
        private int packageId;
        private PackageTypes packageType;

        // 1 is default and ignored
        private int entityId;

        public NetworkMagicCommand(int packageId, PackageTypes packageType, int entityId, Action<T> action)
        {
            this.packageId = packageId;
            this.action = action;
            this.packageType = packageType;
            this.entityId = entityId;
        }

        public void HandleData(T data)
        {
            switch (packageType)
            {
                case PackageTypes.Command:
                    HandleDataForCommand(data);
                    break;
                case PackageTypes.Broadcast:
                    HandleDataForBroadcasted(data);
                    break;
                case PackageTypes.Targeted:
                    HandleDataForTargeted(data);
                    break;
            }
        }

        private void HandleDataForBroadcasted(T data)
        {
            if (NetworkMagic.IsServer)
            {
                action.Invoke(data);
                ByteBuffer buffer = buildBuffer(data);
                sendBufferToAll(buffer);
            }
            else if (NetworkMagic.IsClient)
            {
                action.Invoke(data);
            }
        }

        private void HandleDataForTargeted(T data)
        {
            if (NetworkMagic.IsServer)
            {
                ByteBuffer buffer = buildBuffer(data);
                sendBufferToClient(data.connId, buffer);
            }
            else if (NetworkMagic.IsClient)
            {
                action.Invoke(data);
            }
        }

        private void HandleDataForCommand(T data)
        {
            if (NetworkMagic.IsServer)
            {
                action.Invoke(data);
            }
            else if (NetworkMagic.IsClient)
            {
                ByteBuffer buffer = buildBuffer(data);
                sendBufferToServer(buffer);
            }
        }

        private ByteBuffer buildBuffer(T data)
        {
            var buffer = new ByteBuffer();
            buffer.WriteInteger((int)packageType);
            buffer.WriteInteger(packageId);
            buffer.WriteInteger(entityId);
            DebugLog.Info($"[NetworkMagic] Writing a packet of type {packageType.ToString()}:");
            foreach (var field in typeof(T).GetFields())
            {
                if (field.Name == "connId")
                    continue;

                var value = field.GetValue(data);
                if (value is string)
                {
                    DebugLog.Info($"writing a string for {field.Name}: {value.ToString()}");
                    buffer.WriteString((string)value);
                }
                else if (value is bool)
                {
                    DebugLog.Info($"writing a bool for {field.Name}: {value.ToString()}");
                    buffer.WriteBool((bool)value);
                }
                else if (value is int)
                {
                    DebugLog.Info($"writing an int: {value.ToString()}");
                    buffer.WriteInteger((int)value);
                }
                else if (value is List<string>)
                {
                    List<string> someList = (List<string>)value;
                    DebugLog.Info($"writing a list of string: {string.Join(", ", someList)}");
                    buffer.WriteInteger(someList.Count);
                    foreach (var item in someList)
                    {
                        buffer.WriteString(item);
                    }
                }
            }
            DebugLog.Info("Writing---");
            return buffer;
        }

        private void sendBufferToAll(ByteBuffer buffer)
        {
            Server.ClientManager.SendDataToAll(buffer.ToArray());
        }

        private void sendBufferToServer(ByteBuffer buffer)
        {
            Client.ClientTCP.SendData(buffer.ToArray());
        }

        private void sendBufferToClient(string connId, ByteBuffer buffer)
        {
            Server.ClientManager.SendDataTo(connId, buffer.ToArray());
        }

        public void HandleCommand(string connectionID, ByteBuffer buffer)
        {
            T data = (T)Activator.CreateInstance(typeof(T));
            DebugLog.Info($"[NetworkMagic] Reading a packet {packageType.ToString()}:");
            foreach (var field in typeof(T).GetFields())
            {
                if (field.Name == "connId")
                    continue;

                if (field.FieldType == typeof(string))
                {
                    string value = buffer.ReadString();
                    DebugLog.Info($"Decrypted string: {value}");
                    field.SetValue(data, value);
                }
                else if (field.FieldType == typeof(bool))
                {
                    bool value = buffer.ReadBool();
                    DebugLog.Info($"Decrypted bool: {value}");
                    field.SetValue(data, value);
                }
                else if (field.FieldType == typeof(List<string>))
                {
                    List<string> someList = new List<string>();
                    int length = buffer.ReadInteger();
                    for (int i = 0; i < length; i++)
                    {
                        someList.Add(buffer.ReadString());
                    }
                    DebugLog.Info($"Decrypted list of string: {string.Join(", ", someList)}");
                    field.SetValue(data, someList);
                }
            }
            DebugLog.Info("Reading---");
            data.connId = connectionID;
            HandleData(data);
        }

        public bool HasAction(object otherAction)
        {
            return action == otherAction;
        }

        public void HandleData(NetworkData data)
        {
            HandleData((T)data);
        }
    }


    public interface IRequestor
    {
    }

    public interface IRequestorResponse
    {
        bool Ok { get; }
    }

    public class RequestorResponse : IRequestorResponse
    {
        public bool Ok { get; private set; } = false;

        public RequestorResponse(bool v)
        {
            this.Ok = v;
        }

    }

    public abstract class Requestor
    {
        public int ReqId;
        public int RespId;

        public delegate void ResponseAction(RequestorResponse response);

        // this is horrible, but works, if I keep this in it will need some sort of cleaning just in case requests never come back
        public static Dictionary<int, ResponseAction> responseAwaiting = new Dictionary<int, ResponseAction>();

        public virtual void Send()
        {
            using (var buffer = new ByteBuffer())
            {

            }
        }

        public virtual void SendReq(ByteBuffer oldBuffer, ResponseAction responseAction)
        {
            using (var buffer = new ByteBuffer())
            {
                // 1 means it's a request TODO: add an enum instead
                buffer.WriteInteger(ReqId);
                buffer.WriteBytes(oldBuffer.ToArray());
                Client.ClientTCP.SendData(buffer.ToArray());
            }
            responseAwaiting.Add(RespId, responseAction);
        }

        private static int operationNum = 0;
        private static int GetNextOperationNumber()
        {
            operationNum++;
            if (operationNum > 9999)
                operationNum = 0;
            return operationNum;
        }

        internal abstract void ReceiveReq(int connectionID, ByteBuffer buffer);

        internal virtual void SendResp(ByteBuffer oldBuffer)
        {
            using (var buffer = new ByteBuffer())
            {
                buffer.WriteInteger(RespId);
                buffer.WriteBytes(oldBuffer.ToArray());
                Network.Client.ClientTCP.SendData(buffer.ToArray());
            }
        }

        internal abstract void ReceiveResp(ByteBuffer buffer, ResponseAction responseAction);

        internal void Receive(ByteBuffer buffer, int connectionID = 0)
        {
            if (connectionID == 0) // Client
            {
                if (responseAwaiting.TryGetValue(RespId, out ResponseAction responseAction))
                    ReceiveResp(buffer, responseAction);
                else
                    DebugLog.Warning($"THIS SHOULD NOT HAPPEN, IF IT DOES, CONTACT YOUR AUTHORITIES IMMEDIATELY!!!");
            }
            else // Server
                ReceiveReq(connectionID, buffer);
        }
    }

    class LoginRequestor : Requestor
    {
        public new int ReqId = (int)ClientPackets.CLoginReq;
        public new int RespId = (int)ServerPackets.SLoginResp;

        public void SendReq(string name, ResponseAction responseAction = null)
        {
            using (var buffer = new ByteBuffer())
            {
                buffer.WriteString(name);
                base.SendReq(buffer, responseAction);
            }
        }

        internal override void ReceiveReq(int connectionID, ByteBuffer buffer)
        {
            string name = buffer.ReadString();
            // try to login name
            SendResp(true);
        }

        private void SendResp(bool isLoggedIn = false)
        {
            using (var buffer = new ByteBuffer())
            {
                buffer.WriteBool(isLoggedIn);
                base.SendResp(buffer);
            }
        }

        internal override void ReceiveResp(ByteBuffer buffer, ResponseAction responseAction)
        {
            bool isLoggedIn = buffer.ReadBool();
            responseAction.Invoke(new RequestorResponse(true));
        }
    }
}