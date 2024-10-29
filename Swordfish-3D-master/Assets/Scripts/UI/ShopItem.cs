using Managers;
using SnakeScripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ShopItem : MonoBehaviour
    {
        [Header("Components")] 
        public Image bg;
        public Image icon;
        public TextMeshProUGUI nameTMP;
        public Sprite boughtBG, lockedBG;
        public Image selectionImage;
        
        //[Header("Code Purposes")]
        [HideInInspector]
        public SwordFishParts swParts;

        public void Initialize(SwordFishParts parts, bool isSelected = false)
        {
            icon.sprite = parts.icon;
            nameTMP.text = parts.swItem.itemName;
            bg.sprite = parts.swItem.avalability == ItemAvalability.IsBought ? boughtBG : lockedBG;
            swParts = parts;

            if (isSelected)
            {
                OnPressed();
            }
        }

        public void OnPressed()
        {
            MenuManager.instance.NewShopItemIsSelected(this);
            
            GameResourcesManager.instance.SelectCharacter(swParts, true);
        }
    }
}
