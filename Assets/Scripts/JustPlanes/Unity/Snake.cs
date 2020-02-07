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

        [SerializeField, Tooltip("Use new updating method, bit laggier than old one.")]
        private bool useNewUpdating = true;
        [SerializeField, Tooltip("Minimal distance between each bodies")]
        private float bodyDistance = 1F;
        [SerializeField, Tooltip("Z Coordinate of snake head/bodies.")]
        private float zCoordinate = 10F;
        [SerializeField, Tooltip("Amount of snake body instances.\n\ncannot be changed at runtime.")]
        private int snakeLength = 50;
        [SerializeField]
        private Object snakeHead = null;
        [SerializeField]
        private Object snakeBody = null;

        private GameObject[] snakeBodyInstances = null;
        private GameObject snakeBodyFirstInstance = null;
        private GameObject snakeHeadInstance = null;

        private int lastUpdated = 0;

        #region Unity Methods


        private void Awake()
        {
            snakeHeadInstance = (GameObject)Instantiate(snakeHead);
            snakeHeadInstance.transform.SetParent(transform);
            snakeHeadInstance.name = "SnakeHead";

            List<GameObject> bodies = new List<GameObject>();
            for (int i = 0; i < snakeLength; i++)
            {
                GameObject bodyInstance = (GameObject)Instantiate(snakeBody);
                bodyInstance.transform.SetParent(transform);
                bodyInstance.name = $"SnakeBody-{i + 1}";
                bodies.Add(bodyInstance);
            }
            snakeBodyInstances = bodies.ToArray();
            snakeBodyFirstInstance = snakeBodyInstances[0];
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


            Vector3 headPos = snakeHeadInstance.transform.position;
            Vector3 firstBodyPos = snakeBodyFirstInstance.transform.position;
            float distanceBetween = Vector3.Distance(headPos, firstBodyPos);

            if (distanceBetween > bodyDistance)
            {
                UpdateBody(snakeHeadInstance.transform);
            }
        }

        private void UpdateBody(Transform nextTransform)
        {
            if (useNewUpdating)
            {
                MoveBody(nextTransform);
            }
            else
            {
                TeleportBody(nextTransform);
            }
        }

        private void MoveBody(Transform nextTransform)
        {
            // Reversed loop
            for (int i = snakeBodyInstances.Length - 1; i >= 0; i--)
            {
                if (i == 0)
                {
                    snakeBodyInstances[0].transform.SetPositionAndRotation(nextTransform.position, nextTransform.rotation);
                }
                else
                {
                    Transform targetTransform = snakeBodyInstances[i - 1].transform;
                    snakeBodyInstances[i].transform.SetPositionAndRotation(targetTransform.position, targetTransform.rotation);
                }
            }
            snakeBodyFirstInstance = snakeBodyInstances[0];
        }

        private void TeleportBody(Transform nextTransform)
        {
            int nextToUpdate = lastUpdated + 1 < snakeBodyInstances.Length ? lastUpdated + 1 : 0;
            snakeBodyInstances[nextToUpdate].transform.SetPositionAndRotation(nextTransform.position, nextTransform.rotation);
            lastUpdated = nextToUpdate;
            snakeBodyFirstInstance = snakeBodyInstances[lastUpdated];
        }


    }
}