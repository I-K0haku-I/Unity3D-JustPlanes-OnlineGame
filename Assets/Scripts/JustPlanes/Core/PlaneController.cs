using System;

namespace JustPlanes.Core
{
    public class PlaneController : IPlaneController

    {
        private Plane controllingPlane = null;
        private Player controllingPlayer = null;

        private float verticalInputMultiplier = 0.1F;
        private float horizontalInputMultiplier = 0.1F;

        private float throttle = 30;
        private float maxThrottle = 100;
        private float minThrottle = 30;

        private float vertInputBuffer = 0;
        private float horiInputBuffer = 0;

        private float shootingCoolDown = 0;
        private float maxShootingCoolDown = 0.1F;
        private bool isShooting = false;

        public void FixedTick(float deltaTime)
        {
            HandleControl(deltaTime);
            HandleShooting(deltaTime);
        }

        protected void HandleControl(float deltaTime)
        {
            throttle = Math.Min(Math.Max(throttle + vertInputBuffer * verticalInputMultiplier, maxThrottle), minThrottle);

            Vec2 expectingAngle = Vec2.Angle(controllingPlane.transform2D.rotation + (horiInputBuffer * horizontalInputMultiplier));
            Vec2 expectingPosition = expectingAngle * throttle;

            controllingPlane.transform2D.position = expectingPosition;
            controllingPlane.transform2D.RadiansRotation = expectingAngle.GetRadians();

            ClearBuffer();
        }

        protected void HandleShooting(float deltaTime)
        {
            if (shootingCoolDown > 0)
            {
                shootingCoolDown -= deltaTime;
            }
            else if (isShooting)
            {
                controllingPlane.Shoot();
                shootingCoolDown = maxShootingCoolDown;
            }
        }

        protected void ClearBuffer()
        {
            vertInputBuffer = 0;
            horiInputBuffer = 0;
        }

        public void Resync(Transform2D transform, float throttle)
        {
            controllingPlane.transform2D = transform;
            this.throttle = Math.Min(Math.Max(throttle + vertInputBuffer * verticalInputMultiplier, maxThrottle), minThrottle);
            ClearBuffer();
        }

        public void AddInput(float deltaTime, float hori, float vert, bool shoot)
        {
            vertInputBuffer += Math.Min(Math.Max(vert * deltaTime, 1), -1);
            horiInputBuffer += Math.Min(Math.Max(hori * deltaTime, 1), -1);
            if (!isShooting)
            {
                isShooting = shoot;
            }
        }

        public Plane GetPlane()
        {
            return controllingPlane;
        }

        public Player GetPlayer()
        {
            return controllingPlayer;
        }

        public PlaneController(Player player, Plane plane)
        {
            controllingPlane = plane;
        }
    }

    public interface IPlaneController
    {

        void FixedTick(float deltaTime);

        void AddInput(float deltaTime, float horizontalInput, float verticalInput, bool isShooting);

        Plane GetPlane();

        Player GetPlayer();

    }
}
