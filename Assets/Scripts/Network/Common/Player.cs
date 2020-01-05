using System;

namespace JustPlanes.Network
{
    public class Player
    {
        public string Name;
        public string ConnID;
        public int X;
        public int Y;

        public Player(string name, int x, int y)
        {
            Name = name;
            X = x;
            Y = y;
        }

        public Player(string connID, string name, int x, int y)
        {
            Name = name;
            ConnID = connID;
            X = x;
            Y = y;
        }
    } 
}