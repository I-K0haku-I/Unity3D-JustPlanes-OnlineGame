using UnityEngine;
using JustPlanes.Core.Network;

namespace JustPlanes.Unity
{
    [RequireComponent(typeof(SyncedTransformHolder))]
    public class TestPlane : MonoBehaviour
    {
        private SyncedTransform2D syncedTransform;
        private Vector3 newPos = Vector3.zero;

        // public void SetPositionAndRotation(float x, float y, float rotation)
        // {
        //     Vector3 newPos = Vector3.Lerp(transform.position, new Vector3(x, y, transform.position.z), positionLerpRate);
        //     transform.SetPositionAndRotation(newPos, new Quaternion(0, 0, rotation, 0));
        // }

        void Start()
        {
            syncedTransform = GetComponent<SyncedTransformHolder>().SyncedTransform;
            newPos.z = transform.position.z;
        }


        // Update is called once per frame
        void Update()
        {
            syncedTransform.Update(Time.deltaTime);
            // transform.position = new Vector3(transform.position.x + 10f * Time.deltaTime, transform.position.y, transform.position.z);
            newPos.x = syncedTransform.Position.X;
            newPos.y = syncedTransform.Position.Y;
            // newPos = Vector3.Lerp(transform.position, newPos, Mathf.SmoothStep(0f, 1f, positionLerpRate));
            transform.SetPositionAndRotation(newPos, new Quaternion(0, 0, syncedTransform.Rotation, 0));

            // float t;
            // Vector2 position;
            // for (int i = 0; i < syncedTransform.stateBuffer.Length; i++)
            // {
            //     t = i / (syncedTransform.stateBuffer.Length - 1f);
            //     position = (2f * t * t * t - 3f * t * t + 1f) * p0;
            // }
        }
    }
}