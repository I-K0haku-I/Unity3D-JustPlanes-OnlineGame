using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JustPlanes.Core;


namespace JustPlanes.Unity
{
    public class GameManager : MonoBehaviour, UI.ISceneManager
    {
        public static GameManager instance;

        public Authenticator authenticator;
        public PlayerManager playerManager;

        [SerializeField]
        private UI.PlayerListPresenter playerListPrefab;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                // TODO: remove this when we have scene manager and call it from there
                StartMainMenu();
            }
        }

        public void StartMainMenu()
        {
            authenticator = new Authenticator();
        }

        public void StartGame()
        {
            playerManager = new PlayerManager();

            GameObject obj = new GameObject("PlayerList");
            obj.AddComponent<UI.PlayerListPresenter>();
            obj.AddComponent<UI.PlayerListView>();
        }

        void Update()
        {

        }

        public void DisplayGame()
        {
            StartGame();
        }
    }

}