using JustPlanes.Core;
using UnityEngine;

namespace JustPlanes.Unity
{
    public class MonoPlaneController : MonoBehaviour, IPlaneController
    {
        public IPlaneController Controller;
        private float horiBuff;
        private float vertBuff;

        private void Update()
        {
            horiBuff += Input.GetAxis("Horizontal") * Time.deltaTime;
            vertBuff += Input.GetAxis("Vertical") * Time.deltaTime;
        }

        private void FixedUpdate()
        {
            SetInput(horiBuff, vertBuff);
            Update(Time.fixedDeltaTime);
            ClearBuff();
        }

        private void ClearBuff()
        {
            horiBuff = 0;
            vertBuff = 0;
        }

        public void SetInput(float horizontal, float vertical)
        {
            Controller.SetInput(horizontal, vertical);
        }

        public void Update(float deltaTime)
        {
            Controller.Update(deltaTime);
        }
    }
}
