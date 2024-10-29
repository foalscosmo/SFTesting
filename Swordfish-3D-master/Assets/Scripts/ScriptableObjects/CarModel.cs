using Car;
using UnityEngine;

namespace ScriptableObjects
{
	[CreateAssetMenu(fileName = "CarModel", menuName = "Cars/CarModel", order = 1)]
	public class CarModel : ScriptableObject
	{
		public string 	CarName 		= "New Car";
		public int 		CarPrice 		= 1000;
		
		[Header("DefaultValues")] 
		public int Health 				= 100;
		public int Armor 				= 100;
		public int Damage 				= 20;
		[Range(20,150)]
		public float MaxSpeed 			= 50;
		[Range(5,100)]
		public int MaxReverseSpeed 		= 20;
		[Range(50,1000)]
		public int MotorForce 			= 250;
		[Range(250, 1500)]
		public float Mass 				= 500;
		[Range(1.1f, 3f)]
		public float BoostMultiplier 	= 1.3f;
		[Range(1f, 5f)]
		public float BoostTime 			= 1.3f;
		[Range(1f, 20f)]
		public float BoostCoolDownTime 	= 1.3f;
		[Range(20f, 60f)]
		public float MaxSteeringAngle 	= 40f;
		
		
		[Header("Unlockable Parts")]
		public GameObject Frame;
		public GameObject ArmorFrame;
		public GameObject Wheels;
		public GameObject Engine;
		public GameObject Bumper;
		public GameObject BackBumper;
	}
}