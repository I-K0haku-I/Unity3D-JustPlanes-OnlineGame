using System.Collections.Generic;
using UnityEngine;
using JustPlanes.Core.Network;
using System;
using Box2DX.Common;

namespace JustPlanes.Unity
{
    [RequireComponent(typeof(TestPlane))]
    public class SyncedTransformLineDrawer : MonoBehaviour
    {

        private SyncedTransform2D syncedTransform;

        [SerializeField]
        private bool isDrawPredictedPosition;

        [SerializeField]
        private bool isDrawSimulatedPositions = true;
        [SerializeField]
        [Range(0.01f, 2f)]
        private float bezierMultiplier = 0.4f;

        [SerializeField]
        [Range(0.1f, 2f)]
        private float lineLength = 2;
        [SerializeField]
        private float lineDuration = 1f;

        [SerializeField]
        private bool idDrawReceivedState = false;

        [SerializeField]
        private bool isDrawCalculatedInterpHistory;
        [SerializeField]
        private int statesToSaveInHistory = 10;

        void Awake()
        {
        }

        void Start()
        {
            syncedTransform = GetComponent<TestPlane>().plane.transform2D;
            syncedTransform.OnPositionReceived += HandlePositionReceived;
            syncedTransform.OnStateCalculated += HandleStateCalculated;
        }

        private List<Transform2DNetworkData> stateHistory = new List<Transform2DNetworkData>();
        private void HandleStateCalculated(Transform2DNetworkData data)
        {
            var copyData = new Transform2DNetworkData();
            copyData.TransferValuesFrom(data);
            stateHistory.Add(copyData);
            if (stateHistory.Count > statesToSaveInHistory)
                stateHistory.RemoveAt(0);
        }

        void Update()
        {
            if (idDrawReceivedState)
                DrawReceivedState();
            if (isDrawSimulatedPositions)
                DrawSimulatedBezier();
            if (isDrawCalculatedInterpHistory)
                DrawCalculatedInterpHistory();
            if (isDrawPredictedPosition)
                DrawPredictedPosition();
        }

        private void DrawPredictedPosition()
        {
            // var predictedVec = new Vector3();
            // predictedVec.x = syncedTransform.PredictedPos.X;
            // predictedVec.y = syncedTransform.PredictedPos.Y;
            // DrawQuad(predictedVec, 0.1f, UnityEngine.Color.red);
            var v = Vector3.zero;
            foreach (var state in syncedTransform.predictedStates)
            {
                v.x = state.Position.X;
                v.y = state.Position.Y;
                DrawQuad(v, 0.05f, UnityEngine.Color.blue);
            }
        }

        private void DrawCalculatedInterpHistory()
        {
            if (stateHistory.Count < 2)
                return;
            var p0 = stateHistory[0];
            var v0 = new Vector3(p0.Position.X, p0.Position.Y);
            var p1 = p0;
            var v1 = v0;
            foreach (var state in stateHistory)
            {
                v1 = new Vector3(state.Position.X, state.Position.Y);
                Debug.DrawLine(v0, v1, UnityEngine.Color.red);
                v0 = v1;
            }
        }

        private List<Vector3> positionVector3s = new List<Vector3>();
        private List<Vec2> positionVec2s = new List<Vec2>();
        private List<Vector3> velocityVector3s = new List<Vector3>();
        private List<Vec2> velocityVec2s = new List<Vec2>();
        private List<Vec2> velocityVec2Normalized = new List<Vec2>();
        private int isDrawnCounter = 0;

        private void HandlePositionReceived(Transform2DNetworkData data)
        {
            positionVector3s.Add(new Vector3(data.Position.X, data.Position.Y, 0));
            if (positionVector3s.Count > syncedTransform.StateAmount)
                positionVector3s.RemoveAt(0);

            positionVec2s.Add(data.Position);
            if (positionVec2s.Count > syncedTransform.StateAmount)
                positionVec2s.RemoveAt(0);

            velocityVec2s.Add(new Vec2(data.Velocity.X, data.Velocity.Y));
            if (velocityVec2s.Count > syncedTransform.StateAmount)
                velocityVec2s.RemoveAt(0);
            
            var v = new Vec2(data.Velocity.X, data.Velocity.Y);
            v.Normalize();
            velocityVec2Normalized.Add(v);
            if (velocityVec2Normalized.Count > syncedTransform.StateAmount)
                velocityVec2Normalized.RemoveAt(0);

            velocityVector3s.Add(new Vector3(data.Velocity.X, data.Velocity.Y));
            if (velocityVector3s.Count > syncedTransform.StateAmount)
            {
                velocityVector3s.RemoveAt(0);
                if (isDrawnCounter > 0)
                    isDrawnCounter -= 1;
            }
        }

        private void DrawSimulatedBezier()
        {
            if (positionVec2s.Count < 4 || isDrawnCounter >= positionVector3s.Count)
                return;

            // var distance = (positionVecs[0] - positionVecs[positionVecs.Count - 1]).magnitude;
            // var p0 = positionPoints[positionPoints.Count - 4];
            // var v0 = positionVecs[positionVecs.Count - 4];
            // var p1 = p0;
            // var v1 = v0;
            // var points = new Vec2[4];
            // positionPoints.CopyTo(positionPoints.Count - 4, points, 0, 4);
            // for (float i = 0; i <= distance; i += distance / lineAmount)
            // {
            //     p1 = DoLerp(points, i / distance);
            //     v1 = new Vector3(p1.X, p1.Y, 0);
            //     DrawQuad(v1, 0.03f, UnityEngine.Color.magenta);
            //     Debug.DrawLine(v0, v1, UnityEngine.Color.red);
            //     p0 = p1;
            //     v0 = v1;
            // }
            // Debug.DrawLine(v0, positionVecs[positionVecs.Count - 1], UnityEngine.Color.red);

            var distance = 0f;
            var c = isDrawnCounter;
            for (; c < positionVector3s.Count - 1; c++)
            {
                distance = (positionVector3s[c] - positionVector3s[c + 1]).magnitude;
                if (distance < lineLength)
                    Debug.DrawLine(positionVector3s[c], positionVector3s[c + 1], UnityEngine.Color.red, lineDuration);
                else
                {
                    Vector3 v0 = positionVector3s[c];
                    Vector3 v1 = Vector3.zero;
                    Vec2 lerpResult = Vec2.Zero;
                    for (float i = 0; i <= distance; i += lineLength)
                    {
                        lerpResult = Core.JPUtils.DoLerpHermite(positionVec2s[c], velocityVec2s[c] * bezierMultiplier, positionVec2s[c + 1], velocityVec2s[c + 1] * bezierMultiplier, i / distance);
                        v1.x = lerpResult.X;
                        v1.y = lerpResult.Y;
                        Debug.DrawLine(v0, v1, UnityEngine.Color.red, lineDuration);
                        v0 = v1;
                    }
                    Debug.DrawLine(v1, positionVector3s[c + 1], UnityEngine.Color.red, lineDuration);
                }
            }
            isDrawnCounter = c;
        }

        private Vec2 DoLerp(Vec2[] points, float t)
        {
            return Core.JPUtils.DoLerpCube(points[0], points[1], points[2], points[3], t);
        }

        private void DrawReceivedState()
        {
            // Vector3 pos = Vector3.zero;
            // Vector3 vel = Vector3.zero;
            // foreach (var state in stateHistory)
            // {
            //     pos.x = state.Position.X;
            //     pos.y = state.Position.Y;
            //     vel.x = state.Velocity.X;
            //     vel.y = state.Velocity.Y;
            //     DrawQuad(pos, 0.08f, UnityEngine.Color.yellow);
            //     Debug.DrawLine(pos, vel, UnityEngine.Color.green);
            // }
            for (int i = 0; i < positionVector3s.Count; i++)
            {
                DrawQuad(positionVector3s[i], 0.08f, UnityEngine.Color.yellow);
                Debug.DrawLine(positionVector3s[i], positionVector3s[i] + velocityVector3s[i] * bezierMultiplier, UnityEngine.Color.green);
            }
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