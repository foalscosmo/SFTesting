using System.Collections.Generic;
using UnityEngine;

public class TESTTESTDELETEME : MonoBehaviour
{
    private void Start()
    {
        List<int> list = new List<int>(){3,15,1,0,2};
        
        LinkedList<int > linkd = new LinkedList<int>(list);
        
        foreach (var i in linkd)
        {
            Debug.Log(i);
        }
    }
}
