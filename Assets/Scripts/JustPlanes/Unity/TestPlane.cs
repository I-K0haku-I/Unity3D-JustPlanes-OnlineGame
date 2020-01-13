using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JustPlanes.Core.Network;

namespace JustPlanes.Unity
{
    public class TestPlane : MonoBehaviour, ITransformHolder
    {
        private SyncedTransform2D transform2D;

        public float GetTime()
        {
            return Time.time;
        }

        public void SetPositionAndRotation(float x, float y, float rotation)
        {
            DebugLog.Warning(x.ToString());
            gameObject.transform.SetPositionAndRotation(new Vector3(x, y, 0), new Quaternion(0, 0, rotation, 0));
        }

        // Start is called before the first frame update
        void Start()
        {
            transform2D = new SyncedTransform2D(666, this);

        }

        // Update is called once per frame
        void Update()
        {
            transform2D.Update(Time.deltaTime);
        }
    }
}