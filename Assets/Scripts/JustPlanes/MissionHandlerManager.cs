using JustPlanes.Network;
using System.Collections.Generic;
using UnityEngine;

namespace JustPlanes
{

    public class MissionHandlerManager : MonoBehaviour
    {
        private static readonly string _prefix = "MH Mgr:";

        public static MissionHandlerManager Instance;

        public MissionEvent OnMissionAssignEvent = new MissionEvent();
        public MissionEvent OnMissionUpdateEvent = new MissionEvent();
        public MissionEvent OnMissionCompleteEvent = new MissionEvent();

        private Queue<MissionHandler> _missionHandlers = new Queue<MissionHandler>();
        private MissionHandler _currentMission = null;

        private void Awake()
        {
            if (Instance != null)
            {
                DebugLog.Severe(this, $"{_prefix} MissionHandlerManager instance already exists!!");
                Destroy(this);
            }

            Instance = this;
        }

        private void Start()
        {
            NetworkManager.instance.OnMissionAdd.AddListener((handler) => AddMission(handler));
            NetworkManager.instance.OnMissionUpdate.AddListener((val) => UpdateMission(val));
            NetworkManager.instance.OnMissionComplete.AddListener(() => CompleteMission());
        }

        // MissionHandlerManager sequence
        // Add => Check => Assign => Update => Check => Complete => Check => Assign...

        public void AddMission(MissionHandler handler, bool check = true)
        {
            DebugLog.Fine(this, $"{_prefix} Adding new mission: {MH_ToString(handler)}");

            _missionHandlers.Enqueue(handler);

            CheckMission(check);
        }

        public void UpdateMission(int val, bool check = true)
        {
            DebugLog.Fine(this, $"{_prefix} Updating mission: {val}, {MH_ToString(_currentMission)}");

            if (_currentMission == null)
            {
                DebugLog.Severe(this, $"{_prefix} Cannot update mission because current mission is null!!");
                return;
            }

            //_currentMission.enemiesKilled += val;
            OnMissionUpdateEvent.Invoke(_currentMission);

            CheckMission(check);
        }

        public void CompleteMission(bool check = true)
        {
            DebugLog.Fine(this, $"{_prefix} Completing mission: {MH_ToString(_currentMission)}");

            if (_currentMission == null)
            {
                DebugLog.Severe(this, $"{_prefix} Cannot complete mission because current mission is null!!");
                return;
            }

            OnMissionCompleteEvent.Invoke(_currentMission);
            _currentMission = null;

            CheckMission(check);
        }

        public void AssignMission()
        {
            if (_missionHandlers.Count != 0)
            {
                if (_currentMission != null)
                {
                    DebugLog.Warning(this, $"{_prefix} Overwriting mission!: {MH_ToString(_currentMission)}");
                }

                _currentMission = _missionHandlers.Dequeue();
                DebugLog.Fine(this, $"{_prefix} Assigned mission: {MH_ToString(_currentMission)}");

                OnMissionAssignEvent.Invoke(GetCurrentMission());
            }
        }

        public void CheckMission(bool check = true)
        {
            if (check)
            {
                DebugLog.Finest(this, $"{_prefix} Checking!");

                if (_currentMission == null)
                {
                    AssignMission();
                }
            }
        }

        public MissionHandler GetCurrentMission()
        {
            return _currentMission;
        }

        private string MH_ToString(MissionHandler handler)
        {
            if (handler != null)
            {
                return $"{{{handler.type}, progress {handler.enemiesKilled}, expecting {handler.enemiesToKill}, has done {handler.IsDone}}}";
            }
            else
            {
                return $"{{null}}";
            }
        }

    }
}
