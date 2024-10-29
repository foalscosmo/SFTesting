using UnityEngine;

namespace Car
{
	public class SecondaryCarController : MonoBehaviour
	{
		private Rigidbody 	_rigidbody;
		private Collider 	_collider;
		private float 		_horizontalInput;
		private Vector3 	_direction;

		public float Speed = 10f;

		private void Awake()
		{
			_rigidbody 	= GetComponent<Rigidbody>();
			_collider 	= GetComponent<Collider>();
			_direction 	= transform.forward;
			AdjustCarCenter();
		}

		private void AdjustCarCenter()
		{
			var center = _rigidbody.centerOfMass;
			center.z += _collider.bounds.extents.z / 2;
			_rigidbody.centerOfMass = center;
		}

		private void FixedUpdate()
		{
			GetInputs();
			SetDirections();
			Steer();
			Accelerate();
			Draw();
		}
		
		private void GetInputs()
		{
			_horizontalInput = Input.GetAxis("Horizontal");
		}
		
		private void SetDirections()
		{
			_direction = transform.forward;
			_direction.x = _horizontalInput;
		}

		private void Accelerate()
		{
			//_rigidbody.velocity = _direction * Speed * Time.deltaTime;
			//_rigidbody.AddForce(_direction * Speed * Time.deltaTime);
		}

		private void Steer()
		{
			transform.RotateAround(transform.position, transform.up,Vector3.SignedAngle(transform.forward,_direction,Vector3.up)*Time.deltaTime);
		}

		private void Draw()
		{
			Debug.DrawRay(transform.position,_direction,Color.blue,0.2f);
		}
	}
}
