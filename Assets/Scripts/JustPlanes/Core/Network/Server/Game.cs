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

    public class Game : IGame
    {
        public ConcurrentQueue<int> queue = new ConcurrentQueue<int>();
        public bool IsRunning;
        private int tickRate = 10;  // ticks per second
        internal ConcurrentQueue<string> msgQueue = new ConcurrentQueue<string>();
        internal ConcurrentQueue<string> clientConnectedQueue = new ConcurrentQueue<string>();
        internal ConcurrentQueue<string> clientDisconnectedQueue = new ConcurrentQueue<string>();
        internal Dictionary<string, string> clients = new Dictionary<string, string>();
        // internal ConcurrentDictionary<string, Client> clients = new ConcurrentDictionary<string, Client>();
        // public Dictionary<string, Player

        // private UnitSpawner unitSpawner = new UnitSpawner();
        public PlayerManager playerManager;
        public Authenticator auth;
        public PhysicsManager Physics = new PhysicsManager();
        private TestPlane testPlane;
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

        public static Stopwatch GameTimer;

        public void StartLoop()
        {
            float secondsToTick = 1f / tickRate;
            float lastUpdateElapsed = 0f;
            float lastFrameElapsed = 0f;
            float toSleep = 0f;
            float toSleepDept = 0f;
            List<float> listOfUpdateTimes = new List<float>();
            listOfUpdateTimes.Add(0f);
            Stopwatch stopwatchUpdate = new Stopwatch();
            Stopwatch stopwatchLastFrame = new Stopwatch();
            GameTimer = new Stopwatch();
            GameTimer.Start();
            NetworkMagic.IsServer = true;
            auth = new Authenticator();
            playerManager = new PlayerManager();
            testPlane = new TestPlane(this, 3f, 10f, 666);

            // playerManager.AddPlayer("Test1");
            // playerManager.AddPlayer("Test2");
            // unitSpawner.Start();
            IsRunning = true;
            while (IsRunning)
            {
                lastFrameElapsed = (float)stopwatchLastFrame.ElapsedMilliseconds / 1000f;
                stopwatchLastFrame.Restart();

                // toSleepDept += lastUpdateElapsed;
                // if (toSleepDept > secondsToTick)
                //     toSleepDept -= secondsToTick;
                // else
                //     Thread.Sleep((int)(secondsToTick * 1000));

                if (listOfUpdateTimes.Count > 300)
                    listOfUpdateTimes.RemoveRange(0, 150);

                // DebugLog.Warning($"{listOfUpdateTimes.Average()}");
                Thread.Sleep((int)(Math.Max(secondsToTick - listOfUpdateTimes.Average(), 0f) * 1000));

                stopwatchUpdate.Restart();
                Update(lastFrameElapsed);
                lastUpdateElapsed = (float)stopwatchUpdate.ElapsedMilliseconds / 1000f;
                listOfUpdateTimes.Add(lastUpdateElapsed);
            }
        }

        private void Update(float timeDelta)
        {
            if (msgQueue.TryDequeue(out string result))
                Console.WriteLine(result);
            
            // Console.WriteLine(GameTimer.Elapsed);

            while (handleDataQueue.TryDequeue(out var action))
                action();

            while (clientDisconnectedQueue.TryDequeue(out var connID))
            {
                playerManager.RemovePlayer(clients[connID]);
                clients.Remove(connID);
            }
            while (clientConnectedQueue.TryDequeue(out var connID))
            {
                clients.Add(connID, "");
            }

            Physics.Update(timeDelta);
            testPlane.Update(timeDelta);

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

        public float GetTime()
        {
            return GameTimer.ElapsedMilliseconds / 1000f;   
        }

        public PhysicsManager GetPhysicsManager()
        {
            return Physics;
        }
    }

}