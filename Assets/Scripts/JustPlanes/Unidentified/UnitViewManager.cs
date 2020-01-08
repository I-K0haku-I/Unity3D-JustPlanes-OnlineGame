using System.Collections.Generic;
using UnityEngine;
using JustPlanes.Core;

namespace JustPlanes.Unity
{

    public class UnitViewManager : MonoBehaviour
    {
        private static readonly string _prefix = "UV Mgr:";

        public static UnitViewManager Instance;
        [SerializeField] public Object balloonUnit;
        [SerializeField] public Object testUnit;
        [SerializeField] public Object playerUnit;
        [SerializeField] public Transform UnitParent;
        [SerializeField] public Transform PlayerParent;

        private Dictionary<string, Unit> onlineUnits = new Dictionary<string, Unit>();
        private Dictionary<string, PlayerView> onlinePlayers = new Dictionary<string, PlayerView>();

        public UnitViewEvent OnUnitViewAddEvent = new UnitViewEvent();
        public UnitViewEvent OnUnitViewRemoveEvent = new UnitViewEvent();
        public UnitViewEvent OnUnitViewDamageEvent = new UnitViewEvent();

        public PlayerViewEvent OnPlayerViewAddEvent = new PlayerViewEvent();
        public PlayerViewEvent OnPlayerViewRemoveEvent = new PlayerViewEvent();

        private void Awake()
        {
            if (Instance != null)
            {
                DebugLog.Severe(this, $"{_prefix} UnitViewManager instance already exists!!");
                Destroy(this);
            }

            Instance = this;
        }

        private void Start()
        {
            #region HandleUnitEvent

            // TOOD: can give methods directly here
            NetworkManagerOld.instance.OnUnitAdd.AddListener((u) => Spawn(u));
            NetworkManagerOld.instance.OnUnitDies.AddListener((u) => Kill(u));
            NetworkManagerOld.instance.OnUnitGetsDamaged.AddListener((u, i) => Damage(u, i));

            #endregion HandleUnitEvent

            #region HandlePlayerEvent

            NetworkManagerOld.instance.OnPlayerAdd.AddListener((p) => Spawn(p));
            NetworkManagerOld.instance.OnPlayerRemove.AddListener((p) => Remove(p));

            #endregion HandlePlayerEvent
        }

        private GameObject Spawn(Object obj, string id, float x, float y)
        {
            DebugLog.Finest(this, $"{_prefix} Spawning Object {{{obj.name}, {id}, {x}, {y}}}");

            GameObject clonedObj = (GameObject)Instantiate(obj, new Vector3(x, y, 0.0F), new Quaternion(), UnitParent);
            clonedObj.name = (obj.name + "-" + id);
            return clonedObj;
        }

        private GameObject Spawn(Player player)
        {
            DebugLog.Fine(this, $"{_prefix} Spawning Player {{{player.Name}}}");

            GameObject playerObj = Spawn(playerUnit, player.Name, player.X, player.Y);

            // Disable object because we still don't need it!
            playerObj.SetActive(false);

            PlayerView playerView = playerObj.GetComponent<PlayerView>();
            playerView.player = player;

            onlinePlayers.Add(player.Name, playerView);

            OnPlayerViewAddEvent.Invoke(playerView);
            DebugLog.Finest(this, $"{_prefix} Spawned Player: {playerView.ToString()}");
            return playerObj;
        }

        private void Remove(Player player)
        {
            DebugLog.Fine(this, $"{_prefix} Removing Player {{{player.Name}}}");

            if (onlinePlayers.TryGetValue(player.Name, out PlayerView playerView))
            {
                OnPlayerViewRemoveEvent.Invoke(playerView);
                onlinePlayers.Remove(player.Name);

                Destroy(playerView.gameObject);

                DebugLog.Finest(this, $"{_prefix} Removed Player: {playerView.ToString()}");
                return;
            }
            else
            {
                DebugLog.Severe(this, $"{_prefix} Could not find player {{{player.Name}}}!!");
                return;
            }
        }

        private GameObject Spawn(Core.Unit unit)
        {
            DebugLog.Fine(this, $"{_prefix} Spawning Unit {{{unit.ID}}}");

            GameObject unitObj = Spawn(balloonUnit, unit.ID, unit.X, unit.Y);
            Unit unitView = unitObj.GetComponent<Unit>();
            unitView.unit = unit;

            onlineUnits.Add(unit.ID, unitView);

            OnUnitViewAddEvent.Invoke(unitView);
            DebugLog.Finest(this, $"{_prefix} Spawned Unit: {unitView.ToString()}");
            return unitObj;
        }


        private void Kill(Core.Unit unit)
        {
            DebugLog.Fine(this, $"{_prefix} Killing Unit {{{unit.ID}}}");

            if (onlineUnits.TryGetValue(unit.ID, out Unit unitView))
            {
                Core.Unit u = unitView.unit;

                if (!u.IsDead() | !unit.IsDead())
                {
                    DebugLog.Warning(this, $"{_prefix} Unit {{{unit.ID}}} is not dead, but called kill.");
                }

                OnUnitViewRemoveEvent.Invoke(unitView);
                unitView.OnUnitDeathEvent.Invoke(unitView.gameObject);

                onlineUnits.Remove(unit.ID);

                Destroy(unitView.gameObject);

                DebugLog.Finest(this, $"{_prefix} Killed Unit {unitView.ToString()}");
                return;
            }
            else
            {
                DebugLog.Severe(this, $"{_prefix} Could not find unit {{{unit.ID}}}!!");
                return;
            }
        }

        private void Damage(Core.Unit unit, int amount)
        {
            DebugLog.Fine(this, $"{_prefix} Damaging Unit {{{unit.ID}}}");

            if (onlineUnits.TryGetValue(unit.ID, out Unit unitView))
            {
                Core.Unit u = unitView.unit;

                u.hp -= amount;

                OnUnitViewDamageEvent.Invoke(unitView);
            }
            else
            {
                DebugLog.Severe(this, $"{_prefix} Could not find unit [{unit.ID}]!!");
                return;
            }
        }

        public Dictionary<string, Unit> GetOnlineUnits()
        {
            return new Dictionary<string, Unit>(onlineUnits);
        }

    }
}