using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace JustPlanes.Network
{
    public class UnitSpawner
    {

        private long timer;
        private long spawnInterval = 5000;

        private Dictionary<string, Unit> units = new Dictionary<string, Unit>();
        public Queue<Unit> unitsToSend = new Queue<Unit>();
        private int maxUnitAmount = 10;

        public UnitSpawner()
        {

        }

        internal void Tick(long elapsedMilliseconds)
        {
            timer += elapsedMilliseconds;
            if (timer >= spawnInterval)
            {
                timer -= spawnInterval;

                if (units.Count < maxUnitAmount)
                {
                    string id = Guid.NewGuid().ToString();
                    int x = 0;
                    int y = 0;
                    Unit unit = new Unit(id, x, y);
                    units.Add(unit.ID, unit);
                    unitsToSend.Enqueue(unit);
                    Console.WriteLine($"[GAME] Spawning unit: {unit.ID}");
                }
            }
        }

        internal void Start()
        {
            timer = 0;
        }
    }

}