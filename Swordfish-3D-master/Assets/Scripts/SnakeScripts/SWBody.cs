using System;
using System.Collections;
using Car;
using UnityEngine;

namespace SnakeScripts
{
    public class SWBody : MonoBehaviour
    {
        public SWController Sw;
        public bool isHead = false;

        public void Kill()
        {
            Sw.GetHit(10000, Vector3.back);
        }
    }
}
