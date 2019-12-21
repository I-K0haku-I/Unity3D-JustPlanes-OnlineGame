using System;
using UnityEngine;
using UnityEngine.Events;

namespace JustPlanes.UI
{
    interface IAuthenticator
    {
        void TryLogin(string name);
        UnityEvent<ILoginResponse> OnLogin(); // won't be an actual UnityEvent, probably
    }

    interface IUIManager
    {
        void DisplayGame();
    }

    interface ILoginResponse
    {
        bool Ok { get; }
    }

    class MainMenuPresenter : MonoBehaviour
    {
        private IAuthenticator auth; // not a monobehavior
        private IUIManager manager;
        private MainMenuView menu;

        private void Awake()
        {
            auth = new Authenticator();
            menu.OnLoginSubmit.AddListener(HandleLoginInput);
            menu.OnLoginFinish.AddListener(HandleLoginFinish);
            auth.OnLogin().AddListener(HandleLogin);
        }

        private void HandleLogin(ILoginResponse resp)
        {
            if (resp.Ok)
                menu.SucceedLoad();
            else
                menu.FailLoad();
        }

        private void HandleLoginFinish()
        {
            manager.DisplayGame();
        }

        private void HandleLoginInput(string name)
        {
            menu.StartLoad(); // should probably be done in menu itself
            auth.TryLogin(name);
        }
    }
}