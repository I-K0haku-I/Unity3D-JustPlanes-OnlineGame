using System;
using System.Drawing;
using System.Diagnostics;
using System.Collections.Generic;

namespace JustPlanes.Core
{
    public class Rectangle
    {
        private int X;
        private int Y;
        private int Width;
        private int Height;

        public Rectangle(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public Point GetRandomPoint()
        {
            var rand = new Random();
            return new Point(rand.Next(X, X + Width), rand.Next(Y - Height, Y));
        }
    }

    public class UnitSpawner
    {

        private long timer;
        private long spawnInterval = 5000;

        public Dictionary<string, Unit> units = new Dictionary<string, Unit>();
        public Queue<Unit> unitsToSend = new Queue<Unit>();
        private int maxUnitAmount = 10;
        private Rectangle spawnRect = new Rectangle(-100, 100, 200, 200);

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
                    // TODO: make Unit hold Guid by them selves instead of string
                    string id = Guid.NewGuid().ToString();
                    Point point = spawnRect.GetRandomPoint();
                    Unit unit = new Unit(id, point);
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