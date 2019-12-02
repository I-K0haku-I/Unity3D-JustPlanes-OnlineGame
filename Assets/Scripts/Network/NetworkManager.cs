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

    public class NetworkManager : MonoBehaviour
    {
        public static NetworkManager instance;

        [SerializeField]
        private bool isOnline = false;
        [SerializeField]
        private string serverAddress = "localhost";
        
        private List<Player> players = new List<Player>();
        public Player[] playerDisplayed = new Player[100];
        private PlayerEvent OnPlayerAdd = new PlayerEvent();
        private int playerCount = 0;

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

        private void OnApplicationQuit()
        {
            if (!isOnline)
                return;

            ClientTCP.Disconnect();
        }

        public void AddPlayer(Player player)
        {
            players.Add(player);
            playerDisplayed[playerCount] = player;
            playerCount++;
            OnPlayerAdd.Invoke(player);
        }
    }
}