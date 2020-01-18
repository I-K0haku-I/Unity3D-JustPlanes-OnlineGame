using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;

namespace JustPlanes.Core.Network
{
    public class SyncedTransform2D
    {
        public PointF Position = new PointF();
        public float Rotation = 0;
        private int EntityId;

        // private
        private List<float> stateTimeOffsets = new List<float>();
        private Transform2DNetworkData[] stateBuffer;

        private Action<Transform2DNetworkData> transmitState;
        private int stateAmount = 20;
        private int tickRate = 1;
        private float tickTriggerAmount;
        private float tickTimer = 0;

        public float InterpolationBackTime = 0.1f;

        private ITransformHolder gameObject;

        private Transform2DNetworkData stateLastSent = new Transform2DNetworkData();
        private Transform2DNetworkData stateToSend = new Transform2DNetworkData();
        private Transform2DNetworkData currentState = new Transform2DNetworkData();

        public SyncedTransform2D(int entityId, ITransformHolder gameObject)
        {
            this.EntityId = entityId;
            tickTriggerAmount = 1f / tickRate;
            this.gameObject = gameObject;

            stateBuffer = new Transform2DNetworkData[stateAmount];
            stateTimeOffsets.Add(0f);
            stateBuffer[0] = new Transform2DNetworkData() { Timestamp = 0.0000000001f, Position = new PointF(0, 0), Rotation = 0f };
            stateBuffer[1] = new Transform2DNetworkData() { Timestamp = 0f, Position = new PointF(0, 0), Rotation = 0f };
            transmitState = NetworkMagic.RegisterAtAllClients<Transform2DNetworkData>(0, TransmitState_AtAllClients, entityId);
        }

        public void Update(float deltaTime)
        {
            if (NetworkMagic.IsServer)
                UpdateServer(deltaTime);
            else
                UpdateClient(deltaTime);
        }

        private void UpdateServer(float deltaTime)
        {
            tickTimer += deltaTime;
            if (!(tickTimer >= tickTriggerAmount)) return;

            tickTimer -= tickTriggerAmount;
            if (Position != stateLastSent.Position)
                stateToSend.Position = Position;
            if (Math.Abs(Rotation - stateLastSent.Rotation) > 0.01)
                stateToSend.Rotation = Rotation;
            if (Position == stateLastSent.Position && Math.Abs(Rotation - stateLastSent.Rotation) < 0.01) return;

            stateToSend.Timestamp = GetWorldTime();
            // DebugLog.Warning($"SENDING {stateToSend.Timestamp}");
            stateLastSent.TransferValuesFrom(stateToSend);
            transmitState(stateToSend);
        }

        private void UpdateClient(float deltaTime)
        {
            float serverTime = GetWorldTime() + (float)stateTimeOffsets.Average();
            float interpolationTime = serverTime - InterpolationBackTime;
            int stateIndex = 0;
            for (; stateIndex < stateBuffer.Length; stateIndex++)
            {
                if (stateBuffer[stateIndex].Timestamp <= interpolationTime)
                    break;
            }
            if (stateIndex == stateBuffer.Length)
                stateIndex--;

            Transform2DNetworkData endState = stateBuffer[Math.Max(stateIndex - 1, 0)];
            Transform2DNetworkData startState = stateBuffer[stateIndex];
            // DebugLog.Warning($"[Transform2D] amount: {endState.Position.ToString()}");
            float amount = 1f;
            if (endState.Timestamp != startState.Timestamp)
                amount = (interpolationTime - startState.Timestamp) / (endState.Timestamp - startState.Timestamp);
            // DebugLog.Warning("[SyncedTransform2D] amount " + amount.ToString());
            currentState.Position = DoLerp(startState.Position, endState.Position, amount);
            currentState.Rotation = DoLerp(startState.Rotation, endState.Rotation, amount);
            DebugLog.Warning("[SyncedTransform2D] new pos " + currentState.Position.ToString());
            // gameObject.SetPositionAndRotation(currentState.Position.X, currentState.Position.Y, currentState.Rotation);
            Position = currentState.Position;
            Rotation = currentState.Rotation;
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
            if (stateBuffer.Length > 1 && data.Timestamp <= stateBuffer[0].Timestamp)
                return;
            
            if (stateTimeOffsets[0] == 0f)
                stateTimeOffsets.RemoveAt(0);
            stateTimeOffsets.Add(data.Timestamp - GetWorldTime());
            if (stateTimeOffsets.Count > stateAmount)
                stateTimeOffsets.RemoveAt(0);

            for (int i = stateBuffer.Length - 1; i >= 1; i--)
                stateBuffer[i] = stateBuffer[i - 1];
            // Array.Copy(stateBuffer, 0, stateBuffer, 1, stateBuffer.Length - 1);
            stateBuffer[0] = data;

            // DebugLog.Warning($"[SyncedTransform2D-{EntityId}: {data.Position.ToString()}");
            // DebugLog.Warning($"[SyncedTransform2D-{EntityId}: {string.Join(", ", stateBuffer.Select((state, i) => state.Position.ToString()))}");
        }
    }

    public interface ITransformHolder
    {
        float GetTime();
        // void SetPositionAndRotation(float x, float y, float rotation);
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