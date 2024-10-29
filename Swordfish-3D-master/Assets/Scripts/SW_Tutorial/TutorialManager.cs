using System.Collections;
using System.ComponentModel;
using Managers;
using SnakeScripts;
using UnityEngine;
using UnityEngine.UI;

namespace SW_Tutorial
{
    public class TutorialManager : MonoBehaviour
    {
        public enum TutorialStages
        {
            StageMovement,
            StageBoost,
            StageKill,
            StageDestroyObjects
        }
        
        public enum TutorialStates
        {
            StatePaused,
            StateInProgress
        }
        
        public static TutorialManager instance;

        public TutorialStages currentTutorialStage = TutorialStages.StageMovement;
        public TutorialStates currentTutorialState = TutorialStates.StatePaused;

        public SwordFishParts tutorialParts;

        public Text tutorialText;

        public Text boostCounterText;

        public string[] tutorialStageTexts;

        public float movementTime = 5;

        public GameObject swordfishAIsHolder;
        public GameObject destructibleHolder;
        public GameObject handAnimation;
        public GameObject boostAnimation;

        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            StartTutorial();
        }

        private void StartTutorial()
        {
            currentTutorialStage = TutorialStages.StageMovement;
            currentTutorialState = TutorialStates.StatePaused;
            OnNextStage();
        }

        private void OnNextStage()
        {
            tutorialText.text = tutorialStageTexts[(int) currentTutorialStage];

            boostAnimation.SetActive(currentTutorialStage == TutorialStages.StageBoost);
            boostCounterText.gameObject.SetActive(currentTutorialStage == TutorialStages.StageBoost);
            handAnimation.SetActive(currentTutorialStage == TutorialStages.StageMovement);
            swordfishAIsHolder.SetActive(currentTutorialStage == TutorialStages.StageKill);
            destructibleHolder.SetActive(currentTutorialStage == TutorialStages.StageDestroyObjects);
            
           

            //startStageButton.gameObject.SetActive(true);
        }

        public void StartTutorialProgress()
        {
            currentTutorialState = TutorialStates.StateInProgress;

            if (currentTutorialStage == TutorialStages.StageMovement)
                handAnimation.SetActive(false);
            if(currentTutorialStage == TutorialStages.StageBoost)
                boostAnimation.SetActive(false);
            

            if (currentTutorialStage == TutorialStages.StageMovement)
                StartCoroutine(WaitForMovementStage());
            //startStageButton.gameObject.SetActive(false);
        }

        public void MissionAccomplished()
        {
            if (currentTutorialStage == TutorialStages.StageKill)
            {
                //FINISH
                currentTutorialState = TutorialStates.StatePaused;
                tutorialText.text = "NOW YOU ARE READY FOR BATTLE!";
                StartCoroutine(FinishTutorial());
                PlayerPrefs.SetInt("Tutorial", 1);
                return;
            }
            
            currentTutorialStage = (TutorialStages) ((int) currentTutorialStage + 1);
            currentTutorialState = TutorialStates.StatePaused;
            OnNextStage();
        }

        private IEnumerator FinishTutorial()
        {
            yield return new WaitForSeconds(1);
            MySceneManager.instance.LoadSceneAdditiveAfterUnloading(MyScenes.SW_ArenaScene.ToString(),MyScenes.SW_Tutorial.ToString());
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0) && currentTutorialState == TutorialStates.StatePaused)
            {
                StartTutorialProgress();
            }
        }

        private IEnumerator WaitForMovementStage()
        {
            yield return new WaitForSeconds(movementTime);
            MissionAccomplished();
        }

        public void ChangeTutorialStage(TutorialStages stage)
        {
            
        }

        public void ChangeTutorialState(TutorialStates state)
        {
            
        }

        private int _killCount;
        public void GotKill()
        {
            _killCount++;

            if (_killCount == 4)
            {
                MissionAccomplished();
            }
        }
        
        private int _destructionCount;
        public void DestroyedObject()
        {
            _destructionCount++;

            if (_destructionCount == 4)
            {
                MissionAccomplished();
            }
        }
    }
}
