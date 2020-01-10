using System;
using System.Linq;
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
        public static bool IsConnected => Client.ClientTCP.IsConnected;

        private static Dictionary<(int, string), object> registeredActions = new Dictionary<(int, string), object>();

        public static Action<T> Register<T>(NetworkCommand<T> command) where T : NetworkData
        {
            var actionKey = (command.EntityId, command.Action.Method.Name);
            if (registeredActions.ContainsKey(actionKey))
            {
                DebugLog.Warning($"CONTAINS THIS ACTION ALREADY: {command.Action.Method.Name}");
                return (Action<T>)registeredActions[actionKey];
            }

            var handleDict = GetHandlersDict(command.PackageType);
            var key = (command.PackageId, command.EntityId);
            if (handleDict.ContainsKey(key))
                throw new Exception($"There already is a command with the id \"{command.PackageId}, {command.EntityId}\", choose another one");

            handleDict.Add(key, (IHandleCommand)command);
            registeredActions.Add((command.EntityId, command.Action.Method.Name), (Action<T>)command.HandleData);
            return command.HandleData;
        }

        private static IDictionary<(int, int), IHandleCommand> GetHandlersDict(PackageTypes packageType)
        {
            switch (packageType)
            {
                case PackageTypes.AtAllClients:
                    return atAllClientsHandlers;
                case PackageTypes.AtClient:
                    return atClientHandlers;
                case PackageTypes.AtServer:
                    return atServerHandlers;
                default:
                    return null;
            }
        }

        public static Action<T> GetHandler<T>(string name, int entityId)
        {
            return (Action<T>)registeredActions[(entityId, name)];
        }

        public static void Receive(int packageType, string connectionId, int packetId, int entityId, ByteBuffer buffer)
        {
            var key = (packetId, entityId);
            var handlersDict = GetHandlersDict((PackageTypes)packageType);

            if (handlersDict.TryGetValue(key, out var command))
            {
                DebugLog.Warning($"[NetworkMagic] Trying to handle command \"{((PackageTypes)packageType).ToString()}\" with key ({packetId}, {entityId}).");
                command.HandleBuffer(connectionId, buffer);
            }
            else
                DebugLog.Warning($"[NetworkMagic] Trying to handle command \"{((PackageTypes)packageType).ToString()}\" with key ({packetId}, {entityId}), but it's not registered.");
        }
    }

    public class NetworkData
    {
        public string ConnId;
    }

    public interface IHandleCommand
    {
        void HandleBuffer(string connectionId, ByteBuffer buffer);
        void HandleData(NetworkData obj);
    }

    public enum PackageTypes
    {
        AtServer,
        AtClient,
        AtAllClients
    }

    public class AtServerCommand<T> : NetworkCommand<T> where T : NetworkData
    {
        public AtServerCommand(int packageId, int entityId, Action<T> action) : base(packageId, PackageTypes.AtServer, entityId, action) { }

        public override void HandleData(T data)
        {
            if (NetworkMagic.IsServer)
            {
                Action.Invoke(data);
            }
            else if (NetworkMagic.IsClient)
            {
                var buffer = BuildBuffer(data);
                SendBufferToServer(buffer);
            }
        }
    }

    public class AtClientCommand<T> : NetworkCommand<T> where T : NetworkData
    {
        public AtClientCommand(int packageId, int entityId, Action<T> action) : base(packageId, PackageTypes.AtClient, entityId, action) { }

        public override void HandleData(T data)
        {
            if (NetworkMagic.IsServer)
            {
                var buffer = BuildBuffer(data);
                SendBufferToClient(data.ConnId, buffer);
            }
            else if (NetworkMagic.IsClient)
            {
                Action.Invoke(data);
            }
        }
    }

    public class AtAllClientsCommand<T> : NetworkCommand<T> where T : NetworkData
    {
        public AtAllClientsCommand(int packageId, int entityId, Action<T> action) : base(packageId, PackageTypes.AtAllClients, entityId, action) { }

        public override void HandleData(T data)
        {
            if (NetworkMagic.IsServer)
            {
                Action.Invoke(data);
                var buffer = BuildBuffer(data);
                SendBufferToAll(buffer);
            }
            else if (NetworkMagic.IsClient)
            {
                Action.Invoke(data);
            }
        }
    }

    public abstract class NetworkCommand<T> : IHandleCommand where T : NetworkData
    {
        public Action<T> Action;
        public int PackageId;
        public PackageTypes PackageType;

        // 1 is default and ignored
        public int EntityId;

        public NetworkCommand(int packageId, PackageTypes packageType, int entityId, Action<T> action)
        {
            this.PackageId = packageId;
            this.Action = action;
            this.PackageType = packageType;
            this.EntityId = entityId;
            NetworkMagic.Register<T>(this);
        }

        public abstract void HandleData(T data);

        protected ByteBuffer BuildBuffer(T data)
        {
            var buffer = new ByteBuffer();
            buffer.WriteInteger((int)PackageType);
            buffer.WriteInteger(PackageId);
            buffer.WriteInteger(EntityId);
            DebugLog.Info($"[NetworkMagic] Writing a packet of type {PackageType.ToString()}:");
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

        protected void SendBufferToAll(ByteBuffer buffer)
        {
            Server.ClientManager.SendDataToAll(buffer.ToArray());
        }

        protected void SendBufferToServer(ByteBuffer buffer)
        {
            Client.ClientTCP.SendData(buffer.ToArray());
        }

        protected void SendBufferToClient(string connId, ByteBuffer buffer)
        {
            Server.ClientManager.SendDataTo(connId, buffer.ToArray());
        }

        public void HandleBuffer(string connectionId, ByteBuffer buffer)
        {
            var data = (T)Activator.CreateInstance(typeof(T));
            DebugLog.Info($"[NetworkMagic] Reading a packet {PackageType.ToString()} with data type {typeof(T).Name}:");
            foreach (var field in typeof(T).GetFields())
            {
                if (field.Name == nameof(NetworkData.ConnId))
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

        public static Action<T> CreateAtServer(int packageId, Action<T> action, int entityId)
        {
            return new AtServerCommand<T>(packageId, entityId, action).HandleData;
        }

        public static Action<T> CreateAtClient(int packageId, Action<T> action, int entityId)
        {
            return new AtClientCommand<T>(packageId, entityId, action).HandleData;
        }

        public static Action<T> CreateAtAllClients(int packageId, Action<T> action, int entityId)
        {
            return new AtAllClientsCommand<T>(packageId, entityId, action).HandleData;
        }

        public void HandleData(NetworkData data)
        {
            HandleData((T)data);
        }
    }
}