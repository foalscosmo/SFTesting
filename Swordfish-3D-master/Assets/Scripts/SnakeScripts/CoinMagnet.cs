using System.Collections;
using CandyCoded.HapticFeedback;
using Car;
using DG.Tweening;
using Managers;
using SW_Tutorial;
using UnityEngine;

namespace SnakeScripts
{
    public class CoinMagnet : MonoBehaviour
    {
        private Vector3         _startingScale;
        private SWController   _controllingSw;
        private bool            _isInitialized;

        public CoinMagnet Initialize(SWController snake)
        {
            if (_isInitialized)
            {
                this.transform.localScale = _startingScale;
                return this;
            }
            
            _isInitialized = true;
            _controllingSw = snake;
            _startingScale = this.transform.localScale;
            
            var amount = (int)_controllingSw.playerData.GetPowerupAffection(PowerUpTypes.Magnet);
            transform.localScale = _startingScale * amount;

            if (TutorialManager.instance != null)
                return this;
            PowerUpManager.instance.RegisterActionOnPowerUpEnd(PowerUpTypes.Magnet, () =>
            {
                transform.localScale = _startingScale;
            });
            
            return this;
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Coin"))
            {
                StartCoroutine(OnCollected(other.transform.GetComponent<Coin>()));
                if (_controllingSw.SwType == SWType.Player)
                {
                    HapticFeedback.LightFeedback();
                }
            }
        }

        private IEnumerator OnCollected(Coin coin)
        {
            coin.transform.DOMove(this.transform.position + Vector3.up, 0.2f);
            yield return new WaitForSeconds(0.2f);
            coin.Disappear();
            _controllingSw.GotCollectAble(coin);
        }
    }
}
