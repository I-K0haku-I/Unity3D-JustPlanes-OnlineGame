using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Box2DX.Common;

namespace JustPlanes.Unity
{
    public class BezierInteractiveTester : MonoBehaviour
    {
        [SerializeField]
        private float step = .1f;
        [SerializeField]
        private float bezierMultiplier = 1f;

        [SerializeField]
        private List<Transform> positionsTrans;
        [SerializeField]
        private List<Transform> tangentsTrans;

        private List<Vec2> positions;
        private List<Vec2> tangents;
        private List<Vec2> velocities;

        public float length1 = 0f;
        public float length2 = 0f;

        private void Start()
        {
            positions = new List<Vec2>() { Vec2.Zero, Vec2.Zero };
            tangents = new List<Vec2>() { Vec2.Zero, Vec2.Zero };
            velocities = new List<Vec2>() { Vec2.Zero, Vec2.Zero };
            var tmp = Vector3.zero;
            for (int i = 0; i < tangentsTrans.Count; i++)
            {
                tmp = positionsTrans[i].position;
                positions[i] = new Vec2(tmp.x, tmp.y);
                tmp = tangentsTrans[i].position;
                tangents[i] = new Vec2(tmp.x, tmp.y);
                velocities[i] = tangents[i] - positions[i];
            }
        }

        private void Update()
        {
            length1 = (positionsTrans[0].position - tangentsTrans[0].position).magnitude;
            length2 = (positionsTrans[1].position - tangentsTrans[1].position).magnitude;

            var tmp = Vector3.zero;
            for (int i = 0; i < tangentsTrans.Count; i++)
            {
                tmp = positionsTrans[i].position;
                positions[i] = new Vec2(tmp.x, tmp.y);
                tmp = tangentsTrans[i].position;
                tangents[i] = new Vec2(tmp.x, tmp.y);
                velocities[i] = tangents[i] - positions[i];
            }
            for (int i = 0; i < positionsTrans.Count; i++)
            {
                Debug.DrawLine(positionsTrans[i].position, tangentsTrans[i].position, Color.green);
            }

            var distance = 0f;
            for (int c = 0; c < positionsTrans.Count - 1; c++)
            {

                distance = (positionsTrans[c].position - positionsTrans[c + 1].position).magnitude;
                if (distance < step)
                    Debug.DrawLine(positionsTrans[c].position, positionsTrans[c + 1].position, UnityEngine.Color.red);
                else
                {
                    Vector3 v0 = positionsTrans[c].position;
                    Vector3 v1 = Vector3.zero;
                    Vec2 lerpResult = Vec2.Zero;
                    for (float i = 0; i <= distance; i += step)
                    {
                        lerpResult = Core.JPUtils.DoLerpHermite(positions[c], velocities[c] * bezierMultiplier, positions[c + 1], velocities[c + 1] * bezierMultiplier, i / distance);
                        v1.x = lerpResult.X;
                        v1.y = lerpResult.Y;
                        Debug.DrawLine(v0, v1, UnityEngine.Color.red);
                        v0 = v1;
                    }
                    Debug.DrawLine(v1, positionsTrans[c + 1].position, UnityEngine.Color.red);
                }
            }
        }
    }

}