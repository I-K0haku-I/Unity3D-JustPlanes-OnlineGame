using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using Box2DX.Common;
using JustPlanes.Core.Network;

namespace JustPlanes.Core
{
    // TODO: redo this to be "Unit" class

    public class TestPlane
    {
        public PhysicsBody body;
        public SyncedTransform2D transform2D;
        private IGame game;
        private Action<InputNetworkData> handleInput;
        private float speed;
        private Queue<InputNetworkData> inputQueue = new Queue<InputNetworkData>();
        private int tickRate = 10;
        private float tickTriggerAmount;
        private float tickTimer = 0;

        public TestPlane(IGame game, float posX, float posY, int syncedTransformId)
        {
            tickTriggerAmount = 1f / tickRate;
            this.body = game.GetPhysicsManager().CreateBody(posX, posY, 5, 5);
            speed = 10;
            this.body.SetVelocity(speed);
            transform2D = new SyncedTransform2D(syncedTransformId, game, body);
            this.game = game;

            handleInput = NetworkMagic.RegisterAtServer<InputNetworkData>(0, HandleInput_AtServer, 123);
        }

        private void HandleInput_AtServer(InputNetworkData data)
        {
            DebugLog.Warning($"[TestPlane] Received input horiz: {data.h}, vert: {data.v}");
            inputQueue.Enqueue(data);
        }

        public void Update(float deltaTime)
        {
            tickTimer += deltaTime;

            while (inputQueue.Count > 0)
            {
                var data = inputQueue.Dequeue();
                body.SetAngularVelocity(100f * data.h);
                speed += data.v * 10f * 0.1f;
                body.SetVelocity(speed);
            }
            transform2D.Update(deltaTime);

            // Looks complicated, but all this is doing is making sure the speed gets updated
            Vec2 dir = body.GetDirection(transform2D.Rotation);
            Vec2 vel = transform2D.Velocity;
            // this checks if the velocity is backwards or forwards
            if (System.Math.Abs((body.Dot(dir, vel) / (dir.Length() * vel.Length())) + 1f) < 0.1f)
            {
                speed = vel.Length() * -1;
            }
            else
            {
                speed = vel.Length();
            }

            DebugLog.Warning($"[TestPlane] position: X: {transform2D.Position.X}, Y: {transform2D.Position.Y}");
        }

        public void HandleInput(float h, float v)
        {
            if (!(tickTimer >= tickTriggerAmount)) return;
            tickTimer -= tickTriggerAmount;
            var data = new InputNetworkData() { h = h, v = v };
            HandleInput_AtServer(data);
            handleInput?.Invoke(data);
        }
    }

    public class InputNetworkData : NetworkData
    {
        public float h;
        public float v;
    }

    public interface IGame
    {
        PhysicsManager GetPhysicsManager();
        float GetTime();
    }
}