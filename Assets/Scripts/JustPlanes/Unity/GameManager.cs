using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JustPlanes.Core;


namespace JustPlanes.Unity
{
    public class GameManager : MonoBehaviour, UI.ISceneManager, IGame
    {
        public static GameManager instance;

        public Authenticator Authenticator;
        public PlayerManager PlayerManager;
        public PhysicsManager Physics = new PhysicsManager();

        [SerializeField]
        private GameObject mainMenuUIManager;
        [SerializeField]
        private GameObject playerListUIManager;
        [SerializeField]
        private GameObject testPlane;
        [SerializeField]
        private FollowerCamera camFollower;

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
            StartCoroutine("TryConnecting");
            StartMainMenu();
        }

        IEnumerator TryConnecting()
        {
            while (true)
            {
                if (Core.Network.NetworkMagic.IsConnected)
                    break;
                // TODO: add game data reset when disconnecting
                NetworkManager.instance.StartConnection();
                yield return new WaitForSeconds(1f);
            }
        }

        public void StartMainMenu()
        {
            Authenticator = new Authenticator();
            mainMenuUIManager.SetActive(true);
        }

        public void StartGame()
        {
            mainMenuUIManager.SetActive(false);
            PlayerManager = new PlayerManager();
            playerListUIManager.SetActive(true);
            testPlane.SetActive(true);
            // Camera.main.transform.parent = testPlane.transform;
        }

        void Update()
        {

        }

        private void FixedUpdate()
        {
            Physics.Update(Time.fixedDeltaTime);
        }

        public void DisplayGame()
        {
            StartGame();
        }

        public PhysicsManager GetPhysicsManager()
        {
            return Physics;
        }

        public float GetTime()
        {
            return Time.realtimeSinceStartup;
        }
    }

}