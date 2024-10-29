using System;
using System.Collections.Generic;
using Managers;
using SW_Tutorial;
using UnityEngine;

namespace NoMonoClasses
{
    [System.Serializable]
    public class PlayerData
    {
        public string playerName = "user name";
        public string selectedCharacterName;
        public int selectedCharacterSkin = 0;
        public int totalKills = 0;
        public int totalDeaths = 0;
        public int coins;
        public int gems;
        
        //Level
        public int currentLevel = 1;
        public int currentExp = 0;
        public int expToNextLevelAtStart = 100;

        public int expToNextLevel
        {
            get { return expToNextLevelAtStart * currentLevel; }
        }

        [Header("Power Ups")] 
        public PowerUpStats[] PowerUpsStats;

        public PlayerData()
        {
            
        }
        
        public PlayerData(PlayerData data)
        {
            this.playerName = data.playerName;
            this.selectedCharacterName = data.selectedCharacterName;
            this.selectedCharacterSkin = data.selectedCharacterSkin;
            this.totalKills = data.totalKills;
            this.totalDeaths = data.totalDeaths;
            this.coins = data.coins;
            this.gems = data.gems;
            this.PowerUpsStats = data.PowerUpsStats;
        }

        public void AddExp(int expAmount)
        {
            currentExp += expAmount;
            //EXP SHEVSEBIS ANIMACIA

            if (expToNextLevel <= currentExp)
            {
                currentExp -= expToNextLevel;
                currentLevel++;
                
                //AMOVAGDOT LEVEL UP PANEL
            }
            
            XMLManager.instance.SaveGameData();
        }
        
        public int GetPowerUpAmount(PowerUpTypes type)
        {
            for (var i = 0; i < PowerUpsStats.Length; i++)
            {
                if (type == PowerUpsStats[i].type)
                    return PowerUpsStats[i].ownedAmount;
            }

            return 0;
        }

        public void AddPowerUp(PowerUpTypes type, int amount)
        {
            for (var i = 0; i < PowerUpsStats.Length; i++)
            {
                if (type == PowerUpsStats[i].type)
                {
                    PowerUpsStats[i].ownedAmount += amount;
                    break;
                }
            }
            
            XMLManager.instance.SaveGameData();
        }

        public void Initialize()
        {
            for (var i = 0; i < PowerUpsStats.Length; i++)
            {
                if (!GameResourcesManager.instance._powerUp.ContainsKey(PowerUpsStats[i].type))
                    GameResourcesManager.instance._powerUp.Add(PowerUpsStats[i].type, PowerUpsStats[i]);
            }
        }

        public float GetPowerupAffectionForShop(PowerUpTypes type)
        {
            return PowerUpsStats[(int) type].affection *
                   PowerUpsStats[(int) type].inAppPurchaseAffection;
        }

        public float GetPowerupAffection(PowerUpTypes type)
        {
            if (TutorialManager.instance != null)
                return 1;
            
            if (GameResourcesManager.instance._powerUp.ContainsKey(type))
                return GameResourcesManager.instance._powerUp[type].isCurrentlyActive ? 
                    GameResourcesManager.instance._powerUp[type].affection * GameResourcesManager.instance._powerUp[type].inAppPurchaseAffection: 
                    GameResourcesManager.instance._powerUp[type].normalAmount * GameResourcesManager.instance._powerUp[type].inAppPurchaseAffection;
            
            return 1;
        }
}

    [Serializable]
    public class PowerUpStats
    {
        public PowerUpTypes type;
        public int          ownedAmount;
        public float        affection;
        public bool         isCurrentlyActive;
        public float        timeActive;
        public float        normalAmount;
        public float        inAppPurchaseAffection = 1;
    }
}
