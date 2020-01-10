using System;
using System.Linq;
using System.Diagnostics;
using System.Collections.Concurrent;
using System.Threading;
using System.Collections.Generic;

namespace JustPlanes.Core.Network.Server
{

    public static class GameRunner
    {
        public static Game Game;

        public static void Initialize()
        {
            Game = new Game();
        }

        public static void Run()
        {
            Game.StartLoop();
        }

        public static void Stop()
        {
            Game.Stop();
        }
    }

    public class Game
    {
        public ConcurrentQueue<int> queue = new ConcurrentQueue<int>();
        public bool IsRunning;
        private long tickRate = 10;  // ticks per second
        internal ConcurrentQueue<string> msgQueue = new ConcurrentQueue<string>();
        internal ConcurrentQueue<string> clientConnectedQueue = new ConcurrentQueue<string>();
        internal ConcurrentQueue<string> clientDisconnectedQueue = new ConcurrentQueue<string>();
        internal Dictionary<string, string> clients = new Dictionary<string, string>();
        // internal ConcurrentDictionary<string, Client> clients = new ConcurrentDictionary<string, Client>();
        // public Dictionary<string, Player

        // private UnitSpawner unitSpawner = new UnitSpawner();
        public PlayerManager playerManager;
        public Authenticator auth;
        // internal ConcurrentQueue<Tuple<string, int>> damageQueue = new ConcurrentQueue<Tuple<string, int>>();
        // internal List<Unit> unitDeathToSend = new List<Unit>();

        internal bool AddPlayerName(string connId, string name)
        {
            if (clients.ContainsValue(name))
                return false;
            clients[connId] = name;
            playerManager.AddPlayer(name);
            return true;
        }

        internal List<Tuple<string, int>> damageToSend = new List<Tuple<string, int>>();

        // public MissionHandler mission = new MissionHandler();
        private List<int> missionUpdates = new List<int>();
        public ConcurrentQueue<Action> handleDataQueue = new ConcurrentQueue<Action>();

        public void StartLoop()
        {
            int millisecondsToTick = (int)(1000 / tickRate);
            int lastTickElapsed = 0;
            int toSleep = 0;
            int toSleepDept = 0;
            Stopwatch stopwatch = new Stopwatch();
            NetworkMagic.IsServer = true;
            auth = new Authenticator();
            playerManager = new PlayerManager();
            // playerManager.AddPlayer("Test1");
            // playerManager.AddPlayer("Test2");
            // unitSpawner.Start();
            IsRunning = true;
            while (IsRunning)
            {
                lastTickElapsed = (int)stopwatch.ElapsedMilliseconds;
                toSleep = millisecondsToTick + toSleepDept - lastTickElapsed;
                if (toSleep <= 0)
                    toSleepDept += (toSleep * -1);
                else
                    Thread.Sleep(toSleep);
                stopwatch.Restart();
                Update(lastTickElapsed);
            }
        }

        private void Update(long elapsedMilliseconds)
        {
            if (msgQueue.TryDequeue(out string result))
                Console.WriteLine(result);

            while (handleDataQueue.TryDequeue(out var action))
            {
                action();
            }

            while (clientDisconnectedQueue.TryDequeue(out var connID))
            {
                playerManager.RemovePlayer(clients[connID]);
                clients.Remove(connID);
            }
            while (clientConnectedQueue.TryDequeue(out var connID))
            {
                clients.Add(connID, "");
            }

            // while (damageQueue.TryDequeue(out var damageItem))
            // {
            //     if (unitSpawner.units.ContainsKey(damageItem.Item1))
            //     {
            //         Unit u = unitSpawner.units[damageItem.Item1];
            //         int damage = damageItem.Item2;
            //         u.hp -= damage;
            //         damageToSend.Add(damageItem);
            //         if (u.IsDead())
            //         {
            //             unitDeathToSend.Add(u);
            //             unitSpawner.units.Remove(u.ID);
            //             mission.Progress();
            //             missionUpdates.Add(1);
            //         }
            //     }
            //     else
            //     {
            //         Console.WriteLine($"Tried to damage but id does not exist: {damageItem.Item1}");
            //     }
            // }

            // unitSpawner.Tick(elapsedMilliseconds);
            // foreach (var unit in unitSpawner.unitsToSend)
            // {
            //     DataSender.SendUnitSpawned(unit);
            // }
            // unitSpawner.unitsToSend.Clear();

            // if (damageToSend.Count > 0)
            // {
            //     DataSender.SendUnitsDamage(damageToSend);
            //     damageToSend.Clear();
            // }

            // if (unitDeathToSend.Count > 0)
            // {
            //     DataSender.SendUnitsDied(unitDeathToSend);
            //     unitDeathToSend.Clear();
            // }

            // if (missionUpdates.Count > 0)
            // {
            //     DataSender.SendMissionUpdate(missionUpdates);
            //     missionUpdates.Clear();
            // }

            // if (mission.IsDone)
            // {
            //     DataSender.SendMissionComplete();
            //     mission.Reset();
            //     DataSender.SendGiveMission(mission);
            // }
        }

        // internal Unit GetUnit(string id)
        // {
        //     return unitSpawner.units[id];
        // }

        // internal List<Unit> GetUnits()
        // {
        //     return unitSpawner.units.Values.ToList();
        // }

        public void Stop()
        {
            Console.WriteLine("CANCELED GAME LOOP!! EXITING...");
            IsRunning = false;
        }
    }

}