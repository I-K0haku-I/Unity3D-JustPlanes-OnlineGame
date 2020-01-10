using System;
using System.Linq;
using System.Collections.Generic;

namespace JustPlanes.Core.Network
{
    // TODO: OK, maybe I should not use package id and use combination of action name and entity id

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

        private static Dictionary<(int, string), object> registeredCommands = new Dictionary<(int, string), object>();

        private static void Register<T>(NetworkCommand<T> command) where T : NetworkData
        {
            var handleDict = GetHandlersDict(command.PackageType);
            var key = (command.PackageId, command.EntityId);
            if (handleDict.ContainsKey(key))
                throw new Exception($"There already is a command with the id \"{command.PackageId}, {command.EntityId}\", choose another one");

            handleDict.Add(key, command);
            registeredCommands.Add((command.EntityId, command.Action.Method.Name), command);
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

        public static Action<T> GetHandler<T>(string name, int entityId) where T : NetworkData
        {
            return ((IHandleCommand)registeredCommands[(entityId, name)]).HandleData;
        }

        public static void Receive(int packageType, string connectionId, int packetId, int entityId, ByteBuffer buffer)
        {
            var key = (packetId, entityId);
            var handlersDict = GetHandlersDict((PackageTypes)packageType);

            if (handlersDict.TryGetValue(key, out var command))
            {
                DebugLog.Warning($"[NetworkMagic] Handling command \"{((PackageTypes)packageType).ToString()}\" with key ({packetId}, {entityId}).");
                command.HandleBuffer(connectionId, buffer);
            }
            else
                DebugLog.Warning($"[NetworkMagic] Trying to handle command \"{((PackageTypes)packageType).ToString()}\" with key ({packetId}, {entityId}), but it's not registered.");
        }

        public static Action<T> RegisterAtServer<T>(int packageId, Action<T> action, int entityId) where T : NetworkData
        {
            var actionKey = (entityId, action.Method.Name);
            if (registeredCommands.ContainsKey(actionKey))
            {
                DebugLog.Warning($"CONTAINS THIS ACTION ALREADY: {action.Method.Name}");
                return GetHandler<T>(action.Method.Name, entityId);
            }

            var command = new AtServerCommand<T>(packageId, entityId, action);
            Register(command);
            return command.HandleData;
        }

        public static Action<T> RegisterAtClient<T>(int packageId, Action<T> action, int entityId) where T : NetworkData
        {
            var actionKey = (entityId, action.Method.Name);
            if (registeredCommands.ContainsKey(actionKey))
            {
                DebugLog.Warning($"CONTAINS THIS ACTION ALREADY: {action.Method.Name}");
                return GetHandler<T>(action.Method.Name, entityId);
            }

            var command = new AtClientCommand<T>(packageId, entityId, action);
            Register(command);
            return command.HandleData;
        }

        public static Action<T> RegisterAtAllClients<T>(int packageId, Action<T> action, int entityId) where T : NetworkData
        {
            var actionKey = (entityId, action.Method.Name);
            if (registeredCommands.ContainsKey(actionKey))
            {
                DebugLog.Warning($"CONTAINS THIS ACTION ALREADY: {action.Method.Name}");
                return GetHandler<T>(action.Method.Name, entityId);
            }

            var command = new AtAllClientsCommand<T>(packageId, entityId, action);
            Register(command);
            return command.HandleData;
        }
    }
}