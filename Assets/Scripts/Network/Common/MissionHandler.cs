
using System;

namespace JustPlanes.Network
{
    public class MissionHandler
    {
        // TODO: add ID if we want more of the same mission type
        public int enemiesKilled;
        public int enemiesToKill = 25;
        
        public bool IsDone = false;

        public MissionTypes type = MissionTypes.MTKILLRats;

        public MissionHandler()
        {

        }

        internal void Start()
        {
            enemiesKilled = 0;
        }

        internal void Tick(long elapsedMilliseconds)
        {

        }

        internal void Progress()
        {
            if (IsDone)
                return;

            enemiesKilled += 1;
            if (enemiesKilled >= enemiesToKill)
                IsDone = true;
        }

        internal void Update(int enemiesKilledDelta)
        {
            enemiesKilled += enemiesKilledDelta;
            if (enemiesKilled >= enemiesToKill)
                IsDone = true;
        }

        internal void Reset()
        {
            Start();
            IsDone = false;
        }
    }
}