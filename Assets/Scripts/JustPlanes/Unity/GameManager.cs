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
        private GameObject mainMenuUIManager;
        [SerializeField]
        private GameObject playerListUIManager;

        private void Awake()
        {
            mainMenuUIManager.SetActive(false);
            playerListUIManager.SetActive(false);
            if (instance == null)
            {
                instance = this;
                // TODO: remove this when we have scene manager and call it from there
            }
        }

        private void Start()
        {
            StartMainMenu();
        }

        public void StartMainMenu()
        {
            NetworkManager.instance.StartConnection();
            authenticator = new Authenticator();
            mainMenuUIManager.SetActive(true);
        }

        public void StartGame()
        {
            playerManager = new PlayerManager();
            playerListUIManager.SetActive(true);
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