using System;
using System.Drawing;

namespace JustPlanes.Core
{
    public class Unit
    {
        public string ID;

        public int maxHP = 100;

        public int hp;

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
            hp = maxHP;
        }
        public Unit(string id, int _hp, Point point)
        {
            ID = id;
            this.point = point;
            hp = _hp;
        }
        
        public Unit(string id, int x, int y)
        {
            ID = id;
            X = x;
            Y = y;
            hp = maxHP;
        }

        public Unit(string id, int _hp, int x, int y)
        {
            ID = id;
            X = x;
            Y = y;
            hp = _hp;
        }

        internal bool IsDead()
        {
            return hp <= 0;
        }
    }

}