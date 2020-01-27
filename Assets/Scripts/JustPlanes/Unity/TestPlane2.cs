using UnityEngine;


namespace JustPlanes.Unity
{
    public class TestPlane2 : MonoBehaviour
    {
        [SerializeField]
        private int syncId;

        public Core.TestPlane plane;
        private Vector3 newPos = Vector3.zero;
        private Quaternion newQuat = Quaternion.identity;

        private void Awake()
        {
            plane = new Core.TestPlane(GameManager.instance, 0, 0, syncId);
        }

        private void Update()
        {
            float v = Input.GetAxis("Vertical");
            float h = Input.GetAxis("Horizontal");


            newPos.x = plane.transform2D.Position.X;
            newPos.y = plane.transform2D.Position.Y;
            newQuat = Quaternion.AngleAxis(plane.transform2D.Rotation, Vector3.back);
            transform.SetPositionAndRotation(newPos, newQuat);
        }
    }
}