using UnityEngine;

namespace NoMonoClasses
{
    public class Player
    {
        public int ID;
        public string playerName;
        public int collectedCoins;
        public int currentKillCount;
        public int collectedGems;

        public Player(string name)
        {
            playerName = name;
            ResetStats();
        }

        public void ResetStats()
        {
            collectedCoins = 0;
            collectedGems = 0;
            currentKillCount = 0;
        }
    }
}
