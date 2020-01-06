using UnityEngine.Events;
using UnityEngine;
using JustPlanes.Core;

namespace JustPlanes.Unity.UI
{

    public class MissionUIModel : MonoBehaviour
    {

        public MissionEvent OnMissionChangedEvent = new MissionEvent();
        public FloatEvent OnMissionProgressEvent = new FloatEvent();
        public UnityEvent OnMissionCompletedEvent = new UnityEvent();

        private MissionHandler _missionHandler;


        public void SetProgress(int progress)
        {
            DebugLog.Finer($"MUI_Model: Provided progress: {progress}");

            _missionHandler.enemiesKilled += progress;
            OnMissionProgressEvent.Invoke(_missionHandler.enemiesKilled);
        }

        public void SetMissionHandler(MissionHandler handler)
        {
            DebugLog.Finer($"MUI_Model: Setting mission handler: {handler.type} expecting {handler.enemiesToKill} kills with current kills {handler.enemiesKilled}");

            _missionHandler = handler;
            OnMissionChangedEvent.Invoke(_missionHandler);
        }

        public void SetComplete(bool done)
        {
            DebugLog.Finer($"MUI_Model: Setting complete state as: {done}");
            _missionHandler.IsDone = true;
            OnMissionCompletedEvent.Invoke();
        }


    }
}