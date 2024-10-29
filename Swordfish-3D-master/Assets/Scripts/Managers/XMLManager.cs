using System.Collections;
using System.IO;
using System.Xml.Serialization;
using AI;
using NoMonoClasses;
using UnityEngine;

namespace Managers
{
    public class XMLManager : MonoBehaviour
    {
        public  static XMLManager instance;

        private string path;

        [HideInInspector]
        public bool savedDataFound;

        public SWDataBase database;

        void Awake()
        {
            // SINGLETONE
            instance = this;
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo( "en-US" );

            
            if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
            {
                path = Application.streamingAssetsPath + Path.DirectorySeparatorChar +  "item_data.xml";
            }
            else
            {
                //path = Path.Combine(Application.persistentDataPath, "MSF_data.xml");
                 path = Path.Combine(Application.persistentDataPath, "SW_Data");
                 //path = Path.Combine(path, "MSF_data.xml");
            }
        }

        private void Start()
        {
            LoadSavedData();
        }
        
        // Save Data To File
        public void SaveItem()
        {       
            var serializer = new XmlSerializer(typeof(SWDataBase));
            var stream = new FileStream(path, FileMode.Create);
            serializer.Serialize(stream, database);
            stream.Close();
            //Debug.Log("File Saved");
        }

        // Save Data
        public void SaveGameData()
        {
            database.allItems         = GameResourcesManager.instance.GetAllSWItems();
            database.playerData       = GameResourcesManager.instance.playerData;
            database.selectedGameMode = GameResourcesManager.instance.currentlySelectedGameMode;

            if (SW_AIStrengthManager.instance != null)
            {
                database.currentStrongAIPercentageClassic =
                    SW_AIStrengthManager.instance.currentStrongAIPercentageClassic;
                
                database.currentStrongAIPercentageTeamMode =
                    SW_AIStrengthManager.instance.currentStrongAIPercentageTeamMode;
            }
            
            //var newPlayerData = new PlayerData(database.playerData);
            
            SaveItem();
        }

        // Load Data
        public void LoadSavedData()
        {
            StartCoroutine(LoadSavedDataCoroutine());
        }

        public void DeleteSavedData()
        {
            File.Delete(path);
        }

        private Coroutine _waitForResponseCoroutine;

        private IEnumerator WaitForResponse(float time)
        {
            yield return new WaitForSeconds(time);
            InitManagers();
        }

        private IEnumerator LoadSavedDataCoroutine()
        {
            if (File.Exists(path))
            {
                var serializer = new XmlSerializer(typeof(SWDataBase));
                var stream = new FileStream(path, FileMode.Open);
                database = serializer.Deserialize(stream) as SWDataBase;
                stream.Close();
            }
            else
            {
                SaveItem();
            }
            
            yield return new WaitForEndOfFrame();
            
            MySceneManager.instance.LoadSceneAdditive(MyScenes.SW_Managers.ToString());
        }

        private void OnSceneLoadComplete(AsyncOperation obj)
        {
            InitManagers();
        }

        public void InitManagers(bool dataRetrievedCorrectly = false)
        {
            //MSFTimeManager.instance.Initialize(InitLoadedData);
            if (_waitForResponseCoroutine != null)
                StopCoroutine(_waitForResponseCoroutine);
            //database = Saver.GetCurrenetSave().mergeSFDB;
            if (database.isFirstLoad)
            {
                database.isFirstLoad = false;
            }
            else
            {
                savedDataFound = true;
            }
            //MSFGameManager.instance.Initialize(dataRetrievedCorrectly);
        }
    }
    
    [System.Serializable]
    public class SWDataBase
    {
        public PlayerData playerData = new PlayerData();

        public SWItem[] allItems;
        
        public string LastTimeOnline;

        public bool isFirstLoad = true;

        public bool soundOn = true, musicOn = true;

        public int highestScoreClassic;

        public GameModes selectedGameMode = GameModes.Classic;
        
        public float currentStrongAIPercentageClassic = 0;
        public float currentStrongAIPercentageTeamMode = 0;
    }

    [System.Serializable]
    public class SWItem
    {
        public string itemName;
        public Price price;
        public ItemAvalability avalability;
    }

    [System.Serializable]
    public enum ItemAvalability
    {
        IsBought,
        IsLocked
    }
    
    [System.Serializable]
    public enum ItemPriceType
    {
        Coin,
        Gem
    }

    [System.Serializable]
    public class Price
    {
        public ItemPriceType priceType;
        public int           amount;
    }
}