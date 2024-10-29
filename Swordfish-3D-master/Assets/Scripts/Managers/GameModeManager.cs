using System;
using Car;
using UnityEngine;

namespace Managers
{
    public class GameModeManager : MonoBehaviour
    {
        public static GameModeManager instance;

        [HideInInspector]public GameModes currentGameMode = GameModes.Classic;
        [HideInInspector]public int playerCountInGame = 20;
        [HideInInspector]public Teams[] teams;
        public Color[] teamColors;
        public Sprite[] teamSprites;

        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            Initialize(GameResourcesManager.instance.currentlySelectedGameMode);
        }

        public Color GetSWColor(SWController sw)
        {
            if (currentGameMode == GameModes.Classic)
                return sw.shirtColor;
            else
                return teamColors[(int) sw.Team];
        }
        
        public Color GetSWColor(Teams team)
        {
            return teamColors[(int) team];
        }
        
        public Sprite GetSWTeamSprite(Teams team)
        {
            return teamSprites[(int) team];
        }
        

        public void Initialize(GameModes selectedGameMode)
        {
            currentGameMode = selectedGameMode;

            switch (currentGameMode)
            {
                case GameModes.Classic:
                    playerCountInGame = 15;
                    break;
                case GameModes.TeamOfTwo:
                    playerCountInGame = 20;
                    
                    teams = new Teams[2];
                    
                    break;
                case GameModes.TeamOfThree:
                    playerCountInGame = 21;
                    
                    teams = new Teams[3];
                    break;
                case GameModes.TeamOfFour:
                    playerCountInGame = 20;
                    
                    teams = new Teams[4];
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            for (var i = 0; i < teams.Length; i++)
            {
                teams[i] = (Teams) i;
            }
            
            GameManager.Instance.StartGame(currentGameMode);
            UIManager.Instance.InitializeTeams();
        }
    }
}
