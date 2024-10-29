using Managers;
using UnityEngine;

namespace SnakeScripts
{
    [CreateAssetMenu(fileName = "SwordFishParts", menuName = "SwordFishes/SwordFishParts", order = 1)]
    public class SwordFishParts : ScriptableObject
    {
        public GameObject headPrefab;
        public GameObject bodyPrefab;
        public GameObject tailPrefab;
        public GameObject frontNosePrefab;
        public GameObject backNosePrefab;

        public int bodyCount;

        public int swordFishIndex;

        public Sprite icon;

        public SWItem swItem;
    }
}