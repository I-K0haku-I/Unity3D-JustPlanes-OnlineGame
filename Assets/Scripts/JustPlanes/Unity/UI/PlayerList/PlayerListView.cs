using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace JustPlanes.Unity.UI
{

    public class PlayerListView : MonoBehaviour
    {
        private Dictionary<string, TextMeshProUGUI> playerPanels = new Dictionary<string, TextMeshProUGUI>();

        public void AddPanel(string name)
        {
            GameObject panelObj = new GameObject($"Panel for {name}");
            TextMeshProUGUI textComp = panelObj.AddComponent<TextMeshProUGUI>();
            textComp.SetText(name);

            playerPanels.Add(name, textComp);
        }

        public void RemovePanel(string name)
        {
            if (playerPanels.TryGetValue(name, out TextMeshProUGUI text))
            {
                Destroy(text.gameObject);
            }
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

    }
}