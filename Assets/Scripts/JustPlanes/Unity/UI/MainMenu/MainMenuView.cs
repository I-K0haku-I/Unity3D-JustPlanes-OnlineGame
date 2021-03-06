using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace JustPlanes.Unity.UI
{
    class MainMenuView : MonoBehaviour
    {
        public event Action<string> OnLoginSubmit;
        public event Action OnLoginFinish;
        public event Action OnRetryServer;

        private Button popUpBtn;
        private TMP_InputField textField;
        private Button submitBtn;

        [SerializeField]
        private GameObject popUp;
        [SerializeField]
        private TMP_Text popUpMessageText;
        [SerializeField]

        private void Awake()
        {
            popUpBtn = popUp.GetComponentInChildren<Button>();
            popUpBtn.onClick.AddListener(() => popUp.SetActive(false));

            textField = GetComponentInChildren<TMP_InputField>();
            submitBtn = GetComponentInChildren<Button>();
            submitBtn.onClick.AddListener(() => OnLoginSubmit.Invoke(textField.text));
        }

        internal void DisplayNormal()
        {
            popUp.SetActive(false);
        }

        public void DisplayFailPopup(string msg)
        {
            popUpMessageText.SetText(msg);
            popUp.SetActive(true);
            popUpBtn.gameObject.SetActive(true);
        }

        public void DisplayServerNotFound()
        {
            popUpMessageText.SetText("No connection to server established. Please reopen the game and make sure to use the right address.");
            popUp.SetActive(true);
            popUpBtn.gameObject.SetActive(false);
        }

        public void DisplayWaitingForServer()
        {
            popUpMessageText.SetText("Waiting for server response...");
            popUp.SetActive(true);
            popUpBtn.gameObject.SetActive(false);
        }

        public void DisplayStartingGame()
        {
            popUpMessageText.SetText("Starting the game...");
            popUp.SetActive(true);
            popUpBtn.gameObject.SetActive(false);
            OnLoginFinish.Invoke();
        }
    }
}