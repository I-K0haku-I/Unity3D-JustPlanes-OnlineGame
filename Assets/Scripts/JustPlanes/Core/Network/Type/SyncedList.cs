using JustPlanes.Core.Network;
using System;
using System.Collections;
using System.Collections.Generic;

namespace JustPlanes.Core
{
    public class SyncedList<T> : ICollection<T>
    {
        public int EntityId;
        public List<T> ItemList { get; } = new List<T>();

        public int Count => ((ICollection<T>)ItemList).Count;
        public bool IsReadOnly => ((ICollection<T>)ItemList).IsReadOnly;

        private readonly Action<SingleData<T>> addItem;
        private readonly Action<SingleData<T>> removeItem;
        private readonly Action<NetworkData> initialize;
        private readonly Action<ListData<T>> handleInitialize;

        public event Action<T> OnItemAdd;
        public event Action<T> OnItemRemove;

        public SyncedList(int entityId)
        {
            EntityId = entityId;

            initialize = NetworkMagic.RegisterAtServer<NetworkData>(0, CmdInitialize, entityId);
            handleInitialize = NetworkMagic.RegisterAtClient<ListData<T>>(0, TargetedHandleInitialize, entityId);
            addItem = NetworkMagic.RegisterAtAllClients<SingleData<T>>(0, BroadcastAdd, entityId);
            removeItem = NetworkMagic.RegisterAtAllClients<SingleData<T>>(1, BroadcastRemove, entityId);

            if (NetworkMagic.IsClient)
            {
                initialize(new NetworkData());
            }
        }


        #region NetworkCommand Implementation

        private void CmdInitialize(NetworkData data)
        {
            ListData<T> resp = new ListData<T> { ConnId = data.ConnId, DataList = ItemList };
            handleInitialize(resp);
        }

        private void TargetedHandleInitialize(ListData<T> listData)
        {
            foreach (T data in listData.DataList)
            {
                ItemList.Add(data);
                OnItemAdd?.Invoke(data);
            }
            DebugLog.Warning($"[SyncedList-{EntityId}] Initialized, data: {string.Join(", ", ItemList)}");
        }

        private void BroadcastAdd(SingleData<T> item)
        {
            ItemList.Add(item.Data);
            OnItemAdd?.Invoke(item.Data);
        }

        private void BroadcastRemove(SingleData<T> item)
        {
            if (!ItemList.Contains(item.Data))
            {
                DebugLog.Warning($"Tried to remove an item list, but item does not exist.");
            }

            ItemList.Remove(item.Data);
            OnItemRemove?.Invoke(item.Data);
        }

        #endregion NetworkCommand Implementation


        #region ICollection<T> Implementation

        public void Add(T item)
        {
            addItem(new SingleData<T>() { Data = item });
        }

        public bool Remove(T item)
        {
            bool r = ItemList.Contains(item);
            removeItem(new SingleData<T>() { Data = item });
            return r;
        }

        public void Clear()
        {
            foreach (T item in ItemList)
            {
                Remove(item);
            }
        }

        public bool Contains(T item)
        {
            return ItemList.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            ItemList.CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ItemList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ItemList.GetEnumerator();
        }

        #endregion ICollection<T> Implementation

    }
}
