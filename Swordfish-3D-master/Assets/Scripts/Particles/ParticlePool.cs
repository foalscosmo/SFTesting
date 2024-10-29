using System;
using UnityEngine;

namespace Particles
{
    public class ParticlePool
    {
        private int _particleAmount;
        private ParticleSystem[] _normalParticle;

        public ParticlePool(ParticleSystem normalPartPrefab, Transform holder, int amount = 10)
        {
            _particleAmount = amount;
            _normalParticle = new ParticleSystem[_particleAmount];

            for (int i = 0; i < _particleAmount; i++)
            {
                //Instantiate 10 NormalParticle
                _normalParticle[i] = GameObject.Instantiate(normalPartPrefab, holder) as ParticleSystem;
            }
        }

        public ParticleSystem GetAvailableParticle( )
        {
            var firstObject = _normalParticle[0];
            ShiftUp();

            return firstObject;
        }

        public int GetAmount()
        {
            return _particleAmount;
        }

        private void ShiftUp()
        {
            ParticleSystem firstObject;

            firstObject = _normalParticle[0];
            Array.Copy(_normalParticle, 1, _normalParticle, 0, _normalParticle.Length - 1);

            _normalParticle[_normalParticle.Length - 1] = firstObject;

        }
    }
}