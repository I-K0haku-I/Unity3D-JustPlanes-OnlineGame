using UnityEngine;

namespace JustPlanes
{
    public class FollowerCamera : MonoBehaviour
    {

        public Camera cam;
        public GameObject target;
        public Vector3 offset;
        public float velocityOffset = 0.6F;
        public float smoothTime = 0.3F;
        private Vector3 camVelocity = Vector3.zero;
        private Rigidbody targetRigidbody;

        // Start is called before the first frame update
        private void Start()
        {
            targetRigidbody = target.GetComponent<Rigidbody>();
        }

        // Update is called once per frame
        private void Update()
        {
            // Define a target position above and behind the target transform
            Vector3 targetPosition = target.transform.TransformPoint(offset) + (targetRigidbody.velocity * velocityOffset);
            Vector3 currentPosition = transform.position;

            Vector3 towardVec = Vector3.MoveTowards(currentPosition, targetPosition, 500);
            Vector3 camTarget = Vector3.SmoothDamp(currentPosition, towardVec, ref camVelocity, smoothTime);

            transform.position = camTarget;

            //Debug.Log("Target: " + target.transform.forward + "Offset Pos: " + targetPosition + ", TargetVelocity: " + targetRigidbody.velocity);

        }
    }
}