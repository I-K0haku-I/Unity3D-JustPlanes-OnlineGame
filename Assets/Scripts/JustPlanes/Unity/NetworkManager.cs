using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using JustPlanes.Core;
using JustPlanes.Core.Network;
using JustPlanes.Core.Network.Client;

namespace JustPlanes.Unity
{

    public class NetworkManager : MonoBehaviour
    {
        public static NetworkManager instance;

        [SerializeField]
        private bool isOnline = true;
        [SerializeField]
        private bool isSelfInitOnPlay = false;
        [SerializeField]
        public string serverAddress = "127.0.0.1";
        [SerializeField]
        public int serverPort = 5569;

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

            if (isSelfInitOnPlay)
                StartConnection();
        }

        public void StartConnection()
        {
            ClientTCP.ServerAddress = serverAddress;
            ClientTCP.ServerPort = serverPort;
            ClientTCP.InitializingNetworking();
        }

        private void OnApplicationQuit()
        {
            if (!isOnline)
                return;

            ClientTCP.Disconnect();
        }

    }
}