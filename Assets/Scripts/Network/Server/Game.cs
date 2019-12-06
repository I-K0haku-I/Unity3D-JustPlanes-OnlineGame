using System;
using System.Diagnostics;
using System.Collections.Concurrent;
using System.Threading;

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
            // if (unitSpawner.unitsToSend.TryDequeue(out Unit unit))
            //     DataSender.SendUnitSpawned(unit);
        }

        public static void Stop()
        {
            Console.WriteLine("CANCELED GAME LOOP!! EXITING...");
            IsRunning = false;
        }
    }
    
}