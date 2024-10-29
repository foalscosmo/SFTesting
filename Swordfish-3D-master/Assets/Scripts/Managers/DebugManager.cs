using UnityEngine;
using UnityEngine.UI;

namespace Managers
{
    public class DebugManager : MonoBehaviour
    {
        public static DebugManager instance;
        
        public Slider MaxSpeedSlider;
        public Slider BoostStrengthSlider, NewhandlingSlider, BoostRegenSlider, NewBoostTimeOnSlider;

        public GameObject sliderHodlerrrr;

        private void Awake()
        {
            instance = this;
        }
        
        public void InitDebug(float maxSpeed = 0, float bumpForce = 25f, float bumpTimer = 0.5f, float boostStrength = 3f,
            float handlingSpeed = 0.4f, float boostColdown = 0.5f, float newBoostTimeon = 0.3f)
        {
			
            var active = sliderHodlerrrr.activeSelf;
            sliderHodlerrrr.SetActive(true);
            
            
            //1
            NewBoostTimeOnSlider.value = newBoostTimeon;
            ChangeBoostTimeon(newBoostTimeon);
            
            //2
            BoostStrengthSlider.value = boostStrength;
            ChangeBoostvalFromSlider(boostStrength);
            
            //3
            NewhandlingSlider.value = handlingSpeed;
            Changehandling(handlingSpeed);
            
            //4
            MaxSpeedSlider.value = maxSpeed;
            ChangeMaxSpeed(maxSpeed);
            
            //5
            BoostRegenSlider.value = boostColdown;
            UpdateBoostCoolDown(boostColdown);
			
            GameManager.Instance.UpdateMaxSpeed(maxSpeed);

            NewBoostTimeOnSlider.onValueChanged.AddListener(ChangeBoostTimeon);
            BoostStrengthSlider.onValueChanged.AddListener(ChangeBoostvalFromSlider);
            MaxSpeedSlider.onValueChanged.AddListener(ChangeMaxSpeed);
            NewhandlingSlider.onValueChanged.AddListener(Changehandling);
            BoostRegenSlider.onValueChanged.AddListener(UpdateBoostCoolDown);
            sliderHodlerrrr.SetActive(active);
        }

        //1
        private void ChangeBoostTimeon(float val)
        {
            GameManager.Instance.UpdateBoostActiveTime(val);
        }
		
        //2
        private void ChangeBoostvalFromSlider(float force)
        {
            GameManager.Instance.UpdateBoostValue(force);
        }
        
        //3
        private void Changehandling(float handling)
        {
            GameManager.Instance.UpdateHandlingSpeed(handling);
        }
        
        //4
        public void ChangeMaxSpeed(float speed)
        {
            GameManager.Instance.UpdateMaxSpeed(speed);
        }
        
        //5
        private void UpdateBoostCoolDown(float cooldown)
        {
            GameManager.Instance.UpdateBoostCoolDown(cooldown);
        }
    }
}
