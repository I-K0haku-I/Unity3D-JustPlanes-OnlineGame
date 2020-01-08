using System;
using UnityEngine;

using JustPlanes.Core;
using JustPlanes.Core.Network;

namespace JustPlanes.Unity.UI
{
    interface ISceneManager
    {
        void DisplayGame();
    }


    [RequireComponent(typeof(MainMenuView))]
    class MainMenuPresenter : MonoBehaviour
    {
        private Authenticator auth;
        private ISceneManager sceneManager;
        private MainMenuView menu;

        private bool IsConnected { get { return NetworkMagic.IsConnected; }}

        private void Awake()
        {
            menu = GetComponent<MainMenuView>();
            menu.OnLoginSubmit += HandleLoginInput;
            menu.OnLoginFinish += () =>
            {
                DebugLog.Warning("Start the game scene");
                gameObject.SetActive(false);
                sceneManager.DisplayGame();
            };
        }

        private void Start()
        {
            sceneManager = Unity.GameManager.instance;
            auth = Unity.GameManager.instance.Authenticator;
            auth.OnLoginFailed += menu.DisplayFailPopup;
            auth.OnLoginSucceeded += (string msg) => menu.DisplayStartingGame();
        }

        private void Update()
        {
            if (IsConnected)
                menu.DisplayNormal();
            else
                menu.DisplayServerNotFound();
        }

        private void HandleLoginInput(string name)
        {
            if (name == null || name == "" || auth.isAuthenticated)
                return;
            menu.DisplayWaitingForServer();
            auth.TryLogin(name);
        }
    }
}
