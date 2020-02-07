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
        private float zCoordinate = 10f;
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
            snake.transform2D.OnStateCalculated += HandleStateCalculated;
        }

        private void Update()
        {
            snake.Update(Time.deltaTime);
        }


        #endregion Unity Methods



        private void HandleStateCalculated(Transform2DNetworkData obj)
        {
            Vector3 nextPos = new Vector3(obj.Position.X, obj.Position.Y, zCoordinate);
            Quaternion rotation = Quaternion.AngleAxis(obj.Rotation, Vector3.back);

            snakeHeadInstance.transform.SetPositionAndRotation(nextPos, rotation);
        }

    }
}
