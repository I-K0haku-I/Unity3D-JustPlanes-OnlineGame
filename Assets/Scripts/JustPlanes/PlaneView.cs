using UnityEngine;

namespace JustPlanes
{

    public class PlaneView : MonoBehaviour
    {

        public float maxThrottle = 20.0F;
        public float minThrottle = 10.0F;
        public float throttleMultiplier = 10.0F;
        public float speedMultiplier = 1.5F;
        public float turnSpeedMultiplier = 150.0F;
        public float bulletCoolDownTime = 0.2f;
        public KeyCode scoreboardKey = KeyCode.Tab;

        protected float _currentThrottle;
        protected float _currentBulletCoolDown;

        public GameObject bullet;
        public Transform bulletParent;
        public Rigidbody rb;
        public HealthBody hp;

        private void Awake()
        {
            bullet = Resources.Load("Bullet") as GameObject;
            rb = GetComponent<Rigidbody>();
            hp = GetComponent<HealthBody>();
        }

        private void Start()
        {
            // Set owner to prevent damaging self
            bullet.GetComponent<BulletView>().owner = gameObject;
            // Listen to event that triggers on death
            hp.OnDeathEvent.AddListener((o) => Death(o));
        }

        private void Update()
        {
            // Control
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            SetThrottle(_currentThrottle + (vertical * throttleMultiplier * Time.deltaTime));
            transform.Rotate(Vector3.back, horizontal * Time.deltaTime * turnSpeedMultiplier);

            rb.velocity = GetSpeed();

            if (Input.GetButton("Fire1"))
            {
                Shoot();
            }
        }

        public void SetThrottle(float throttle)
        {
            _currentThrottle = throttle;

            // apprently System.Math.Clamp doesn't exist? :thinking:
            _currentThrottle = Mathf.Clamp(throttle, minThrottle, maxThrottle);
        }

        public Vector3 GetSpeed()
        {
            return transform.up * _currentThrottle * speedMultiplier;
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
                GameObject obj = Instantiate(bullet, transform.position, transform.rotation, bulletParent);
                obj.name = "Bullet<" + gameObject.name + ">";
                _currentBulletCoolDown = bulletCoolDownTime;
            }
        }
    }
}
