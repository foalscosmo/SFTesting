using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyTimer : MonoBehaviour
{
	public float DestroyTime;

	private void Start ()
	{
		StartCoroutine(DestroyObject());
	}

	private IEnumerator DestroyObject()
	{
		yield return new WaitForSeconds(DestroyTime);
		Destroy(this.gameObject);
	}
}
