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

            unitSpawner.Tick(elapsedMilliseconds);
            foreach (var unit in unitSpawner.unitsToSend)
            {
                DataSender.SendUnitSpawned(unit);
            }
            unitSpawner.unitsToSend.Clear();

            while (damageQueue.TryDequeue(out var damageItem))
            {
                if (unitSpawner.units.ContainsKey(damageItem.Item1))
                {
                    Unit u = unitSpawner.units[damageItem.Item1];
                    int damage = damageItem.Item2;
                    u.hp -= damage;
                    if (u.IsDead())
                        DataSender.SendUnitDied(u);
                }
                else
                {
                    Console.WriteLine($"Tried to damage but id does not exist: {damageItem.Item1}");
                }
            }

            // foreach ((var, var) id, damage in damageQueue.)
            // if (unitSpawner.unitsToSend.TryDequeue(out Unit unit))
            //     DataSender.SendUnitSpawned(unit);
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