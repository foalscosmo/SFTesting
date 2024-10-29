using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SnakeScripts
{
    public class Coin : MonoBehaviour
    {
        public Action<Coin> onCoinCollected;

        private Coroutine _dissapearCoroutine;

        public GameObject coin, gem;
        public bool isCoin = true;
        public void Throw(bool _isCoin = true)
        {
            this.isCoin = _isCoin;
            coin.SetActive(this.isCoin);
            gem.SetActive(!this.isCoin);
            
            var up = Vector3.right;
            up = Quaternion.Euler(0, Random.Range(-180,180), 0) * up;
            
            this.transform.position =  this.transform.position + up * Random.Range(3, 8);
            _dissapearCoroutine = StartCoroutine(Timer());
        }

        private IEnumerator Timer()
        {
            yield return new WaitForSeconds(10f);
            Disappear();
        }

        public void Disappear()
        {
            if (_dissapearCoroutine != null)
            {
                StopCoroutine(_dissapearCoroutine);
            }
            onCoinCollected?.Invoke(this);
            onCoinCollected = null;
        }
    }
}
