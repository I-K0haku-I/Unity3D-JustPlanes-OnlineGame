using UnityEngine;
using UnityEngine.UI;

namespace JustPlanes
{
    public class UnitView : MonoBehaviour
    {

        public Network.Unit unit;
        public GameObjectEvent OnUnitDeathEvent = new GameObjectEvent();

        public Slider slider;

        private void Start()
        {
            if (slider == null)
                slider = this.gameObject.GetComponentInChildren<Slider>();
            slider.value = (float)unit.hp / unit.maxHP;
        }

        private void Update()
        {
            slider.value = (float)unit.hp / unit.maxHP;
        }
    }
}