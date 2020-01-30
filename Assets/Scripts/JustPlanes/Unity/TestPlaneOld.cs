using UnityEngine;
using JustPlanes.Core.Network;

namespace JustPlanes.Unity
{
    public class TestPlaneOldITHink : MonoBehaviour
    {
        private SyncedTransform2D syncedTransform;
        private Vector3 newPos = Vector3.zero;
        private Quaternion newQuat = Quaternion.identity;
        private Core.PhysicsBody phys;

        // public void SetPositionAndRotation(float x, float y, float rotation)
        // {
        //     Vector3 newPos = Vector3.Lerp(transform.position, new Vector3(x, y, transform.position.z), positionLerpRate);
        //     transform.SetPositionAndRotation(newPos, new Quaternion(0, 0, rotation, 0));
        // }

        void Awake()
        {
            phys = GameManager.instance.Physics.CreateBody(5, 5, 5, 5);
        }

        void Start()
        {
            // syncedTransform = GetComponent<SyncedTransformHolder>().SyncedTransform;
            // newPos.z = transform.position.z;
        }

        [SerializeField]
        [Range(0.5f, 10f)]
        private float baseSpeed = 1f;
        [SerializeField]
        [Range(25f, 100f)]
        private float rotateSpeed = 1f;
        private float speed;

        // Update is called once per frame
        void Update()
        {
            // syncedTransform.Update(Time.deltaTime);
            // newPos.x = syncedTransform.Position.X;
            // newPos.y = syncedTransform.Position.Y;
            // transform.SetPositionAndRotation(newPos, new Quaternion(0, 0, syncedTransform.Rotation, 0));
            float v = Input.GetAxis("Vertical");
            float h = Input.GetAxis("Horizontal");

            phys.SetAngularVelocity(rotateSpeed * h);
            speed += v * Time.deltaTime * baseSpeed;
            phys.SetVelocity(speed);
            newPos.x = phys.body.GetPosition().X;
            newPos.y = phys.body.GetPosition().Y;
            newQuat = Quaternion.AngleAxis(phys.body.GetAngle(), Vector3.back);
            transform.SetPositionAndRotation(newPos, newQuat);
        }
    }
}