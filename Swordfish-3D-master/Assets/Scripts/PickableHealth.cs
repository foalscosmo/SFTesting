using System.Collections;
using System.Collections.Generic;
using Car;
using UnityEngine;

public class PickableHealth : MonoBehaviour
{
	public int Health = 100;

	private void OnTriggerEnter(Collider other)
	{
		if (other.transform.CompareTag("Player"))
		{
			var carController = other.GetComponentInParent<SWController>();
			if (carController)
			{
				carController.HealthRegen(Health);
			}

			StartCoroutine(Respawn());
		}
	}

	private IEnumerator Respawn()
	{
		gameObject.SetActive(false);
		yield return new WaitForSeconds(10f);
		
		gameObject.SetActive(true);
	}
}
