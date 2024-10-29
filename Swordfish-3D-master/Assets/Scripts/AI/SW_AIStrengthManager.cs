using System.Collections.Generic;
using Car;
using Managers;
using UnityEngine;

namespace AI
{
    public class SW_AIStrengthManager : MonoBehaviour
    {
        public static SW_AIStrengthManager instance;

        public SW_AIStrength[] aiTypes;

        [HideInInspector] 
        public List<SWController> currentAISwordFishes = new List<SWController>();

        public float currentStrongAIPercentageClassic;
        public float currentStrongAIPercentageTeamMode;

        private int _killCount;

        private SW_AIStrength[] _allAiStrengths;
        
        private void Awake()
        {
            instance = this;
        }

        public void Initialize(GameModes _mode, int _playerCount)
        {
            currentStrongAIPercentageClassic = XMLManager.instance.database.currentStrongAIPercentageClassic;
            currentStrongAIPercentageTeamMode = XMLManager.instance.database.currentStrongAIPercentageTeamMode;
            
            GetAIStrengths(_mode,_playerCount);
        }

        public void InitializeSW(SWController _aiSW)
        {
            currentAISwordFishes.Add(_aiSW);
            _aiSW.aiStrength = _allAiStrengths[currentAISwordFishes.Count - 1];
        }

        private SW_AIStrength GetAiStrength(SW_StrengthType type)
        {
            return aiTypes[(int) type];
        }

        private void MakeAIStronger()
        {
            if (GameManager.Instance.currentGameMode == GameModes.Classic)
                currentStrongAIPercentageClassic += 5;
            else
                currentStrongAIPercentageTeamMode += 5;
            
            XMLManager.instance.SaveGameData();
        }

        private void MakeAIWeaker()
        {
            if (GameManager.Instance.currentGameMode == GameModes.Classic)
            {
                currentStrongAIPercentageClassic -= 5;
                if (currentStrongAIPercentageClassic < 0)
                    currentStrongAIPercentageClassic = 0;
            }
            else
            {
                currentStrongAIPercentageTeamMode -= 5;
                if (currentStrongAIPercentageTeamMode < 0)
                    currentStrongAIPercentageTeamMode = 0;
            }
            
            XMLManager.instance.SaveGameData();
        }

        public void PlayerGotKill()
        {
            _killCount++;

            if (_killCount == 3)
            {
                _killCount = 0;
                MakeAIStronger();
            }
        }

        public void PlayerGotKilled(int killCount)
        {
            
            if(killCount < 5)
                MakeAIWeaker();
        }

        private SW_AIStrength[] GetAIStrengths(GameModes _mode, int _playerCount)
        {
            var percentage = _mode == GameModes.Classic
                ? currentStrongAIPercentageClassic
                : currentStrongAIPercentageTeamMode;

            var newStrengths = new SW_AIStrength[_playerCount];

            if (percentage < 100)
            {
                var oneAITypeWeight = percentage / (float) _playerCount;
                var counter = 0.0f;
                
                for (var i = 0; i < newStrengths.Length; i++)
                {
                    counter += oneAITypeWeight;

                    if (counter < percentage)
                        newStrengths[i] = GetAiStrength(SW_StrengthType.Normal);
                    else
                        newStrengths[i] = GetAiStrength(SW_StrengthType.Weak);
                }
            }
            else
            {
                var oneAITypeWeight = (percentage - 100) / (float) _playerCount;
                var counter = 0.0f;
                
                for (var i = 0; i < newStrengths.Length; i++)
                {
                    counter += oneAITypeWeight;

                    if (counter < percentage)
                        newStrengths[i] = GetAiStrength(SW_StrengthType.Strong);
                    else
                        newStrengths[i] = GetAiStrength(SW_StrengthType.Normal);
                }
            }

            _allAiStrengths = newStrengths;
            UpdateCurrentPlayers();
            return newStrengths;
        }

        private void UpdateCurrentPlayers()
        {
            if(currentAISwordFishes.Count == 0)
                return;
            
            for (var i = 0; i < currentAISwordFishes.Count; i++)
            {
                currentAISwordFishes[i].aiStrength = _allAiStrengths[i];
            }
        }
    }
}
