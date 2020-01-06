using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace JustPlanes.Unity.UI
{

    public class PlayerListView : MonoBehaviour
    {
        [SerializeField]
        private GameObject playerListUI;
        [SerializeField]
        private Transform panelHolder;
        [SerializeField]
        private GameObject panelPrefab;


        private Dictionary<string, PlayerListPanel> playerPanels = new Dictionary<string, PlayerListPanel>();

        public void AddPanel(string name)
        {
            GameObject panelObj = Instantiate(panelPrefab);
            panelObj.transform.SetParent(panelHolder, false);

            PlayerListPanel panel = panelObj.GetComponent<PlayerListPanel>();
            panel.SetPlayerName(name);
            playerPanels.Add(name, panel);
        }

        public void RemovePanel(string name)
        {
            if (playerPanels.TryGetValue(name, out PlayerListPanel panel))
            {
                Destroy(panel);
            }
        }

        public void Show()
        {
            playerListUI.SetActive(true);
        }

        public void Hide()
        {
            playerListUI.SetActive(false);
        }

    }
}