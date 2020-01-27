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
        private Action<InputNetworkData> receiveInput;
        private float speed;

        public TestPlane(IGame game, float posX, float posY, int syncedTransformId)
        {
            this.body = game.GetPhysicsManager().CreateBody(posX, posY, 5, 5);
            transform2D = new SyncedTransform2D(syncedTransformId, game, body);
            this.game = game;

            receiveInput = NetworkMagic.RegisterAtServer<InputNetworkData>(0, ReceiveInput_AtServer, 123);
        }

        private void ReceiveInput_AtServer(InputNetworkData data)
        {
            body.SetAngularVelocity(100f * data.h);
            speed += data.v * 10f;
            body.SetVelocity(speed);
        }

        public void Update(float deltaTime)
        {
            transform2D.Update(deltaTime);
            // DebugLog.Warning($"[TestPlane] position: {transform2D.position.ToString()}");
        }

        public void ReceiveInput(float h, float v)
        {
            var data = new InputNetworkData() { h = h, v = v };
            receiveInput?.Invoke(data);
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