using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableObject : MonoBehaviour
{
	private Collider _mainCollider;
	private Rigidbody[] _partRigidBodies;
	private Collider[] _partColliders;
	private bool _destroyed = false;

	public float GravityMultiplier = 5f;

	private void Awake()
	{
		_mainCollider = GetComponent<Collider>();
		_partRigidBodies = GetComponentsInChildren<Rigidbody>();
		_partColliders = GetComponentsInChildren<Collider>();
		SetPartsKinematic(true);
		
		//IgnoreCollision(true);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Bumper"))
		{
			SetPartsKinematic(false);
			StartCoroutine(DestroyTimer());
			StartCoroutine(Test());
		}
	}

	private void SetPartsKinematic(bool value)
	{
		for (var i = 0; i < _partRigidBodies.Length; i++)
		{
			_partRigidBodies[i].isKinematic = value;
			_destroyed = !value;
		}
		for (var i = 0; i < _partColliders.Length; i++)
		{
			if (_partColliders[i] != _mainCollider)
				_partColliders[i].isTrigger = value;
		}
	}
	
	private void IgnoreCollision(bool value)
	{
		for (var i = 0; i < _partColliders.Length; i++)
		{
			for (var j = 0; j < _partColliders.Length; j++)
			{
				
				Physics.IgnoreCollision(_partColliders[i], _partColliders[j],value);
			}
		}
	}

	private void FixedUpdate()
	{
		/*if (_destroyed)
		{
			for (var i = 0; i < _partRigidBodies.Length; i++)
			{
				_partRigidBodies[i].AddForce(Vector3.down * GravityMultiplier);
				Debug.DrawRay(_partRigidBodies[i].transform.position,_partRigidBodies[i].velocity,Color.blue,0.1f);
			}
		}*/

		if (!_enablee && _destroyed)
		{
			for (var i = 0; i < _partRigidBodies.Length; i++)
			{
				_partRigidBodies[i].velocity = Vector3.ClampMagnitude(_partRigidBodies[i].velocity, 2);
			}
		}

		if (_goDown)
		{
			for (var i = 0; i < _partColliders.Length; i++)
			{
				_partColliders[i].isTrigger = true;
			}
			for (var i = 0; i < _partRigidBodies.Length; i++)
			{
				_partRigidBodies[i].velocity = Vector3.down;
			}
		}
	}

	private bool _goDown = false;
	public IEnumerator DestroyTimer()
	{
		yield return new WaitForSeconds(4f);
		for (var i = 0; i < _partRigidBodies.Length; i++)
		{
			_partRigidBodies[i].freezeRotation = true;
		}
		_goDown = true;
		yield return new WaitForSeconds(5f);
		Destroy(this.gameObject);
	}

	private bool _enablee = false;
	public IEnumerator Test()
	{
		yield return new WaitForSeconds(0.3f);
		_enablee = true;
		//IgnoreCollision(false);
	}
}
