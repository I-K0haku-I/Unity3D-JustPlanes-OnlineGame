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
            transform2D.position.X = transform2D.position.X + 10 * deltaTime;
            DebugLog.Warning($"[TestPlane] position: {transform2D.position.ToString()}, Y: {transform2D.position.Y}");
        }

        public float GetTime()
        {
            return Server.Game.GameTimer.Elapsed.Milliseconds / 1000;
        }

        public void SetPositionAndRotation(float x, float y, float rotation)
        {
            position.X = x;
            position.Y = y;
        }
    } 
}