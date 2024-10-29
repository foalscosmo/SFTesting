using System;
using System.Collections;
using Car;
using DG.Tweening;
using Managers;
using SnakeScripts;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public enum SelectionPanelTypes
    {
        PanelMainMenu,
        PanelCharacters,
        PanelBoosts,
        PanelCoins,
        PanelModes
    }
    
    public class MenuManager : MonoBehaviour
    {
        public static MenuManager instance;
        
        [Header("MainMenu")]
        public GameObject mainMenuPanel;
        public Sprite[] priceIcons;
        public Text coinText, gemText;
        public Image selectedCharacterImage;
        public RectTransform mainIconRect;
        public Vector2 mainMenuIconSize;
        public Vector2 shopIconSize;
        
        [Header("Shop")]
        public GameObject          shopPanel;
        public GameObject          shopItemPrefab;
        public RectTransform       shopGridLayoutRect;
        public RectTransform       shopGridLayoutRect2;
        public RectTransform       shopContentRect;
        public GameObject          shopSelectButton;
        public GameObject          shopUnlockButton;
        public Text                shopUnlockPrice;
        public Image               shopUnlockPriceIcon;
        public ShopItem            shopSelectedShopItem;
        public RectTransform       mainIconShopPosition;
        public RectTransform       mainIconMenuPosition;
        
        [Header("PowerUps")]
        public GameObject powerUpPanel;

        [Header("SelectionPanel")]    
        public SelectionPanelTypes    currentlySelectedPanel;
        public RectTransform          selectionPanel;
        public Button[]               selectionPanelButtons;
        public Text[]                 _selectionPanelButtonTexts;
        public RectTransform[]        selectionPanelPositions;
        public Sprite                 selectionPanelSelectedSprite;
        public Sprite                 selectionPanelNormalSprite;
        public GameObject             exitButton;
        public Color                  selectionPanelTextActiveColor;
        public GameObject selectionPanelMainMenu;
        public GameObject selectionPanelOn;

        [Header("SelectionGameMode")] 
        public GameObject selectionGameModeGameObj;
        public GameObject selectionGameModeClassic;
        public GameObject selectionGameModeTeam;
        public Text selectionGameModeMainText;
        public Text selectionGameModeTeamVSText;
        
        [Header("LevelProgress")]    
        public GameObject             levelProgressPanel;
        public Text                   levelProgressPlayerName;
        public Text                   levelProgressCurrentExp;
        public Text                   levelProgressExpNeeded;
        public Text                   levelProgressCurrentLevel;
        public Image                  levelProgressFiller;

        [Header("Game Modes")] 
        public GameObject gameModesPanel;
        public GameObject gameModesTeamModesPanel;
        public Animator[] teamButtonAnims;
        public Text[]     _teamButtonTexts = null;
        public Image[]    teamModeImages;
        public Color      deactivatedTeamModeColor, activeTeamModeColor;
        public Text       teamModeFinalText;

        private Animator _selectedTeamModeAnimation;
        

        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            Time.timeScale = 1f;
            
            selectionPanel.parent.gameObject.SetActive(true);
            shopPanel.gameObject.SetActive(false);
            powerUpPanel.gameObject.SetActive(false);

            _selectionPanelButtonTexts = new Text[selectionPanelButtons.Length];
            for (var i = 0; i < _selectionPanelButtonTexts.Length; i++)
            {
                _selectionPanelButtonTexts[i] = selectionPanelButtons[i].GetComponentInChildren<Text>(true);
            }
            
            PrepareSelectedCharacter(GameResourcesManager.instance.currentlySelectedCharacter);
            UpdateResourceTexts();
            OnPanelChange(SelectionPanelTypes.PanelMainMenu);
            ShowHideLevelProgress(true);
        }

        public void UpdateResourceTexts()
        {
            coinText.text = GameResourcesManager.instance.playerData.coins.ToString();
            gemText.text  = GameResourcesManager.instance.playerData.gems .ToString();
        }

        #region Game Modes Panel
        
        public void GoToGameModes(bool open)
        {
            if (_teamButtonTexts == null || _teamButtonTexts.Length == 0)
            {
                _teamButtonTexts = new Text[teamButtonAnims.Length];
                
                for (var i = 0; i < _teamButtonTexts.Length; i++)
                {
                    _teamButtonTexts[i] = teamButtonAnims[i].GetComponentInChildren<Text>(true);
                }
            }
            
            if (GameResourcesManager.instance.currentlySelectedGameMode != GameModes.Classic)
                _selectedTeamModeAnimation = teamButtonAnims[(int) GameResourcesManager.instance.currentlySelectedGameMode - 1];
            
            if (currentlySelectedPanel == SelectionPanelTypes.PanelModes && open)
            {
                open = false;
                OnPanelChange(SelectionPanelTypes.PanelMainMenu);
            }
            
            gameModesPanel.SetActive(open);
            if (open)
            {
                OnPanelChange(SelectionPanelTypes.PanelModes);
                if (GameResourcesManager.instance.currentlySelectedGameMode != GameModes.Classic)
                {
                    if (GameResourcesManager.instance.currentlySelectedGameMode == GameModes.Classic)
                    {
                        teamModeImages[0].color = activeTeamModeColor;
                    }
                    else
                    {
                        teamModeImages[1].color = activeTeamModeColor;
                        teamModeFinalText.text = ((int)GameResourcesManager.instance.currentlySelectedGameMode + 1).ToString();
                        
                        _teamButtonTexts[(int) GameResourcesManager.instance.currentlySelectedGameMode - 1].color =
                            activeTeamModeColor;
                    }
                }
            }
        }

        public void ShowTeamModesPanel(bool show)
        {
            for (var i = 0; i < teamButtonAnims.Length; i++)
            {
                if (GameResourcesManager.instance.currentlySelectedGameMode != GameModes.Classic  && _selectedTeamModeAnimation != teamButtonAnims[i])
                {
                    _teamButtonTexts[i].color = deactivatedTeamModeColor;
                }
                teamButtonAnims[i].Play("InstantHide");
            }

            gameModesTeamModesPanel.SetActive(show);
            if(GameResourcesManager.instance.currentlySelectedGameMode != GameModes.Classic && _selectedTeamModeAnimation != null && show)
                _selectedTeamModeAnimation.Play("Appear_Animation");
        }

        public void SelectTeamMode(int teamMode)
        {
            if (!_canPress) return;
            teamButtonAnims[teamMode].Play("Appear_Animation");
            
            for (var i = 0; i < _teamButtonTexts.Length; i++)
            {
                _teamButtonTexts[i].color = deactivatedTeamModeColor;
            }
            
            _teamButtonTexts[teamMode].color = activeTeamModeColor;
            
            if(_selectedTeamModeAnimation != null)
                _selectedTeamModeAnimation.Play("Hide_Animation");

            _selectedTeamModeAnimation = teamButtonAnims[teamMode];
            StartCoroutine(WaitForSelection(teamMode + 1));
        }

        private bool _canPress = true;
        
        private IEnumerator WaitForSelection(int gameMode)
        {
            _canPress = false;
            yield return new WaitForSeconds(0.5f);
            SelectGameMode(gameMode);
            _canPress = true;
        }

        public void SelectGameMode(int gameMode)
        {
            for (var i = 0; i < teamModeImages.Length; i++)
            {
                teamModeImages[i].color = deactivatedTeamModeColor;
                _teamButtonTexts[i].color = deactivatedTeamModeColor;
            }
            
            //TODO: PLAY SOUND
            ShowTeamModesPanel(false);
            GameResourcesManager.instance.currentlySelectedGameMode = (GameModes) gameMode;
            if (GameResourcesManager.instance.currentlySelectedGameMode == GameModes.Classic)
            {
                teamModeImages[0].color = activeTeamModeColor;
                _selectedTeamModeAnimation = null;
            }
            else
            {
                teamModeImages[1].color = activeTeamModeColor;
                teamModeFinalText.text = (gameMode + 1).ToString();
                _teamButtonTexts[gameMode - 1].color = activeTeamModeColor;
            }
            
            ExitPanels();
        }

        #endregion

        #region Level Progress

        public void UpdateLevelProgress()
        {
            var playerData = GameResourcesManager.instance.playerData;
            
            levelProgressCurrentExp.text     = playerData.currentExp.ToString();
            levelProgressExpNeeded.text      = "/"+playerData.expToNextLevel;
            levelProgressFiller.fillAmount   = (float)playerData.currentExp / (float)playerData.expToNextLevel;
            levelProgressPlayerName.text     = playerData.playerName;
            levelProgressCurrentLevel.text   = playerData.currentLevel.ToString();
        }

        public void ShowHideLevelProgress(bool show)
        {
            levelProgressPanel.SetActive(show);
            UpdateLevelProgress();
        }

        #endregion

        #region Selection Panel

        private void OnPanelChange(SelectionPanelTypes panelType)
        {
            currentlySelectedPanel = panelType;
            

            exitButton.SetActive(currentlySelectedPanel != SelectionPanelTypes.PanelMainMenu);

            //var newPos = SwitchToRectTransform(selectionPanel, selectionPanelPositions[(int) panelType]);
            //selectionPanel.DOAnchorPos(newPos, 0.1f);

            var prefAnchorsMin = selectionPanel.anchorMin;
            var prefAnchorsMax = selectionPanel.anchorMax;
            selectionPanel.anchorMin = selectionPanelPositions[(int) panelType].anchorMin;
            selectionPanel.anchorMax = selectionPanelPositions[(int) panelType].anchorMax;

            var timer = 0f;
            if (selectionPanel.anchorMin == prefAnchorsMin && selectionPanel.anchorMax == prefAnchorsMax)
                timer = 0.1f;

            selectionPanel.DOAnchorPos(selectionPanelPositions[(int) panelType].anchoredPosition, timer);// = selectionPanelPositions[(int) panelType].anchoredPosition;

            for (var i = 1; i < selectionPanelButtons.Length; i++)
            {
                selectionPanelButtons[i].image.sprite = selectionPanelNormalSprite;
                _selectionPanelButtonTexts[i].color = Color.white;
            }

            if (panelType != SelectionPanelTypes.PanelMainMenu)
            {
                selectionGameModeGameObj.SetActive(false);
                _selectionPanelButtonTexts[(int) panelType].color = selectionPanelTextActiveColor;
                selectionPanelButtons[(int) panelType].image.sprite = selectionPanelSelectedSprite;
                selectionPanelOn.SetActive(true);
                selectionPanelMainMenu.SetActive(false);
                
                
                
                if (currentlySelectedPanel == SelectionPanelTypes.PanelCharacters)
                {
                    mainIconRect.anchorMin = mainIconShopPosition.anchorMin;
                    mainIconRect.anchorMax = mainIconShopPosition.anchorMax;
                
                    mainIconRect.DOAnchorPos(mainIconShopPosition.anchoredPosition,0.02f);

                    //selectedCharacterImage.GetComponent<RectTransform>().sizeDelta = shopIconSize;
                    if (Math.Abs(selectedCharacterImage.transform.localScale.x - Vector3.one.x) < 0.05f)
                        selectedCharacterImage.transform.localScale =
                            selectedCharacterImage.transform.localScale * shopIconSize.magnitude /
                            mainMenuIconSize.magnitude;
                } 
            }
            else
            {
                selectionPanelOn.SetActive(false);
                selectionPanelMainMenu.SetActive(true);
                selectionGameModeGameObj.SetActive(true);
                
                var isTeamMode = GameResourcesManager.instance.currentlySelectedGameMode != GameModes.Classic;
                
                selectionGameModeTeam.SetActive(isTeamMode);
                selectionGameModeClassic.SetActive(!isTeamMode);
                
                //selectedCharacterImage.GetComponent<RectTransform>().sizeDelta = mainMenuIconSize;
                selectedCharacterImage.transform.localScale = Vector2.one;
                
                if (isTeamMode)
                {
                    selectionGameModeTeamVSText.text =
                        ((int) GameResourcesManager.instance.currentlySelectedGameMode + 1).ToString();
                    selectionGameModeMainText.text = "TEAM BATTLE";
                }
                else
                {
                    selectionGameModeMainText.text = "CLASSIC";
                }
                
                mainIconRect.anchorMin = mainIconMenuPosition.anchorMin;
                mainIconRect.anchorMax = mainIconMenuPosition.anchorMax;
                
                mainIconRect.DOAnchorPos(mainIconMenuPosition.anchoredPosition,0.05f);
            }
        }

        public void ExitPanels()
        {
            GoToShop(false);
            GoToGameModes(false);
            GoToPowerUps(false);
            OnPanelChange(SelectionPanelTypes.PanelMainMenu);
            exitButton.SetActive(false);
        }
        
        private Vector2 SwitchToRectTransform(RectTransform from, RectTransform to)
        {
            Vector2 localPoint;
            Vector2 fromPivotDerivedOffset = new Vector2(from.rect.width * from.pivot.x + from.rect.xMin, from.rect.height * from.pivot.y + from.rect.yMin);
            Vector2 screenP = RectTransformUtility.WorldToScreenPoint(null, from.position);
            screenP += fromPivotDerivedOffset;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(to, screenP, null, out localPoint);
            Vector2 pivotDerivedOffset = new Vector2(to.rect.width * to.pivot.x + to.rect.xMin, to.rect.height * to.pivot.y + to.rect.yMin);
            return to.anchoredPosition + localPoint - pivotDerivedOffset;
        }

        #endregion

        #region Shop

        public void NewShopItemIsSelected(ShopItem item)
        {
            if(shopSelectedShopItem != null)
                shopSelectedShopItem.selectionImage.gameObject.SetActive(false);

            shopSelectedShopItem = item;
            shopSelectedShopItem.selectionImage.gameObject.SetActive(true);
        }
        
        public void InitializeShop()
        {
            if(shopGridLayoutRect.childCount > 0)
                return;
            
            for (var i = 0; i < GameResourcesManager.instance.allSwordFishSOs.Length; i++)
            {
                var parent = i % 2 == 0 ? shopGridLayoutRect : shopGridLayoutRect2;
                var shopItem = GameObject.Instantiate(shopItemPrefab,parent).GetComponent<ShopItem>();
                shopItem.Initialize(GameResourcesManager.instance.allSwordFishSOs[i], 0 == string.Compare(GameResourcesManager.instance.allSwordFishSOs[i].swItem.itemName, 
                    GameResourcesManager.instance.currentlySelectedCharacter.swItem.itemName, StringComparison.Ordinal));
            }
            
            // RESIZE AND REPOSITION
            GridLayoutResizer(shopGridLayoutRect.gameObject,1);
            GridLayoutResizer(shopGridLayoutRect2.gameObject,1);
            //var offset = 2224 - Screen.width;
            shopContentRect.sizeDelta = new Vector2(shopContentRect.sizeDelta.x, shopGridLayoutRect.rect.height + 300);
        }
        
        private void GridLayoutResizer(GameObject go, int columnCount)
        {
            GridLayoutGroup gridLayoutGroup;
            RectTransform rectTransform;

            int rowCount;

            gridLayoutGroup = go.GetComponent<GridLayoutGroup>();
            rectTransform = go.GetComponent<RectTransform>();
            
            rowCount = Mathf.CeilToInt((float)go.transform.childCount / (float)columnCount);
            

            var hight = gridLayoutGroup.cellSize.y * (rowCount) + gridLayoutGroup.spacing.y * (rowCount);
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x,hight);
        }

        public void GoToShop(bool isEntering)
        {
            if (currentlySelectedPanel == SelectionPanelTypes.PanelCharacters && isEntering)
            {
                isEntering = false;
                OnPanelChange(SelectionPanelTypes.PanelMainMenu);
            }
            
            var canvas = shopPanel.transform.GetComponent<Canvas>();
            if (isEntering)
            {
                if (canvas) canvas.enabled = true;
                InitializeShop();
                StartCoroutine(ShopScrollTo(1f));
                OnPanelChange(SelectionPanelTypes.PanelCharacters);
            }
            else
            {
                GameResourcesManager.instance.ResetCharacter();
                if (canvas) canvas.enabled = false;
            }
            
            // coinsCount.transform.parent.gameObject.SetActive(isEntering);
            // coinsCount.text = MSFint.GetAbbriviature(MSFGameManager.instance.playerData.coins);
            shopPanel.SetActive(isEntering);
        }

        private IEnumerator ShopScrollTo(float val)
        {
            yield return new WaitForEndOfFrame();
            shopPanel.GetComponentInChildren<ScrollRect>(true).verticalNormalizedPosition = val;
        }

        public void Select()
        {
            GameResourcesManager.instance.FinalizeSelect();
            CheckItemAvailability(GameResourcesManager.instance.currentlySelectedCharacter);
            ExitPanels();
        }
        
        public void UnlockCharacter()
        {
            var bought = GameResourcesManager.instance.BuyItem(GameResourcesManager.instance.temporarilySelectedCharacter);

            if (bought)
            {
                GameResourcesManager.instance.FinalizeSelect();
                UpdateResourceTexts();
                
                CheckItemAvailability(GameResourcesManager.instance.temporarilySelectedCharacter);
                shopSelectedShopItem.Initialize(GameResourcesManager.instance.temporarilySelectedCharacter, true);
                NewShopItemIsSelected(shopSelectedShopItem);
                
                ExitPanels();
            }
            else
            {
                Debug.Log("Not enough resources");
            }
        }

        public void CheckItemAvailability(SwordFishParts parts)
        {
            var select = parts.swItem.avalability == ItemAvalability.IsBought;
            
            shopSelectButton.SetActive(select);
            shopUnlockButton.SetActive(!select);
            
            if(GameResourcesManager.instance.currentlySelectedCharacter == parts)
                shopSelectButton.SetActive(false);
            
            shopUnlockPrice.text = parts.swItem.price.amount.ToString();
            shopUnlockPriceIcon.sprite = priceIcons[(int) parts.swItem.price.priceType];
        }

        #endregion

        #region Power Ups

        public void GoToPowerUps(bool open)
        {
            if (currentlySelectedPanel == SelectionPanelTypes.PanelBoosts && open)
            {
                open = false;
                OnPanelChange(SelectionPanelTypes.PanelMainMenu);
            }
            
            powerUpPanel.SetActive(open);
            if (open)
                OnPanelChange(SelectionPanelTypes.PanelBoosts);
        }

        #endregion


        public void OnAdPressed()
        {
            Application.OpenURL("https://play.google.com/store/apps/details?id=com.appidea.pixelswordfishio&hl=en");
        }

        public void StartGame()
        {
            //_shopCharacterGameObject.SetActive(false);
            OnPanelChange(SelectionPanelTypes.PanelMainMenu);

            if (!PlayerPrefs.HasKey("Tutorial"))
            {
                LoadTutorial();
            }
            else
            {
                MySceneManager.instance.LoadSceneAdditiveAfterUnloading(MyScenes.SW_ArenaScene.ToString(),
                    MyScenes.SW_Menu.ToString());
            }
        }

        public void LoadTutorial()
        {
            MySceneManager.instance.LoadSceneAdditiveAfterUnloading(MyScenes.SW_Tutorial.ToString(),
                MyScenes.SW_Menu.ToString());
        }

        private GameObject _shopCharacterGameObject;

        public void PrepareSelectedCharacter(SwordFishParts parts)
        {
            selectedCharacterImage.sprite = parts.icon;
        }
    }
}
