using JustPlanes.Network;
using UnityEngine;

namespace JustPlanes
{

    public class LocalClient : MonoBehaviour
    {

        [SerializeField] public Transform unitParent;

        private Object testUnit;

        private void Awake()
        {
            testUnit = Resources.Load("TestUnit");
        }

        private void Start()
        {
            NetworkManager.instance.OnUnitAdd.AddListener(
                (u) => Spawn(u));
        }

        private void Spawn(Unit unit)
        {
            Spawn(testUnit, unit.ID, unit.X, unit.Y);
        }

        private void Spawn(Object obj, string id, float x, float y)
        {
            GameObject clonedObj = (GameObject)Instantiate(obj, new Vector3(x, y, 0.0F), new Quaternion(), unitParent);
            clonedObj.name = (obj.name + "-" + id);
        }

    }
}