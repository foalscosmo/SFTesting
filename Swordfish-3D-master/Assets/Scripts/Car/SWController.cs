using System;
using System.Collections;
using System.Collections.Generic;
using AI;
using CameraControllers;
using DG.Tweening;
using Managers;
using NoMonoClasses;
using Particles;
using SnakeScripts;
using UnityEngine;
using CandyCoded.HapticFeedback;
using Random = UnityEngine.Random;

namespace Car
{
	public enum SWType
	{
		Player,
		AI
	}

	public enum Teams
	{
		Team1,
		Team2,
		Team3,
		Team4
	}

	public class SWController : MonoBehaviour
	{
		[Header("SNAKE")]
		public List<Transform> 	bodyParts = new List<Transform>();
		public float 			distanceBetweenBodyPartsOnBoost = 2.0f;
		public Transform 		bodyHolder;
		public SwordFishParts 	swParts;
		[Header("END SNAKE")]
		#region Private Params
		protected float 		_horizontalInput;
		protected float 		_verticalInput;
		protected float 		_arrowDistance;
		private float 		_steeringAngle;
		protected Rigidbody 	_rigidbody;
		private bool 		_isBraking = false;
		private float 		_oldMaxMagnitude;
		protected Vector3 	_prevPos;
		private float 		_curSpeed;
		private bool 		_reverse = false;
		private CarBuilder 	_carBuilder;
		private float 		_prevMagnitude;
		protected CoinMagnet  _coinMagnet;
		#endregion

		#region Public Params
		public SWType SwType;

		public Player player;
	
		public float SpeedMultiplierOnBoost = 1.5f;
		public float MaxSpeed = 80;
		public float BoostTime = 1f;
		public int Health = 100;
		public Teams Team;
		public GameObject crown;
		[HideInInspector] 
		public bool hasCrown = false;

		public int MaxHealth = 100;
		
		[Header("Boost Loading")] 
		public GameObject allBoostobjects;
		public GameObject loadingMaterialGameobj;
		protected Material _loadingMaterial;
		
		#endregion

		#region Arrow
		public GameObject joystickArrowRotator;
		public GameObject joystickArrowGameObj;
		public float minArrowPozLimit = 6f, maxArrowLimit = 10f;
		private bool _updateArrowPos = false;
		private float _newMinArrowPozLimit, _newMaxArrowLimit;
		private void Start()
		{
			isFreshlyRespawned = true;
			bodyParts[2].gameObject.SetActive(false);
			bodyParts[1].gameObject.SetActive(false);
			var smokeParticle = transform.parent.GetComponentInChildren<ParticleSystem>();
			smokeParticle.transform.localScale = new Vector3(10, 10, 10);
			allBoostobjects.transform.localScale = new Vector3(0.5f,0.5f,0.5f);
			crown.transform.parent.localScale = new Vector3(0.5f, 0.5f, 0.5f);
		}
		
		protected void InitArrow()
		{
			if (SwType == SWType.Player)
			{
				_updateArrowPos = true;
				joystickArrowRotator.SetActive(true);
			}
			else
			{
				joystickArrowRotator.SetActive(false);
			}
		}
		
		private void Update()
		{
			if (SwType == SWType.Player && !crown.activeSelf)
			{
				gainedCrown = false;
			}
			crown.transform.localPosition = bodyParts[0].localScale.x switch
			{
				<= 2 => new Vector3(0.0f, 4, 0.0f),
				> 2 and < 3 => new Vector3(0.0f, 8, 0.0f),
				> 3 and < 5 => new Vector3(0.0f, 10, 0.0f),
				> 5 and < 7 => new Vector3(0.0f, 12, 0.0f),
				> 7 and < 9 => new Vector3(0.0f, 14, 0.0f),
				> 9 and < 11 => new Vector3(0.0f, 16, 0.0f),
				> 11 and < 14 => new Vector3(0.0f, 18, 0.0f),
				> 14 and < 18 => new Vector3(0.0f, 20, 0.0f),
				_ => crown.transform.localPosition
			};
		}

		private void UpdateArrowPosition(Vector3 lookAtPos)
		{
			if(!_updateArrowPos) return;

			if(_weaponControllers.Length == 0 || _weaponControllers[0] == null)
				return;
			
			var offset = Vector3.Distance(joystickArrowRotator.transform.position,
				_weaponControllers[0].lastWeapon.position);

			_newMinArrowPozLimit = minArrowPozLimit + offset;
			_newMaxArrowLimit = maxArrowLimit + offset;
			
			joystickArrowRotator.transform.position = _rigidbody.transform.position;
			
			joystickArrowRotator.transform.DOLookAt(lookAtPos, 0.01f);

			var newLocalPos = joystickArrowGameObj.transform.localPosition;
			newLocalPos.z = Mathf.Clamp(_arrowDistance / 75 + _newMinArrowPozLimit, _newMinArrowPozLimit, _newMaxArrowLimit);
			joystickArrowGameObj.transform.localPosition = newLocalPos;
		}

		#endregion
		
		#region RespawnFlickering

		private bool _isFreshlyRespawned;
		private MeshRenderer[] _allMeshRenderers = null;

		public bool isFreshlyRespawned
		{
			get { return _isFreshlyRespawned;}
			set
			{
				_isFreshlyRespawned = value;
				if (_isFreshlyRespawned)
				{
					StartCoroutine(Flickering());
				}
			}
		}

		private IEnumerator Flickering()
		{
			yield return new WaitForEndOfFrame();
			var index = 0;
			if (SwType == SWType.Player)
			{
				CameraTopDown.Instance.ChangeCameraZoom(0);
			}
			_allMeshRenderers = this.transform.parent.GetComponentsInChildren<MeshRenderer>(true);
			
			while (isFreshlyRespawned)
			{
				
				transform.localScale = new Vector3(1.25f, 1.25f, 1.25f);
				allBoostobjects.transform.localScale = new Vector3(0.5f,0.5f,0.5f);
				yield return new WaitForSeconds(0.1f);
				index++;
				for (var i = 0; i < _allMeshRenderers.Length; i++)
				{
					_allMeshRenderers[i].enabled = !_allMeshRenderers[i].enabled;
				}

				if (index == 15)
					isFreshlyRespawned = false;
			}
			
			for (var i = 0; i < _allMeshRenderers.Length; i++)
			{
				_allMeshRenderers[i].enabled = true;
			}
		}

		#endregion

		[Header("Rotations on turn")]
		private float curFrameSideRot = 0;

		public Material shirtMaterial;
		public Color shirtColor;

		private Vector3[] _prevPositions = null;

		private LineRenderer _lineRenderer;

		private bool canTest;
		private float _distanceBetweenPartsMultiplier = 1f;
		
		
		private float[] _distanceBetweenPartMultiplier = null;

		public bool isVisible = false;

		//[HideInInspector]
		public SW_AIStrength aiStrength;

		private bool gainedCrown;
		public void SetAsKing(bool isKing)
		{
			if (hasCrown == isKing)
				return;

			crown.SetActive(isKing); 
			hasCrown = isKing;
			if (SwType == SWType.Player && !gainedCrown && crown.activeSelf)
			{
				SoundManager.instance.PlaySound(SoundTypesEnum.Crown);
				gainedCrown = true;
			}
			UIManager.Instance.SetCrownActive(isKing, this);
		}

		

		private bool particleTest;
		private float multiplierAmount;
		public void UpdateBodyPartPositions()
		{
			if (_distanceBetweenPartMultiplier == null)
			{
				_distanceBetweenPartMultiplier = new float[bodyParts.Count - 1];

				multiplierAmount = 0.8f;
				for (var i = 1; i < bodyParts.Count; i++)
				{
					if (i == bodyParts.Count - 1)
					{
						multiplierAmount = 0.4f;
						
						_distanceBetweenPartMultiplier[i - 1] =
							bodyParts[i - 1].GetComponentInChildren<MeshRenderer>().bounds.size.z * multiplierAmount;
					}
					else
					{
						_distanceBetweenPartMultiplier[i - 1] =
							bodyParts[i].GetComponentInChildren<MeshRenderer>().bounds.size.z * multiplierAmount;
					}
				}
				
				loadingMaterialGameobj.transform.parent.SetParent(bodyParts[bodyParts.Count/2],false);
			}
			
			for (var i = 1; i < bodyParts.Count; i++)
			{
				var heading = bodyParts[i - 1].position - bodyParts[i].position;

				var offset = _boost ? distanceBetweenBodyPartsOnBoost : _distanceBetweenPartMultiplier[i-1];
				
				var rot = i == 1 ? transform.forward : bodyParts[i - 1].forward;
				var angle = Vector3.SignedAngle(bodyParts[i].forward, rot, Vector3.up);
				
				var multiplier = 0.14f;
				
				bodyParts[i].position = Vector3.Lerp(bodyParts[i].position,
					bodyParts[i].position + heading.normalized * (heading.magnitude - offset) * _distanceBetweenPartsMultiplier,
					Time.fixedDeltaTime * 25);
				
				bodyParts[i].RotateAround(bodyParts[i - 1].position,Vector3.up,angle*multiplier);
				bodyParts[i].LookAt(bodyParts[i - 1]);
			}
		}

		public void UpdateScores(bool spawnScoreText = true)
		{
			UIManager.Instance.UpdateKillCount(this, player.currentKillCount);
			UIManager.Instance.UpdateScores();

			if (SwType == SWType.Player && spawnScoreText)
				UIManager.Instance.SpawnScoreText(this, 1);
		}

		public void MainInit()
		{
			AwakeTest();
			StartTest();
			InitArrow();
		}
		
		private void AwakeTest()
		{
			if (SwType == SWType.Player && GameResourcesManager.instance != null)
			{
				swParts = GameResourcesManager.instance.currentlySelectedCharacter;
				playerData =  GameResourcesManager.instance.playerData;
			}
			
			if (SwType == SWType.AI)
			{
				swParts =
					GameResourcesManager.instance.allSwordFishSOs[Random.Range(0,
						GameResourcesManager.instance.allSwordFishSOs.Length)];
			}
			
			SWBuilder.BuildSnake(swParts,this);
			GetComponentInChildren<Renderer>();
			_rigidbody = GetComponent<Rigidbody>();
		}

		private void StartTest()
		{
			if (SwType == SWType.Player && PowerUpManager.instance != null)
			{
				playerData.Initialize();
				PowerUpManager.instance.InitializePowerUps(playerData);
			}

			Initialize();
			_verticalInput = 1;
		}

		private void SetNoseSizes(int size)
		{
			for (var i = 0; i < _weaponControllers.Length; i++)
			{
				_weaponControllers[i].ResetWeapon();
				_weaponControllers[i].EnlargeNose(size );
			}
		}
		
		private void EnlargeNoseSizes(int size)
		{
			for (var i = 0; i < _weaponControllers.Length; i++)
			{
				_weaponControllers[i].EnlargeNose(size );
			}
		}

		public void ResetStats()
		{
			isDead = false;
			var amount = 3;
			
			if (SwType == SWType.Player)
			{
				joystickArrowRotator.SetActive(true);
				PowerUpManager.instance.InitializePowerUps(playerData); 
				var offset = (int) playerData.GetPowerupAffection(PowerUpTypes.BiggerNoseOnStart);
				amount += offset;
				
				PowerUpManager.instance.RegisterActionOnPowerUpEnd(PowerUpTypes.BiggerNoseOnStart, () =>
					{
						EnlargeNoseSizes(-offset);
					});
			}

			SetNoseSizes(amount);
			
			Health = MaxHealth;
			_aiCanBoost = true;

			var pos = this.transform.position;
			pos.y = bodyParts[0].position.y;
			
			for (var i = 0; i < bodyParts.Count; i++)
			{
				bodyParts[i].position = pos;
			}
			
			_rigidbody.isKinematic = false;
			_boostVal = 1.0f;
			_boostAvailable = true;
			_boost = false;

			
			for (var i = 0; i < transform.childCount; i++)
			{
				transform.GetChild(i).gameObject.SetActive(true);
			}
			
			if (SwType == SWType.AI)
			{
				allBoostobjects.SetActive(false);

				StartCoroutine(AfterRespawn());
			}
			else
			{
				SoundManager.instance.PlayVoiceLine(SoundTypesEnum.GameStartVoiceLine,false);
			}
		}

		private IEnumerator AfterRespawn()
		{
			yield return new WaitForEndOfFrame();
			StartCoroutine(AIMakeTurnDecision());
			_startMakingPlayerLikeDecisions = false;
			if (_playerLikeDecisionCoroutine != null)
			{
				StopCoroutine(_playerLikeDecisionCoroutine);
			}

			_playerLikeDecisionCoroutine = StartCoroutine(AIMakePlayerLikeAction());
		}
		
		protected SWWeaponController[] _weaponControllers;

		// [HideInInspector] 
		public PlayerData playerData = null;
		
		protected virtual void Initialize()
		{
			//->>>>>>>>ES SHESACVLELIA
			_weaponControllers  = this.transform.parent.GetComponentsInChildren<SWWeaponController>();

			var amount = 3;
			if (SwType == SWType.Player && playerData != null)
			{
				var offset = (int)playerData.GetPowerupAffection(PowerUpTypes.BiggerNoseOnStart);
				amount += offset;
				
				PowerUpManager.instance.RegisterActionOnPowerUpEnd(PowerUpTypes.BiggerNoseOnStart, () =>
				{
					EnlargeNoseSizes(-offset);
				});
			}
			
			for (var i = 0; i < _weaponControllers.Length; i++)
			{
				if (i == _weaponControllers.Length - 1)
					_weaponControllers[i].Initialize(swParts.backNosePrefab,this,amount);
				else
					_weaponControllers[i].Initialize(swParts.frontNosePrefab,this,amount);
			}
			
			var snakeParts = this.transform.parent.GetComponentsInChildren<SWBody>();
			
			for (var i = 0; i < snakeParts.Length; i++)
			{
				snakeParts[i].Sw = this;
			}

			_coinMagnet = GetComponentInChildren<CoinMagnet>().Initialize(this);
			
			player = new Player(UIManager.Instance.names[Random.Range(0,UIManager.Instance.names.Length)]);

			_playerLikeDecisionCoroutine = StartCoroutine(AIMakePlayerLikeAction());
			
			shirtMaterial = new Material(shirtMaterial);
			shirtMaterial.EnableKeyword("_Texture");
			shirtMaterial.color = shirtColor;

			UIManager.Instance.RegisterToPlayerslist(this);
			
			if (SwType == SWType.Player)
			{
				SoundManager.instance.PlayVoiceLine(SoundTypesEnum.GameStartVoiceLine,false);
				GameManager.Instance.SetPlayer(this);
				_verticalInput = 1;

				_loadingMaterial = loadingMaterialGameobj.GetComponent<SpriteRenderer>().material;
				
				if (allBoostobjects)
					allBoostobjects.SetActive(true);
			}
			else
			{
				_boostAvailable = true;
				StartCoroutine(UpdateIenum());
				if (allBoostobjects)
					allBoostobjects.SetActive(false);
			}

		
			_carBuilder = GetComponent<CarBuilder>();
			_prevPos = transform.position;
			_prevPos.y = 0;
			MaxHealth = Health;
		

			InitColors();
			
			UIManager.Instance.RegisterCar(this);
			GameManager.Instance.RegisterCar(this);
		}

		protected void InitColors()
		{
			if (SwType == SWType.Player)
				shirtColor = GameManager.Instance.playerColor;
			else
				shirtColor = GameManager.Instance.GetFreeColor();
			
			shirtMaterial.color = shirtColor;
		}

		public float newHandlingSpeed = 0.4f;
		
		public void SetInput(float horizontalInput,float verticalInput, bool brake, float arrowDistance)
		{
			_verticalInput = verticalInput;
			_horizontalInput = horizontalInput;
			_arrowDistance = arrowDistance;

			CkeckBoost(!InputManager.Instance.JoystickIsActive());
			OnBrake(brake);
		}

		protected bool _boostAvailable = false;
		private float _currentBoostTimer = 0;
		public float boostTimer = 0.5f;
		
		protected void CkeckBoost(bool joistickisInActive)
		{
			if(_boost)
				return;
			
			if (!joistickisInActive)
			{
				_currentBoostTimer += Time.deltaTime;
				if (_currentBoostTimer <= boostTimer)
				{
					_loadingMaterial.SetFloat("_Progress",_currentBoostTimer/boostTimer);
				}
				else
				{
					_loadingMaterial.SetFloat("_Progress",1);
					_boostAvailable = true;
				}

			}
			else
			{
				_currentBoostTimer = 0.0f;
				if (_boostAvailable)
				{
					Boost();
					_boostAvailable = false;
				}
				else
				{
					_loadingMaterial.SetFloat("_Progress",0.0f);
				}
			}
		}

		private IEnumerator AIBoost()
		{
			_boostAvailable = false;
			yield return new WaitForSeconds(boostTimer + _newBoostTimeOn);
			_boostAvailable = true;
		}
		

		private float CalculateSpeed()
		{
			var curPos = transform.position;
			curPos.y = 0;
			var posDiff = Vector3.Distance(_prevPos, curPos);
			_prevPos = curPos;
			
			return ((posDiff * 50) * 18) / 5;
		}

		private void OnTriggerEnter(Collider other)
		{
			if (other.transform.CompareTag("Death"))
			{
				if (SwType == SWType.Player)
				{
					Death();
				}
				else
				{
					GetHit(100000,Vector3.down);
				}
			}

			if (other.CompareTag("Destructible"))
			{
				if (SwType == SWType.Player)
				{
					SoundManager.instance.PlaySound(SoundTypesEnum.Destructible);
					HapticFeedback.LightFeedback();
				}
			}
		}

		#region Car Controls
		public float _newBoostTimeOn = 0.3f;
		protected  void Accelerate()
		{
			if (_accelerationModifier < 1)
				_accelerationModifier += Time.deltaTime * 2;

			if (_accelerationModifier > 1) 
				_accelerationModifier = 1.0f;

			if (_isGrounded)
			{
				Vector3 forwardVelocity = _rigidbody.transform.forward * MaxSpeed * _boostVal * _accelerationModifier;
				_rigidbody.velocity = new Vector3(forwardVelocity.x, _rigidbody.velocity.y, forwardVelocity.z);

				_curSpeed = CalculateSpeed();
			}
		}

		#region AI Actions
		
		private float _angle;
		public float maxBoostAngle = 20;
		public bool AIDesicionMade = false;

		private enum TargetType
		{
			Car,
			Coin
		}
		public GameObject target;
		

		public Action<SWController> stateChanged;

		private void OnTargetStateChange(SWController sw = null)
		{
			if (sw != null)
			{
				ChooseNewTarget(TargetType.Car, true);
				if (sw.stateChanged != null) sw.stateChanged -= OnTargetStateChange;
			}
		}

		/// <summary>
		/// Chooses new target for basic AI.
		/// </summary>
		/// <param name="newTargetType"> New target type for AI.</param>
		/// <param name="makeDecisionForcefuly"> Stops previous decision if true. </param>
		private void ChooseNewTarget(TargetType newTargetType, bool makeDecisionForcefuly = false)
		{
			if(makeDecisionForcefuly && _AiMakingDecisionCoroutine!= null)
				StopCoroutine(_AiMakingDecisionCoroutine);

			target = GameManager.Instance.CheckIfCoinIsInRange(this.transform.position);

			if (target == null && newTargetType == TargetType.Car)
			{
				target = GameManager.Instance.GetNearestCarTarget(this).gameObject;
				if (target != null)
				{
					target.GetComponent<SWController>().stateChanged += OnTargetStateChange;
				}
					
			}
			
			if (this.gameObject.activeSelf)
				_AiMakingDecisionCoroutine = StartCoroutine(AIMakeDecision());
		}

		#endregion


		private Vector3 lookAtPos;
		private Vector2 test = Vector2.down;
		
		protected void Steer()
		{
			if (_gotBumped)
			{
				return;
			}

			test.x = _horizontalInput;
			test.y = _verticalInput;
			
			if (SwType == SWType.Player)
			{
				if (test.magnitude < 0.1f)
				{
					_arrowDistance = 0;
					UpdateArrowPosition(_rigidbody.transform.position + 10*_rigidbody.transform.forward);
					_angle = 0;
					_rigidbody.transform.DOLookAt(_rigidbody.transform.position + _rigidbody.transform.forward * 5,1);
					return;
				}

				var dir = (new Vector3(_horizontalInput, 0, _verticalInput)) * 100f;
			
				var lookAtPosNew = _rigidbody.transform.position + dir;
				
				UpdateArrowPosition(lookAtPosNew);

				_angle = Vector3.SignedAngle(_rigidbody.transform.forward, (lookAtPosNew-_rigidbody.transform.position).normalized, Vector3.up);

				var handleSpeed = newHandlingSpeed;
				
				if (Mathf.Abs(_angle) < 30)
				{
					handleSpeed /= 5;
				}
								
				_rigidbody.transform.DOLookAt(lookAtPosNew,handleSpeed);
				return;
			}
			else
			{
				if (!AIDesicionMade)
					ChooseNewTarget(TargetType.Car);

				if (target == null)
				{
					return;
				}

				if (_startMakingPlayerLikeDecisions)
				{
					lookAtPos = _rigidbody.transform.position + (_rigidbody.transform.forward * 2);
					lookAtPos.y = 0.0f;

					lookAtPos = RotateAround(lookAtPos, Vector3.up, _newLookAtAngle);//.Euler(0, _newLookAtAngle, 0) * lookAtPos;
					
					_rigidbody.transform.DOLookAt(lookAtPos,newHandlingSpeed);
					return;
				}
				
				lookAtPos = target.transform.position;
				lookAtPos.y = 0.0f;

				_angle = Vector3.SignedAngle(_rigidbody.transform.forward, (lookAtPos-_rigidbody.transform.position).normalized, Vector3.up);


				if (_aiCanBoost && Mathf.Abs(_angle) < aiStrength.maxBoostAngle )
				{
					Boost();
					if (this.gameObject.activeSelf)
						StartCoroutine(AICanBoostDecision());
				}
				
				if(_turnDecisionmade)
					return;
				if (this.gameObject.activeSelf)
					StartCoroutine(AIMakeTurnDecision());

				if (Mathf.Abs(_angle) < 1)
				{
					_angle = 0;
					return;
				}

				_rigidbody.transform.DOLookAt(lookAtPos,newHandlingSpeed);
			}
		}
		
		private Vector3 RotateAround(Vector3 center, Vector3 axis, float angle) {
			Vector3 pos = center;
			Quaternion rot = Quaternion.AngleAxis(angle, axis); // get the desired rotation
			Vector3 dir = pos - this.transform.position; // find current direction relative to center
			dir = rot * dir; // rotate the direction

			return this.transform.position + dir;
		}

		private bool _turnDecisionmade = false;
		
		private IEnumerator AIMakeTurnDecision()
		{
			_turnDecisionmade = true;
			yield return new WaitForSeconds(Random.Range(aiStrength.minAITurnDecisionTime, aiStrength.maxAITurnDecisionTime));
			_turnDecisionmade = false;
		}

		private IEnumerator AIMakeDecision()
		{
			AIDesicionMade = true;
			yield return new WaitForSeconds(aiStrength.targetPickUpDecisionTime);
			AIDesicionMade = false;
		}

		private bool _aiCanBoost = true;
		
		private IEnumerator AICanBoostDecision()
		{
			_aiCanBoost = false;
			yield return new WaitForSeconds(Random.Range(aiStrength.minBoostTime,aiStrength.maxBoostTime));
			_aiCanBoost = true;
		}

		private bool _startMakingPlayerLikeDecisions = false;
		private float _newLookAtAngle;
		private Coroutine _playerLikeDecisionCoroutine;

		private IEnumerator AIMakePlayerLikeAction()
		{
			while (true)
			{
				_startMakingPlayerLikeDecisions = false;
				yield return new WaitForSeconds(Random.Range(aiStrength.minPlayerLikeActionTimer,
					aiStrength.maxPlayerLikeActionTimer));
				_startMakingPlayerLikeDecisions = true;
				StartCoroutine(AIRandomMovement());
				yield return new WaitForSeconds(Random.Range(0.5f, 1.5f));
			}
		}

		private enum RandomAction
		{
			Rotate,
			LeftRight,
			BackFront
		}

		private IEnumerator AIRandomMovement()
		{
			var speed = _newLookAtAngle = Random.Range(20f, 50f);

			var random = Random.Range(0, 99);
			var randomAction = RandomAction.Rotate;

			if (random < 33)
				randomAction = RandomAction.Rotate;
			else if(random < 66) 
				randomAction = RandomAction.BackFront;
			else
				randomAction = RandomAction.LeftRight;

			var toRight = Random.Range(0, 100) <= 50;

			var startingVal = _newLookAtAngle;
			
			while (_startMakingPlayerLikeDecisions)
			{
				yield return new WaitForEndOfFrame();
				
				switch (randomAction)
				{
					case RandomAction.Rotate:

						if (toRight)
							_newLookAtAngle = speed * 3;
						else
							_newLookAtAngle = -speed * 3;
						
						break;
					case RandomAction.LeftRight:

						if (toRight)
						{
							_newLookAtAngle = speed * 3;
						}
						else
						{
							_newLookAtAngle = -speed * 3;
						}

						yield return new WaitForSeconds(Random.Range(0.15f, 0.2f));
						toRight = !toRight;
						
						break;
					case RandomAction.BackFront:
						
						if (toRight)
						{
							_newLookAtAngle = speed;
						}
						else
						{
							_newLookAtAngle = -speed;
						}
						
						
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
		}

		[HideInInspector]
		public bool _boost = false;

		private Coroutine _boostCoroutine;
		private Coroutine _backToNormalCoroutine;
		public virtual void Boost()
		{
			if (_boost) return;
			if (SwType == SWType.Player)
			{
				SoundManager.instance.PlaySound(SoundTypesEnum.SnekBoost);
			}

			if (SwType == SWType.AI)
			{
				StartCoroutine(AIBoost());
			}
			
			_boost = true;
			var cam = GameManager.Instance.TopViewCamera.GetComponentInChildren<CameraTopDown>();
			if (_boostCoroutine != null)
			{
				StopCoroutine(_boostCoroutine);
			}
			_boostCoroutine = StartCoroutine(BoostIenum(cam));
		}
		
		private IEnumerator BoostIenum(CameraTopDown cam)
		{
			StartCoroutine(BoostValChanger());
			yield return new WaitForSeconds(_newBoostTimeOn);
			_boost = false;
		}

		private float _boostVal = 1f;
		public float maxBoostvalSetter = 3f;
		
		private IEnumerator BoostValChanger()
		{
			_boostVal = maxBoostvalSetter;
			while (_boost)
			{
				yield return new WaitForSeconds(_newBoostTimeOn);
				_boostVal = 1f;
			}
		}
	
		public virtual void OnBrake(bool isDown)
		{
			if (_isBraking == isDown) return;
		
			_isBraking = isDown;
			if (_isBraking)
			{
				_reverse = false;
				_verticalInput = -1;
			}
			else
			{
				_verticalInput = 1;
			}
		}

		private bool _isGrounded = true;
		private float _groundedTimer = 0;

		#endregion

		private int _timer = 0;
		protected IEnumerator UpdateIenum()
		{
			while (true)
			{
				yield return new WaitForSeconds(0.2f);
				if (_curSpeed < 10f || _reverse)
				{
					_timer += 1;
					if (_timer > 15)
					{
						_timer = 0;
						_verticalInput = _reverse ? 1 : -1;
						_reverse = !_reverse;
					}
				}
				else
				{
					_timer = 0;
					_verticalInput = 1;
				}
			}
		}

		protected float _boostEmptier = 1.0f;

		public void FixedUpdate()
		{
			if(!GameManager.Instance.theGameIsStarted)
				return;
			
			Steer();
			Accelerate();
			//UpdateBodyPartPositions();
			
			//Empty boost circle slider.
			if (SwType == SWType.Player && _boost)
			{
				_boostEmptier -= Time.deltaTime / _newBoostTimeOn;
				_loadingMaterial.SetFloat("_Progress",_boostEmptier);
			}
			else
			{
				_boostEmptier = 1.0f;
			}
		}

		#region Health

		public void GotKill()
		{
			player.currentKillCount += 1;
			UpdateScores(false);
			
			
			var canEnlarge = false;
			
			var amount = 1;

			if (SwType == SWType.Player)
			{
				SW_AIStrengthManager.instance.PlayerGotKill();
			}

			switch (player.currentKillCount)
			{
				case 1:
					canEnlarge = true;
					break;
				case 2:
					canEnlarge = true;
					break;
				case 4:
					canEnlarge = true;
					break;
				case 6:
					canEnlarge = true;
					break;
				case 8:
					canEnlarge = true;
					break;
				case 11:
					canEnlarge = true;
					break;
				case 14:
					canEnlarge = true;
					break;
				case 17:
					canEnlarge = true;
					break;
				case 21:
					canEnlarge = true;
					break;
				case 25:
					canEnlarge = true;
					break;
				case 29:
					canEnlarge = true;
					break;
				case 33:
					canEnlarge = true;
					break;
				case 38:
					canEnlarge = true;
					break;
				default:
					canEnlarge = false;
					break;
			}
			
			if(!canEnlarge)
				return;
			
			if (SwType == SWType.Player)
			{
				amount *= (int) playerData.GetPowerupAffection(PowerUpTypes.DoubleGrowSpeed);
			}

			EnlargeNoseSizes(amount);
		}

		private float _accelerationModifier = 1.0f;

		private bool _gotBumped = false;
		private Coroutine _AiMakingDecisionCoroutine = null;

		public float BumpForce = 25f;
		public float BumpStunTimer = 0.5f;
		
		public bool GetHit(int damage, Vector3 forward)
		{
			if (SwType == SWType.Player)
			{
				SoundManager.instance.PlaySound(SoundTypesEnum.CarBump);
				SoundManager.instance.PlaySound(SoundTypesEnum.SnekDeath);
			}
			
			Health -= damage;

			if (Health < 0)
			{
				Death();
				return true;
			}
			return false;
		}

		public GameObject body;

		public bool isDead = false;
		
		private void Death()
		{
			isDead = true;
			bodyParts[0].transform.localScale = new Vector3(1.25f, 1.25f, 1.25f);
			GameManager.Instance.UnregisterCar(this);
			UIManager.Instance.UnregisterCar(this);
			for (var i = 0; i < transform.childCount; i++)
			{
				transform.GetChild(i).gameObject.SetActive(false);
			}
			
			body.SetActive(false);
			
			ParticleManager.Instance.PlayParticle(transform.position,ParticleType.Explosion);
			
			stateChanged?.Invoke(this);

			if (SwType == SWType.Player)
			{
				HapticFeedback.HeavyFeedback();
				CameraTopDown.Instance.ChangeCameraZoom(0);
				bodyParts[0].transform.localScale = new Vector3(1.25f, 1.25f, 1.25f);
				allBoostobjects.transform.localScale = new Vector3(1f,1f,1f);
				SW_AIStrengthManager.instance.PlayerGotKilled(player.currentKillCount);
				joystickArrowRotator.SetActive(false);
				CameraTopDown.Instance.ZoomSlider.value = 0f;
				SoundManager.instance.PlayVoiceLine(SoundTypesEnum.LoseVoiceLine,false);
				
				if (GameManager.Instance.currentGameMode == GameModes.Classic)
				{
					UIManager.Instance.GameOverlast(this);
				}
				else
				{
					player.ResetStats();
					UpdateScores(false);
				}
				PowerUpManager.instance.ResetAfterDeath(playerData);
			}
			else
			{
				player.ResetStats();
				UpdateScores(false);
			}
			
			GameManager.Instance.SWDeath(this);
		}

		
		public void HealthRegen(int healthRegenAmount)
		{
			Health = Mathf.Clamp(Health + healthRegenAmount, 0, MaxHealth);
			UIManager.Instance.UpdateHealthBar(this,healthRegenAmount, false);
			ParticleManager.Instance.PlayFollowingParticle(transform,ParticleType.HealthPickUp);
		}
	
		#endregion

		public void GotCollectAble(Coin coin)
		{
			var amount = 1 * (int)playerData.GetPowerupAffection(PowerUpTypes.DoubleCoins);
			if (coin.isCoin)
				player.collectedCoins += amount;
			else
				player.collectedGems += amount;
			
			
			if (SwType == SWType.Player)
			{
				UIManager.Instance.UpdateCoins(this,amount);
				SoundManager.instance.PlaySound(SoundTypesEnum.SnekCoinGain);
			}
		}

		
		public void ShopTest()
		{
			SWBuilder.BuildSnakeForShop(transform,swParts);
		}

		private void OnCollisionEnter(Collision other)
		{
			ChooseNewTarget(TargetType.Car, true);
		}
	}
}