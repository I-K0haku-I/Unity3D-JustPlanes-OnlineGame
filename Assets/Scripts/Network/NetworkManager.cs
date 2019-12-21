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
    [System.Serializable]
    public class MissionEvent : UnityEvent<MissionHandler>
    {
    }

    [System.Serializable]
    public class IntegerEvent : UnityEvent<int>
    {

    }

    public class NetworkManager : MonoBehaviour, IPlayerHolder
    {
        public static NetworkManager instance;

        [SerializeField]
        private bool isOnline = false;
        [SerializeField]
        private string serverAddress = "localhost";

        public List<Player> players = new List<Player>();
        public Player[] playerDisplayed = new Player[100];
        public PlayerEvent OnPlayerAdd = new PlayerEvent();
        private int playerCount = 0;

        private Dictionary<string, Unit> units = new Dictionary<string, Unit>();
        public UnitEvent OnUnitAdd = new UnitEvent();
        public StringEvent OnReceiveMsg = new StringEvent();
        public UnitEvent OnUnitDies = new UnitEvent();
        public UnitDamageEvent OnUnitGetsDamaged = new UnitDamageEvent();

        public MissionHandler mission = new MissionHandler();

        public MissionEvent OnMissionAdd = new MissionEvent();
        public IntegerEvent OnMissionUpdate = new IntegerEvent();
        public UnityEvent OnMissionComplete = new UnityEvent();
        public PlayerEvent OnPlayerRemove = new PlayerEvent();

        public NetworkMagic net = new NetworkMagic();

        internal void AcknowledgeMissionComplete()
        {
            OnMissionComplete.Invoke();
        }


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

        internal void UpdateMission(int enemiesKilledDelta)
        {
            mission.Update(enemiesKilledDelta);
            OnMissionUpdate.Invoke(enemiesKilledDelta);
        }

        internal void AcknowledgeUnitDied(string id)
        {
            units.TryGetValue(id, out Unit unit);
            if (unit == null)
                return;
            OnUnitDies.Invoke(unit);
            units.Remove(unit.ID);
        }

        internal void AddMission(MissionTypes type, int target, int start)
        {
            mission = new MissionHandler(this);
            mission.enemiesToKill = target;
            mission.enemiesKilled = start;
            OnMissionAdd.Invoke(mission);
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

        public void RemovePlayer(Player player)
        {
            players.Remove(player);
            playerDisplayed[playerCount] = null;
            playerCount++;
            OnPlayerRemove.Invoke(player);
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

        public int GetPlayerAmount()
        {
            return players.Count;
        }
    }
}