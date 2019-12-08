using System;
using System.Drawing;

namespace JustPlanes.Network
{
    public class Unit
    {
        public string ID;

        public int hp = 100;

        public int X
        {
            get { return point.X; }
            private set { point.X = value; }
        }

        public int Y
        {
            get { return point.Y; }
            private set { point.Y = value; }
        }

        private Point point;

        public Unit(string id, Point point)
        {
            ID = id;
            this.point = point;
        }

        public Unit(string id, int x, int y)
        {
            ID = id;
            X = x;
            Y = y;
        }

        internal bool IsDead()
        {
            return hp <= 0;
        }
    }

}