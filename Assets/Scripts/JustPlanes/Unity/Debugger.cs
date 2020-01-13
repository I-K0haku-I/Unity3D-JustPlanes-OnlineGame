using System.Collections.Generic;
using System.Text;
using UnityEngine;
using JustPlanes.Core.Network;

namespace JustPlanes.Unity
{

    public class Debugger : MonoBehaviour
    {
        [SerializeField] public KeyCode addTestPlayer = KeyCode.U;
        [SerializeField] public Level loggingLevel = Level.L_ALL;
        [SerializeField] public bool setLogLevel = false;

        private const string glyphs = "abcdefghijklmnopqrstuvwxyz123456789";
        private const string line = "------------------------------------------------------------------------------------------";
        private const string space = "                 ";
        private int deaths = 0;

        private void Awake()
        {
            if (setLogLevel)
            {
                DebugLog.Warning($"<color=#FF69B4>[Debugger] change log level is enabled! {loggingLevel}</color>");
                // DebugLog.SetLevel(loggingLevel);
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(addTestPlayer))
            {
                string name = $"testplayer:{GenerateRandomName(6)}";
                var action = NetworkMagic.GetHandler<Core.NameNetworkData>("BroadcastAdd", GameManager.instance.PlayerManager.Players.EntityId);
                action(new Core.NameNetworkData() { Name = name });
                DebugLog.Info($"<color=#FF69B4>[Debugger] Added new test player: {name}</color>");
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