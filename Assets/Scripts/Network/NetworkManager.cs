using System;
using System.Collections.Generic;
using UnityEngine;
using JustPlanes.Network.Client;
using UnityEngine.Events;

namespace JustPlanes.Network
{
    [System.Serializable]
    public class PlayerEvent : UnityEvent<Player>
    {

    }

    [System.Serializable]
    public class UnitEvent : UnityEvent<Unit>
    {

    }

    [System.Serializable]
    public class StringEvent : UnityEvent<string>
    {

    }

    [System.Serializable]
    public class UnitDamageEvent : UnityEvent<Unit, int>
    {

    }

    public class NetworkManager : MonoBehaviour
    {
        public static NetworkManager instance;

        [SerializeField]
        private bool isOnline = false;
        [SerializeField]
        private string serverAddress = "localhost";

        private List<Player> players = new List<Player>();
        public Player[] playerDisplayed = new Player[100];
        public PlayerEvent OnPlayerAdd = new PlayerEvent();
        private int playerCount = 0;

        private Dictionary<string, Unit> units = new Dictionary<string, Unit>();
        public UnitEvent OnUnitAdd = new UnitEvent();
        public StringEvent OnReceiveMsg = new StringEvent();
        public UnitEvent OnUnitDies = new UnitEvent();
        public UnitDamageEvent OnUnitGetsDamaged = new UnitDamageEvent();

        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            if (!isOnline)
                return;

            DontDestroyOnLoad(this);
            UnityThread.initUnityThread();

            ClientHandleData.InitializePackets(this);
            ClientTCP.ServerAddress = serverAddress;
            ClientTCP.InitializingNetworking();
        }

        internal void AcknowledgeUnitDied(string id)
        {
            units.TryGetValue(id, out Unit unit);
            if (unit == null)
                return;
            OnUnitDies.Invoke(unit);
            units.Remove(unit.ID);
        }

        internal void AcknowledgeUnitDamaged(string id, int dmg)
        {
            units.TryGetValue(id, out Unit unit);
            if (unit == null)
                return;
            Debug.Log($"{id} got hit for {dmg} DMG!");
            OnUnitGetsDamaged.Invoke(unit, dmg);
        }

        private void OnApplicationQuit()
        {
            if (!isOnline)
                return;

            ClientTCP.Disconnect();
        }

        public void AddPlayer(Player player)
        {
            Debug.Log($"JOINED: {player.Name}");
            players.Add(player);
            playerDisplayed[playerCount] = player;
            playerCount++;
            OnPlayerAdd.Invoke(player);
        }

        public void AddUnit(Unit unit)
        {
            Debug.Log($"Spawned: {unit.ID}");
            units.Add(unit.ID, unit);
            OnUnitAdd.Invoke(unit);
        }

        internal void ReceivedMsg(string msg)
        {
            Debug.Log(msg);
            OnReceiveMsg.Invoke(msg);
        }

        public void DamageUnit(Unit unit, int damage)
        {
            DataSender.SendUnitDamaged(unit, damage);
        }
    }
}