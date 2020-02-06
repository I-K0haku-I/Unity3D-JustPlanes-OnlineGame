using JustPlanes.Core.Network;
using System.Collections.Generic;
using UnityEngine;


namespace JustPlanes.Unity
{
    public class Snake : MonoBehaviour
    {

        public Core.Snake snake = null;


        [SerializeField]
        private int syncId = 4523;

        [SerializeField, Tooltip("Update rate of snake body.\n\nbody will not update until SyncedTransform2D updates this amount of time.")]
        private int updateRate = 0;
        [SerializeField, Tooltip("Z Coordinate of snake head/bodies.")]
        private float zCoordinate = 10F;
        [SerializeField, Tooltip("Amount of snake body instances.\n\ncannot be changed at runtime.")]
        private int snakeLength = 50;
        [SerializeField, Tooltip("Smooth time of snake head.\n\nused for Vector3.SmoothDamp.")]
        private float smoothTime = 0.1F;
        [SerializeField, Tooltip("Max speed of snake head.\n\nused for Vector3.SmoothDamp.")]
        private float maxSpeed = 500F;
        [SerializeField]
        private Object snakeHead = null;
        [SerializeField]
        private Object snakeBody = null;

        private int updateReceived = 0;
        private Queue<GameObject> snakeBodyInstances = new Queue<GameObject>();
        private GameObject snakeHeadInstance = null;

        private Vector3 velocity = Vector3.zero;

        #region Unity Methods


        private void Awake()
        {
            snakeHeadInstance = (GameObject)Instantiate(snakeHead);
            snakeHeadInstance.transform.SetParent(transform);
            snakeHeadInstance.name = "SnakeHead";

            for (int i = 0; i < snakeLength; i++)
            {
                GameObject bodyInstance = (GameObject)Instantiate(snakeBody);
                bodyInstance.transform.SetParent(transform);
                bodyInstance.name = $"SnakeBody-{i + 1}";
                snakeBodyInstances.Enqueue(bodyInstance);
            }

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
            updateReceived++;

            Vector3 nextPos = new Vector3(obj.Position.X, obj.Position.Y, zCoordinate);
            Vector3 smoothed = Vector3.SmoothDamp(snakeHeadInstance.transform.position, nextPos, ref velocity, smoothTime, maxSpeed);
            Quaternion rotation = Quaternion.AngleAxis(Mathf.Atan2(velocity.x, velocity.y) * Mathf.Rad2Deg, Vector3.back);

            snakeHeadInstance.transform.SetPositionAndRotation(smoothed, rotation);
            if (updateReceived > updateRate)
            {
                updateReceived = 0;
                UpdateBody(snakeHeadInstance.transform);
            }
        }

        private void UpdateBody(Transform nextTransform)
        {
            GameObject body = snakeBodyInstances.Dequeue();
            body.transform.SetPositionAndRotation(nextTransform.position, nextTransform.rotation);
            snakeBodyInstances.Enqueue(body);
        }


    }
}