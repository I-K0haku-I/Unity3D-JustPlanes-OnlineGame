using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using Box2DX.Common;

namespace JustPlanes.Core.Network
{
    public class SyncedTransform2D
    {
        public event Action<Transform2DNetworkData> OnPositionReceived;
        public event Action<Transform2DNetworkData> OnStateCalculated;
        public Vec2 Position = new Vec2();
        public float Rotation = 0;
        public Vec2 Velocity = new Vec2();
        private int EntityId;

        // private
        private List<float> stateTimeOffsets = new List<float>();
        public Transform2DNetworkData[] stateBuffer;

        private Action<Transform2DNetworkData> transmitState;
        public int StateAmount = 20;
        private int tickRate = 10;
        private float tickTriggerAmount;
        private float tickTimer = 0;

        public float InterpolationBackTime = 0.1f;

        public Vec2 PredictedPos = Vec2.Zero;

        private IGame game;
        private PhysicsBody body;

        private Transform2DNetworkData stateLastSent = new Transform2DNetworkData();
        private Transform2DNetworkData stateToSend = new Transform2DNetworkData();
        private Transform2DNetworkData currentState = new Transform2DNetworkData();
        private List<Transform2DNetworkData> lastSecondStates = new List<Transform2DNetworkData>();
        private float lastSecondStateTimer = 0f;
        private float lastSecondStateSaveRate = 0.1f;
        private float lastSecondStateSaveTime = 1f;

        public SyncedTransform2D(int entityId, IGame game, PhysicsBody body = null)
        {
            this.EntityId = entityId;
            tickTriggerAmount = 1f / tickRate;
            this.game = game;
            this.body = body;

            stateBuffer = new Transform2DNetworkData[StateAmount];
            stateTimeOffsets.Add(0f);
            stateBuffer[0] = new Transform2DNetworkData() { Timestamp = 0.0000000001f, Position = new Vec2(0, 0), Rotation = 0f };
            stateBuffer[1] = new Transform2DNetworkData() { Timestamp = 0f, Position = new Vec2(0, 0), Rotation = 0f };
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
            if (body != null)
            {
                Position = body.body.GetPosition();
                Rotation = body.body.GetAngle();
                Velocity = body.body.GetLinearVelocity();
            }

            tickTimer += deltaTime;
            if (!(tickTimer >= tickTriggerAmount)) return;

            tickTimer -= tickTriggerAmount;
            // if (Position != stateLastSent.Position)
            stateToSend.Position = Position;
            // if (System.Math.Abs(Rotation - stateLastSent.Rotation) > 0.01)
            stateToSend.Rotation = Rotation;
            stateToSend.Velocity = Velocity;
            if (Position == stateLastSent.Position && System.Math.Abs(Rotation - stateLastSent.Rotation) < 0.01) return;

            stateToSend.Timestamp = GetWorldTime();
            // DebugLog.Warning($"SENDING {stateToSend.Timestamp}");
            stateLastSent.TransferValuesFrom(stateToSend);
            transmitState(stateToSend);
            DebugLog.Warning($"[SyncedTransform2D] state {stateToSend.Timestamp} sent: X: {stateToSend.Position.X}, Y: {stateToSend.Position.Y}");
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

            var startState = stateBuffer[System.Math.Max(stateIndex - 3, 0)];
            var midState = stateBuffer[System.Math.Max(stateIndex - 2, 0)];
            var midState2 = stateBuffer[System.Math.Max(stateIndex - 1, 0)];
            var endState = stateBuffer[stateIndex];
            // DebugLog.Warning($"[Transform2D] amount: {endState.Position.ToString()}");

            float amount = 1f;
            if (endState.Timestamp != startState.Timestamp)
                amount = (interpolationTime - startState.Timestamp) / (endState.Timestamp - startState.Timestamp);
            currentState.Timestamp = interpolationTime;
            // DebugLog.Warning("[SyncedTransform2D] amount " + amount.ToString());

            // currentState.Position = JPUtils.DoLerpHermite(startState.Position, startState.Velocity, midState.Position, midState.Velocity, amount);
            currentState.Position = JPUtils.DoLerpCube(startState.Position, midState.Position, midState2.Position, endState.Position, amount);
            currentState.Velocity = JPUtils.DoLerpCube(startState.Velocity, midState.Velocity, midState2.Velocity, endState.Velocity, amount);
            currentState.Rotation = JPUtils.DoLerp(startState.Rotation, endState.Rotation, amount);
            OnStateCalculated?.Invoke(currentState);
            DebugLog.Warning($"[SyncedTransform2D] new pos X: {currentState.Position.X}, Y: {currentState.Position.Y}");
            // gameObject.SetPositionAndRotation(currentState.Position.X, currentState.Position.Y, currentState.Rotation);

            // do something else here to consider physic state too instead of just setting it
            // body.body.SetXForm(currentState.Position, currentState.Rotation);

            // Position = currentState.Position;
            // Rotation = currentState.Rotation;
            // Velocity = currentState.Velocity;

            if (body != null)
            {
                PredictedPos = currentState.Position + currentState.Velocity * InterpolationBackTime;
                // could predict rotation here too
                DebugLog.Warning($"[SyncedTransform2D] new predicted pos X: {PredictedPos.X}, Y: {PredictedPos.Y}");
                body.SetWithLerp(PredictedPos, currentState.Rotation, currentState.Velocity, deltaTime);
                Position = body.body.GetPosition();
                Rotation = body.body.GetAngle();
                Velocity = body.body.GetLinearVelocity();
            }
            else
            {
                Position = currentState.Position;
                Rotation = currentState.Rotation;
                Velocity = currentState.Velocity;
            }

            lastSecondStateTimer += deltaTime;
            if (lastSecondStateTimer > lastSecondStateSaveRate)
            {
                lastSecondStateTimer -= lastSecondStateSaveRate;
                lastSecondStates.Add(currentState.GetCopy());
                if (lastSecondStates.Count > (int)(lastSecondStateSaveTime/lastSecondStateSaveRate))
                    lastSecondStates.RemoveAt(0);
            }
        }

        private float GetWorldTime()
        {
            return game.GetTime();
        }

        private void TransmitState_AtAllClients(Transform2DNetworkData data)
        {
            if (stateBuffer.Length > 1 && data.Timestamp <= stateBuffer[0].Timestamp)
                return;

            if (stateTimeOffsets[0] == 0f)
                stateTimeOffsets.RemoveAt(0);
            stateTimeOffsets.Add(data.Timestamp - GetWorldTime());
            if (stateTimeOffsets.Count > StateAmount)
                stateTimeOffsets.RemoveAt(0);

            for (int i = stateBuffer.Length - 1; i >= 1; i--)
                stateBuffer[i] = stateBuffer[i - 1];
            // Array.Copy(stateBuffer, 0, stateBuffer, 1, stateBuffer.Length - 1);
            stateBuffer[0] = data;

            // TODO: implement correction of position
            // first store last second of states every 0.1s
            // use timestamp to get a past state, either use lerping or just take the closest state.
            // test if it is correct, if not, then reset all and world.step() with time offset
            foreach (var state in lastSecondStates)
            {
                if (data.Timestamp > state.Timestamp)
                    continue;
                
                if (System.Math.Abs((data.Position - state.Position).Length()) < 0.1f)
                    break;
                
                // TODO: implement rewind
                body.DoCrazyShit(state);
                Position = body.body.GetPosition();
                Rotation = body.body.GetAngle();
                Velocity = body.body.GetLinearVelocity();
                break;
            }

            OnPositionReceived?.Invoke(data);

            // DebugLog.Warning($"[SyncedTransform2D-{EntityId}: {data.Position.ToString()}");
            // DebugLog.Warning($"[SyncedTransform2D-{EntityId}: {string.Join(", ", stateBuffer.Select((state, i) => state.Position.ToString()))}");
        }
    }

    // public interface IGame
    // {
    //     Vec2 GetPosition();
    //     float GetRotation();
    //     float GetTime();
    //     void SetPosition(Vec2 position);
    //     void SetRotation(float rotation);
    //     // void SetPositionAndRotation(float x, float y, float rotation);
    // }

    public class Transform2DNetworkData : NetworkData
    {
        public float Timestamp;
        public Vec2 Position;
        public float Rotation;
        public Vec2 Velocity;

        public void TransferValuesFrom(Transform2DNetworkData stateToSend)
        {
            this.Timestamp = stateToSend.Timestamp;
            this.Position = stateToSend.Position;
            this.Rotation = stateToSend.Rotation;
            this.Velocity = stateToSend.Velocity;
        }

        public Transform2DNetworkData GetCopy()
        {
            var state = new Transform2DNetworkData();
            state.TransferValuesFrom(this);
            return state;
        }
    }
}