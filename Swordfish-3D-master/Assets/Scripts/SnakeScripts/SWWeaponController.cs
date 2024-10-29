using System;
using System.Collections.Generic;
using System.Linq;
using Car;
using UnityEngine;

namespace SnakeScripts
{
    public class SWWeaponController : MonoBehaviour
    {
        public bool isFront = true;

        public bool isSide = false;
        
        private SWController _controllingSwordFish;
        
        private GameObject _nosePrefab;
        
        private List<SWWeapon> _weapons = new List<SWWeapon>();

        private float _noseLength;

        public Transform weaponStartingPosition;

        public Transform parent;

        [HideInInspector]
        public Transform lastWeapon;

        public bool IsMainPlayer { get; private set; }

        private void Awake()
        {
            parent = this.transform;
        }

        private void Start()
        {
            if (_controllingSwordFish.SwType == SWType.Player) IsMainPlayer = true;
        }

        public void Initialize(GameObject nosePrefab, SWController owner, int noseLength)
        {
            if(parent == null)
                parent = this.transform.childCount > 0 ? this.transform.GetChild(0) : this.transform;
            
            _controllingSwordFish = owner;
            _nosePrefab = nosePrefab;
            
            EnlargeNose(noseLength);
        }

        public void ResetWeapon()
        {
            for (var i = 0; i < _weapons.Count; i++)
            {
                GameObject.Destroy(_weapons[i].gameObject);
            }
            
            _weapons.Clear();
        }
        
        public void EnlargeNose(int enlargementAmount)
        {
            if (enlargementAmount > 0)
            {
                if(_weapons.Count >= 1)
                    return;
                
                for (var i = 1; i < enlargementAmount; i++)
                {
                    var newNose = GameObject.Instantiate(_nosePrefab, parent).GetComponentInChildren<SWWeapon>();
                    newNose.Initialize(_controllingSwordFish);
                    newNose.transform.position = weaponStartingPosition.position;

                    if (_weapons.Count > 0)
                    {
                        if (isSide)
                        {
                            newNose.transform.localPosition += Vector3.forward * _noseLength * _weapons.Count;
                        }
                        else
                        {
                            var oldPos = newNose.transform.localPosition;
                    
                            if(isFront)
                                oldPos.z += _noseLength * _weapons.Count;
                            else
                                oldPos.z -= _noseLength * _weapons.Count;
                    
                            newNose.transform.localPosition = oldPos;
                        }
                    }
                    else
                    {
                        _noseLength = 0.5f;
                    }
                    
                    _weapons.Add(newNose);
                }
                
                RefreshWeapons();
            }
            else if(enlargementAmount < 0)
            {
                var amount = _weapons.Count - 1 + enlargementAmount;
                
                for (var i = _weapons.Count - 1; i > amount; i--)
                {
                    if(i < 0)
                        continue;
                    
                    GameObject.Destroy(_weapons[i].gameObject);
                    _weapons.RemoveAt(i);
                }
                
                RefreshWeapons();
            }
        }

        private void RefreshWeapons()
        {
            if(_weapons.Count == 0)
                return;
            
            for (var i = 0; i < _weapons.Count; i++) _weapons[i].canKill = false;
     
            _weapons[_weapons.Count - 1].canKill = true;
            lastWeapon = _weapons[_weapons.Count - 1].transform;
        }
    }
}
