using System;
using System.Collections.Generic;

namespace JustPlanes.Core.Network
{
    public static class NetworkMagic
    {
        private static Dictionary<(int, int), IHandleCommand> atServerHandlers = new Dictionary<(int, int), IHandleCommand>();
        private static Dictionary<(int, int), IHandleCommand> atClientHandlers = new Dictionary<(int, int), IHandleCommand>();
        private static Dictionary<(int, int), IHandleCommand> atAllClientsHandlers = new Dictionary<(int, int), IHandleCommand>();

        public static bool IsServer;
        internal static bool IsClient;
        // private static HashSet<string> registeredActions = new HashSet<string>();
        private static Dictionary<(int, string), object> registeredActions = new Dictionary<(int, string), object>();

        public static bool IsConnected => Client.ClientTCP.IsConnected;

        private static Action<T> GenericRegister<T>(int packageId, Action<T> action, int entityId, PackageTypes packType, IDictionary<(int, int), IHandleCommand> handleDict) where T : NetworkData
        {
            var actionKey = (entityId, action.Method.Name);
            if (registeredActions.ContainsKey(actionKey))
            {
                DebugLog.Warning($"CONTAINS THIS ACTION ALREADY: {action.Method.Name}");
                return (Action<T>)registeredActions[actionKey];
            }

            var key = (id: packageId, entityId);
            if (handleDict.ContainsKey(key))
                throw new Exception($"There already is a command with the id \"{packageId}, {entityId}\", choose another one");

            var operation = NetworkMagicCommand<T>.Create(packType, packageId, entityId, action);
            handleDict.Add(key, (IHandleCommand)operation);
            registeredActions.Add((entityId, action.Method.Name), (Action<T>)operation.HandleData);
            return operation.HandleData;
        }

        internal static Action<T> GetHandler<T>(string name, int entityId)
        {
            return (Action<T>)registeredActions[(entityId, name)];
        }

        public static Action<T> RegisterAtServer<T>(int id, Action<T> action, int entityId = 1) where T : NetworkData
        {
            return NetworkMagic.GenericRegister<T>(id, action, entityId, PackageTypes.AtServer, atServerHandlers);
        }

        public static Action<T> RegisterOnClient<T>(int id, Action<T> action, int entityId = 1) where T : NetworkData
        {
            return NetworkMagic.GenericRegister<T>(id, action, entityId, PackageTypes.AtClient, atClientHandlers);
        }

        public static Action<T> RegisterAtAllClients<T>(int id, Action<T> action, int entityId = 1) where T : NetworkData
        {
            return NetworkMagic.GenericRegister<T>(id, action, entityId, PackageTypes.AtAllClients, atAllClientsHandlers);
        }

        internal static void Receive(int packageType, string connectionId, int packetId, int entityId, ByteBuffer buffer)
        {
            var key = (packetID: packetId, entityId);
            if (packageType == (int)PackageTypes.AtServer)
            {
                if (atServerHandlers.TryGetValue(key, out var command))
                    command.HandleCommand(connectionId, buffer);
                else
                    DebugLog.Warning($"Trying to handle command {packetId}, but it's not registered.");
            }
            else if (packageType == (int)PackageTypes.AtClient)
            {
                if (atClientHandlers.TryGetValue(key, out var command))
                    command.HandleCommand(connectionId, buffer);
                else
                    DebugLog.Warning($"Trying to handle targeted {packetId}, but it's not registered.");
            }
            else if (packageType == (int)PackageTypes.AtAllClients)
            {
                if (atAllClientsHandlers.TryGetValue(key, out var command))
                    command.HandleCommand(connectionId, buffer);
                else
                    DebugLog.Warning($"Trying to handle broadcasted {packetId}, but it's not registered.");
            }
        }
    }

    public class NetworkData
    {
        public string ConnId;
    }

    public interface IHandleCommand
    {
        bool HasAction(object action);
        void HandleCommand(string connectionId, ByteBuffer buffer);
        void HandleData(NetworkData obj);
    }

    public enum PackageTypes
    {
        AtServer,
        AtClient,
        AtAllClients
    }

    public class AtServerCommand<T> : NetworkMagicCommand<T> where T : NetworkData
    {
        public AtServerCommand(int packageId, int entityId, Action<T> action) : base(packageId, PackageTypes.AtServer, entityId, action)
        {

        }

        public override void HandleData(T data)
        {
            if (NetworkMagic.IsServer)
            {
                action.Invoke(data);
            }
            else if (NetworkMagic.IsClient)
            {
                var buffer = BuildBuffer(data);
                SendBufferToServer(buffer);
            }
        }
    }

    public abstract class NetworkMagicCommand<T> : IHandleCommand where T : NetworkData
    {
        protected readonly Action<T> action;
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

        public abstract void HandleData(T data);

        private void HandleDataForAtAllClients(T data)
        {
            if (NetworkMagic.IsServer)
            {
                action.Invoke(data);
                var buffer = BuildBuffer(data);
                sendBufferToAll(buffer);
            }
            else if (NetworkMagic.IsClient)
            {
                action.Invoke(data);
            }
        }

        private void HandleDataForAtClient(T data)
        {
            if (NetworkMagic.IsServer)
            {
                var buffer = BuildBuffer(data);
                sendBufferToClient(data.ConnId, buffer);
            }
            else if (NetworkMagic.IsClient)
            {
                action.Invoke(data);
            }
        }

        protected ByteBuffer BuildBuffer(T data)
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
                switch (value)
                {
                    case string s:
                        DebugLog.Info($"writing a string for {field.Name}: {s.ToString()}");
                        buffer.WriteString(s);
                        break;
                    case bool b:
                        DebugLog.Info($"writing a bool for {field.Name}: {b.ToString()}");
                        buffer.WriteBool(b);
                        break;
                    case int i:
                        DebugLog.Info($"writing an int: {i.ToString()}");
                        buffer.WriteInteger(i);
                        break;
                    case List<string> list:
                    {
                        DebugLog.Info($"writing a list of string: {string.Join(", ", list)}");
                        buffer.WriteInteger(list.Count);
                        foreach (var item in list)
                        {
                            buffer.WriteString(item);
                        }

                        break;
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

        protected void SendBufferToServer(ByteBuffer buffer)
        {
            Client.ClientTCP.SendData(buffer.ToArray());
        }

        private void sendBufferToClient(string connId, ByteBuffer buffer)
        {
            Server.ClientManager.SendDataTo(connId, buffer.ToArray());
        }

        public void HandleCommand(string connectionId, ByteBuffer buffer)
        {
            var data = (T)Activator.CreateInstance(typeof(T));
            DebugLog.Info($"[NetworkMagic] Reading a packet {packageType.ToString()}:");
            foreach (var field in typeof(T).GetFields())
            {
                if (field.Name == "connId")
                    continue;

                if (field.FieldType == typeof(string))
                {
                    var value = buffer.ReadString();
                    DebugLog.Info($"Decrypted string: {value}");
                    field.SetValue(data, value);
                }
                else if (field.FieldType == typeof(bool))
                {
                    var value = buffer.ReadBool();
                    DebugLog.Info($"Decrypted bool: {value}");
                    field.SetValue(data, value);
                }
                else if (field.FieldType == typeof(List<string>))
                {
                    var someList = new List<string>();
                    var length = buffer.ReadInteger();
                    for (var i = 0; i < length; i++)
                    {
                        someList.Add(buffer.ReadString());
                    }
                    DebugLog.Info($"Decrypted list of string: {string.Join(", ", someList)}");
                    field.SetValue(data, someList);
                }
            }
            DebugLog.Info("Reading---");
            data.ConnId = connectionId;
            HandleData(data);
        }

        public bool HasAction(object otherAction)
        {
            return action == (Action<T>) otherAction;
        }

        public void HandleData(NetworkData data)
        {
            HandleData((T)data);
        }

        public static IHandleCommand Create(PackageTypes packType, int packageId, int entityId, Action<T> action1)
        {
            switch (packType)
            {
                case PackageTypes.AtServer:
                    return new AtServerCommand<T>(packageId, entityId, action1);
                case PackageTypes.AtClient:
                    return new AtServerCommand<T>(packageId, entityId, action1);
                case PackageTypes.AtAllClients:
                    return new AtServerCommand<T>(packageId, entityId, action1);
                default:
                    return null;
            }
        }
    }
}