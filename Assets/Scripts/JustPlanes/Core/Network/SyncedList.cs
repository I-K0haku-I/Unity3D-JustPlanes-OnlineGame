using System;
using System.Collections.Generic;

namespace JustPlanes.Core.Network
{
    public class SyncedListOfString
    {
        public int EntityId;
        public List<string> ItemList => itemListServer;
        private List<string> itemListServer = new List<string>();

        private Action<NameNetworkData> addItem;
        private Action<NameNetworkData> removeItem;
        private Action<NetworkData> initialize;
        private Action<ListOfStringData> handleInitialize;

        public event Action<string> OnItemAdd;
        public event Action<string> OnItemRemove;

        public SyncedListOfString(int entityId)
        {
            this.EntityId = entityId;

            initialize = NetworkMagic.RegisterAtServer<NetworkData>(0, Initialize_AtServer, entityId);
            handleInitialize = NetworkMagic.RegisterAtClient<ListOfStringData>(0, HandleInitialize_AtClient, entityId);
            addItem = NetworkMagic.RegisterAtAllClients<NameNetworkData>(0, Add_AtAllClients, entityId);
            removeItem = NetworkMagic.RegisterAtAllClients<NameNetworkData>(1, Remove_AtAllClients, entityId);

            if (NetworkMagic.IsClient)
                initialize(new NetworkData());
        }

        private void HandleInitialize_AtClient(ListOfStringData data)
        {
            foreach (var name in data.ItemList)
            {
                itemListServer.Add(name);
                OnItemAdd?.Invoke(name);
            }
            DebugLog.Warning($"[SyncedListOfString-{EntityId}] Initialized, data: {string.Join(", ", itemListServer)}");
        }

        private void Initialize_AtServer(NetworkData data)
        {
            var resp = new ListOfStringData { ConnId = data.ConnId, ItemList = itemListServer };
            handleInitialize(resp);
        }

        public void Add(string item)
        {
            addItem(new NameNetworkData() { Name = item });
        }

        private void Add_AtAllClients(NameNetworkData data)
        {
            // if (!NetworkMagic.IsServer)
            //     return;

            itemListServer.Add(data.Name);
            OnItemAdd?.Invoke(data.Name);
        }

        public void Remove(string item)
        {
            removeItem(new NameNetworkData() { Name = item });
        }

        private void Remove_AtAllClients(NameNetworkData data)
        {
            // if (!NetworkMagic.IsServer)
            //     return;

            if (!itemListServer.Contains(data.Name))
                DebugLog.Warning($"Tried to remove an item from empty list.");


            itemListServer.Remove(data.Name);
            OnItemRemove?.Invoke(data.Name);
        }
    }

}