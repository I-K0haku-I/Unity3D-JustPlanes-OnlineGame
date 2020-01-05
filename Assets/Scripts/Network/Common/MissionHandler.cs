
using System;

namespace JustPlanes.Network
{
    public class MissionHandler
    {
        // TODO: add ID if we want more of the same mission type
        public int enemiesKilled;
        public int enemiesToKill = 5;

        public bool IsDone = false;

        public MissionTypes type = MissionTypes.MTKILLRats;

        private Random rand = new Random();
        private IPlayerHolder playerHolder;

        public MissionHandler()
        {

        }

        public MissionHandler(IPlayerHolder _playerHolder)
        {
            playerHolder = _playerHolder;
        }


        internal void Start()
        {
            enemiesKilled = 0;
            int playerAmount = 0;
            if (Server.GameRunner.Game.IsRunning)
                playerAmount = Server.GameRunner.Game.clients.Count;
            else if (playerHolder != null)
                playerAmount = playerHolder.GetPlayerAmount();
            enemiesToKill = rand.Next(1 + playerAmount, 6 + playerAmount);
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

    public interface IPlayerHolder
    {
        int GetPlayerAmount();
    }
}