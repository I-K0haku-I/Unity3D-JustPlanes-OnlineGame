using UnityEngine;
using UnityEngine.UI;

namespace JustPlanes.Unity
{
    public class Unit : MonoBehaviour
    {

        public Core.Unit unit;
        public GameObjectEvent OnUnitDeathEvent = new GameObjectEvent();

        public Slider slider;

        private void Start()
        {
            if (slider == null)
            {
                slider = gameObject.GetComponentInChildren<Slider>();
            }

            if (unit != null)
            {
                slider.value = (float)unit.hp / unit.maxHP;
            }
        }

        private void Update()
        {
            if (unit != null)
            {
                slider.value = (float)unit.hp / unit.maxHP;
            }
        }


        public override string ToString()
        {
            if (unit != null)
            {
                return $"{{UnitView: {unit.ID}, {unit.hp}, {unit.X}, {unit.Y}}}";
            }
            else
            {
                return "{{UNIT_IS_NULL}}";
            }
        }
    }
}