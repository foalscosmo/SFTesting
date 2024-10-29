using System;
using System.Collections;
using System.Collections.Generic;
using AI;
using CameraControllers;
using CandyCoded.HapticFeedback;
using Car;
using SnakeScripts;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Managers
{
	[System.Serializable]
	public enum GameModes
	{
		Classic,
		TeamOfTwo,
		TeamOfThree,
		TeamOfFour
	}
	
	public class GameManager : MonoBehaviour
	{
		public GameModes currentGameMode = GameModes.Classic;
		
		public Dictionary<GameObject, SWController> AllActiveSW 	= new();
		
		public Dictionary<Teams, Dictionary<GameObject, SWController>> TeamsWithActiveSwordFishes 	= new Dictionary<Teams, Dictionary<GameObject, SWController>>();
		public Dictionary<Teams, Dictionary<GameObject, SWController>> DeadCars 	= new Dictionary<Teams, Dictionary<GameObject, SWController>>();

		public GameObject swPrefab;
		public GameObject swordFishesParent;
		
		[Header("Colors")]
		public List<Color> freeColorsList;
		public Color playerColor;
		
		public static GameManager Instance;
		
		private SWController _player;
		public SWController Player
		{
			get
			{
				if (!_player) _player = FindObjectOfType<SWController>();
				return _player;
			}
			set { _player = value; }
		}

		[Header("Camera")]
		public bool IsTopDown;
		public GameObject TopViewCamera, BackViewCamera;
		[HideInInspector]
		public Transform ActiveCamera;
		public Camera ActiveCam;

		public string MotorForce, Acceleration, Slide, Steer, MaxSpeed, BackZoom, BackLook, BackSpeed, 
			BackYOffset, FrontZoom, FrontOffset,FrontFlow,FrontIsOrtho,FrontOrthoZoom,BumpForce,BumpTimer,BoostMultiplier,VerticalCameramovement, NewBoostActiveTime, BoostCooldown, HandlingSpeed;

		public GameObject Coin;

		[Header("Temporary AI")] 
		public float DistanceForBiggerTurn;
		public float AngleForBiggerTurn;
		public float MaxAngleForBiggerTurn;

		[Header("GamePlay")] 
		public float RespawnTime = 3f;
		public Transform[] Team1Positions, Team2Positions;
		public bool theGameIsStarted = false;
		
		public int[] teamTickets;

		[SerializeField] private bool hasStartGame;
		public float startTimer;
		public float gameDuration = 60f;

		private void Awake()
		{
			Instance = this;
			Application.targetFrameRate = 60;
			ActiveCamera = GameObject.FindGameObjectWithTag("MainCamera").transform;
			ActiveCam = ActiveCamera.GetComponent<Camera>();
		}

		private void Start()
		{
			startTimer = 0f;
			hasStartGame = true;
			StartCoroutine(rame());
			InvokeRepeating("SaveTwo", 10f, 10f);
		}

		private void Update()
		{
			// if (hasStartGame)
			// {
			// 	startTimer += Time.deltaTime;
			//
			// 	if (startTimer >= gameDuration)
			// 	{
			// 		HapticFeedback.HeavyFeedback();
			// 		CameraTopDown.Instance.ChangeCameraZoom(0);
			// 		_player.bodyParts[0].transform.localScale = new Vector3(1.25f, 1.25f, 1.25f);
			// 		_player.allBoostobjects.transform.localScale = new Vector3(1f,1f,1f);
			// 		SW_AIStrengthManager.instance.PlayerGotKilled(_player.player.currentKillCount);
			// 		_player.joystickArrowRotator.SetActive(false);
			// 		CameraTopDown.Instance.ZoomSlider.value = 0f;
			// 		SoundManager.instance.PlayVoiceLine(SoundTypesEnum.LoseVoiceLine,false);
			// 	
			// 		if (Instance.currentGameMode == GameModes.Classic)
			// 		{
			// 			UIManager.Instance.GameOverlast(_player);
			// 		}
			// 		Instance.SWDeath(_player);
			// 		hasStartGame = false; 
			//
			// 	}
			// }
		}

		public void StartGame(GameModes gameMode)
		{
			currentGameMode = gameMode;

			_teamsWithTickets = (int) gameMode + 1;

			var allSWs = new List<SWController>();
			
			SW_AIStrengthManager.instance.Initialize(gameMode, GameModeManager.instance.playerCountInGame );

			if (currentGameMode == GameModes.Classic)
			{
				for (var i = 0; i < GameModeManager.instance.playerCountInGame; i++)
				{
					allSWs.Add(PrepareSW());
					
					if (i == 0)
						allSWs[0].SwType = SWType.Player;
				}
			}
			else
			{
				teamTickets = new int[GameModeManager.instance.teams.Length];
				for (var i = 0; i < teamTickets.Length; i++)
				{
					teamTickets[i] = 50;
				}
				
				
				for (var i = 0; i < GameModeManager.instance.teams.Length; i++)
				{
					TeamsWithActiveSwordFishes.Add((Teams)i,new Dictionary<GameObject, SWController>());
					DeadCars.Add((Teams)i,new Dictionary<GameObject, SWController>());
					
					for (var j = 0; j < GameModeManager.instance.playerCountInGame / GameModeManager.instance.teams.Length; j++)
					{
						allSWs.Add(PrepareSW((Teams) i, true));
						
						if (i == 0 && j == 0)
							allSWs[0].SwType = SWType.Player;
					}

				}
			}
			
			for (var i = 0; i < allSWs.Count; i++)
			{
				allSWs[i].MainInit();
			}

			theGameIsStarted = true;
		}
		
		/// <summary>
		/// added to disable body parts of snakes 
		/// </summary>
		private SWController PrepareSW(Teams team = Teams.Team1, bool flicker = false)
		{
			var swController = GameObject.Instantiate(swPrefab,swordFishesParent.transform).GetComponentInChildren<SWController>();
			swController.Team = team;
			swController.transform.position = Team1Positions[Random.Range(0, Team1Positions.Length)].position;
			swController.isFreshlyRespawned = flicker;

			SW_AIStrengthManager.instance.InitializeSW(swController);
			
			/*var maxTime = Random.Range(minBoostTime, maxBoostTime);
			var maxAngle = Random.Range(minBoostAngle, maxBoostAngle);
			swController.maxBoostAngle = maxAngle;
			swController.maxBoostTime = maxTime;*/
			
			return swController;
		}

		public Color GetFreeColor()
		{
			var random = 0;//Random.Range(0, freeColorsList.Count);
			var color = freeColorsList[random];
			//freeColorsList.RemoveAt(random);
			return color;
		}

		IEnumerator rame()
		{
			yield return new WaitForSeconds(0.2f);
			
			RestoreValues();
			ChangeCamera();
			ChangeCamera();
		}

		public void SetPlayer(SWController player)
		{
			_player = player;
		}

		public void ChangeCamera()
		{
			IsTopDown = !IsTopDown;
			BackViewCamera.SetActive(!IsTopDown);
			TopViewCamera.SetActive(IsTopDown);
			ActiveCamera = GameObject.FindGameObjectWithTag("MainCamera").transform;
			ActiveCam = ActiveCamera.GetComponent<Camera>();
		}

		public void Restart()
		{
			PowerUpManager.instance.ResetPowerUps(Player.playerData);
			
			SaveValues();
			var randomNumber = Random.Range(0, Team1Positions.Length);
			
			_player.player.ResetStats();
			_player.UpdateScores(false);
			
			_player.transform.position =
				_player.Team == Teams.Team1 ? Team1Positions[randomNumber].position : Team2Positions[randomNumber].position;
			SWReSpawn(_player);
			UIManager.Instance.gameOverpanel.SetActive(false);
			UIManager.Instance.gameOverPanelTeamMode.SetActive(false);
			//SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}

		public void Pause(float time)
		{
			Time.timeScale = time;
		}

		public void RestoreValues()
		{
			var ac2 = BackViewCamera.activeSelf;
			BackViewCamera.SetActive(true);
			var ac3 = TopViewCamera.activeSelf;
			TopViewCamera.SetActive(true);
			
			if (!PlayerPrefs.HasKey("Saved"))
			{
				SaveValues();
			}
			//Car settings.
			var bumpTimer = PlayerPrefs.GetFloat(BumpTimer);
			
			DebugManager.instance.InitDebug(PlayerPrefs.GetFloat(MaxSpeed),PlayerPrefs.GetFloat(BumpForce ),bumpTimer,PlayerPrefs.GetFloat(BoostMultiplier ),
				PlayerPrefs.GetFloat(HandlingSpeed ), PlayerPrefs.GetFloat(BoostCooldown ), 
				PlayerPrefs.GetFloat(NewBoostActiveTime ));
				
			//BackCamera settings.
			var backCam = FindObjectOfType<BackCamera>();
			backCam.InitDebug(PlayerPrefs.GetFloat(BackLook ), PlayerPrefs.GetFloat(BackSpeed ),
				PlayerPrefs.GetFloat(BackYOffset ), PlayerPrefs.GetFloat(BackZoom ));
			
			BackViewCamera.SetActive(ac2);

			//FrontCamera settings.
			var frontCam = FindObjectOfType<CameraTopDown>();
			frontCam.InitDebug(PlayerPrefs.GetFloat(FrontFlow ), PlayerPrefs.GetFloat(FrontOffset ),
				PlayerPrefs.GetFloat(FrontZoom ), PlayerPrefs.GetInt(FrontIsOrtho ) != 0, PlayerPrefs.GetFloat(FrontOrthoZoom ),
				PlayerPrefs.GetFloat(VerticalCameramovement ));
			
			TopViewCamera.SetActive(ac3);
		}

		public void SaveTwo()
		{
			SaveValues();
		}
		
		public void SaveValues()
		{
			PlayerPrefs.SetFloat("Saved",1);
			//Car settings.
			var wheel = _player.GetComponentInChildren<WheelCollider>();
			
			
			if (wheel != null)
			{
				PlayerPrefs.SetFloat(Acceleration,wheel.forwardFriction.stiffness);
				PlayerPrefs.SetFloat(Slide		,wheel.sidewaysFriction.stiffness);
			}
			PlayerPrefs.SetFloat(MaxSpeed			,_player.MaxSpeed);
			PlayerPrefs.SetFloat(BumpForce			,_player.BumpForce);
			PlayerPrefs.SetFloat(BumpTimer			,_player.BumpStunTimer);
			PlayerPrefs.SetFloat(BoostMultiplier			,_player.maxBoostvalSetter);
			PlayerPrefs.SetFloat(NewBoostActiveTime ,_player._newBoostTimeOn);
			PlayerPrefs.SetFloat(HandlingSpeed			,_player.newHandlingSpeed);
			PlayerPrefs.SetFloat(BoostCooldown			,_player.boostTimer);
				
			//BackCamera settings.
			var backCam = BackViewCamera.GetComponent<BackCamera>();
			PlayerPrefs.SetFloat(BackLook		,backCam.BackViewCameraLookSpeed);
			PlayerPrefs.SetFloat(BackSpeed		,backCam.BackViewCameraFollowSpeed);
			PlayerPrefs.SetFloat(BackYOffset	,backCam.BackViewCameraLookYOffset);
			PlayerPrefs.SetFloat(BackZoom		,backCam.GetComponentInChildren<Camera>().fieldOfView);

			//FrontCamera settings.
			var frontCam = TopViewCamera.GetComponent<CameraTopDown>();
			var cam = frontCam.GetComponentInChildren<Camera>();
			PlayerPrefs.SetFloat(FrontFlow		,frontCam.CameraFlow);
			PlayerPrefs.SetFloat(FrontOffset	,frontCam.SpeedOffset);
			PlayerPrefs.SetFloat(FrontZoom		,cam.fieldOfView - frontCam._defaultCameraFieldOfView);
			PlayerPrefs.SetInt	(FrontIsOrtho	,cam.orthographic ? 1 : 0);
			//var zoom  == "Def" ? 5 : cam.orthographicSize;
			PlayerPrefs.SetFloat(FrontOrthoZoom	,5);
			PlayerPrefs.SetFloat(VerticalCameramovement, frontCam.transform.eulerAngles.x);
		}
		
		public void RegisterCar(SWController sw)
		{
			if (currentGameMode == GameModes.Classic)
			{
				if (!AllActiveSW.ContainsKey(sw.gameObject))
				{
					AllActiveSW.Add(sw.gameObject,sw);
				}
			}
			else
			{
				if (!TeamsWithActiveSwordFishes[sw.Team].ContainsKey(sw.gameObject))
				{
					TeamsWithActiveSwordFishes[sw.Team].Add(sw.gameObject, sw);
				}
				if (DeadCars[sw.Team].ContainsKey(sw.gameObject))
				{
					DeadCars[sw.Team].Remove(sw.gameObject);
				}
			}
		}
		
		public void UnregisterCar(SWController sw)
		{
			if (currentGameMode == GameModes.Classic)
			{
				if (AllActiveSW.ContainsKey(sw.gameObject))
				{
					AllActiveSW.Remove(sw.gameObject);
				}
			}
			else
			{
				if (TeamsWithActiveSwordFishes[sw.Team].ContainsKey(sw.gameObject))
				{
					TeamsWithActiveSwordFishes[sw.Team].Remove(sw.gameObject);
				}
				if (!DeadCars[sw.Team].ContainsKey(sw.gameObject))
				{
					DeadCars[sw.Team].Add(sw.gameObject, sw);
				}
			}
		}

		public void ThrowCoins(SWController snake)
		{
			var coinsCount = 2 + snake.player.currentKillCount / 2;
			
			if (snake.hasCrown)
				coinsCount += 5;
			
			CoinsPoolController.instance.InstantiateCoins(snake.transform.position,coinsCount);
		}
		
		public void ThrowCoins(Vector3 pos)
		{
			var coinsCount = Random.Range(0,2);
			
			CoinsPoolController.instance.InstantiateCoins(pos,coinsCount);
		}

		public SWController GetNearestCarTarget(SWController sw)
		{
			Dictionary<GameObject, SWController> dic = new Dictionary<GameObject, SWController>();
			if (currentGameMode != GameModes.Classic)
			{
				for (var i = 0; i < GameModeManager.instance.teams.Length; i++)
				{
					if (sw.Team != (Teams) i)
					{
						foreach (var kvp in TeamsWithActiveSwordFishes[(Teams)i])
						{
							dic.Add(kvp.Key,kvp.Value);
						}
					}
				}
			}
			else
			{
				dic = AllActiveSW;
			}
			
			var pos = sw.transform.position;
			var nearest = 100000f;
			
			var carToFollow = sw;
			foreach (var kvp in dic)
			{
				var dist = Vector3.Distance(kvp.Key.transform.position, pos);
				if (kvp.Value != sw && dist < nearest)
				{
					nearest = dist;
					carToFollow = kvp.Value;
				}
			}

			return carToFollow;
		}

		public GameObject CheckIfCoinIsInRange(Vector3 pos)
		{
			if (CoinsPoolController.instance.activeCoins.Count <= 0)
				return null;
			
			foreach (var coin in CoinsPoolController.instance.activeCoins)
			{
				if (Vector3.Distance(pos, coin.position) < 8f)
					return coin.gameObject;
			}

			return null;
		}

		
		public float GetInputForDirection(Vector3 from, Vector3 to, Vector3 forward)
		{
			var ang = Vector3.SignedAngle(from - to, forward, Vector3.up);
			ang = ang < 0 ? -(180 + ang) : 180 - ang;
			var dist = Vector3.Distance(from, to);
			//Debug.Log("ang-" + ang +"-dist-" +  dist);
			if (Mathf.Abs(ang) > AngleForBiggerTurn && dist < DistanceForBiggerTurn && Mathf.Abs(ang) < MaxAngleForBiggerTurn)
			{
				return 0;
			}
			var angle = ang/180;
			return angle;
		}
		
		//NEW AI
		public float GetInputForDirectionHorizontal(Vector3 from, Vector3 to, Vector3 forward)
		{
			var ang = Vector3.SignedAngle(from - to, forward, Vector3.up);
			ang = ang < 0 ? -(180 + ang) : 180 - ang;
			var dist = Vector3.Distance(from, to);
			//Debug.Log("ang-" + ang +"-dist-" +  dist);
			if (Mathf.Abs(ang) > AngleForBiggerTurn && dist < DistanceForBiggerTurn && Mathf.Abs(ang) < MaxAngleForBiggerTurn)
			{
				return 0;
			}
			var angle = ang/180;
			return angle;
		}
		
		public float GetInputForDirectionVertical(Vector3 from, Vector3 to, Vector3 forward)
		{
			var ang = Vector3.SignedAngle(from - to, forward, Vector3.up);
			ang = ang < 0 ? -(180 + ang) : 180 - ang;
			var dist = Vector3.Distance(from, to);
			//Debug.Log("ang-" + ang +"-dist-" +  dist);
			if (Mathf.Abs(ang) > AngleForBiggerTurn && dist < DistanceForBiggerTurn && Mathf.Abs(ang) < MaxAngleForBiggerTurn)
			{
				return 0;
			}
			var angle = ang/180;
			return angle;
		}

		#region DEBUG_CONTROLLER

		public void UpdateMaxSpeed(float speed)
		{
			if (currentGameMode == GameModes.Classic)
			{
				if (AllActiveSW.Count <= 0) return;
				
				foreach (var swController in AllActiveSW)
				{
					swController.Value.MaxSpeed = speed;
				}
			}
			else
			{
				if (TeamsWithActiveSwordFishes.Count <= 0) return;
				foreach (var team in TeamsWithActiveSwordFishes)
				{
					if (team.Value.Count <= 0) return;
					
					foreach (var swController in team.Value)
					{
						swController.Value.MaxSpeed = speed;
					}
				}
			}
		}
		
		public void UpdateHandlingSpeed(float speed)
		{
			if (currentGameMode == GameModes.Classic)
			{
				if (AllActiveSW.Count <= 0) return;
				
				foreach (var swController in AllActiveSW)
				{
					swController.Value.newHandlingSpeed = speed;
				}
			}
			else
			{
				if (TeamsWithActiveSwordFishes.Count <= 0) return;
				foreach (var team in TeamsWithActiveSwordFishes)
				{
					if (team.Value.Count <= 0) return;
					
					foreach (var swController in team.Value)
					{
						swController.Value.newHandlingSpeed = speed;
					}
				}
			}
		}
		
		public void UpdateBoostCoolDown(float speed)
		{
			if (currentGameMode == GameModes.Classic)
			{
				if (AllActiveSW.Count <= 0) return;
				
				foreach (var swController in AllActiveSW)
				{
					swController.Value.boostTimer = speed;
				}
			}
			else
			{
				if (TeamsWithActiveSwordFishes.Count <= 0) return;
				foreach (var team in TeamsWithActiveSwordFishes)
				{
					if (team.Value.Count <= 0) return;
					
					foreach (var swController in team.Value)
					{
						swController.Value.boostTimer = speed;
					}
				}
			}
		}
		
		public void UpdateBoostValue(float val)
		{
			if (currentGameMode == GameModes.Classic)
			{
				if (AllActiveSW.Count <= 0) return;
				
				foreach (var swController in AllActiveSW)
				{
					swController.Value.maxBoostvalSetter = val;
				}
			}
			else
			{
				if (TeamsWithActiveSwordFishes.Count <= 0) return;
				foreach (var team in TeamsWithActiveSwordFishes)
				{
					if (team.Value.Count <= 0) return;
					
					foreach (var swController in team.Value)
					{
						swController.Value.maxBoostvalSetter = val;
					}
				}
			}
		}
		
		public void UpdateBoostActiveTime(float val)
		{
			if (currentGameMode == GameModes.Classic)
			{
				if (AllActiveSW.Count <= 0) return;
				
				foreach (var swController in AllActiveSW)
				{
					swController.Value._newBoostTimeOn = val;
				}
			}
			else
			{
				if (TeamsWithActiveSwordFishes.Count <= 0) return;
				foreach (var team in TeamsWithActiveSwordFishes)
				{
					if (team.Value.Count <= 0) return;
					
					foreach (var swController in team.Value)
					{
						swController.Value._newBoostTimeOn = val;
					}
				}
			}
		}

		#endregion


		private Teams GetEnemy(Teams team)
		{
			return team == Teams.Team1 ? Teams.Team2 : Teams.Team1;
		}

		public bool IsEnemy(SWController sw1, SWController sw2)
		{
			if (currentGameMode == GameModes.Classic)
				return true;
			
			return sw1.Team != sw2.Team;
		}

		#region Respawn
		
		public void SWDeath(SWController sw)
		{
			//blabla
			sw.gameObject.SetActive(false);
			
			if (_player != sw || currentGameMode != GameModes.Classic)
			{
				StartCoroutine(WaitForRespawn(sw));
			}
		}

		private int _teamsWithTickets = 4;

		private void SWReSpawn(SWController sw)
		{
			/*if(car.CarType != CarType.Player && currentGameMode == GameModes.ZombieCollector)
				return;*/

			if (currentGameMode != GameModes.Classic)
			{
				if (teamTickets[(int) sw.Team] > 0)
				{
					teamTickets[(int) sw.Team]--;
					UIManager.Instance.UpdateTeamTickets(sw.Team,teamTickets[(int) sw.Team]);

					if (teamTickets[(int) sw.Team] == 0)
					{
						_teamsWithTickets--;

						if (sw == _player || sw.Team == _player.Team)
						{
							UIManager.Instance.GameOverlast(sw);
						}
						else if (_teamsWithTickets == 1)
						{
							UIManager.Instance.GameOverlast(sw,true);
							//TODO
							//GAME OVER
							//WON TEAM ROMELIC DARCHA
						}
						return;
					}
				}
			}
			
			sw.gameObject.SetActive(true);
			sw.ResetStats();
			RegisterCar(sw);
			UIManager.Instance.RegisterCar(sw);
			UIManager.Instance.UpdateKillCount(sw,sw.player.currentKillCount);
			

			sw.isFreshlyRespawned = true;
		}
		
		private IEnumerator WaitForRespawn(SWController sw)
		{
			if (sw.SwType == SWType.Player)
			{
				UIManager.Instance.WaitForRespawn();
			}
			yield return new WaitForSeconds(RespawnTime - 0.1f);
			
			var randomNumber = Random.Range(0, Team1Positions.Length);
			/*sw.transform.position =
				sw.Team == Teams.Team1 ? Team1Positions[randomNumber].position : Team2Positions[randomNumber].position;*/

			sw.transform.position = Team1Positions[randomNumber].position;
			
			yield return new WaitForSeconds(0.1f);
			SWReSpawn(sw);
		}

		#endregion
	}
}
