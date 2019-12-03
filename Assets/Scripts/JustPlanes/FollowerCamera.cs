using UnityEngine;

namespace JustPlanes
{
    public class FollowerCamera : MonoBehaviour
    {

        public Camera cam;
        public GameObject target;
        public Vector3 offset;
        public float smoothTime = 0.3F;
        private Vector3 velocity = Vector3.zero;

        // Start is called before the first frame update
        private void Start()
        {
        }

        // Update is called once per frame
        private void Update()
        {
            // Define a target position above and behind the target transform
            Vector3 targetPosition = target.transform.TransformPoint(offset);

            // Debug.Log("Target: " + target.transform.forward + "Offset Pos: " + targetPosition + ", Offset: " + offset);

            // Smoothly move the camera towards that target position
            cam.transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        }
    }
}