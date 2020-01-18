using System.Drawing;


namespace JustPlanes.Core.Network
{
    public class TestPlane : ITransformHolder
    {
        private SyncedTransform2D transform2D;
        private PointF position;
        public TestPlane()
        {
            transform2D = new SyncedTransform2D(666, this); 
        }

        public void Update(float deltaTime)
        {
            transform2D.Update(deltaTime);
            transform2D.Position.X = transform2D.Position.X + 10f * deltaTime;
            // DebugLog.Warning($"[TestPlane] position: {transform2D.position.ToString()}");
        }

        public float GetTime()
        {
            return Server.Game.GameTimer.ElapsedMilliseconds / 1000f;
        }
    } 
}