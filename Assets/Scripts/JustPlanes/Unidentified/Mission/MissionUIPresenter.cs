using UnityEngine;

namespace JustPlanes.Unity.UI
{

    public class MissionUIPresenter : MonoBehaviour
    {

        [SerializeField] private MissionUIView _view = null;
        [SerializeField] private MissionUIModel _model = null;

        public void Awake()
        {
            if (!_view | !_model)
            {
                DebugLog.Severe("MUI_Presenter: View or Model is not assigned!!");
                return;
            }


            #region Model=>View

            _model.OnMissionChangedEvent.AddListener((type) => _view.SetMission(type));
            _model.OnMissionProgressEvent.AddListener((val) => _view.SetProgress(val));
            _model.OnMissionCompletedEvent.AddListener(() => _view.ToggleUIPanel(false));

            #endregion

            #region View=>Model

            _view.OnMissionAddedEvent.AddListener((handler) => _model.SetMissionHandler(handler));
            _view.OnMissionUpdatedEvent.AddListener((val) => _model.SetProgress(val));
            _view.OnMissionCompletedEvent.AddListener(() => _model.SetComplete(true));

            #endregion

        }
    }
}
