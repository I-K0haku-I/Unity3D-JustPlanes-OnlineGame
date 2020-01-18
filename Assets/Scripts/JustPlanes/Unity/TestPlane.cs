using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JustPlanes.Core.Network;

namespace JustPlanes.Unity
{
    public class TestPlane : MonoBehaviour, ITransformHolder
    {
        private SyncedTransform2D syncedTransform;

        [SerializeField]
        [Range(0f, 1f)]
        private float positionLerpRate = 0.3f;

        public float GetTime()
        {
            return Time.time;
        }

        // public void SetPositionAndRotation(float x, float y, float rotation)
        // {
        //     Vector3 newPos = Vector3.Lerp(transform.position, new Vector3(x, y, transform.position.z), positionLerpRate);
        //     transform.SetPositionAndRotation(newPos, new Quaternion(0, 0, rotation, 0));
        // }

        // Start is called before the first frame update
        void Start()
        {
            syncedTransform = new SyncedTransform2D(666, this);
            newPos.z = transform.position.y;
        }

        private Vector3 newPos = new Vector3();

        // Update is called once per frame
        void Update()
        {
            syncedTransform.Update(Time.deltaTime);

            newPos.x = syncedTransform.Position.X;
            newPos.y = syncedTransform.Position.Y;
            newPos = Vector3.Lerp(transform.position, newPos, Mathf.SmoothStep(0f, 1f, positionLerpRate));
            transform.SetPositionAndRotation(newPos, new Quaternion(0, 0, syncedTransform.Rotation, 0));
        }
    }
}