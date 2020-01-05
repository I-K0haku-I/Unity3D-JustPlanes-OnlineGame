using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace JustPlanes.Unity
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;

        public Authenticator authenticator;
        public PlayerManager playerManager;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                playerManager = new PlayerManager();
                authenticator = new Authenticator(playerManager);
            }
        }

        void Update()
        {

        }
    }

}