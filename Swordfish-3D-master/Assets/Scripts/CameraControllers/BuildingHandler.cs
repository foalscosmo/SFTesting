using System;
using System.Collections.Generic;
using DG.Tweening;
using SnakeScripts;
using UnityEngine;

namespace CameraControllers
{
    public class BuildingHandler : MonoBehaviour
    {
        [SerializeField] private List<GameObject> buildingObjs = new();
        private const int EndValue = 0;
        private const int StartValue = 1;
        private const float Duration = 0.3f;
        private bool _hasAppearedInTrigger;
        private void Start() => _hasAppearedInTrigger = false;
        private void OnTriggerEnter(Collider other)
        {
            var weaponController = other.GetComponent<SWWeaponController>();
            if (weaponController != null && weaponController.IsMainPlayer) DisableBuildingsWithTween();
        }
        private void OnTriggerStay(Collider other)
        {
            if(_hasAppearedInTrigger) return;
            var weaponController = other.GetComponent<SWWeaponController>();
            if (weaponController != null && weaponController.IsMainPlayer) DisableBuildingsWithTween();
            _hasAppearedInTrigger = true;
        }
        private void OnTriggerExit(Collider other)
        {
            var weaponController = other.GetComponent<SWWeaponController>();
            if (weaponController != null && weaponController.IsMainPlayer) EnableBuildingsWithTween();
        }
        private void DisableBuildingsWithTween()
        {
            foreach (var building in buildingObjs) building.transform.DOScale(EndValue, Duration);
        }
        private void EnableBuildingsWithTween()
        {
            foreach (var building in buildingObjs) building.transform.DOScale(StartValue, Duration);
        }
    }
}