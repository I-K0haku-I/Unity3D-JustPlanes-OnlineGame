using UnityEngine;

namespace JustPlanes
{

    [RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(UnitView))]
    public class PlaneView : MonoBehaviour
    {

        public float maxThrottle = 20.0F;
        public float minThrottle = 10.0F;
        public float throttleMultiplier = 10.0F;
        public float speedMultiplier = 1.5F;
        public float turnSpeedMultiplier = 150.0F;
        public float bulletCoolDownTime = 0.2f;
        public float yBound = 150.0F;
        public float xBound = 150.0F;
        public float maxOutOfBoundTime = 10.0F;


        protected float _currentThrottle;
        protected float _currentBulletCoolDown;
        protected float _currentOutOfBoundTime;

        public GameObject bullet;
        public Transform bulletParent;
        public Rigidbody rb;
        // TODO: this is smells a bit, not sure why
        // Note: this will be PlayerView soonish (after you actually made Player class extending Unit class) - SleepyNewbie
        public UnitView uv;

        private void Awake()
        {
            bullet = Resources.Load("Bullet") as GameObject;
            rb = GetComponent<Rigidbody>();
            uv = GetComponent<UnitView>();

            // TODO: do not new unit inside planeview - should be assigned somewhere
            //       also change it to Guid.NewGuid() when GUID change has done

            uv.unit = new Network.Unit("local:playerplane", 0, 0);
        }

        private void Start()
        {
            // Set owner to prevent damaging self
            bullet.GetComponent<BulletView>().owner = gameObject;
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

            SetThrottle(_currentThrottle + (vertical * throttleMultiplier * Time.deltaTime));
            transform.Rotate(Vector3.back, horizontal * Time.deltaTime * turnSpeedMultiplier);

            rb.velocity = GetSpeed();

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

        public void SetThrottle(float throttle)
        {
            _currentThrottle = throttle;

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
