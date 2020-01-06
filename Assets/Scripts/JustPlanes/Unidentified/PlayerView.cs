using UnityEngine;

namespace JustPlanes.Unity
{

    public class PlayerView : MonoBehaviour
    {

        public Core.Player player;


        public override string ToString()
        {
            return $"{{PlayerView: {player.Name}, {player.X}, {player.Y}}}";
        }

    }
}
