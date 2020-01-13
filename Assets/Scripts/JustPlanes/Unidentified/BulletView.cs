using System.Collections;
using UnityEngine;

namespace JustPlanes.Unity
{

    [RequireComponent(typeof(Rigidbody))]
    public class BulletView : MonoBehaviour
    {

        public float aliveTime = 30.0F;
        public float speed = 30.0F;
        public int damage = 5;
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

            if (obj != owner && obj.GetComponent<Unit>() != null)
            {
                Unit unitView = obj.GetComponent<Unit>();
                if (unitView.unit.ID.StartsWith("local:"))
                {
                    // TODO: don't leave random debugs like that
                    // I don't even know why this is here, it's not self explanatory at all
                    // also, why do we even care about local or not local units? we only display server units

                    // Note: this is not debug.
                    //       there are some local units exist (PlaneView) on project currently.
                    //       until it'll be fixed or will be no longer needed, this will stay here to try-not-to-send-invalid-packets.
                    // DebugLog.Warning(this, $"Bullet has found local units (shouldn't happen!)!!");
                    return;
                }

                NetworkManagerOld.instance.DamageUnit(unitView.unit, damage);
                Destroy(gameObject);
            }
        }

        private IEnumerator BulletTimer()
        {
            yield return new WaitForSeconds(aliveTime);
            Destroy(gameObject);
        }
    }
}
