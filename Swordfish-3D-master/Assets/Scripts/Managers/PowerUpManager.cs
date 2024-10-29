using System;
using System.Collections;
using System.Collections.Generic;
using NoMonoClasses;
using UnityEngine;

namespace Managers
{
    public class PowerUpManager : MonoBehaviour
    {
        public static PowerUpManager instance;
        
        public Dictionary<PowerUpTypes, Action> onPowerUpEndTasks = new Dictionary<PowerUpTypes, Action>();
        public HashSet<Coroutine> powerUpEnCoroutines = new HashSet<Coroutine>();

        private void Awake()
        {
            instance = this;
        }

        public void InitializePowerUps(PlayerData data)
        {
            for (var i = 0; i < data.PowerUpsStats.Length; i++)
            {
                InitializePowerUp(data.PowerUpsStats[i]);
            }
            
            XMLManager.instance.SaveGameData();
        }
        
        public void InitializePowerUp(PowerUpStats powerUp)
        {
            if (powerUp.ownedAmount > 0)
            {
                powerUp.ownedAmount -= 1;
                powerUp.isCurrentlyActive = true;
                powerUpEnCoroutines.Add(StartCoroutine(PowerUpTimer(powerUp)));
            }
        }

        public void RegisterActionOnPowerUpEnd(PowerUpTypes type, Action action)
        {
            if (!onPowerUpEndTasks.ContainsKey(type))
            {
                onPowerUpEndTasks.Add(type, action);
            }
            
            if (onPowerUpEndTasks.ContainsKey(type))
            {
                onPowerUpEndTasks[type] = action;
            }
        }

        private void OnDestroy()
        {
            foreach (var coroutine in powerUpEnCoroutines)
            {
                if(coroutine!= null)StopCoroutine(coroutine);
            }
        }

        public IEnumerator PowerUpTimer(PowerUpStats powerUp)
        {
            yield return new WaitForEndOfFrame();
            UIManager.Instance.InGameSetPowerUpActive(GameManager.Instance.Player,true,powerUp.type );
            UIManager.Instance.InGameSetPowerUpProgress(GameManager.Instance.Player,1,powerUp.type );
            
            var progress = 0.0f;
            while (powerUp.isCurrentlyActive)
            {
                yield return new WaitForSeconds(1f);
                
                progress += 1f;
                UIManager.Instance.InGameSetPowerUpProgress(GameManager.Instance.Player,(powerUp.timeActive-progress)/powerUp.timeActive,powerUp.type );

                if (progress >= powerUp.timeActive)
                {
                    powerUp.isCurrentlyActive = false;

                    if (onPowerUpEndTasks.ContainsKey(powerUp.type))
                    {
                        onPowerUpEndTasks[powerUp.type]?.Invoke();
                    }
                    
                    UIManager.Instance.InGameSetPowerUpActive(GameManager.Instance.Player,false,powerUp.type );
                }
            }
        }

        public void ResetPowerUps(PlayerData data)
        {
            foreach (var kvp in onPowerUpEndTasks)
            {
                kvp.Value?.Invoke();
            }
            
            foreach (var coroutine in powerUpEnCoroutines)
            {
                if(coroutine!= null)StopCoroutine(coroutine);
            }

            for (var i = 0; i < data.PowerUpsStats.Length; i++)
            {
                data.PowerUpsStats[i].isCurrentlyActive = false;
                
                if (onPowerUpEndTasks.ContainsKey(data.PowerUpsStats[i].type))
                {
                    onPowerUpEndTasks[data.PowerUpsStats[i].type]?.Invoke();
                }
                    
                UIManager.Instance.InGameSetPowerUpActive(GameManager.Instance.Player,false,data.PowerUpsStats[i].type );
            }
        }

        public void ResetAfterDeath(PlayerData data)
        {
            foreach (var powerUp in data.PowerUpsStats)
            {
                powerUp.ownedAmount = 0;
                powerUp.timeActive = 0f;
            }
        }
    }
}
