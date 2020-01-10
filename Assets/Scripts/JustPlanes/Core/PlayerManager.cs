using System.Collections.Generic;

using JustPlanes.Core.Network;

namespace JustPlanes.Core
{

    // TODO: this is a good example where managing a list becomes a annoying and you have to start writing methods for every single thing
    // instead, it should be a custom data type for List that sends out its state to clients and clients synchronize it on their side
    // you could then have ownership, and transfer it to who is supposed to control that variable right now
    // you could have other settings then too, like how often it updates or how important it is so you could prioritize it when the network is loaded

    public class PlayerManager
    {
        public SyncedListOfString Players;

        public PlayerManager()
        {
            Players = new SyncedListOfString(465);
        }

        public void AddPlayer(string name)
        {
            Players.Add(name);
        }

        public void RemovePlayer(string name)
        {
            Players.Remove(name);
        }
    }

    // public class PlayerManagerOld
    // {
    //     public List<string> players = new List<string>();

    //     public event Action<string> OnPlayerJoin;
    //     public event Action<string> OnPlayerQuit;


    //     private Action<NameNetworkData> addPlayer;
    //     private Action<NameNetworkData> removePlayer;
    //     private Action<NetworkData> initialize;
    //     private Action<ListOfStringData> handleInitialize;

    //     public PlayerManagerOld()
    //     {
    //         addPlayer = NetworkMagic.RegisterBroadcasted<NameNetworkData>(1, BroadcastedAddPlayer);
    //         removePlayer = NetworkMagic.RegisterBroadcasted<NameNetworkData>(2, BroadcastedRemovePlayer);
    //         initialize = NetworkMagic.RegisterCommand<NetworkData>(2, CmdInitializePlayerManager);
    //         handleInitialize = NetworkMagic.RegisterTargeted<ListOfStringData>(2, TargetedHandleInitialize);

    //         initialize(new NetworkData());
    //     }

    //     private void TargetedHandleInitialize(ListOfStringData data)
    //     {
    //         players = data.itemList;
    //         foreach (string name in data.itemList)
    //             OnPlayerJoin?.Invoke(name);
    //         DebugLog.Warning($"[PlayerManager] Initialized, players: {string.Join(", ", players)}");
    //     }

    //     private void CmdInitializePlayerManager(NetworkData data)
    //     {
    //         ListOfStringData resp = new ListOfStringData { connId = data.connId };
    //         resp.itemList = players;
    //         handleInitialize(resp);
    //     }

    //     public void AddPlayer(string name)
    //     {
    //         addPlayer(new NameNetworkData() { Name = name });
    //     }

    //     private void BroadcastedAddPlayer(NameNetworkData data)
    //     {
    //         DebugLog.Warning($"[PlayerManager] Player added: {data.Name}");
    //         players.Add(data.Name);
    //         OnPlayerJoin?.Invoke(data.Name);
    //     }

    //     public void RemovePlayer(string name)
    //     {
    //         if (players.Contains(name))
    //             removePlayer(new NameNetworkData() { Name = name });
    //         else
    //             DebugLog.Warning($"[PlayerManager] Tried to remove a player with no name yet, not broadcasting.");
    //     }

    //     private void BroadcastedRemovePlayer(NameNetworkData data)
    //     {
    //         DebugLog.Warning($"[PlayerManager] Player removed: {data.Name}");
    //         OnPlayerQuit?.Invoke(data.Name);
    //         players.Remove(data.Name);
    //     }

    //     public bool IsNameTaken(string name)
    //     {
    //         return players.Contains(name);
    //     }
    // }

    public class ListOfStringData : NetworkData
    {
        public List<string> ItemList;
    }

}