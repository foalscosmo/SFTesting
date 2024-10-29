using System;
using System.ComponentModel;
using Car;
using ScriptableObjects;
using UnityEngine;

[Serializable]
public class CarBuilder : MonoBehaviour
{
	[Header("Car Specs")] 
	
	[ShowOnly]public string 	CarName 			= "New Car";
	[ShowOnly]public int 		CarPrice 			= 0;
	[ShowOnly]public int 		Health 				= 0;
	[ShowOnly]public int 		Armor 				= 0;
	[ShowOnly]public int 		Damage 				= 0;
	[ShowOnly]public float 		MaxSpeed 			= 0;
	[ShowOnly]public int 		MaxReverseSpeed 	= 0;
	[ShowOnly]public int 		MotorForce 			= 0;
	[ShowOnly]public float 		Mass 				= 0;
	[ShowOnly]public float 		BoostMultiplier 	= 0;
	[ShowOnly]public float 		BoostTime 			= 0;
	[ShowOnly]public float 		BoostCoolDownTime 	= 0;
	[ShowOnly]public float 		MaxSteeringAngle 	= 0;

	[HideInInspector] 
	public SWController SwController;
	[HideInInspector] 
	public GameObject Wheels;
	[HideInInspector] 
	public GameObject Engine;
	[HideInInspector] 
	public GameObject ArmorFrame;
	[HideInInspector] 
	public GameObject Bumper;
	[HideInInspector] 
	public GameObject Frame;
	[HideInInspector] 
	public GameObject BackBumper;
	
	public CarModel Car;

	private BoxCollider _collider;

	public void BuildCar()
	{
		if (Car.Wheels) 		Wheels 			= GameObject.Instantiate(Car.Wheels,transform);
		if (Car.Engine) 		Engine 			= GameObject.Instantiate(Car.Engine,transform);
		if (Car.ArmorFrame) 	ArmorFrame 		= GameObject.Instantiate(Car.ArmorFrame,transform);
		if (Car.Bumper) 		Bumper 			= GameObject.Instantiate(Car.Bumper,transform);
		if (Car.Frame) 			Frame 			= GameObject.Instantiate(Car.Frame,transform);
		if (Car.BackBumper) 	BackBumper 		= GameObject.Instantiate(Car.BackBumper,transform);

		SwController = gameObject.AddComponent<SWController>();
		var rigidBody = SwController.GetComponent<Rigidbody>();
		if (!rigidBody)
			rigidBody = SwController.gameObject.AddComponent<Rigidbody>();

		
		CarName 			= Car.CarName;
		CarPrice 			= Car.CarPrice;
		Health 				= Car.Health;
		Armor 				= Car.Armor;
		Damage 				= Car.Damage;
		MaxSpeed 			= SwController.MaxSpeed = Car.MaxSpeed;
		Mass 				= rigidBody.mass = Car.Mass;
		BoostMultiplier 	= SwController.SpeedMultiplierOnBoost = Car.BoostMultiplier;
		BoostTime 			= SwController.BoostTime = Car.BoostTime;
		BoostCoolDownTime 	= Car.BoostCoolDownTime;

		name = CarName;

		tag = "Player";

		CreateBoxCollider();
	}
	
	public void DestroyCar()
	{
		if (Wheels) 		GameObject.DestroyImmediate(Wheels);
		if (Engine) 		GameObject.DestroyImmediate(Engine);
		if (ArmorFrame) 	GameObject.DestroyImmediate(ArmorFrame);
		if (Bumper) 		GameObject.DestroyImmediate(Bumper);
		if (Frame) 			GameObject.DestroyImmediate(Frame);
		if (BackBumper) 	GameObject.DestroyImmediate(BackBumper);
		
		CarName 			= "New Car";
		CarPrice 			= 0;
		Health 				= 0;
		Armor 				= 0;
		Damage 				= 0;
		MaxSpeed 			= 0;
		MaxReverseSpeed 	= 0;
		MotorForce 			= 0;
		Mass 				= 0;
		BoostMultiplier 	= 0;
		BoostTime 			= 0;
		BoostCoolDownTime 	= 0;
		MaxSteeringAngle 	= 0;
		
		name = CarName;
		tag = "Untagged";

		if (_collider) GameObject.DestroyImmediate(_collider);
		if (!SwController) return;
		GameObject.DestroyImmediate(SwController,true);
	}

	public void CreateBoxCollider()
	{
		_collider = gameObject.GetComponent<BoxCollider>();
		if (!_collider) _collider = gameObject.AddComponent<BoxCollider>();
		if (Frame!= null)
		{
			var rend = Frame.GetComponentInChildren<MeshRenderer>();
			_collider.center = rend.bounds.center;
			_collider.size = rend.bounds.size;
		}
	}
}
