using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using JustPlanes.Core;

namespace JustPlanes.Unity.UI
{

    public class MissionPanelView : MonoBehaviour, ITogglableUI
    {
        private static readonly string _prefix = "MP View:";

        [SerializeField] private GameObject _panel = null;
        [SerializeField] private TextMeshProUGUI _descriptionText = null;
        [SerializeField] private TextMeshProUGUI _progressText = null;
        [SerializeField] private Slider _progressBar = null;

        private bool _inProgress = false;
        private string _descriptionFormat = "";
        private string _progressFormat = "";

        private void Awake()
        {
            if (!_panel | !_descriptionText | !_progressText | !_progressBar)
            {
                DebugLog.Severe(this, $"{_prefix} UI Component not assigned!!");
                Destroy(this);
                return;
            }

            SetVisible(false);
        }

        public void Assign(string descriptionFormat, string progressFormat, int currProg, int targProg, bool update = true)
        {
            DebugLog.Finest(this, $"{_prefix} Assigning message: {currProg}, {targProg}");

            _inProgress = true;

            _panel.transform.SetAsLastSibling();
            _descriptionFormat = descriptionFormat;
            _progressFormat = progressFormat;

            _progressBar.maxValue = targProg;

            if (update)
            {
                UpdateMessage(currProg, targProg);
            }

            SetVisible(true);
        }

        public void UpdateMessage(int currProg, int targProg)
        {
            DebugLog.Finest(this, $"{_prefix} Updating message: {currProg}, {targProg}");

            _progressBar.value = currProg;
            _descriptionText.SetText(FormatMessage(_descriptionFormat, currProg, targProg));
            _progressText.SetText(FormatMessage(_progressFormat, currProg, targProg));
        }

        public void Complete(int currProg, int targProg)
        {
            DebugLog.Finest(this, $"{_prefix} Completing mission: {currProg}, {targProg}");

            _inProgress = false;

            _descriptionFormat = "Mission completed!";
            UpdateMessage(currProg, targProg);

            StartCoroutine("FinalizeComplete");
        }

        private IEnumerator FinalizeComplete()
        {
            DebugLog.Finest(this, $"{_prefix} Coroutine started!!");
            yield return new WaitForSeconds(5.0F);
            if (!_inProgress)
            {
                SetVisible(false);
            }
            DebugLog.Finest(this, $"{_prefix} Coroutine ended!!");
        }


        private string FormatMessage(string msg, int currProg, int targProg)
        {
            DebugLog.Finest(this, $"{_prefix} Formatting message: {msg}, {currProg}, {targProg}");

            string formatted = msg;
            formatted = formatted.Replace("{progress}", "" + currProg);
            formatted = formatted.Replace("{max}", "" + targProg);

            DebugLog.Finest(this, $"{_prefix} Formatted message: {formatted}");
            return formatted;
        }

        #region ITogglableUI

        public void SetVisible(bool visible)
        {
            DebugLog.Finer(this, $"{_prefix} Updating visibility of {gameObject.name} to {visible}");

            _panel.SetActive(visible);
        }

        public bool IsVisible()
        {
            return _panel.activeInHierarchy;
        }

        #endregion ITogglableUI

    }
}
