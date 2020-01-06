using System.Collections.Generic;
using UnityEngine;

namespace JustPlanes.Unity.UI
{

    public class PlayerListPanelManager : MonoBehaviour, ITogglableUI
    {
        private static readonly string _prefix = "PLPanel Mgr:";

        [SerializeField] public GameObject playerListPanel = null;
        [SerializeField] public KeyCode showPlayerList = KeyCode.Tab;

        public Dictionary<PlayerView, PlayerListPanel> playerListPanelPairs = new Dictionary<PlayerView, PlayerListPanel>();

        //private readonly bool _isLocallyVisible;

        private void Start()
        {
            UnitViewManager.Instance.OnPlayerViewAddEvent.AddListener((pv) => AddPlayer(pv));
            UnitViewManager.Instance.OnPlayerViewRemoveEvent.AddListener((pv) => RemovePlayer(pv));

            SetVisible(false, true);
        }

        private void Update()
        {
            if (Input.GetKeyDown(showPlayerList))
            {
                SetVisible(true, true);
            }
            else if (Input.GetKeyUp(showPlayerList))
            {
                SetVisible(false, true);
            }
        }


        public void AddPlayer(PlayerView playerView)
        {
            DebugLog.Finer(this, $"{_prefix} Adding player {playerView.ToString()}");

            GameObject obj = (GameObject)Instantiate(PlayerListPanel.panelObject, PlayerListPanel.parentTransform);
            PlayerListPanel panel = obj.GetComponent<PlayerListPanel>();

            panel.SetPlayerView(playerView);

            playerListPanelPairs.Add(playerView, panel);
        }

        public void RemovePlayer(PlayerView playerView)
        {
            DebugLog.Finer(this, $"{_prefix} Removing player panel of {playerView.ToString()}");

            if (playerListPanelPairs.TryGetValue(playerView, out PlayerListPanel panel))
            {
                playerListPanelPairs.Remove(playerView);

                Destroy(panel.gameObject);
                DebugLog.Finest(this, $"{_prefix} Removed player panel of {playerView.ToString()}");

                return;
            }
            else
            {
                DebugLog.Severe(this, $"{_prefix} Could not find PlayerView {playerView.ToString()}!!");
                return;
            }
        }

        #region ITogglableUI

        public void SetVisible(bool visible, bool isLocal)
        {
            if (visible && !isLocal && !playerListPanel.activeSelf)
            {
                return;
            }
            DebugLog.Finer(this, $"{_prefix} Toggling PlayerList to {visible}");
            playerListPanel.SetActive(visible);
        }

        public void SetVisible(bool visible)
        {
            SetVisible(visible, false);
        }

        public bool IsVisible()
        {
            return playerListPanel.activeInHierarchy;
        }

        #endregion ITogglableUI

    }
}