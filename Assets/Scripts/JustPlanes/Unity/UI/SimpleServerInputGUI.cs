using UnityEngine;
using UnityEngine.SceneManagement;

namespace JustPlanes.Unity.UI
{
    public class SimpleServerInputGUI : MonoBehaviour
    {

        public string serverDomain = "127.0.0.1";
        private void OnGUI()
        {

            serverDomain = GUI.TextField(new Rect(10, 10, 200, 20), serverDomain, 25);
            if (GUI.Button(new Rect(10, 30, 200, 20), "Start"))
            {
                NetworkManagerOld.instance.serverAddress = serverDomain;
                NetworkManagerOld.instance.StartConnection();
                SceneManager.LoadScene("SampleScene");
            }
        }
    }

}