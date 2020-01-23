using UnityEngine;
using Net = JustPlanes.Core.Network;

namespace JustPlanes.Unity
{
    public class SyncedTransformHolder : MonoBehaviour, Net.ITransformHolder
    {
        [SerializeField]
        private int Id;
        public Net.SyncedTransform2D SyncedTransform;

        public float GetTime()
        {
            return Time.time;
        }

        void Awake()
        {
            SyncedTransform = new Net.SyncedTransform2D(Id, this);
        }

    }
}