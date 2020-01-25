using System.Collections.Generic;
using UnityEngine;
using JustPlanes.Core.Network;
using D = System.Drawing;
using System;
using System.Drawing;

namespace JustPlanes.Unity
{
    [RequireComponent(typeof(SyncedTransformHolder))]
    public class SyncedTransformLineDrawer : MonoBehaviour
    {

        private SyncedTransform2D syncedTransform;

        [SerializeField]
        [Range(1, 30)]
        private int lineAmount = 2;

        [SerializeField]
        private bool isDrawDebugLine = true;

        void Awake()
        {
        }

        void Start()
        {
            syncedTransform = GetComponent<SyncedTransformHolder>().SyncedTransform;
            syncedTransform.OnPositionReceived += HandlePositionReceived;
        }

        void Update()
        {
            if (isDrawDebugLine)
                DrawBezier();
        }


        private List<Vector3> positionVecs = new List<Vector3>();
        private List<D.PointF> positionPoints = new List<D.PointF>();
        private void HandlePositionReceived(Transform2DNetworkData data)
        {
            positionVecs.Add(new Vector3(data.Position.X, data.Position.Y, 0));
            if (positionVecs.Count > syncedTransform.StateAmount)
                positionVecs.RemoveAt(0);
            positionPoints.Add(data.Position);
            if (positionPoints.Count > syncedTransform.StateAmount)
                positionPoints.RemoveAt(0);
        }

        private void DrawBezier()
        {
            if (positionPoints.Count < 4)
                return;

            var distance = (positionVecs[0] - positionVecs[positionVecs.Count - 1]).magnitude;
            var p0 = positionPoints[positionPoints.Count - 4];
            var v0 = positionVecs[positionVecs.Count - 4];
            var p1 = p0;
            var v1 = v0;
            var points = new PointF[4];
            positionPoints.CopyTo(positionPoints.Count - 4, points, 0, 4);
            for (float i = 0; i <= distance; i += distance/ lineAmount)
            {
                p1 = DoLerp(points, i / distance);
                v1 = new Vector3(p1.X, p1.Y, 0);
                DrawQuad(v1, 0.03f, UnityEngine.Color.magenta);
                Debug.DrawLine(v0, v1, UnityEngine.Color.red);
                p0 = p1;
                v0 = v1;
            }
            Debug.DrawLine(v0, positionVecs[positionVecs.Count - 1], UnityEngine.Color.red);

            foreach (var vec in positionVecs)
                DrawQuad(vec, 0.08f, UnityEngine.Color.yellow);
        }

        private PointF DoLerp(PointF[] points, float t)
        {
            return Core.JPUtils.DoLerpCube(points[0], points[1], points[2], points[3], t);
        }

        // private void DrawBezier()
        // {
        //     Vector3[] posToLerp = new Vector3[10];
        //     positionVecs.Reverse();
        //     int index = 0;
        //     foreach (var pos in positionVecs)
        //     {
        //         DrawQuad(pos, 0.08f, UnityEngine.Color.green);
        //         posToLerp[index++] = pos;
        //         if (index > 4)
        //             break;
        //     }
        //     positionVecs.Reverse();

        //     if (index < 4)
        //         return;

        //     var va = posToLerp[0];
        //     var vb = posToLerp[1];
        //     var vc = posToLerp[2];
        //     var vd = posToLerp[3];
        //     var pa = new D.PointF(posToLerp[0].x, posToLerp[0].y);
        //     var pb = new D.PointF(posToLerp[1].x, posToLerp[1].y);
        //     var pc = new D.PointF(posToLerp[2].x, posToLerp[2].y);
        //     var pd = new D.PointF(posToLerp[3].x, posToLerp[3].y);
        //     var distance = (posToLerp[0] - posToLerp[3]).magnitude;
        //     var p0 = pa;
        //     var p1 = Core.JPUtils.DoLerpCube(pa, pb, pc, pd, stepsForLerpedDebugLine / distance);
        //     var v0 = va;
        //     var v1 = new Vector3(p1.X, p1.Y, 0);
        //     Debug.DrawLine(v0, v1, UnityEngine.Color.red);
        //     for (float i = stepsForLerpedDebugLine; i <= distance; i += stepsForLerpedDebugLine)
        //     {
        //         p0 = p1;
        //         v0 = v1;
        //         p1 = Core.JPUtils.DoLerpCube(pa, pb, pc, pd, i / distance);
        //         v1 = new Vector3(p1.X, p1.Y, 0);
        //         Debug.DrawLine(v0, v1, UnityEngine.Color.red);
        //     }
        //     p0 = p1;
        //     v0 = v1;
        //     p1 = Core.JPUtils.DoLerpCube(pa, pb, pc, pd, 1);
        //     v1 = new Vector3(p1.X, p1.Y, 0);
        //     Debug.DrawLine(v0, v1, UnityEngine.Color.red);
        // }

        // private void DrawDebugLineSimpler()
        // {
        //     Debug.DrawLine(transform.position, newPos, Color.red, 1f);

        //     for (int i = 0; i < syncedTransform.stateBuffer.Length; i++)
        //         if (syncedTransform.stateBuffer[i] != null)
        //             DrawQuad(new Vector3(syncedTransform.stateBuffer[i].Position.X, syncedTransform.stateBuffer[i].Position.Y, 0), 0.08f, Color.green);
        // }

        // private void DrawDebugLine()
        // {
        //     if (syncedTransform.stateBuffer.Length < 0)
        //         return;
        //     Vector3? lastPos = null;
        //     Vector3? pos = null;
        //     for (int i = 0; i < syncedTransform.stateBuffer.Length; i++)
        //     {
        //         if (syncedTransform.stateBuffer[i] == null)
        //             continue;

        //         var state = syncedTransform.stateBuffer[i];
        //         pos = new Vector3(state.Position.X, state.Position.Y, 0);
        //         if (lastPos == null)
        //         {
        //             lastPos = pos;
        //             continue;
        //         }
        //         Debug.DrawLine((Vector3)lastPos, (Vector3)pos, Color.red);
        //         DrawQuad((Vector3)pos, 0.08f, Color.green);
        //         lastPos = pos;
        //     }
        // }

        private void DrawQuad(Vector3 pos, float size, UnityEngine.Color color)
        {
            float halfSize = size / 2f;
            Debug.DrawLine(new Vector3(pos.x - halfSize, pos.y - halfSize, pos.z), new Vector3(pos.x + halfSize, pos.y - halfSize, pos.z), color);
            Debug.DrawLine(new Vector3(pos.x + halfSize, pos.y - halfSize, pos.z), new Vector3(pos.x + halfSize, pos.y + halfSize, pos.z), color);
            Debug.DrawLine(new Vector3(pos.x + halfSize, pos.y + halfSize, pos.z), new Vector3(pos.x - halfSize, pos.y + halfSize, pos.z), color);
            Debug.DrawLine(new Vector3(pos.x - halfSize, pos.y + halfSize, pos.z), new Vector3(pos.x - halfSize, pos.y - halfSize, pos.z), color);
        }
    }
}