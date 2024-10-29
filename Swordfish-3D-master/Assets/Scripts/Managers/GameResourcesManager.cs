using System.Collections.Generic;
using Io.Helpers;
using NoMonoClasses;
using SnakeScripts;
using UI;
using UnityEngine;

namespace Managers
{
    public class GameResourcesManager : MonoBehaviour
    {
        public static GameResourcesManager instance;

        [HideInInspector] 
        public bool _isFirstTutorial = false;

        public SwordFishParts currentlySelectedCharacter;
        public SwordFishParts temporarilySelectedCharacter;
        public PlayerData     playerData;
        
        private GameObject     _shopCharacterGameObject;

        public SwordFishParts[] allSwordFishSOs;
        
        public Dictionary<string, SwordFishParts> allSwordFishesDic = new Dictionary<string, SwordFishParts>();

        public Dictionary<PowerUpTypes, PowerUpStats> _powerUp = new Dictionary<PowerUpTypes, PowerUpStats>();

        public GameModes currentlySelectedGameMode = GameModes.Classic;

        [HideInInspector] 
        private GameObject loadingPanel;

        public void ShowLoadingPanel(bool show)
        {
            loadingPanel.SetActive(show);
        }

        private void Awake()
        {
            instance = this;
            Initialize();
            
            loadingPanel = GameObject.FindWithTag("Loading");
            MySceneManager.instance.LoadSceneAdditive("SW_Menu");
        }

        public bool BuyItem(SwordFishParts parts)
        {
            if (parts.swItem.price.priceType == ItemPriceType.Coin)
            {
                if (playerData.coins >= parts.swItem.price.amount)
                {
                    playerData.coins -= parts.swItem.price.amount;
                    parts.swItem.avalability = ItemAvalability.IsBought;
                    XMLManager.instance.SaveGameData();
                    return true;
                }

                return false;
            }

            if (playerData.gems >= parts.swItem.price.amount)
            {
                playerData.gems -= parts.swItem.price.amount;
                parts.swItem.avalability = ItemAvalability.IsBought;
                XMLManager.instance.SaveGameData();
                return true;
            }

            return false;
        }
        

        public void Initialize()
        {
            //Create sw dictionary
            for (var i = 0; i < allSwordFishSOs.Length; i++)
            {
                if(!allSwordFishesDic.ContainsKey(allSwordFishSOs[i].swItem.itemName))
                    allSwordFishesDic.Add(allSwordFishSOs[i].swItem.itemName, allSwordFishSOs[i]);

                if (i != 0)
                {
                    allSwordFishSOs[i].swItem.avalability = ItemAvalability.IsLocked;
                }
            }
            
            playerData = XMLManager.instance.database.playerData;
            
            if (XMLManager.instance.database.allItems != null) // IS FIRST LOAD
            {
                InitializeLoadedSwItems(XMLManager.instance.database.allItems);
            }
            else // ISN'T FIRST LOAD
            {
                XMLManager.instance.SaveGameData();
            }

            currentlySelectedGameMode = XMLManager.instance.database.selectedGameMode;
            currentlySelectedCharacter = temporarilySelectedCharacter = allSwordFishesDic[playerData.selectedCharacterName];
        }

        public SWItem[] GetAllSWItems()
        {
            var swItems = new SWItem[allSwordFishSOs.Length];

            for (var i = 0; i < swItems.Length; i++)
            {
                swItems[i] = allSwordFishSOs[i].swItem;
            }

            return swItems;
        }

        public void InitializeLoadedSwItems(SWItem[] savedSwordFishes)
        {
            for (var i = 0; i < savedSwordFishes.Length; i++)
            {
                if(allSwordFishesDic.ContainsKey(savedSwordFishes[i].itemName))
                    allSwordFishesDic[savedSwordFishes[i].itemName].swItem = savedSwordFishes[i];
            }
        }

        public void SelectCharacter(SwordFishParts parts, bool isTemporarily)
        {
            if (isTemporarily)
                temporarilySelectedCharacter = parts;
            else
                currentlySelectedCharacter = parts;

            MenuManager.instance.CheckItemAvailability(parts);
            MenuManager.instance.PrepareSelectedCharacter(parts);
        }
        
        public void ResetCharacter()
        {
            MenuManager.instance.PrepareSelectedCharacter(currentlySelectedCharacter);
        }

        public void FinalizeSelect()
        {
            if(temporarilySelectedCharacter.swItem.avalability == ItemAvalability.IsLocked)
                return;
            
            currentlySelectedCharacter = temporarilySelectedCharacter;
            MenuManager.instance.PrepareSelectedCharacter(currentlySelectedCharacter);
            playerData.selectedCharacterName = currentlySelectedCharacter.swItem.itemName;
            XMLManager.instance.SaveGameData();
        }
    }
}
