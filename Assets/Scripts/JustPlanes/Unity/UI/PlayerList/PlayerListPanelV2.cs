using TMPro;
using UnityEngine;

namespace JustPlanes.Unity.UI
{
    //
    public class PlayerListPanelV2 
    {
        [SerializeField]
        private GameObject Panel;
        [SerializeField]
        private TextMeshProUGUI PlayerNameText;

        public void SetPlayerName(string name)
        {
            PlayerNameText.SetText(name);
        }

    }
}
