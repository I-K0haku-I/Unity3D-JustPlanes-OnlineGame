using System;
using System.Linq;
using System.Collections.Generic;

namespace JustPlanes.Core.Network
{
    public interface IHandleCommand
    {
        void HandleBuffer(string connectionId, ByteBuffer buffer);
        void HandleData(NetworkData obj);
    }

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
}