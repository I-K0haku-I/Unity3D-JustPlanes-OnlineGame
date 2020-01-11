using System;
using UnityEngine;

namespace JustPlanes.Unity
{
    public abstract class MonoEntity<T> : MonoBehaviour where T : Core.Entity
    {
        protected bool isInitialized = false;
        protected T entityImpl;

        public void Initialize(T entityImpl)
        {
            if (isInitialized)
            {
                throw new InvalidOperationException("MonoEntity already initialized. - Do not call MonoEntity#Initialize() twice.");
            }
            this.entityImpl = entityImpl;
            isInitialized = true;
        }

        private void Update()
        {
            if (isInitialized)
            {
                Core.Transform2D expecting = entityImpl.transform2D;
                gameObject.transform.SetPositionAndRotation(new Vector3(expecting.position.x, expecting.position.y), Quaternion.AngleAxis(expecting.rotation, Vector3.back));
            }
        }
    }
}
