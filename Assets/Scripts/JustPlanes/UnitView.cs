using System;
using UnityEngine;

namespace JustPlanes
{
    public class UnitView : MonoBehaviour
    {

        public HealthBody hp;

        // Start is called before the first frame update
        private void Start()
        {
            hp = GetComponent<HealthBody>();
            hp.OnDeathEvent.AddListener((o) => Destroy(o));
        }

        // Update is called once per frame
        private void Update()
        {

        }
    }
}