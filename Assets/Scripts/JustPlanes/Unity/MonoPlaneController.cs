using JustPlanes.Core;
using System;
using UnityEngine;

namespace JustPlanes.Unity
{
    public class MonoPlaneController : MonoBehaviour, IPlaneController
    {
        private bool isInitialized = false;
        private IPlaneController controllerImpl = null;

        public void Initialize(IPlaneController controllerImpl)
        {
            if (isInitialized)
            {
                throw new InvalidOperationException("MonoPlaneController already initialized.");
            }
            this.controllerImpl = controllerImpl;
            isInitialized = true;
        }

        private void Update()
        {
            if (isInitialized)
            {
                AddInput(Time.deltaTime, Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), Input.GetButtonDown("Fire1"));
            }
        }

        private void FixedUpdate()
        {
            FixedTick(Time.deltaTime);
        }

        public void FixedTick(float deltaTime)
        {
            controllerImpl.FixedTick(deltaTime);
            Transform2D expectingTransform = controllerImpl.GetPlane().transform2D;
            Vec2 pos = expectingTransform.position;
            float rot = expectingTransform.rotation;
            this.transform.SetPositionAndRotation(new Vector3(pos.x, pos.y), Quaternion.AngleAxis(rot, Vector3.back));
        }

        public void AddInput(float deltaTime, float horizontalInput, float verticalInput, bool isShooting)
        {
            controllerImpl.AddInput(deltaTime, horizontalInput, verticalInput, isShooting);
        }

        public Core.Plane GetPlane()
        {
            return controllerImpl.GetPlane();
        }

        public Player GetPlayer()
        {
            return controllerImpl.GetPlayer();
        }
    }
}
