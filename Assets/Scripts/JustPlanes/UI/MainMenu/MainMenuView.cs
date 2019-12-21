using UnityEngine;
using UnityEngine.Events;
using JustPlanes.Network;

namespace JustPlanes.UI
{
    interface IMainMenuView
    {
        void StartLoad();
        void FailLoad();
        void SucceedLoad();
    }

    class MainMenuView : MonoBehaviour, IMainMenuView
    {
        public StringEvent OnLoginSubmit = new StringEvent();
        public UnityEvent OnLoginFinish = new UnityEvent();

        /// <summary>
        /// Stop loading circle. Show some fail message. Show text field again.
        /// </summary>
        public void FailLoad()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Starts loading circle. Close text field.
        /// </summary>
        public void StartLoad()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Remove loading circle. Show some succeed message.
        /// </summary>
        public void SucceedLoad()
        {
            throw new System.NotImplementedException();
        }
    }
}