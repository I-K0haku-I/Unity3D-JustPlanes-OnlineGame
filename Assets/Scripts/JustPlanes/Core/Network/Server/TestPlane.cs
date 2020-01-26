using System;
using System.Diagnostics;
using System.Drawing;
using Box2DX.Common;
using JustPlanes.Core.Network;

namespace JustPlanes.Core
{
    // TODO: redo this to be "Unit" class

    public class TestPlane
    {
        private PhysicsBody body;
        public SyncedTransform2D transform2D;
        private IGame game;
        private float speed;

        public TestPlane(IGame game, float posX, float posY, int syncedTransformId)
        {
            this.body = game.GetPhysicsManager().CreateBody(posX, posY, 5, 5);
            transform2D = new SyncedTransform2D(syncedTransformId, game, body);
            this.game = game;
        }

        public void Update(float deltaTime)
        {
            transform2D.Update(deltaTime);
            // DebugLog.Warning($"[TestPlane] position: {transform2D.position.ToString()}");
        }

        public void ReceiveInput(float h, float v)
        {
            // TODO: WIP
            body.SetAngularVelocity(100f * h);
            speed += v * 10f;
            body.SetVelocity(speed);
        }
    }

    public interface IGame
    {
        PhysicsManager GetPhysicsManager();
        float GetTime();
    }
}