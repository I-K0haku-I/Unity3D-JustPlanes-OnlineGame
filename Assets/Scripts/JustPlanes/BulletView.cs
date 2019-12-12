using System.Collections;
using UnityEngine;

namespace JustPlanes
{

    [RequireComponent(typeof(Rigidbody))]
    public class BulletView : MonoBehaviour
    {

        public float aliveTime = 30.0F;
        public float speed = 30.0F;
        public float damage = 5.0F;
        public GameObject owner;

        public Rigidbody2D rb;

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            rb.velocity = transform.up * speed;

            StartCoroutine("BulletTimer");
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            GameObject obj = collision.gameObject;

            if (obj != owner && obj.GetComponent<UnitView>() != null)
            {
                UnitView unitView = obj.GetComponent<UnitView>();
                if (unitView.unit.ID.StartsWith("local:"))
                {
                    // TODO: don't leave random debugs like that
                    // I don't even know why this is here, it's not self explanatory at all
                    // also, why do we even care about local or not local units? we only display server units
                    Debug.Log("Found local unit, ejecting!");
                    return;
                }
                // instead of rounding, you should just cast to int
                // or just change damage to ints, I don't see a reason to work with floats here
                Network.NetworkManager.instance.DamageUnit(unitView.unit, Mathf.RoundToInt(damage));
                Destroy(this.gameObject);
            }
        }

        private IEnumerator BulletTimer()
        {
            yield return new WaitForSeconds(aliveTime);
            Destroy(gameObject);
        }
    }
}
