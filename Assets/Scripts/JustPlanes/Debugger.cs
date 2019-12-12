using JustPlanes.Network;
using System.Text;
using UnityEngine;

namespace JustPlanes
{

    public class Debugger : MonoBehaviour
    {

        [SerializeField] public KeyCode spawnTestUnit = KeyCode.U;

        private const string glyphs = "abcdefghijklmnopqrstuvwxyz123456789";

        // Update is called once per frame
        private void Update()
        {
            if (Input.GetKeyDown(spawnTestUnit))
            {
                Network.Unit debugUnit = new Network.Unit("local:" + GenerateRandomName(6), Random.Range(-10, 10), Random.Range(-10, 10));
                NetworkManager.instance.AddUnit(debugUnit);
                Debug.Log("Added new debug unit: " + debugUnit.ID + " at " + debugUnit.X + ", " + debugUnit.Y);
            }

        }

        private string GenerateRandomName(int length)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < length; i++)
            {
                sb.Append(glyphs[Random.Range(0, glyphs.Length)]);
            }

            return sb.ToString();
        }
    }
}