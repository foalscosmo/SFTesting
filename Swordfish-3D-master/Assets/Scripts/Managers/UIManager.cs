using System;
using System.Collections;
using System.Collections.Generic;
using Car;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIForCars
{
	public HealthBar healthBar;
	public GameObject Arrow;
	public Image ArrowImage;
	public TextMeshProUGUI zombieCountOnArrow;

	public UIForCars(HealthBar healthBar, GameObject arrow, Image arrowImage, TextMeshProUGUI zombieCount = null)
	{
		this.healthBar = healthBar;
		Arrow = arrow;
		ArrowImage = arrowImage;
		zombieCountOnArrow = zombieCount;

		if (zombieCountOnArrow != null)
			zombieCountOnArrow.enabled = false;
	}
}

public class UIManager : MonoBehaviour
{
	public static UIManager Instance;
	
	private Coroutine _driftCoroutine;

	public GameObject HealthBarPrefab;
	public Vector3    HealthBarYOffset = new Vector3(0,2f,0);

	public GameObject TextAnimPrefab;

	public string[] names;
    
	private Dictionary<SWController, UIForCars> _activeCars = new Dictionary<SWController, UIForCars>();

	[Header("Coins")] 
	public TextMeshProUGUI CoinsText;
	
	[Header("Gems")] 
	public TextMeshProUGUI gemsTextMain;
	

	//public Text timer;
	
	[Header("Radar")] 
	public GameObject Arrow;
	public Color AllyColor;
	public Color EnemyColor;
	public Canvas MainCanvas;
	
	[Header("LeaderBoard")] 
	public Text[] nameTexts;
	public Text[] scoreTexts;
	private List<SWController> players = new List<SWController>();
	
	[Header("GameOver")] 
	public GameObject gameOverpanel;
	public GameObject gameOverPanelTeamMode;
	public Text textEarnedCoinsTeam, textKillCountTeam, textWinLoseTeam, textEarnedGemsTeam, textBonusCoins;
	public Text killCount, bestKillCount, earnedCoins, topPercentage, earnedGems;


	[Header("Teams")] 
	public GameObject teamsCanvas;
	public GameObject[] teamsGameObj;
	public Text[] teamsTickets;
	public Image[] teamsTicketImages;
	
	[Header("Collect double coins")] 
	public GameObject doubleCoinsPanel;
	
	

	public float DistForMaxScale, MinScale, MaxScale;

	public Animator WinLoseRespawnPanelAnimator;

	#region Scores

	public void RegisterToPlayerslist(SWController sw)
	{
		players.Add(sw);
	}

	private void FixedUpdate()
	{
		DetermineKing();
	}

	
	//New Set King Method
	private void DetermineKing()
	{
		SWController largestPlayer = null; 
		float largestScale = 0f; 
		bool isDuplicateScale = false; 

		for (int i = 0; i < players.Count; i++)
		{
			float currentPlayerScale = players[i].transform.localScale.magnitude;

			if (currentPlayerScale > largestScale)
			{
				largestScale = currentPlayerScale;
				largestPlayer = players[i];
				isDuplicateScale = false;
			}
			else if (Mathf.Approximately(currentPlayerScale, largestScale))
			{
				isDuplicateScale = true;
			}
		}

		for (int i = 0; i < players.Count; i++)
		{
			players[i].SetAsKing(false);
		}

		if (largestPlayer != null && !isDuplicateScale)
		{
			largestPlayer.SetAsKing(true);
			//Debug.Log($"{largestPlayer.name} has been set as the king with a scale of {largestScale}");
		}
		else
		{
			//Debug.Log("No king has been assigned due to equal scales.");
		}
	}
	public void UpdateScores()
	{
		var temp = players[0];
		var smallest = 0;
		
		for (var i = 0; i < players.Count - 1; i++) {
			smallest = i;
			for (var j = i + 1; j < players.Count; j++) {
					if (players[j].player.currentKillCount < players[smallest].player.currentKillCount) {
						smallest = j;
					}
			}
			temp = players[smallest];
			players[smallest] = players[i];
			players[i] = temp;
		}

		//SETKING
		 // for (var i = players.Count-1; i > 0; i--)
		 // {
		 // 	players[i].SetAsKing(players.Count - 1 == i);
		 // }

		var index = 0;
		for (var i = players.Count-1; i > 0; i--)
		{
			index++;
			nameTexts[players.Count-1-i].text = players[i].player.playerName;
			scoreTexts[players.Count-1-i].text = players[i].player.currentKillCount.ToString();

			if (GameManager.Instance.currentGameMode == GameModes.Classic)
			{
				var color = players[i].SwType == SWType.Player ? Color.green : Color.white;

				if (index == 1)
				{
					color = Color.yellow;
				}
			
				scoreTexts[players.Count-1-i].color = nameTexts[players.Count-1-i].color = color;
			}
			else
			{
				var color = GameModeManager.instance.GetSWColor(players[i]);

				if (index == 1)
				{
					color = Color.yellow;
				}
			
				scoreTexts[players.Count-1-i].color = nameTexts[players.Count-1-i].color = color;
			}

			if (index == 5)
			{
				break;
			}
		}
	}

	public void GameOverlast(SWController sw, bool won = false)
	{
		if (GameManager.Instance.currentGameMode == GameModes.Classic)
		{
			gameOverpanel.SetActive(true);
		
			killCount.text 		= sw.player.currentKillCount.ToString();
		
			var bestKills = sw.player.currentKillCount;
			if (PlayerPrefs.HasKey("BestKills"))
			{
				var savedKills = PlayerPrefs.GetInt("BestKills");
				if(savedKills < bestKills)
					PlayerPrefs.SetInt("BestKills", bestKills);
			}
			else
			{
				PlayerPrefs.SetInt("BestKills", bestKills);
			}

			bestKillCount.text = PlayerPrefs.GetInt("BestKills").ToString();
			earnedCoins	 .text = ": " + sw.player.collectedCoins;
			earnedGems	 .text = ": " + sw.player.collectedGems;

			topPercentage.text = "BETTER THAN 35% OF PLAYERS";
		}
		else
		{
			gameOverPanelTeamMode.SetActive(true);
			
			textKillCountTeam.text 		= sw.player.currentKillCount.ToString();
			textEarnedCoinsTeam.text = ": " + sw.player.collectedCoins;
			textEarnedGemsTeam.text = ": " + sw.player.collectedGems;
			textWinLoseTeam.text = won ? "YOUR TEAM - WON" : "YOUR TEAM - LOST";
			textWinLoseTeam.color = won ? Color.green : Color.red;

			textBonusCoins.gameObject.SetActive(won);
			if (won)
			{
				textBonusCoins.text = "25 COINS BONUS";
			}
			
			Time.timeScale = 0;

			sw.player.collectedCoins += 25;
		}
		
		foreach (var keyValuePair in _activeCars)
		{
			keyValuePair.Value.Arrow.SetActive(false);
		}

		sw.playerData.coins += sw.player.collectedCoins;
		sw.playerData.gems += sw.player.collectedGems;
		sw.playerData.AddExp(sw.player.currentKillCount);
		XMLManager.instance.SaveGameData();
		
		sw.player.ResetStats();
		sw.UpdateScores(false);
	}
	
	public void GameOverContinue()
	{
		Time.timeScale = 1;
		UpdateCoins(GameManager.Instance.Player,0);
		GameManager.Instance.Restart();
	}

	public void RestartFromTeamMode()
	{
		Time.timeScale = 1;
		MySceneManager.instance.LoadSceneAdditiveAfterUnloading(MyScenes.SW_ArenaScene.ToString(),MyScenes.SW_ArenaScene.ToString());
	}

	#endregion

	public void InitializeTeams()
	{
		teamsCanvas.SetActive(true);
		for (var i = 0; i < teamsGameObj.Length; i++)
		{
			teamsGameObj[i].SetActive(false);
		}

		if (GameModeManager.instance.currentGameMode == GameModes.Classic)
		{
			teamsCanvas.SetActive(false);
		}
		else
		{
			for (var i = 0; i < GameModeManager.instance.teams.Length; i++)
			{
				teamsGameObj[i].SetActive(true);
				teamsTickets[i].color = GameModeManager.instance.GetSWColor(GameModeManager.instance.teams[i]);
				teamsTicketImages[i].color = GameModeManager.instance.GetSWColor(GameModeManager.instance.teams[i]);
			}
		}
	}

	public void UpdateTeamTickets(Teams team, int amount)
	{
		teamsTickets[(int) team].text = amount.ToString();
	}

	private void Awake()
	{
		Instance = this;
	}
	
	public void UpdateCoins(SWController sw, int amount)
	{
		CoinsText.text = sw.player.collectedCoins.ToString();
		if (SceneManager.GetActiveScene().buildIndex != 4) return;
			gemsTextMain.text = sw.player.collectedGems.ToString();
		earnedGems.text = sw.player.collectedCoins.ToString();
		if (amount != 0)
			SpawnScoreText(sw, amount);
	}

	#region HealthBars

	public void SetCrownActive(bool activate, SWController sw)
	{
		if (_activeCars.ContainsKey(sw))
		{
			_activeCars[sw].healthBar.crown.SetActive(activate);
		}
	}
	
	public void InGameSetPowerUpProgress(SWController sw,float progress, PowerUpTypes type)
	{
		if(sw == null)
			return;
		
		if(_activeCars.ContainsKey(sw) && _activeCars[sw].healthBar != null && _activeCars[sw].healthBar.powerUps.ContainsKey(type))
			_activeCars[sw].healthBar.powerUps[type].progressPowerUp.fillAmount = progress;
		
	}

	public void InGameSetPowerUpActive(SWController sw,bool activate, PowerUpTypes type)
	{
		if(_activeCars.ContainsKey(sw))
			_activeCars[sw].healthBar.powerUps[type].gameObject.SetActive(activate);
		
	}
	public void UpdateKillCount(SWController sw, int count)
	{
		if(_activeCars.ContainsKey(sw))
			_activeCars[sw].healthBar.zombieCount.text = count.ToString();
	}
	
	public void RegisterCar(SWController sw)
	{
		if (_activeCars.ContainsKey(sw)) return;
		_activeCars.Add(sw,AssignHealthBar(sw));

		if (sw.SwType == SWType.Player)
		{
			foreach (var keyValuePair in _activeCars)
			{
				keyValuePair.Value.Arrow.SetActive(true);
			}
		}
	}

	public void SpawnScoreText(SWController sw, int Amount)
	{
		if (_activeCars.ContainsKey(sw))
		{
			_activeCars[sw].healthBar.ShowScore(Amount);
		}
	}

	public void UnregisterCar(SWController sw)
	{
		if (!_activeCars.ContainsKey(sw)) return;
		if(_activeCars[sw].healthBar != null)Destroy(_activeCars[sw].healthBar.gameObject);
		if(_activeCars[sw].Arrow != null)Destroy(_activeCars[sw].Arrow.gameObject);
		_activeCars.Remove(sw);
	}

	public void UpdateHealthBar(SWController sw, int amount, bool isDamage = true, bool zombieHit = false)
	{
		if (_activeCars.ContainsKey(sw) && _activeCars[sw].healthBar.Bar)
		{
			if (zombieHit)
			{
				_activeCars[sw].healthBar.zombieDamage.text = "-"+amount.ToString();
				_activeCars[sw].healthBar.animator.Play("GetHitByZombie");
			}
			
			//DAMAGE TEXT
			if (!zombieHit)
			{
				var text = GameObject.Instantiate(TextAnimPrefab, sw.transform.position, Quaternion.identity)
					.GetComponentInChildren<TextMesh>();
				text.color = isDamage ? Color.red : Color.green;
				text.text = amount.ToString();
				text.transform.LookAt(GameObject.FindGameObjectWithTag("MainCamera").transform.position);
				text.transform.eulerAngles = text.transform.eulerAngles + new Vector3(180, 0, 180);
				StartCoroutine(DestroyHealthText(text.gameObject, 3));
			}
			_activeCars[sw].healthBar.Bar.fillAmount = (float)sw.Health / (float)sw.MaxHealth;
		}
	}

	private IEnumerator DestroyHealthText(GameObject objToDestroy, float timer)
	{
		yield return new WaitForSeconds(timer);
		Destroy(objToDestroy);
	}

	public Transform healthBarsHolder;

	private UIForCars AssignHealthBar(SWController sw)
	{
		var hb = GameObject.Instantiate(HealthBarPrefab, sw.transform.position + HealthBarYOffset,Quaternion.identity).GetComponent<HealthBar>();
		hb.transform.position += HealthBarYOffset;
		hb.MaxHealth = sw.Health;
		hb.transform.SetParent(healthBarsHolder, true);
		var arrow = GameObject.Instantiate(Arrow);
		var arrowImage = arrow.GetComponentInChildren<Image>(true);
		TextMeshProUGUI arrowZombieCount = null;
		arrow.transform.SetParent(MainCanvas.transform,false);

		
		arrowZombieCount = arrow.GetComponentInChildren<TextMeshProUGUI>(true);
		hb.nameText.color = arrowImage.color = arrowZombieCount.color = sw.shirtColor = hb.zombieCount.color = GameModeManager.instance.GetSWColor(sw);

		if (GameManager.Instance.currentGameMode != GameModes.Classic)
		{
			hb.flag.gameObject.SetActive(true);
			hb.flag.sprite = GameModeManager.instance.GetSWTeamSprite(sw.Team);
		}
		
		/*if (GameManager.Instance.currentGameMode != GameModes.Classic)
		{
			arrowZombieCount = arrow.GetComponentInChildren<TextMeshProUGUI>();
			hb.nameText.color = arrowImage.color = hb.zombieCount.color = GameModeManager.instance.GetSWColor(sw);
			//arrowImage.color = sw.Team == Teams.Team1 ? AllyColor : EnemyColor;
			//hb.zombieCount.gameObject.SetActive(false);
		}
		else
		{
			arrowZombieCount = arrow.GetComponentInChildren<TextMeshProUGUI>();
			hb.nameText.color = arrowImage.color = hb.zombieCount.color = arrowZombieCount.color = sw.shirtColor;
		}*/
		
		hb.nameText.text = sw.player.playerName;

		arrowZombieCount.enabled = arrowImage.enabled = false;
		
		return new UIForCars(hb,arrow, arrowImage,arrowZombieCount);
	}


	/*private IEnumerator TimerIenumerator()
	{
		while (true)
		{
			yield return new WaitForSeconds(1);
			timeLeft -= 1;
			timer.text = "0" + ((int) (timeLeft / 60)).ToString() + ":" + ((int) timeLeft % 60).ToString();
			if (timeLeft <= 0)
			{
				UIManager.Instance.GameOverlast(GameManager.Instance.Player); 
				Time.timeScale = 0;
			}
		}
	}*/

	private float timeLeft = 120;
	private void Update()
	{
		if(!GameManager.Instance.theGameIsStarted)
			return;
		
		var player = GameManager.Instance.ActiveCam.WorldToScreenPoint(GameManager.Instance.Player.transform.position);

		var cameraForwardOffset = GameManager.Instance.ActiveCamera.forward * 10;
		
		foreach (var kvp in _activeCars)
		{
			/*kvp.Value.healthBar.transform.position = kvp.Key.transform.position + HealthBarYOffset;
			kvp.Value.healthBar.transform.LookAt(GameManager.Instance.ActiveCamera);
			kvp.Value.healthBar.transform.eulerAngles = kvp.Value.healthBar.transform.eulerAngles + new Vector3(180, 0, 180);*/
			
			
			kvp.Value.healthBar.transform.position = kvp.Key.transform.position + HealthBarYOffset;
			kvp.Value.healthBar.transform.LookAt(kvp.Value.healthBar.transform.position + cameraForwardOffset);//(GameManager.Instance.ActiveCamera);
			//kvp.Value.healthBar.transform.eulerAngles = kvp.Value.healthBar.transform.eulerAngles + new Vector3(180, 0, 180);
		}
		
		foreach (var kvp in _activeCars)
		{
			if (kvp.Key.SwType == SWType.Player)
			{
				kvp.Value.ArrowImage.enabled = false;//kvp.Value.zombieCountOnArrow.enabled = false;
			}
			else
			{
				Vector3 scale;
				Vector2 pos = UpdateRadar(kvp.Key.transform.position, GameManager.Instance.Player.transform.position, out scale);
			
				/*if(testDELETEME == kvp.Key && kvp.Key.gameObject.activeSelf)
					Debug.LogWarning(pos);*/
			
				if ((pos.x < 50 || pos.x > Screen.width - 50) || (pos.y < 50 || pos.y > Screen.height - 50))//kvp.Value.ArrowImage.enabled)
				//if(!kvp.Key.isVisible)
				{
					pos = TestMe(kvp.Key.transform.position, GameManager.Instance.Player.transform.position);

					kvp.Value.ArrowImage.enabled = true;//kvp.Value.zombieCountOnArrow.enabled = true;
				
					var angleRad = Mathf.Atan2(pos.y - player.y, pos.x - player.x);
					var angleDeg = (180 / Mathf.PI) * angleRad;
				
					kvp.Value.ArrowImage.transform.rotation = Quaternion.Euler(0, 0, angleDeg - 90);
					kvp.Value.ArrowImage.transform.position = pos;
					
					kvp.Value.Arrow.transform.localScale = scale;
					//kvp.Value.zombieCountOnArrow.text = kvp.Key.player.currentKillCount.ToString();

					if (kvp.Key.crown.gameObject.activeSelf)
					{
						kvp.Value.ArrowImage.color = Color.yellow;
						kvp.Value.Arrow.transform.localScale = scale * 2f;
					}
					else
					{
						kvp.Value.ArrowImage.color = Color.red;
						kvp.Value.Arrow.transform.localScale = scale;
					}
				}
				else
				{
					kvp.Value.ArrowImage.enabled = false;
					//kvp.Value.zombieCountOnArrow.enabled = false;
					return;
				}
			}
		}
	}

	private enum CameraEdges
	{
		BottomLeft,
		TopLeft,
		TopRight,
		BottomRight
	}

	private Vector2 GetEdgePos(CameraEdges edge)
	{
		var pos = Vector2.zero;
		
		switch (edge)
		{
			case CameraEdges.BottomLeft:
				pos = Vector2.zero;
				break;
			case CameraEdges.TopLeft:
				pos.y = Screen.height;
				break;
			case CameraEdges.TopRight:
				pos.x = Screen.width;
				pos.y = Screen.height;
				break;
			case CameraEdges.BottomRight:
				pos.x = Screen.width;
				break;
		}

		return pos;
	}
	
	private Vector3[] _cameraLimits = new Vector3[4];

	public void SetCameraLimits()
	{
		var cam = Camera.main;

		for (var i = 0; i < _cameraLimits.Length; i++)
		{
			Ray ray = cam.ScreenPointToRay(GetEdgePos((CameraEdges)i));
			if (Physics.Raycast(ray, 1000))
			{
				
			}
		}
	}
	
	// Clamp Vectro2 on screen edges
    private Vector2 ClampOnFrame(Vector2 inputVector)
    {
        Vector2 output;
        Vector2 centerPosition = new Vector2(Screen.width, Screen.height) / 2;
        Vector2 vectorFromOrigin = inputVector - centerPosition;

        float targetRatio = Mathf.Abs(vectorFromOrigin.x) / Mathf.Abs(vectorFromOrigin.y);
        float screenRatio = (float)Screen.width / (float)Screen.height;

        if (targetRatio >= screenRatio)
        {
            output = vectorFromOrigin / (Mathf.Abs(vectorFromOrigin.x) / Screen.width * 2) + centerPosition;
        }
        else
        {
            output = vectorFromOrigin / (Mathf.Abs(vectorFromOrigin.y) / Screen.height * 2) + centerPosition;
        }

        return output;
    }

	public Vector2 TestMe(Vector3 pos, Vector3 playerPos)
	{
		var dir = pos - playerPos;
		var mag = dir.magnitude;
		dir = dir / mag;

		pos = playerPos + (dir * 60f);
		
		Vector2 screenPos = Camera.main.WorldToScreenPoint(pos);

		screenPos = ClampOnFrame(screenPos);
		
		return screenPos;
	}

	public Vector3 UpdateRadar(Vector3 pos, Vector3 playerPos, out Vector3 scale)
	{
		var dir = pos - playerPos;
		var mag = dir.magnitude;
		dir = dir / mag;
		
		pos = playerPos + (dir * 60f);
		
		var val = Mathf.Clamp(MaxScale / ((mag - DistForMaxScale) / 20), MinScale, MaxScale);
		scale = DistForMaxScale > mag
			? new Vector3(MaxScale, MaxScale, MaxScale)
			: new Vector3(val,
				val,
				val);
		
		//edited pos with Vector3.zero
		var screenPos = GameManager.Instance.ActiveCam.WorldToScreenPoint(Vector3.zero);
		
		screenPos.x = Mathf.Clamp(screenPos.x, 0, Screen.width);
		screenPos.y = Mathf.Clamp(screenPos.y, 0, Screen.height);
		return screenPos;
	}

	public void GameOver(bool win)
	{
		//WinLoseRespawnPanelAnimator.transform.gameObject.SetActive(true);
		//WinLoseRespawnPanelAnimator.SetBool("Win",win);
	}

	public void BackToMainMenu()
	{
		PowerUpManager.instance.ResetPowerUps(GameManager.Instance.Player.playerData);
		MySceneManager.instance.LoadSceneAdditiveAfterUnloading("SW_Menu", "SW_ArenaScene");
	}

	#endregion

	public void WaitForRespawn()
	{
		//WinLoseRespawnPanelAnimator.Play("Respawn");
	}
}
