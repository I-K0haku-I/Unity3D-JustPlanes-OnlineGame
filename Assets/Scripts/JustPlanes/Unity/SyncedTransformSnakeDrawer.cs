using JustPlanes.Core.Network;
using System.Collections.Generic;
using UnityEngine;

namespace JustPlanes.Unity
{

    [RequireComponent(typeof(TestPlane))]
    public class SyncedTransformSnakeDrawer : MonoBehaviour
    {
        [SerializeField, Tooltip("Update rate of snake body.\n\nbody will not update until SyncedTransform2D updates this amount of time.")]
        private int updateRate = 10;
        [SerializeField, Tooltip("Z Coordinate of snake head/bodies.")]
        private float zCoordinate = 10F;
        [SerializeField, Tooltip("Amount of snake body instances.\n\ncannot be changed at runtime.")]
        private int snakeLength = 5;
        [SerializeField, Tooltip("Smooth time of snake head.\n\nused for Vector3.SmoothDamp.")]
        private float smoothTime = 5F;
        [SerializeField, Tooltip("Max speed of snake head.\n\nused for Vector3.SmoothDamp.")]
        private float maxSpeed = 5F;
        [SerializeField, Tooltip("Parent transform for snake head/bodies.\n\nall head/bodies goes into below this transform.")]
        private Transform parentTransform = null;
        [SerializeField]
        private Object snakeHead = null;
        [SerializeField]
        private Object snakeBody = null;

        private SyncedTransform2D syncedTransform = null;

        private int updateReceived = 0;
        private Queue<GameObject> snakeBodyInstances = new Queue<GameObject>();
        private GameObject snakeHeadInstance = null;

        private Vector3 velocity = Vector3.zero;
        private Vector3 nextPos = Vector3.zero;
        private bool shouldUpdateBody = false;


        #region Unity Methods

        private void Awake()
        {
            snakeHeadInstance = (GameObject)Instantiate(snakeHead);
            snakeHeadInstance.transform.SetParent(parentTransform);
            snakeHeadInstance.name = "SnakeHead";

            for (int i = 0; i < snakeLength; i++)
            {
                GameObject bodyInstance = (GameObject)Instantiate(snakeBody);
                bodyInstance.transform.SetParent(parentTransform);
                bodyInstance.name = $"SnakeBody-{i + 1}";
                snakeBodyInstances.Enqueue(bodyInstance);
            }
        }

        private void Start()
        {
            syncedTransform = GetComponent<TestPlane>().plane.transform2D;
            syncedTransform.OnStateCalculated += HandleStateCalculated;
        }

        private void Update()
        {
            Vector3 smoothed = Vector3.SmoothDamp(snakeHeadInstance.transform.position, nextPos, ref velocity, smoothTime, maxSpeed);
            Quaternion rotation = Quaternion.AngleAxis(Mathf.Atan2(velocity.x, velocity.y) * Mathf.Rad2Deg, Vector3.back);

            snakeHeadInstance.transform.SetPositionAndRotation(smoothed, rotation);
            if (shouldUpdateBody)
            {
                UpdateBody(snakeHeadInstance.transform);
            }
        }

        #endregion Unity Methods


        private void HandleStateCalculated(Transform2DNetworkData obj)
        {
            updateReceived++;
            nextPos = new Vector3(obj.Position.X, obj.Position.Y, zCoordinate);

            if (updateReceived >= updateRate)
            {
                updateReceived = 0;
                shouldUpdateBody = true;
            }
        }

        private void UpdateBody(Transform nextTransform)
        {
            shouldUpdateBody = false;
            GameObject body = snakeBodyInstances.Dequeue();
            body.transform.SetPositionAndRotation(nextTransform.position, nextTransform.rotation);
            snakeBodyInstances.Enqueue(body);
        }

    }
}
