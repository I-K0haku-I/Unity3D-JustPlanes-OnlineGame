using UnityEngine;

namespace JustPlanes
{

    public class PlayerView : MonoBehaviour
    {

        public Network.Player player;


        public override string ToString()
        {
            return $"{{PlayerView: {player.Name}, {player.X}, {player.Y}}}";
        }

    }
}
