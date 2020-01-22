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
        private bool isDrawDebugLine = true;

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
            newPos.z = transform.position.z;
        }

        private Vector3 newPos = Vector3.zero;

        // Update is called once per frame
        void Update()
        {
            syncedTransform.Update(Time.deltaTime);
            // transform.position = new Vector3(transform.position.x + 10f * Time.deltaTime, transform.position.y, transform.position.z);
            newPos.x = syncedTransform.Position.X;
            newPos.y = syncedTransform.Position.Y;
            // newPos = Vector3.Lerp(transform.position, newPos, Mathf.SmoothStep(0f, 1f, positionLerpRate));
            transform.SetPositionAndRotation(newPos, new Quaternion(0, 0, syncedTransform.Rotation, 0));

            if (isDrawDebugLine)
                DrawDebugLine();

            // float t;
            // Vector2 position;
            // for (int i = 0; i < syncedTransform.stateBuffer.Length; i++)
            // {
            //     t = i / (syncedTransform.stateBuffer.Length - 1f);
            //     position = (2f * t * t * t - 3f * t * t + 1f) * p0;
            // }
        }

        private void DrawDebugLine()
        {
            if (syncedTransform.stateBuffer.Length < 0)
                return;
            Vector3? lastPos = null;
            Vector3? pos = null;
            for (int i = 0; i < syncedTransform.stateBuffer.Length; i++)
            {
                if (syncedTransform.stateBuffer[i] == null)
                    continue;

                var state = syncedTransform.stateBuffer[i];
                pos = new Vector3(state.Position.X, state.Position.Y, 0);
                if (lastPos == null)
                {
                    lastPos = pos;
                    continue;
                }
                Debug.DrawLine((Vector3)lastPos, (Vector3)pos, Color.red);
                DrawQuad((Vector3)pos, 0.08f, Color.green);
                lastPos = pos;
            }
        }

        private void DrawQuad(Vector3 pos, float size, Color color)
        {
            float halfSize = size / 2f;
            Debug.DrawLine(new Vector3(pos.x - halfSize, pos.y - halfSize, pos.z), new Vector3(pos.x + halfSize, pos.y - halfSize, pos.z), color);
            Debug.DrawLine(new Vector3(pos.x + halfSize, pos.y - halfSize, pos.z), new Vector3(pos.x + halfSize, pos.y + halfSize, pos.z), color);
            Debug.DrawLine(new Vector3(pos.x + halfSize, pos.y + halfSize, pos.z), new Vector3(pos.x - halfSize, pos.y + halfSize, pos.z), color);
            Debug.DrawLine(new Vector3(pos.x - halfSize, pos.y + halfSize, pos.z), new Vector3(pos.x - halfSize, pos.y - halfSize, pos.z), color);
        }
    }
}