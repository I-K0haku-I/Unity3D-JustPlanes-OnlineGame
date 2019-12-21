using JustPlanes.Network;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace JustPlanes
{

    public class Debugger : MonoBehaviour
    {
        private static readonly string _prefix = "Debugger:";

        [SerializeField] public KeyCode killUnitTest = KeyCode.Y;
        [SerializeField] public Level loggingLevel = Level.L_ALL;
        [SerializeField] public bool setLogLevel = false;
        [SerializeField] public bool testLogger = false;
        [SerializeField] public bool killAllOnTest = false;
        [SerializeField] public bool deathCounter = false;
        [SerializeField] public bool listenMission = false;
        [SerializeField] public bool listenUnit = false;

        private const string glyphs = "abcdefghijklmnopqrstuvwxyz123456789";
        private const string line = "------------------------------------------------------------------------------------------";
        private const string space = "                 ";
        private int deaths = 0;

        private void Awake()
        {
            if (setLogLevel)
            {
                DebugLog.Warning($"{_prefix} change log level is enabled! {loggingLevel}");
                DebugLog.SetLevel(loggingLevel);
            }
        }

        private void Start()
        {
            if (testLogger)
            {
                TestLogger();
            }
            if (deathCounter)
            {
                DebugLog.Warning($"{_prefix} death counter is enabled!");
                NetworkManager.instance.OnUnitDies.AddListener((u) => CountDeath());
            }
            if (listenMission)
            {
                DebugLog.Warning($"{_prefix} listen mission is enabled!");
                NetworkManager.instance.OnMissionAdd.AddListener((handler) => MissionInfo(handler));
                NetworkManager.instance.OnMissionUpdate.AddListener((val) => MissionInfo(val));
                NetworkManager.instance.OnMissionComplete.AddListener(() => MissionInfo());
            }
            if (listenUnit)
            {
                DebugLog.Warning($"{_prefix} listen unit is enabled!");
                NetworkManager.instance.OnUnitAdd.AddListener((unit) => UnitInfo(UnitEventType.Add, unit));
                NetworkManager.instance.OnUnitGetsDamaged.AddListener((unit, val) => UnitInfo(UnitEventType.GetsDamaged, unit, val));
                NetworkManager.instance.OnUnitDies.AddListener((unit) => UnitInfo(UnitEventType.Dies, unit));
            }
        }

        // Update is called once per frame
        private void Update()
        {
            if (Input.GetKeyDown(killUnitTest))
            {
                Dictionary<string, UnitView>.ValueCollection unitViews = UnitViewManager.Instance.GetOnlineUnits().Values;
                if (killAllOnTest)
                {
                    foreach(UnitView unitView in unitViews)
                    {
                        NetworkManager.instance.DamageUnit(unitView.unit, unitView.unit.hp);
                    }
                }
                else
                {
                    UnitView[] viewArrays = new UnitView[0];
                    unitViews.CopyTo(viewArrays, 0);

                    NetworkManager.instance.DamageUnit(viewArrays[0].unit, viewArrays[0].unit.hp);
                }
            }

        }

        private string GenerateRandomName(int length)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < length; i++)
            {
                sb.Append(glyphs[Random.Range(0, glyphs.Length)]);
            }

            return sb.ToString();
        }

        private void CountDeath()
        {
            deaths++;
            DebugLog.Warning(this, $"{_prefix} Unit deaths:\n{space}{deaths}");
        }

        #region MissionInfo

        private void MissionInfo(MissionHandler handler)
        {
            DebugLog.Severe(this, $"{_prefix} Mission has updated: OnMissionAdd\n{space}{handler.type}, {handler.enemiesKilled}, {handler.enemiesToKill}");
        }

        private void MissionInfo(int val)
        {
            DebugLog.Warning(this, $"{_prefix} Mission has updated: OnMissionUpdate\n{space}{val}");
        }

        private void MissionInfo()
        {
            DebugLog.Severe(this, $"{_prefix} Mission has updated: OnMissionComplete\n");
        }

        #endregion MissionInfo

        #region UnitInfo

        private void UnitInfo(UnitEventType type, Unit unit, int val = 0)
        {
            DebugLog.Warning(this, $"{_prefix} Unit has updated: OnUnit{type}\n{space}{unit.ID}, {unit.hp}, {val}");
        }

        private enum UnitEventType
        {
            Add,
            GetsDamaged,
            Dies
        }

        #endregion UnitInfo

        private void TestLogger()
        {
            DebugLog.Log(Icon.LI_INFO, Level.L_ALL, $"{_prefix} Logger test!! it'll spam console but who cares!!\n{line}");

            DebugLog.Finest($"{_prefix} Finest test!\n");
            DebugLog.Finer($"{_prefix} Finer test!\n");
            DebugLog.Fine($"{_prefix} Fine test!\n");
            DebugLog.Config($"{_prefix} Config test!\n");
            DebugLog.Info($"{_prefix} Info test!\n");
            DebugLog.Warning($"{_prefix} Warning test!\n");
            DebugLog.Severe($"{_prefix} Severe test!\n");

            DebugLog.Log(Icon.LI_INFO, Level.L_ALL, $"{_prefix} Logger Level test!!\n{line}");

            DebugLog.SetLevel(Level.L_WARNING);
            DebugLog.Info($"{_prefix} This message shouldnt be seen!\n");
            DebugLog.Warning($"{_prefix} This message should be seen!\n");
            DebugLog.SetLevel(Level.L_ALL);

            DebugLog.Log(Icon.LI_INFO, Level.L_ALL, $"{_prefix} Logger test complete!!\n{line}");
        }


    }
}