using System.Collections.Generic;
using UnityEngine;
using JustPlanes.Core;

namespace JustPlanes.Unity.UI
{

    public class MissionPanelPresenter : MonoBehaviour, ITogglableUI
    {
        private static readonly string _prefix = "MP Presenter:";

        [SerializeField] private MissionPanelView viewRef1 = null;
        [SerializeField] private MissionPanelView viewRef2 = null;

        private MissionPanelView currentView;

        private void Awake()
        {
            if (!viewRef1 || !viewRef2)
            {
                DebugLog.Severe(this, $"{_prefix} UI Component not assigned!!");
                Destroy(this);
                return;
            }
        }

        private void Start()
        {
            currentView = viewRef1;

            MissionHandlerManager.Instance.OnMissionAssignEvent.AddListener((handler) => MissionAssign(handler));
            MissionHandlerManager.Instance.OnMissionUpdateEvent.AddListener((handler) => MissionUpdate(handler));
            MissionHandlerManager.Instance.OnMissionCompleteEvent.AddListener((handler) => MissionComplete(handler));

        //    MissionHandlerManager.Instance.OnMissionAssignEvent.AddListener((handler) => _panelRef.Assign(GetMissionDescriptionString(handler.type), GetMissionProgressString(handler.type), handler.enemiesKilled, handler.enemiesToKill));
        //    MissionHandlerManager.Instance.OnMissionUpdateEvent.AddListener((handler) => _panelRef.UpdateMessage(handler.enemiesKilled, handler.enemiesToKill));
        //    MissionHandlerManager.Instance.OnMissionCompleteEvent.AddListener((handler) => _panelRef.Complete(handler.enemiesKilled, handler.enemiesToKill));
        }

        private void MissionAssign(MissionHandler handler)
        {
            currentView.Assign(GetMissionDescriptionString(handler.type), GetMissionProgressString(handler.type), handler.enemiesKilled, handler.enemiesToKill);
        }

        private void MissionUpdate(MissionHandler handler)
        {
            currentView.UpdateMessage(handler.enemiesKilled, handler.enemiesToKill);
        }

        private void MissionComplete(MissionHandler handler)
        {
            currentView.Complete(handler.enemiesKilled, handler.enemiesToKill);
            ChangePanel();
        }

        private void ChangePanel()
        {
            currentView = currentView == viewRef1 ? viewRef2 : viewRef1;
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
            currentView.SetVisible(visible);
        }

        public bool IsVisible()
        {
            return currentView.IsVisible();
        }

        #endregion ITogglableUI

    }
}