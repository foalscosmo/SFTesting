using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
	
	public class ParticleMine : MonoBehaviour
	{
		public static ParticleMine Instance;


		//public Particle[] ParticlesForPool;
		
		public GameObject DamageParticle, DamageCountParticle, ExplosionParticle;
		public int DamageParticleCount, DamageCountParticleCount, ExplosionParticleCount;

		private GameObject _particleHolder;

		private List<GameObject> available;
		private List<GameObject> busy;

		private void Awake()
		{
			Instance = this;
		}

		private void Start()
		{
			Initialize();
		}

		private void Initialize()
		{
			_particleHolder = new GameObject("Particle Pool");
			_particleHolder.transform.position = Vector3.zero;
		}

		private void CreateParticlePool(GameObject particle, int particleCount, List<GameObject> particleList)
		{
			for (var i = 0; i < particleCount; i++)
			{
				particleList.Add(GameObject.Instantiate(particle,_particleHolder.transform));
			}
		}
	}
}
