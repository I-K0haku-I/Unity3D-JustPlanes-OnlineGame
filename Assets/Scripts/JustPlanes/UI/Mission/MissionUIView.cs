using JustPlanes.Network;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace JustPlanes.UI.MissionPanel
{

    public class MissionUIView : MonoBehaviour
    {

        [SerializeField] private GameObject _uiPanel = null;

        [SerializeField] private TextMeshProUGUI _description = null;
        [SerializeField] private TextMeshProUGUI _progressText = null;
        [SerializeField] private Slider _progressBar = null;

        // Init format
        private string _progressTextFormat = "{0}{1}";
        private string _descriptionTextFormat = "{0}";

        public MissionEvent OnMissionAddedEvent = new MissionEvent();
        public IntegerEvent OnMissionUpdatedEvent = new IntegerEvent();
        public UnityEvent OnMissionCompletedEvent = new UnityEvent();

        private void Awake()
        {
            if (!_uiPanel | !_description | !_progressText | !_progressBar)
            {
                DebugLog.Severe(this, "MUI_View: UI Component not assigned!!");
                Destroy(this);
            }
        }

        private void Start()
        {
            _progressBar.value = 0;
            _progressBar.maxValue = 0;

            //NetworkManager.instance.OnMissionAdd.AddListener((handle) => OnMissionAddedEvent.Invoke(handle));
            //NetworkManager.instance.OnMissionUpdate.AddListener((val) => OnMissionUpdatedEvent.Invoke(val));
            //NetworkManager.instance.OnMissionComplete.AddListener(() => OnMissionCompletedEvent.Invoke());

            ToggleUIPanel(false);
        }

        public void ToggleUIPanel(bool showUI)
        {
            DebugLog.Finer(this, $"MUI_View: Toggling UI Panel as: {showUI}");

            _uiPanel.SetActive(showUI);
        }

        public void SetMission(MissionHandler handler)
        {
            DebugLog.Finer(this, $"MUI_View: Getting/Assigning mission information: {handler.type} expecting {handler.enemiesToKill} kills with current kills {handler.enemiesKilled}");

            _descriptionTextFormat = GetMissionDescriptionString(handler.type);
            _progressTextFormat = GetMissionProgressString(handler.type);

            SetMaxProgress(handler.enemiesToKill);
            SetProgress(handler.enemiesKilled);

            ToggleUIPanel(true);
        }

        public void SetProgress(float progress)
        {
            DebugLog.Finer(this, $"MUI_View: Setting progress as: {progress}, descFormat: {_descriptionTextFormat}, progFormat: {_progressTextFormat}");

            _progressBar.value = progress;
            _description.text = string.Format(_descriptionTextFormat, _progressBar.maxValue);
            _progressText.text = string.Format(_progressTextFormat, _progressBar.value, _progressBar.maxValue);
        }

        public void SetMaxProgress(float max)
        {
            DebugLog.Finer(this, $"MUI_View: Setting max progress as: {max}");

            _progressBar.maxValue = max;
        }

        private string GetMissionDescriptionString(MissionTypes missionType)
        {
            switch (missionType)
            {
                case MissionTypes.MTKILLRats:
                    return "Defeat {0} of units!";
                default:
                    return "INVALID_MISSION_TYPE_{0}";
            }
        }

        private string GetMissionProgressString(MissionTypes missionType)
        {
            switch (missionType)
            {
                case MissionTypes.MTKILLRats:
                default:
                    return "{0}/{1}";
            }
        }

    }
}
