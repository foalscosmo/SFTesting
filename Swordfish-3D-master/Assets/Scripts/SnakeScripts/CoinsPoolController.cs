using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SnakeScripts
{
    public class CoinsPoolController : MonoBehaviour
    {
        public static CoinsPoolController instance;

        public int coinsPoolLength = 100;
        public GameObject coinPrefab;
        
        private Dictionary<GameObject, Coin> _freeCoins = new Dictionary<GameObject, Coin>();
        
        public HashSet<Transform> activeCoins = new HashSet<Transform>();
            

        private void Awake()
        {
            instance = this;

            InitializeCoins();
        }

        private void InitializeCoins()
        {
            for (var i = 0; i < coinsPoolLength; i++)
            {
                var coin = GameObject.Instantiate(coinPrefab, this.transform, false).GetComponentInChildren<Coin>();
                PrepareCoin(coin);
            }
        }

        private void PrepareCoin(Coin coin)
        {
            coin.gameObject.SetActive(false);
            coin.transform.position = new Vector3(1000,1000,1000);
            
            if(!_freeCoins.ContainsKey(coin.gameObject))
                _freeCoins.Add(coin.gameObject, coin);
            
            if(activeCoins.Contains(coin.transform))
                activeCoins.Remove(coin.transform);
            
        }

        private Coin GetFreeCoin()
        {
            return _freeCoins.First().Value;
        }

        public void InstantiateCoins(Vector3 position, int count)
        {
            for (var i = 0; i < count; i++)
            {
                var coin = GetFreeCoin();
                
                //var offset = Quaternion.Euler(0, Random.Range(-180,180), 0) * (Vector3.forward * 2);

                //coin.transform.position = position + offset;
                coin.transform.position = position + (Vector3.up / 2);

                _freeCoins.Remove(coin.gameObject);

                coin.onCoinCollected += OnCoinCollected;

                activeCoins.Add(coin.transform);
                    
                coin.gameObject.SetActive(true);
                coin.Throw(Random.Range(0,100) > 8);
            }
        }

        private void OnCoinCollected(Coin coin)
        {
            PrepareCoin(coin);
        }
    }
}
