using System;
using System.Diagnostics;
using System.Drawing;


namespace JustPlanes.Core.Network
{
    public class TestPlane : ITransformHolder
    {
        private SyncedTransform2D transform2D;
        private PointF position;
        private Random random;
        private Stopwatch timer = new Stopwatch();
        private float yToMove;

        public TestPlane()
        {
            transform2D = new SyncedTransform2D(666, this); 
            timer.Start();
            random = new Random();
            yToMove = random.Next(-10, 10);
        }

        public void Update(float deltaTime)
        {
            if (timer.ElapsedMilliseconds > 0.5)
            {
                yToMove = random.Next(-10, 10);
                timer.Restart();
            }
            transform2D.Position.X = transform2D.Position.X + 10f * deltaTime;
            transform2D.Position.Y = transform2D.Position.Y + yToMove * deltaTime;
            transform2D.Update(deltaTime);
            // DebugLog.Warning($"[TestPlane] position: {transform2D.position.ToString()}");
        }

        public float GetTime()
        {
            return Server.Game.GameTimer.ElapsedMilliseconds / 1000f;
        }
    } 
}