using TMPro;
using UnityEngine;

namespace JustPlanes.UI.PlayerList
{

    public class PlayerListPanel : MonoBehaviour
    {
        private static readonly string _prefix = "PLPanel:";

        [SerializeField] private GameObject _panel = null;
        [SerializeField] private TextMeshProUGUI _usernameText = null;
        [SerializeField] private TextMeshProUGUI _userStatsKillText = null;
        [SerializeField] private TextMeshProUGUI _userStatsDeathText = null;

        private PlayerView _playerView = null;
        public static Object panelObject = null;
        public static Transform parentTransform = null;

        private void Awake()
        {
            if (!_usernameText | !_userStatsKillText | !_userStatsDeathText | !_panel)
            {
                DebugLog.Severe(this, $"{_prefix} UI Component not assigned!!");
                Destroy(this);
                return;
            }

            if (!panelObject)
            {
                DebugLog.Finest(this, $"{_prefix} Assigning first template panel object");

                panelObject = _panel;
                parentTransform = _panel.transform.parent;
            }
        }

        public void SetPlayerView(PlayerView playerView)
        {
            DebugLog.Finest(this, $"{_prefix} Setting PlayerView to {playerView.ToString()}");

            _playerView = playerView;

            SetUsername(playerView.player.Name);
            SetVisible(true);
        }

        public void SetUsername(string username)
        {
            DebugLog.Finest(this, $"{_prefix} Updating username to {username}");

            _usernameText.SetText(username);
            _panel.name = $"{_prefix} {username}";
        }

        public void SetVisible(bool visible)
        {
            DebugLog.Finer(this, $"{_prefix} Updating visibility of {gameObject.name} to {visible}");

            _panel.SetActive(visible);
        }
    }
}