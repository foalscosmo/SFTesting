using System.Collections;
using Car;
using Managers;
using NoMonoClasses;
using SnakeScripts;
using UnityEngine;

namespace SW_Tutorial
{
    public class TutorialSWController : SWController
    {
        private void Awake()
        {
            SWBuilder.BuildSnake(swParts,this);
            GetComponentInChildren<Renderer>();
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            base.SwType = SWType.Player;
            base.swParts = TutorialManager.instance.tutorialParts;
            player = new Player("Tutorial");
            Initialize();
            _verticalInput = 1;
            bodyParts[2].gameObject.SetActive(false);
            bodyParts[1].gameObject.SetActive(false);
            InitArrow();
        }

        public void FixedUpdate()
        {
            Steer();
            Accelerate();
            //UpdateBodyPartPositions();
			
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

        private Vector2 _movement;
        private float _timeMoving = 0;
        
        public void SetInput(float horizontalInput,float verticalInput, bool brake, float arrowDistance)
        {
            _movement.x = horizontalInput;
            _movement.y = verticalInput;

            if (TutorialManager.instance.currentTutorialStage == TutorialManager.TutorialStages.StageMovement && _movement.magnitude < 0.01)
            {
                _timeMoving += Time.deltaTime;
                if (_timeMoving > 7)
                {
                    TutorialManager.instance.MissionAccomplished();
                }
            }
            
            _verticalInput = verticalInput;
            _horizontalInput = horizontalInput;
            _arrowDistance = arrowDistance;
            
            CkeckBoost(!InputManager.Instance.JoystickIsActive());
            
            OnBrake(brake);
        }

        private int _boostCount = 0;
        public override void Boost()
        {
            base.Boost();
            if (TutorialManager.instance.currentTutorialStage == TutorialManager.TutorialStages.StageBoost)
            {
                _boostCount++;
                TutorialManager.instance.boostCounterText.text = _boostCount + "/" + 3;

                if (_boostCount == 3)
                    StartCoroutine(WaitForBoost());
            }
        }

        private IEnumerator WaitForBoost()
        {
            yield return new WaitForSeconds(0.7f);
            TutorialManager.instance.MissionAccomplished();
        }

        protected override void Initialize()
        {
            //->>>>>>>>ES SHESACVLELIA
            _weaponControllers  = this.transform.parent.GetComponentsInChildren<SWWeaponController>();

            var amount = 3;
			
            _weaponControllers[0].Initialize(swParts.frontNosePrefab,this,amount);
            _weaponControllers[1].Initialize(swParts.backNosePrefab,this,amount);
            
            //_weaponControllers[0].lastWeapon = _weaponControllers[0].
            //<<<<<<<<<-ES SHESACVLELIA
			
            var snakeParts = this.transform.parent.GetComponentsInChildren<SWBody>();
			
            for (var i = 0; i < snakeParts.Length; i++)
            {
                snakeParts[i].Sw = this;
            }

            _coinMagnet = GetComponentInChildren<CoinMagnet>().Initialize(this);

            shirtMaterial = new Material(shirtMaterial);
            shirtMaterial.EnableKeyword("_Texture");
            shirtMaterial.color = shirtColor;
			
            if (SwType == SWType.Player)
            {
                SoundManager.instance.PlayVoiceLine(SoundTypesEnum.GameStartVoiceLine,false);
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
            _prevPos = transform.position;
            _prevPos.y = 0;
            MaxHealth = Health;
            InitColors();
        }
    }
}
