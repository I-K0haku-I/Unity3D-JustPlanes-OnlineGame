using UnityEngine;
using UnityEngine.SceneManagement;

namespace JustPlanes
{
    public class ServerInputGUI : MonoBehaviour
    {

        public string serverDomain = "127.0.0.1";
        private void OnGUI()
        {

            serverDomain = GUI.TextField(new Rect(10, 10, 200, 20), serverDomain, 25);
            if (GUI.Button(new Rect(10, 30, 200, 20), "Start"))
            {
                Network.NetworkManager.instance.serverAddress = serverDomain;
                Network.NetworkManager.instance.StartConnection();
                SceneManager.LoadScene("SampleScene");
            }
        }
    }

}