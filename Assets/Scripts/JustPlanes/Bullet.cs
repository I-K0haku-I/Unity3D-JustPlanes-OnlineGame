using System.Collections;
using UnityEngine;

namespace JustPlanesGame
{
    public class Bullet : MonoBehaviour
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
            if (collision.gameObject != owner && collision.gameObject.GetComponent<HealthBody>() != null)
            {
                collision.gameObject.GetComponent<HealthBody>().AddDamage(damage);
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
