using System;
using System.Linq;
using System.Diagnostics;
using System.Collections.Concurrent;
using System.Threading;
using System.Collections.Generic;

namespace JustPlanes.Network.Server
{
    public static class Game
    {
        public static ConcurrentQueue<int> queue = new ConcurrentQueue<int>();
        public static bool IsRunning;
        private static long tickRate = 10;  // ticks per second
        internal static ConcurrentQueue<string> msgQueue = new ConcurrentQueue<string>();
        internal static ConcurrentDictionary<int, Player> players = new ConcurrentDictionary<int, Player>();

        private static UnitSpawner unitSpawner = new UnitSpawner();
        internal static ConcurrentQueue<Tuple<string, int>> damageQueue = new ConcurrentQueue<Tuple<string, int>>();
        internal static List<Unit> unitDeathToSend = new List<Unit>();
        internal static List<Tuple<string, int>> damageToSend = new List<Tuple<string, int>>();

        public static MissionHandler mission = new MissionHandler();
        private static List<int> missionUpdates = new List<int>();

        public static void StartLoop()
        {
            int millisecondsToTick = (int)(1000 / tickRate);
            Stopwatch stopwatch = new Stopwatch();
            unitSpawner.Start();
            IsRunning = true;
            while (IsRunning)
            {
                stopwatch.Restart();
                Thread.Sleep(millisecondsToTick);
                Update(stopwatch.ElapsedMilliseconds);
            }
        }

        private static void Update(long elapsedMilliseconds)
        {
            if (msgQueue.TryDequeue(out string result))
                Console.WriteLine(result);

            // mission.Tick(elapsedMilliseconds);

            while (damageQueue.TryDequeue(out var damageItem))
            {
                if (unitSpawner.units.ContainsKey(damageItem.Item1))
                {
                    Unit u = unitSpawner.units[damageItem.Item1];
                    int damage = damageItem.Item2;
                    u.hp -= damage;
                    damageToSend.Add(damageItem);
                    if (u.IsDead())
                    {
                        unitDeathToSend.Add(u);
                        unitSpawner.units.Remove(u.ID);
                        mission.Progress();
                        missionUpdates.Add(1);
                    }
                }
                else
                {
                    Console.WriteLine($"Tried to damage but id does not exist: {damageItem.Item1}");
                }
            }

            unitSpawner.Tick(elapsedMilliseconds);
            foreach (var unit in unitSpawner.unitsToSend)
            {
                DataSender.SendUnitSpawned(unit);
            }
            unitSpawner.unitsToSend.Clear();

            if (damageToSend.Count > 0)
            {
                DataSender.SendUnitsDamage(damageToSend);
                damageToSend.Clear();
            }

            if (unitDeathToSend.Count > 0)
            {
                DataSender.SendUnitsDied(unitDeathToSend);
                unitDeathToSend.Clear();
            }

            if (missionUpdates.Count > 0)
            {
                DataSender.SendMissionUpdate(missionUpdates);
                missionUpdates.Clear();
            }

            if (mission.IsDone)
            {
                DataSender.SendMissionComplete();
                mission.Reset();
                DataSender.SendGiveMission(mission);
            }
        }

        internal static Unit GetUnit(string id)
        {
            return unitSpawner.units[id];
        }

        internal static List<Unit> GetUnits()
        {
            return unitSpawner.units.Values.ToList();
        }

        public static void Stop()
        {
            Console.WriteLine("CANCELED GAME LOOP!! EXITING...");
            IsRunning = false;
        }

    }

}