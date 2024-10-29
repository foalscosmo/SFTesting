using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum PowerUpTypes{
    Magnet,
    BoostRefill,
    DoubleCoins,
    DoubleGrowSpeed,
    BiggerNoseOnStart,
    FireTrace
}

namespace UI
{
    public class PowerUp : MonoBehaviour
    {
        public PowerUpTypes  powerUpType;
        public Image         filler;
        public Text          textCurrentAmount;
        public TextMeshProUGUI          textAffectionAmount;
        public TextMeshProUGUI          textAffectionAmount2;

        #region InGame

        [Header("In Game")]
        public bool inGame = false;
        public Image iconPowerUp;
        public Image progressPowerUp;

        #endregion

        private void Awake()
        {
            if (!inGame)
                Initialize();
        }

        public void Initialize()
        {
            Fill();
        }

        public void Fill()
        {
            var amount = GameResourcesManager.instance.playerData.GetPowerUpAmount(powerUpType);
            filler.fillAmount = ((float) amount) / 7.0f;
            textCurrentAmount.text = amount.ToString();

            if (powerUpType != PowerUpTypes.BiggerNoseOnStart)
                textAffectionAmount.text = textAffectionAmount2.text = 
                    GameResourcesManager.instance.playerData.GetPowerupAffectionForShop(powerUpType) + "X";
            else
                textAffectionAmount.text = textAffectionAmount2.text = 
                    "+" + GameResourcesManager.instance.playerData.GetPowerupAffectionForShop(powerUpType);
        }
        
        public void OnPowerUpPressed()
        {
            if(GameResourcesManager.instance.playerData.GetPowerUpAmount(powerUpType) < 7) // and ad is loaded
            //watch video if available and than
            GetReward();
            Fill();
        }

        public void GetReward()
        {
            GameResourcesManager.instance.playerData.AddPowerUp(powerUpType, 1);
        }
    }
}
