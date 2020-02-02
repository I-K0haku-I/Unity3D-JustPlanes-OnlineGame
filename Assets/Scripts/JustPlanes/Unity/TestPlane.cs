using UnityEngine;


namespace JustPlanes.Unity
{
    public class TestPlane : MonoBehaviour
    {
        [SerializeField]
        private int syncId;

        [SerializeField]
        [Range(0.01f, 0.9f)]
        private float lerpTime = 2f;

        [SerializeField]
        [Range(0.01f, 1f)]
        private float lerpDistanceTrigger = 0.2f;

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
            plane.Update(Time.deltaTime);
            plane.HandleInput(h, v);

            plane.body.MaxLerpTime = lerpTime;
            plane.body.LerpDistanceTrigger = lerpDistanceTrigger;


            newPos.x = plane.transform2D.Position.X;
            newPos.y = plane.transform2D.Position.Y;
            newQuat = Quaternion.AngleAxis(plane.transform2D.Rotation, Vector3.back);
            transform.SetPositionAndRotation(newPos, newQuat);
        }
    }
}