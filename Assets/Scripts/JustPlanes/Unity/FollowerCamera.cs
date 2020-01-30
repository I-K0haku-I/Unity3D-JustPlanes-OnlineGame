using UnityEngine;

namespace JustPlanes.Unity
{
    public class FollowerCamera : MonoBehaviour
    {

        // [SerializeField]
        // private Camera cam;
        // [SerializeField]
        // private GameObject target;

        public Vector3 offset;
        public float velocityOffset = 0.6F;
        public float smoothSpeed = 0.3F;
        private Vector2 camVelocity = Vector3.zero;
        [SerializeField]
        private TestPlane testPlane;

        // Start is called before the first frame update
        public void Initialize()
        {
            // transform2D = target.GetComponent<TestPlane2>().plane.transform2D;
        }

        // Update is called once per frame
        private void LateUpdate()
        {
            // if (!testPlane.gameObject.activeInHierarchy)
            //     return;
            // // Define a target position above and behind the target transform
            // Box2DX.Common.Vec2 vel = (testPlane.plane.transform2D.Velocity);
            // Vector2 velVector2 = new Vector2(vel.X, vel.Y) * velocityOffset;
            // Vector2 targetPosition = (Vector2)testPlane.gameObject.transform.TransformPoint(offset) + velVector2;
            // Vector2 currentPosition = base.transform.position;

            // Vector2 towardVec = Vector2.MoveTowards(currentPosition, targetPosition, 500);
            // Vector3 camTarget = Vector2.SmoothDamp(currentPosition, towardVec, ref camVelocity, smoothTime);
            // camTarget.z = -20f;

            // base.transform.position = camTarget;

            //Debug.Log("Target: " + target.transform.forward + "Offset Pos: " + targetPosition + ", TargetVelocity: " + targetRigidbody.velocity);

            Vector3 newPos = testPlane.transform.position + testPlane.transform.TransformDirection(offset);
            Vector3 smoothedPos = Vector3.Lerp(transform.position, newPos, smoothSpeed);
                // testPlane.transform.position + testPlane.transform.TransformDirection(offset);
            smoothedPos.z = -20f;
            transform.position = smoothedPos;
            // Vector3 vel = Vector3.zero;
            // newPos = Vector3.SmoothDamp(transform.position, newPos, ref vel, smoothSpeed);
            // transform.position = newPos;
        }
    }
}