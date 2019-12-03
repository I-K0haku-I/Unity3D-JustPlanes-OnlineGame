using UnityEngine;

namespace JustPlanes
{
    public class HealthBody : MonoBehaviour
    {

        [SerializeField] private float _healthPoint;

        public FloatEvent OnHealthChangedEvent = new FloatEvent();
        public GameObjectEvent OnDeathEvent = new GameObjectEvent();

        private void Update()
        {
            if (_healthPoint <= 0)
            {
                OnDeathEvent.Invoke(gameObject);
                Destroy(gameObject);
            }
        }

        public void AddDamage(float damage)
        {
            _healthPoint -= damage;
            OnHealthChangedEvent.Invoke(_healthPoint);
        }
    }
}