using System;
using System.Collections;
using Managers;
#if UNITY_IOS && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Io.Helpers
{
    public class InternetStatusManager : MonoBehaviour
    {
        public static event Action<InternetStatus> OnStatusChanged = delegate { };

        public static InternetStatusManager Instance
        {
            get
            {
                if (instance == null)
                    instance = (new GameObject("InternetManager")).AddComponent<InternetStatusManager>();

                return instance;
            }
        }

        private static InternetStatusManager instance;
        private static bool isAnyConnectionWay = true;

        public static event Action<float> BeforeSpeedUp = delegate { };
        public static event Action<float> AfterSpeedUp = delegate { };

        public static InternetStatus CurrentStatus { get; protected set; }
        public static bool IsSpeedUp { get; protected set; }

        public static bool IsAnyConnectionWay
        {
            get { return isAnyConnectionWay; }
            protected set { isAnyConnectionWay = value; }
        }

        private Coroutine speedUpCorutine;
        private float timeWhenInternetStopWorking;

        private const float PingTestTime = 30;
        private const float PingAnotherTestTime = 2;

        private int currentIp = 0;
        private string ipAdress = "2.18.233.62";
        private Coroutine pingWorker;

#if UNITY_IOS && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern bool _startNotifier(string gameObject, string methd);
#endif

#if UNITY_ANDROID
        private const string anyConnectionString = "Connected";
#endif

        protected void Awake()
        {
            ipAdress = ipAdresses[currentIp];

            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);
            
            CurrentStatus = InternetStatus.Connected;

            InitMessages();

            pingWorker = StartCoroutine(PingWorker());
        }

        public void OnInternetStateChanges(string currentState)
        {
            Debug.Log("New state - " + currentState);

#if UNITY_ANDROID
            if (currentState != anyConnectionString)
            {
                IsAnyConnectionWay = false;
                OnStatusChanged(InternetStatus.Disconected);
            }
            else
            {
                IsAnyConnectionWay = true;
                    if (pingWorker != null)
                        StopCoroutine(pingWorker);
                    pingWorker = StartCoroutine(PingWorker());
            }
#endif

#if UNITY_IOS

            switch (currentState)
            {
                case "NotReachable":
                    IsAnyConnectionWay = false;
                    OnStatusChanged(InternetStatus.Disconected);
                    break;
                default:
                    IsAnyConnectionWay = true;
                    if (pingWorker != null)
                        StopCoroutine(pingWorker);
                    pingWorker = StartCoroutine(PingWorker());
                    break;
            }
#endif
        }

        public static void StartSpeedUp(float forTime)
        {
            if (Instance == null)
                return;
            
            if(SceneManager.GetActiveScene().name == "MSFMainScene")
                return;

            Time.timeScale = 1;
            if (Instance.speedUpCorutine != null)
            {
                IsSpeedUp = false;
                Instance.StopCoroutine(Instance.speedUpCorutine);
            }

            Instance.speedUpCorutine = Instance.StartCoroutine(Instance.SpeedUp(forTime));
        }

        public static void StopSpeedUp()
        {
            if(Instance == null)
                return;

            if (Instance.speedUpCorutine != null)
            {
                IsSpeedUp = false;
                Instance.StopCoroutine(Instance.speedUpCorutine);
            }

            Time.timeScale = 1;
            AfterSpeedUp(0);
        }

        public void OnStatusChangedSpeedUp(InternetStatus internetStatus)
        {
            if (internetStatus == InternetStatus.Connected)
            {
                if (speedUpCorutine != null)
                    StopCoroutine(speedUpCorutine);

                speedUpCorutine = StartCoroutine(SpeedUp(Time.realtimeSinceStartup - timeWhenInternetStopWorking));
                GameResourcesManager.instance.ShowLoadingPanel(false);
            }
            else
            {
                timeWhenInternetStopWorking = Time.realtimeSinceStartup;
                Time.timeScale = 0;

                MySceneManager.instance.LoadSceneAdditiveAfterUnloading(MyScenes.SW_Menu.ToString(),SceneManager.GetActiveScene().name);
            }
        }

        private void InitMessages()
        {
#if UNITY_IOS && !UNITY_EDITOR
                _startNotifier(gameObject.name, "OnInternetStateChanges");
#elif UNITY_ANDROID && !UNITY_EDITOR
                var androidJc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                var jo = androidJc.GetStatic<AndroidJavaObject>("currentActivity");
                var context = jo.Call<AndroidJavaObject>("getApplicationContext");

                AndroidJavaClass clas = new AndroidJavaClass("com.internetmanager.ConnectivityChangeReceiver");
                clas.CallStatic("initialize", context);
                clas.CallStatic("register", gameObject.name, "OnInternetStateChanges");
#else
            OnInternetStateChanges("Connected");
#endif
        }

        private void ChangeStatus(InternetStatus newStatus)
        {
            if (CurrentStatus != newStatus)
            {
                CurrentStatus = newStatus;
                OnStatusChanged(CurrentStatus);
            }
        }

        private IEnumerator SpeedUp(float forTime)
        {
            Time.timeScale = 1;

            if (forTime <= 1)
                yield break;

            if (forTime > 100)
                forTime = 100;

            if (SceneManager.GetActiveScene().buildIndex == 1 || SceneManager.GetActiveScene().name == "OfflineArena")
                yield break;

            BeforeSpeedUp(forTime);

            IsSpeedUp = true;
            Time.maximumDeltaTime = 0.1111f;
            yield return null;

            Time.timeScale = 100;

            yield return new WaitForSecondsRealtime(forTime / 100f);
            Time.timeScale = 1;

            yield return null;

            Time.maximumDeltaTime = 0.33333f;
            IsSpeedUp = false;

            AfterSpeedUp(forTime);

            speedUpCorutine = null;
        }

        private IEnumerator PingWorker()
        {
/*#if UNITY_EDITOR
            yield break;
#endif*/
            int failCount = 0;

            while (IsAnyConnectionWay)
            {
                float testTime = PingTestTime;
                var ping = new Ping(ipAdress);

                float startPingTime = Time.realtimeSinceStartup;

                while (!ping.isDone)
                {
                    yield return null;
                    if (Time.realtimeSinceStartup - startPingTime >= 10)
                        break;
                }

                if (!ping.isDone || ping.time < 1)
                {
                    Debug.Log("PING Fail " + failCount);
                    failCount++;
                    testTime = PingAnotherTestTime;
                    if (failCount >= 2)
                    {
                        currentIp++;
                        if (currentIp >= ipAdresses.Length)
                            currentIp = 0;
                        ipAdress = ipAdresses[currentIp];
                    }

                    if (failCount >= 3)
                    {
                        ChangeStatus(InternetStatus.Disconected);
                    }
                }
                else
                {
                    ChangeStatus(InternetStatus.Connected);
                    failCount = 0;
                }

                ping.DestroyPing();

                yield return new WaitForSecondsRealtime(testTime);
            }
        }

        public enum InternetStatus
        {
            Connected,
            Disconected
        }

        private string[] ipAdresses = new string[]
        {
            "4.2.2.3",
            "2.18.233.62",
            "8.8.8.8",
            "4.2.2.4",
            "1.1.1.1",
            "8.8.4.4",
            "139.130.4.5",
            "4.2.2.2",
            "141.1.1.1",
            "4.2.2.1",
        };
        
        #if UNITY_EDITOR

        private void Update()
        {
            string nextConnection = string.Empty;
            
#if UNITY_ANDROID
            if (CurrentStatus != InternetStatus.Connected)
                nextConnection = anyConnectionString;
#endif

#if UNITY_IOS
            if (CurrentStatus == InternetStatus.Connected)
                nextConnection = "NotReachable";
#endif
            if (Input.GetKeyDown(KeyCode.I))
            {
                OnInternetStateChanges(nextConnection);
                ChangeStatus(CurrentStatus == InternetStatus.Connected ? InternetStatus.Disconected : InternetStatus.Connected);
            }

            if (Input.GetKeyDown(KeyCode.P))
            {
                Debug.Break();
            }
        }
        
        #endif
    }
}
