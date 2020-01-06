using JustPlanes.Core;
using UnityEngine;

namespace JustPlanes.Unity.UI
{

    [RequireComponent(typeof(PlayerListView))]
    public class PlayerListPresenter : MonoBehaviour
    {
        [SerializeField]
        private KeyCode TogglePlayerListKey = KeyCode.Tab;

        private PlayerListView view;
        private PlayerManager playerManager;

        private void Awake()
        {
            view = GetComponent<PlayerListView>();
            view.Hide();
        }

        private void Start()
        {
            playerManager = GameManager.instance.playerManager;

            foreach (string player in playerManager.players)
            {
                view.AddPanel(player);
            }

            playerManager.OnPlayerJoin += view.AddPanel;
            playerManager.OnPlayerQuit += view.RemovePanel;
        }

        private void Update()
        {
            if (Input.GetKeyDown(TogglePlayerListKey))
                view.Show();
            if (Input.GetKeyUp(TogglePlayerListKey))
                view.Hide();
        }
    }
}
