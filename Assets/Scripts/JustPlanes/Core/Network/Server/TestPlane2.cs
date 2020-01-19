using System.Drawing;


namespace JustPlanes.Core.Network
{
    public class TestPlane2 : ITransformHolder
    {
        public static TestPlane2 Instance;

        private SyncedTransform2D transform2D;
        public PointF position = new PointF();
        public float rotation = 0;

        public TestPlane2()
        {
            Instance = this;
            transform2D = new SyncedTransform2D(666, this);
        }

        public void Update(float deltaTime)
        {
            transform2D.Update(deltaTime);
            transform2D.Position.X = transform2D.Position.X + 10 * deltaTime;
            DebugLog.Warning($"[TestPlane2] position: {transform2D.Position.ToString()}, Y: {transform2D.Position.Y}");
        }

        public float GetTime()
        {
            return Server.Game.GameTimer.Elapsed.Milliseconds / 1000;
        }

        public void SetPositionAndRotation(float x, float y, float rotation)
        {
            position.X = x;
            position.Y = y;
            this.rotation = rotation;
        }
    }
}