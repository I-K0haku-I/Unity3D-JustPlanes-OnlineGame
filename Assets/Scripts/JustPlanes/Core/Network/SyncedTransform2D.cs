using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;

namespace JustPlanes.Core.Network
{
    public class SyncedTransform2D
    {
        public PointF position = new PointF();
        public float rotation = 0;
        private int EntityId;

        // private
        private List<float> stateTimeOffsets = new List<float>();
        private Transform2DNetworkData[] stateBuffer;

        private Action<Transform2DNetworkData> transmitState;
        private int stateAmount = 20;
        private int tickRate = 20;
        private float tickTriggerAmount;
        private float tickTimer = 0;

        private float interpolationBackTime = 0.1f;

        private ITransformHolder gameObject;

        private Transform2DNetworkData stateLastSent = new Transform2DNetworkData();
        private Transform2DNetworkData stateToSend = new Transform2DNetworkData();
        private Transform2DNetworkData currentState = new Transform2DNetworkData();

        public SyncedTransform2D(int entityId, ITransformHolder gameObject)
        {
            this.EntityId = entityId;
            tickTriggerAmount = (int)1000 / tickRate;
            this.gameObject = gameObject;

            stateBuffer = new Transform2DNetworkData[stateAmount];
            transmitState = NetworkMagic.RegisterAtServer<Transform2DNetworkData>(0, TransmitState_AtAllClients, entityId);
        }

        public void Update(float deltaTime)
        {
            if (NetworkMagic.IsServer)
            {
                tickTimer += deltaTime;
                if (tickTimer >= tickTriggerAmount)
                {
                    tickTimer -= tickTriggerAmount;
                    if (position != stateLastSent.Position)
                        stateToSend.Position = position;
                    if (rotation != stateLastSent.Rotation)
                        stateToSend.Rotation = rotation;
                    if (position != stateLastSent.Position || rotation != stateLastSent.Rotation)
                    {
                        stateToSend.Timestamp = GetWorldTime();
                        stateLastSent.TransferValuesFrom(stateToSend);
                        transmitState(stateToSend);
                    }
                }
            }
            else
            {
                float serverTime = GetWorldTime() + (float)stateTimeOffsets.Average();
                float interpolationTime = serverTime - interpolationBackTime;
                int stateIndex = 0;
                for (; stateIndex < stateBuffer.Length; stateIndex++)
                {
                    if (stateBuffer[stateIndex].Timestamp <= interpolationTime)
                        break;
                }
                if (stateIndex == stateBuffer.Length)
                    stateIndex--;

                Transform2DNetworkData startState = stateBuffer[Math.Max(stateIndex - 1, 0)];
                Transform2DNetworkData endState = stateBuffer[stateIndex];
                float amount = (interpolationTime - startState.Timestamp) / (endState.Timestamp / startState.Timestamp);
                currentState.Position = DoLerp(startState.Position, endState.Position, amount);
                currentState.Rotation = DoLerp(startState.Rotation, endState.Rotation, amount);
                gameObject.SetPositionAndRotation(currentState.Position.X, currentState.Position.Y, currentState.Rotation);
            }
        }

        private PointF DoLerp(PointF first, PointF second, float amount)
        {
            return new PointF(DoLerp(first.X, second.X, amount), DoLerp(first.Y, second.Y, amount));
        }

        private float DoLerp(float first, float second, float amount)
        {
            return first * (1 - amount) + second * amount;
        }

        private float GetWorldTime()
        {
            return gameObject.GetTime();
        }

        private void TransmitState_AtAllClients(Transform2DNetworkData data)
        {
            stateTimeOffsets.Add(data.Timestamp);
            if (stateTimeOffsets.Count > stateAmount)
                stateTimeOffsets.RemoveAt(0);

            if (stateBuffer[0].Timestamp < data.Timestamp)
            {
                Array.Copy(stateBuffer, 1, stateBuffer, 0, stateBuffer.Length - 1);
                stateBuffer[stateBuffer.Length - 1] = data;
            }

            DebugLog.Warning($"[SyncedTransform2D-{EntityId}: ");
        }
    }

    public interface ITransformHolder
    {
        float GetTime();
        void SetPositionAndRotation(float x, float y, float rotation);
    }

    public class Transform2DNetworkData : NetworkData
    {
        public float Timestamp;
        public PointF Position;
        public float Rotation;

        public void TransferValuesFrom(Transform2DNetworkData stateToSend)
        {
            this.Timestamp = stateToSend.Timestamp;
            this.Position = stateToSend.Position;
            this.Rotation = stateToSend.Rotation;
        }
    }
}