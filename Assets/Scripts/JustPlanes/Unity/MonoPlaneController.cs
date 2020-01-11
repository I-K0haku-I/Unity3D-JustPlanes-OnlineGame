using System;
using JustPlanes.Core;
using UnityEngine;

namespace JustPlanes.Unity
{
    public class MonoPlaneController : MonoBehaviour
    {
        bool isInitialized = false;
        IPlaneController controllerImpl = null;

        private void Awake()
        {
            
        }

        private void Start()
        {
            
        }

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
            this.controllerImpl.Input(Time.deltaTime, Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), Input.GetButtonDown("Fire1"));
        }

        private void FixedUpdate()
        {
            this.controllerImpl.FixedTick(Time.deltaTime);
            Transform2D expectingTransform = this.controllerImpl.GetPlane().transform2D;
        }
    }
}
