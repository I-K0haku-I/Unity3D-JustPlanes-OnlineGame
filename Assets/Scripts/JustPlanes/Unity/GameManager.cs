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

        public Authenticator Authenticator;
        public PlayerManager PlayerManager;

        [SerializeField]
        private GameObject mainMenuUIManager;
        [SerializeField]
        private GameObject playerListUIManager;
        [SerializeField]
        private GameObject testPlane;

        private void Awake()
        {
            mainMenuUIManager.SetActive(false);
            playerListUIManager.SetActive(false);
            testPlane.SetActive(false);
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
            Authenticator = new Authenticator();
            mainMenuUIManager.SetActive(true);
        }

        public void StartGame()
        {
            PlayerManager = new PlayerManager();
            playerListUIManager.SetActive(true);
            testPlane.SetActive(true);
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