using System.Collections.Generic;

namespace JustPlanes.Core.Network
{
    public interface IGenericNetworkData<T>
    {
    }

    public class ListData<T> : NetworkData, IGenericNetworkData<T>
    {
        public List<T> DataList;
    }

    public class SingleData<T> : NetworkData, IGenericNetworkData<T>
    {
        public T Data;
    }
}
