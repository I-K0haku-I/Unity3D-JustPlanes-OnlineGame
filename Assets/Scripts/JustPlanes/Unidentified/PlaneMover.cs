using UnityEngine;

namespace JustPlanes.Unity
{

    [RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(Unit))]
    public class PlaneView : MonoBehaviour
    {

        [SerializeField] public Object bullet;
        [SerializeField] public Transform bulletParent;

        [SerializeField] public float maxThrottle = 20.0F;
        [SerializeField] public float minThrottle = 10.0F;
        [SerializeField] public float throttleMultiplier = 10.0F;
        [SerializeField] public float speedMultiplier = 1.5F;
        [SerializeField] public float turnSpeedMultiplier = 150.0F;
        [SerializeField] public float bulletCoolDownTime = 0.2f;
        [SerializeField] public float yBound = 100.0F;
        [SerializeField] public float xBound = 100.0F;
        [SerializeField] public float maxOutOfBoundTime = 0.1F;


        protected float _currentThrottle;
        protected float _rotationBuffer;
        protected float _currentBulletCoolDown;
        protected float _currentOutOfBoundTime;

        public Rigidbody rb;
        // TODO: this is smells a bit, not sure why
        // Note: this will be PlayerView soonish (after you actually made Player class extending Unit class) - SleepyNewbie
        public Unit uv;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            uv = GetComponent<Unit>();

            // TODO: do not new unit inside planeview - should be assigned somewhere
            //       also change it to Guid.NewGuid() when GUID change has done

            uv.unit = new Core.Unit("local:playerplane", 0, 0);
        }

        private void Start()
        {
            // Listen to event that triggers on death
            uv.OnUnitDeathEvent.AddListener((o) => Death(o));
        }

        private void Update()
        {
            // TODO: controlls should be on another class.
            //       probably on LocalPlayerView.cs => PlaneView.cs

            // Control
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            AddThrottle(vertical);
            AddRotation(horizontal);

            if (Input.GetButton("Fire1"))
            {
                Shoot();
            }

            if (Mathf.Abs(this.transform.position.x) > xBound || Mathf.Abs(this.transform.position.y) > yBound)
            {
                _currentOutOfBoundTime += Time.deltaTime;
            }
            else
            {
                _currentOutOfBoundTime = 0;
            }

            if (_currentOutOfBoundTime > maxOutOfBoundTime)
            {
                gameObject.transform.position = new Vector3(0, 0, gameObject.transform.position.z);
            }
        }

        private void FixedUpdate()
        {
            rb.velocity = GetSpeed();
            rb.MoveRotation(rb.rotation * GetRotation());
            _rotationBuffer = 0;
        }

        public void AddThrottle(float throttle)
        {
            _currentThrottle = Mathf.Clamp(_currentThrottle + (throttle * throttleMultiplier * Time.deltaTime), minThrottle, maxThrottle);
        }

        public void AddRotation(float rotation)
        {
            _rotationBuffer += rotation * Time.deltaTime * turnSpeedMultiplier;
        }

        public Vector3 GetSpeed()
        {
            return transform.up * _currentThrottle * speedMultiplier;
        }

        public Quaternion GetRotation()
        {
            return Quaternion.AngleAxis(_rotationBuffer, Vector3.back);
        }

        // TODO: Explooosion! (animated sprite? particles?)
        private void Death(GameObject obj)
        {

        }

        private void Shoot()
        {
            if (0 <= _currentBulletCoolDown)
            {
                _currentBulletCoolDown -= Time.deltaTime;
                return;
            }
            else
            {
                GameObject obj = (GameObject) Instantiate(bullet, transform.position, transform.rotation, bulletParent);
                // Set owner to prevent damaging self
                obj.GetComponent<BulletView>().owner = gameObject;
                obj.name = "Bullet<" + gameObject.name + ">";
                _currentBulletCoolDown = bulletCoolDownTime;
            }
        }
    }
}
