using JustPlanes.Network;
using UnityEngine;

namespace JustPlanes.UI.MissionPanel
{

    public class MissionPanelPresenter : MonoBehaviour, ITogglableUI
    {
        private static readonly string _prefix = "MP Presenter:";

        [SerializeField] private MissionPanelView _panelRef = null;

        private void Awake()
        {
            if (!_panelRef)
            {
                DebugLog.Severe(this, $"{_prefix} UI Component not assigned!!");
                Destroy(this);
                return;
            }
        }

        private void Start()
        {
            MissionHandlerManager.Instance.OnMissionAssignEvent.AddListener((handler) => _panelRef.Assign(GetMissionDescriptionString(handler.type), GetMissionProgressString(handler.type), handler.enemiesKilled, handler.enemiesToKill));
            MissionHandlerManager.Instance.OnMissionUpdateEvent.AddListener((handler) => _panelRef.UpdateMessage(handler.enemiesKilled, handler.enemiesToKill));
            MissionHandlerManager.Instance.OnMissionCompleteEvent.AddListener((handler) => _panelRef.Complete(handler.enemiesKilled, handler.enemiesToKill));
        }

        #region MissionMessages

        private string GetMissionDescriptionString(MissionTypes missionType)
        {
            switch (missionType)
            {
                case MissionTypes.MTKILLRats:
                    return "Defeat {max} of units!";
                default:
                    return "INVALID_MISSION_TYPE";
            }
        }

        private string GetMissionProgressString(MissionTypes missionType)
        {
            switch (missionType)
            {
                case MissionTypes.MTKILLRats:
                default:
                    return "{progress}/{max}";
            }
        }

        #endregion MissionMessages

        #region ITogglableUI

        public void SetVisible(bool visible)
        {
            DebugLog.Finer(this, $"{_prefix} Toggling PlayerList to {visible}");
            _panelRef.SetVisible(visible);
        }

        public bool IsVisible()
        {
            return _panelRef.IsVisible();
        }

        #endregion ITogglableUI

    }
}