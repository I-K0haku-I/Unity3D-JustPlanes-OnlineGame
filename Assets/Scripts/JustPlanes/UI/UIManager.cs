using System.Collections.Generic;
using UnityEngine;

namespace JustPlanes.UI
{

    public class UIManager : MonoBehaviour
    {

        [SerializeField] public GameObject Canvas = null;
        [SerializeField] public bool HideEachUI = false;
        [SerializeField] public List<GameObject> TogglableUIList;
        [SerializeField] public KeyCode toggleUI = KeyCode.F1;

        private List<ITogglableUI> _togglableUIComps = new List<ITogglableUI>();
        private bool visible = true;

        private void Start()
        {
            foreach (GameObject obj in TogglableUIList)
            {
                foreach (Component comp in obj.GetComponents<Component>())
                {
                    if (comp is ITogglableUI togglableUI)
                    {
                        _togglableUIComps.Add(togglableUI);
                    }
                }
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(toggleUI))
            {
                if (HideEachUI)
                {
                    SetVisible(!visible);
                }
                else
                {
                    Canvas.SetActive(!visible);
                }
                visible = !visible;
            }
        }

        private void SetVisible(bool b)
        {
            foreach(ITogglableUI togglableUI in _togglableUIComps)
            {
                togglableUI.SetVisible(b);
            }
        }

    }
}