using JustPlanes.Core.Network;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JustPlanes.Unity
{
    public class TrailRendererSnake : MonoBehaviour
    {

        public Core.Snake snake = null;

        [SerializeField]
        private int syncId = 4523;
        [SerializeField]
        private float zCoordinate = -10f;
        [SerializeField]
        private Object snakeHead = null;

        private GameObject snakeHeadInstance = null;

        private int lastUpdated;

        #region Unity Methods


        private void Awake()
        {
            snakeHeadInstance = (GameObject)Instantiate(snakeHead);
            snakeHeadInstance.transform.SetParent(transform);
            snakeHeadInstance.name = "SnakeHead";
        }

        private void Start()
        {
            snake = new Core.Snake(GameManager.instance, 0, 0, syncId);
        }
        private Vector3 nextPos = Vector3.zero;
        private Quaternion rotation = Quaternion.identity;
        private void Update()
        {
            snake.Update(Time.deltaTime);

            nextPos = new Vector3(snake.transform2D.Position.X, snake.transform2D.Position.Y, zCoordinate);
            rotation = Quaternion.AngleAxis(snake.transform2D.Rotation, Vector3.back);
            snakeHeadInstance.transform.SetPositionAndRotation(nextPos, rotation);
        }


        #endregion Unity Methods
    }
}
