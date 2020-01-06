using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using JustPlanes.Core;
using JustPlanes.Core.Network;
using JustPlanes.Core.Network.Client;

namespace JustPlanes.Unity
{


    [System.Serializable]
    public class PlayerEvent : UnityEvent<Player>
    {

    }

    [System.Serializable]
    public class UnitEvent : UnityEvent<Core.Unit>
    {

    }

    [System.Serializable]
    public class StringEvent : UnityEvent<string>
    {

    }

    [System.Serializable]
    public class UnitDamageEvent : UnityEvent<Core.Unit, int>
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

    [System.Serializable]
    public class RequestResponseEvent : UnityEvent<IRequestorResponse>
    {

    }

    public class NetworkManager : MonoBehaviour, IPlayerHolder, INetworkManager
    {
        public static NetworkManager instance;

        [SerializeField]
        private bool isOnline = true;
        [SerializeField]
        private bool isSelfInitialize = false;
        [SerializeField]
        public string serverAddress = "localhost";

        public List<Player> players = new List<Player>();
        public Player[] playerDisplayed = new Player[100];
        public PlayerEvent OnPlayerAdd = new PlayerEvent();
        private int playerCount = 0;

        private Dictionary<string, Core.Unit> units = new Dictionary<string, Core.Unit>();
        public UnitEvent OnUnitAdd = new UnitEvent();
        public StringEvent OnReceiveMsg = new StringEvent();
        public UnitEvent OnUnitDies = new UnitEvent();
        public UnitDamageEvent OnUnitGetsDamaged = new UnitDamageEvent();

        public MissionHandler mission = new MissionHandler();

        public MissionEvent OnMissionAdd = new MissionEvent();
        public IntegerEvent OnMissionUpdate = new IntegerEvent();
        public UnityEvent OnMissionComplete = new UnityEvent();
        public PlayerEvent OnPlayerRemove = new PlayerEvent();

        public void AcknowledgeMissionComplete()
        {
            OnMissionComplete.Invoke();
        }


        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                NetworkMagic.IsClient = true;
            }
        }

        private void Start()
        {
            if (!isOnline)
                return;

            DontDestroyOnLoad(this);
            UnityThread.initUnityThread();

            if (isSelfInitialize)
                StartConnection();
        }

        public void StartConnection()
        {
            ClientHandleData.InitializePackets(instance);
            ClientTCP.ServerAddress = serverAddress;
            ClientTCP.InitializingNetworking((IUnityThread)UnityThread.instance);
        }

        public void UpdateMission(int enemiesKilledDelta)
        {
            mission.Update(enemiesKilledDelta);
            OnMissionUpdate.Invoke(enemiesKilledDelta);
        }

        public void AcknowledgeUnitDied(string id)
        {
            units.TryGetValue(id, out Core.Unit unit);
            if (unit == null)
                return;
            OnUnitDies.Invoke(unit);
            units.Remove(unit.ID);
        }

        public void AddMission(MissionTypes type, int target, int start)
        {
            mission = new MissionHandler(this);
            mission.enemiesToKill = target;
            mission.enemiesKilled = start;
            OnMissionAdd.Invoke(mission);
        }


        public void AcknowledgeUnitDamaged(string id, int dmg)
        {
            units.TryGetValue(id, out Core.Unit unit);
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

        public void AddUnit(Core.Unit unit)
        {
            Debug.Log($"Spawned: {unit.ID}");
            units.Add(unit.ID, unit);
            OnUnitAdd.Invoke(unit);
        }

        public void ReceivedMsg(string msg)
        {
            Debug.Log(msg);
            OnReceiveMsg.Invoke(msg);
        }

        public void DamageUnit(Core.Unit unit, int damage)
        {
            // DataSender.SendUnitDamaged(unit, damage);
        }

        public int GetPlayerAmount()
        {
            return players.Count;
        }
    }
}