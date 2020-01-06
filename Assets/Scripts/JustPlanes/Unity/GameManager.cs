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
        private GameObject playerListUIManager;

        private void Awake()
        {
            playerListUIManager.SetActive(false);
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
            playerListUIManager.SetActive(true);

            playerManager = new PlayerManager();
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