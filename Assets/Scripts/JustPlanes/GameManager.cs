using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace JustPlanes.Unity
{
    public class GameManager : MonoBehaviour, UI.ISceneManager
    {
        public static GameManager instance;

        public Authenticator authenticator;
        public PlayerManager playerManager;

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