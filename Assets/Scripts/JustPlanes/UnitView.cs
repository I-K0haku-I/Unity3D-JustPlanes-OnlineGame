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
            {
                slider = gameObject.GetComponentInChildren<Slider>();
            }

            slider.value = (float)unit.hp / unit.maxHP;
        }

        private void Update()
        {
            slider.value = (float)unit.hp / unit.maxHP;
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