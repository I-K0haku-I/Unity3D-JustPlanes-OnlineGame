using System;
using JustPlanes.Network;
using UnityEngine;
using UnityEngine.Events;

namespace JustPlanes.UI
{
    interface ISceneManager
    {
        void DisplayGame();
    }


    class MainMenuPresenter : MonoBehaviour
    {
        private Authenticator auth;
        private ISceneManager manager;
        private MainMenuView menu;

        private void Awake()
        {
            menu = GetComponent<MainMenuView>();
            menu.OnLoginSubmit.AddListener(HandleLoginInput);
            menu.OnLoginFinish.AddListener(HandleLoginFinish);
        }

        private void Start()
        {
            if (!NetworkMagic.IsConnected)
                menu.DisplayServerNotFound();

            auth = Unity.GameManager.instance.authenticator;
            auth.OnLoginFailed += HandleLoginFailed;
            auth.OnLoginSucceeded += HandleLoginSucceeded;
        }

        private void HandleLoginSucceeded(string msg)
        {
            menu.DisplayStartingGame();
        }

        private void HandleLoginFailed(string msg)
        {
            menu.DisplayFailPopup(msg);
        }

        private void HandleLoginFinish()
        {
            DebugLog.Warning("Start the game scene");
            // manager.DisplayGame();
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
