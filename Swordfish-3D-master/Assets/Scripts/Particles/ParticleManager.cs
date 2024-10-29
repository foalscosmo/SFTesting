using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Particles
{
	public enum ParticleType
	{
		Damage,
		Explosion,
		ZombieDeath,
		HealthPickUp,
		Destruction,
		DestructionTree,
		DestructionBrown,
	}
	
	[Serializable]
	public class Particle
	{
		public ParticleSystem ParticleEffects;
		public ParticleType   Type;
		public int 			  PoolLength;
	}
	
	public class ParticleManager : MonoBehaviour
	{
		public static ParticleManager Instance;

		public Particle[] Particles;

		private Dictionary<ParticleType, ParticlePool> _particlePools = new Dictionary<ParticleType, ParticlePool>();

		private void Awake()
		{
			Instance = this;
		}

		private void Start()
		{
			for (var i = 0; i < Particles.Length; i++)
			{
				_particlePools.Add(Particles[i].Type,new ParticlePool(Particles[i].ParticleEffects,transform,Particles[i].PoolLength));
			}
		}

		public void PlayParticle(Vector3 particlePos, ParticleType type)
		{
			if (type == ParticleType.DestructionBrown)
				type = ParticleType.Destruction;
			
			var particleToPlay = _particlePools[type].GetAvailableParticle();

			if (particleToPlay != null)
			{
				if (particleToPlay.isPlaying)
					particleToPlay.Stop();

				particleToPlay.transform.position = particlePos;
				particleToPlay.Play();
			}
		}

		public void PlayFollowingParticle(Transform trans, ParticleType type)
		{
			var particleToPlay = _particlePools[type].GetAvailableParticle();

			if (particleToPlay != null)
			{
				if (particleToPlay.isPlaying)
					particleToPlay.Stop();

				particleToPlay.transform.position = trans.position;
				particleToPlay.Play();
			}

			StartCoroutine(Follower(trans, particleToPlay));
		}

		private IEnumerator Follower(Transform trans, ParticleSystem particle)
		{
			var index = 0;
			var duration = particle.main.duration * 100;
			while (index < duration)
			{
				index++;
				yield return new WaitForSeconds(0.01f);
				particle.transform.position = trans.transform.position;
			}
		}
		
	}
}