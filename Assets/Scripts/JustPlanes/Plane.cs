using UnityEngine;

namespace JustPlanesGame
{

    public class Plane : MonoBehaviour
    {

        public float maxThrottle = 100.0F;
        public float minThrottle = 0.5F;
        public float throttleSpeedMultiplier = 1.5F;
        public float turnSpeedMultiplier = 30.0F;
        public float bulletCoolDownTime = 0.2f;
        public KeyCode scoreboardKey = KeyCode.Tab;

        protected float currentThrottle;
        protected float currentBulletCoolDown;

        public GameObject bullet;
        public Rigidbody rb;
        public HealthBody hp;

        private void Start()
        {
            bullet = Resources.Load("Bullet") as GameObject;
            // Set owner to prevent damaging self
            bullet.GetComponent<Bullet>().owner = gameObject;

            rb = GetComponent<Rigidbody>();
            hp = GetComponent<HealthBody>();

            // Listen to event that triggers on death
            hp.OnDeathEvent.AddListener((o) => OnDeath(o));
        }

        private void Update()
        {
            // Control
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            SetThrottle(currentThrottle + (vertical * Time.deltaTime));

            transform.Rotate(Vector3.back, horizontal * Time.deltaTime * turnSpeedMultiplier);
            rb.velocity = GetSpeed();

            if (Input.GetButtonDown("Fire1"))
            {
                OnShoot();
            }

            // TODO: make scoreboard
            // Scoreboard
            if (Input.GetKeyDown(scoreboardKey))
            {
                // Show/Update scoreboard here
            }

            if (Input.GetKeyUp(scoreboardKey))
            {
                // Remove scoreboard here
            }
        }

        public void SetThrottle(float throttle)
        {
            currentThrottle = throttle;

            if (currentThrottle > maxThrottle)
            {
                currentThrottle = maxThrottle;
            }
            else if (currentThrottle < minThrottle)
            {
                currentThrottle = minThrottle;
            }
        }

        public Vector3 GetSpeed()
        {
            return transform.up * currentThrottle * throttleSpeedMultiplier;
        }

        // TODO: Explooosion! (animated sprite? particles?)
        private void OnDeath(GameObject obj)
        {

        }

        private void OnShoot()
        {
            if (0 <= currentBulletCoolDown)
            {
                currentBulletCoolDown -= Time.deltaTime;
                return;
            }
            else
            {
                GameObject obj = Instantiate(bullet, transform.position, transform.rotation);
                obj.name = "Bullet<" + gameObject.name + ">";
            }
        }
    }
}
