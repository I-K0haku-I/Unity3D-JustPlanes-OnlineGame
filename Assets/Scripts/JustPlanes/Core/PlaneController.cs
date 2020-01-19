using JustPlanes.Core.Network;
using System;
using System.Drawing;

namespace JustPlanes.Core
{
    public class PlaneController : IPlaneController
    {
        protected TestPlane2 controllingPlane = null;

        protected float verticalMoveMultiplier = 0.1F;
        protected float horizontalMoveMultiplier = 0.1F;

        protected float maxThrottle = 100;
        protected float minThrottle = 30;

        protected float Throttle
        {
            get => Throttle;
            set => Math.Min(Math.Max(value, maxThrottle), minThrottle);
        }
        protected float nextAddRotation = 0;

        public void Update(float deltaTime)
        {
            PointF currentPos = controllingPlane.position;
            float currentRot = controllingPlane.rotation;

            float nextRot = currentRot + nextAddRotation * verticalMoveMultiplier;

            float nextHori = Throttle * horizontalMoveMultiplier * deltaTime;
            PointF nextPos = PointUtil.Multiply(PointUtil.Angle(nextRot), nextHori);

            controllingPlane.SetPositionAndRotation(nextPos.X, nextPos.Y, nextRot);
            controllingPlane.Update(deltaTime);
        }

        public void SetInput(float hori, float vert)
        {
            Throttle = hori;
            nextAddRotation = vert;
        }
    }

    public interface IPlaneController
    {

        void Update(float deltaTime);

        void SetInput(float horizontal, float vertical);

    }
}
