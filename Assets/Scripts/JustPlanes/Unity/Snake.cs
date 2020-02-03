using UnityEngine;


namespace JustPlanes.Unity
{
    public class Snake : MonoBehaviour
    {
        [SerializeField]
        private int syncId = 4523;
        
        public Core.Snake snake;

        private void Start()
        {
            snake = new Core.Snake(GameManager.instance, 0, 0, syncId);
        }

        private void Update()
        {
            snake.Update(Time.deltaTime);
        }
    }
}