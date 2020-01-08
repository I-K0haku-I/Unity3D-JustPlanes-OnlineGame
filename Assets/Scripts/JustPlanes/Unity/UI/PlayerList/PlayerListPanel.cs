using System;
using TMPro;
using UnityEngine;

namespace JustPlanes.Unity.UI
{
    //
    public class PlayerListPanel : MonoBehaviour
    {
        [SerializeField]
        public TextMeshProUGUI nameUI;

        private void Awake()
        {

        }

        public void SetPlayerName(string name)
        {
            nameUI.SetText(name);
        }
    }
}
