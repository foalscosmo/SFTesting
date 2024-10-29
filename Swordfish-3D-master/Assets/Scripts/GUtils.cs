using System;
using System.Collections;
using UnityEngine;

public class GUtils : MonoBehaviour
{
    private static GUtils _instance;

    private static GUtils _getGUtils
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameObject("GUtils").AddComponent<GUtils>();
            }

            return _instance;
        }
    }

    private IEnumerator WaitForSecondsIEnum(Action action, float timer)
    {
        yield return new WaitForSeconds(timer);
        action?.Invoke();
    }

    #region Static Functions

    public static void WaitForSeconds(Action action, float timer)
    {
        _getGUtils.StartCoroutine(_getGUtils.WaitForSecondsIEnum(action, timer));
    }

    #endregion
    
}
