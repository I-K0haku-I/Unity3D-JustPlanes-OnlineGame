using System;
using System.Collections.Generic;
using JustPlanes.Core;
using UnityEngine;

namespace JustPlanes.Unity.UI
{

    [RequireComponent(typeof(PlayerListView))]
    public class PlayerListPresenter : MonoBehaviour
    {
        [SerializeField]
        private KeyCode toggleKey = KeyCode.Tab;

        private PlayerManager playerManager;
        private PlayerListView view;

        private void Awake()
        {
            view = GetComponent<PlayerListView>();
            view.Hide();
        }

        private void Start()
        {
            playerManager = GameManager.instance.PlayerManager;

            playerManager.Players.OnItemAdd += view.AddPanel;
            playerManager.Players.OnItemRemove += view.RemovePanel;
        }

        private void Update()
        {
            if (Input.GetKeyDown(toggleKey))
                view.Show();
            if (Input.GetKeyUp(toggleKey))
                view.Hide();
        }
    }
}
