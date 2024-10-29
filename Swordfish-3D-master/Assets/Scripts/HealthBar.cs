using System.Collections.Generic;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
	[HideInInspector]
	public int MaxHealth;

	public Image Bar;

	public GameObject crown;

	public TextMeshProUGUI nameText;
	public TextMeshProUGUI zombieCount;
	public TextMeshProUGUI zombieDamage;
	public Animator animator;
	public Image flag;
	
	public TextMeshProUGUI[] scoreTexts;
	public Animator[] scoreAnimations;
	
	[HideInInspector]
	public GameObject ObjToFollow;
	
	public Transform powerUpsHolder;
	
	public Dictionary<PowerUpTypes, PowerUp> powerUps = new Dictionary<PowerUpTypes, PowerUp>();

	private void Awake()
	{
		var powerUpsArray = powerUpsHolder.GetComponentsInChildren<PowerUp>(true);
		
		for (var i = 0; i < powerUpsArray.Length; i++)
		{
			powerUps.Add(powerUpsArray[i].powerUpType,powerUpsArray[i]);
		}
	}

	public void ShowScore(int score)
	{
		for (var i = 0; i < scoreTexts.Length; i++)
		{
			if (scoreTexts[i].gameObject.activeSelf == false)
			{
				scoreTexts[i].text = "+"+score.ToString();
				scoreTexts[i].gameObject.SetActive(true);
				
				scoreAnimations[i].Play("GainScore");
				break;
			}
		}
	}
}
