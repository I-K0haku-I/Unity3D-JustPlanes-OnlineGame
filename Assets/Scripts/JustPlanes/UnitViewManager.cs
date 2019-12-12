using JustPlanes.Network;
using System.Collections.Generic;
using UnityEngine;

namespace JustPlanes
{

    public class UnitViewManager : MonoBehaviour
    {

        public static UnitViewManager Instance;
        public static Transform UnitParent;

        private Dictionary<string, UnitView> onlineUnits = new Dictionary<string, UnitView>();
        private static Object testUnit;

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError("UnitViewManager instance already exists!!");
                Destroy(this);
            }

            Instance = this;
            testUnit = Resources.Load("TestUnit");
        }

        private void Start()
        {
            NetworkManager.instance.OnUnitAdd.AddListener(
                (u) => Spawn(u));

            NetworkManager.instance.OnUnitDies.AddListener(
                (u) => Kill(u));

            NetworkManager.instance.OnUnitGetsDamaged.AddListener(
                (u, i) => Damage(u, i));
        }

        private GameObject Spawn(Unit unit)
        {
            GameObject unitObj = Spawn(testUnit, unit.ID, unit.X, unit.Y);
            UnitView unitView = unitObj.GetComponent<UnitView>();
            unitView.unit = unit;

            onlineUnits.Add(unit.ID, unitView);

            return unitObj;
        }

        private GameObject Spawn(Object obj, string id, float x, float y)
        {
            GameObject clonedObj = (GameObject)Instantiate(obj, new Vector3(x, y, 0.0F), new Quaternion(), UnitParent);
            clonedObj.name = (obj.name + "-" + id);
            return clonedObj;
        }

        private void Kill(Unit unit)
        {
            if (onlineUnits.TryGetValue(unit.ID, out UnitView uv))
            {
                Unit u = uv.unit;

                if (!u.IsDead())
                {
                    Debug.LogWarning("Unit [" + u.ID + "] is not dead, but called kill.");
                }

                uv.OnUnitDeathEvent.Invoke(uv.gameObject);
                onlineUnits.Remove(unit.ID);
                Destroy(uv);

                Debug.LogFormat("Killed Unit ID: {0} HP: {1}", u.ID, u.hp);
            }
            else
            {
                Debug.LogError("Could not find unit [" + unit.ID + "]!!");
                return;
            }
        }

        private void Damage(Unit unit, int amount)
        {
            if (onlineUnits.TryGetValue(unit.ID, out UnitView uv))
            {
                Unit u = uv.unit;

                u.hp -= amount;
            }
            else
            {
                Debug.LogError("Could not find unit [" + unit.ID + "]!!");
                return;
            }
        }

        public Dictionary<string, UnitView> GetOnlineUnits()
        {
            return new Dictionary<string, UnitView>(onlineUnits);
        }

    }
}