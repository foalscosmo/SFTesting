using CameraControllers;
using CandyCoded.HapticFeedback;
using Car;
using DG.Tweening;
using Managers;
using UnityEngine;

namespace SnakeScripts
{
    public class SWWeapon : MonoBehaviour
    {
        private SWController _controllingSw;
        
        public bool canKill = true;
        
        public void Initialize(SWController swordFishOwner)
        {
            _controllingSw = swordFishOwner;
        }

        public float scaleAmount;

        private void OnTriggerEnter(Collider other)
        {
            if (_controllingSw.isFreshlyRespawned || !canKill)
                return;

            if (other.CompareTag("Snake"))
            {
                var otherSWBody = other.transform.GetComponent<SWBody>();
                var otherSWController = other.transform.parent.GetComponent<SWController>();
                if(otherSWController == null) return;
                if (otherSWController.player.currentKillCount != 0)
                {
                    scaleAmount = otherSWController.player.currentKillCount / 5f;
                }
                else if (otherSWController.player.currentKillCount == 0)
                {
                    scaleAmount = 0.2f;
                }
                
                if (otherSWBody != null && otherSWBody.Sw != null && otherSWBody.Sw != _controllingSw &&
                    !otherSWBody.Sw.isDead)
                {
                    if (otherSWBody.Sw.isFreshlyRespawned)
                        return;

                    if (GameModeManager.instance.currentGameMode != GameModes.Classic &&
                        !GameManager.Instance.IsEnemy(otherSWBody.Sw, _controllingSw)) 
                        return;

                    GameManager.Instance.ThrowCoins(otherSWBody.Sw);
                    otherSWBody.Kill();

                    Vector3 newScale = transform.parent.parent.localScale + new Vector3(scaleAmount, scaleAmount, scaleAmount);
                    newScale = Vector3.Min(newScale, new Vector3(15f, 15f, 15f)); 
                    transform.parent.parent.DOScale(newScale, 0.5f);
                    
                    if (_controllingSw.SwType == SWType.Player)
                    {
                        HapticFeedback.HeavyFeedback();
                        float zoomAmount = scaleAmount is >= 0.2f and <= 0.5f ? 3f : 6f;
                        float maxZoom = 40f;
                        float currentZoom = CameraTopDown.Instance.GetZoomLevel();
                        float newZoom =
                            Mathf.Max(currentZoom + zoomAmount, maxZoom); 

                        CameraTopDown.Instance.DoZoom(newZoom, 0.5f); 
                        SoundManager.instance.PlaySound(SoundTypesEnum.SnekKill);
                    }

                    _controllingSw.GotKill(); 
                }
            }
        }
    }
}