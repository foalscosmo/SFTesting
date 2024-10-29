using SnakeScripts;
//using UnityEditor;
using UnityEngine;

namespace MyEditor
{
    public class SWBuilderForEditor : MonoBehaviour
    {
        public SwordFishParts parts;
        
        public SwordFishParts[] manyParts;
        
        public void PrepareParts()
        {
            SwordFishParts swordFishParts = parts;
            SetTagAndLayer(swordFishParts.bodyPrefab, true);
            SetTagAndLayer(swordFishParts.headPrefab, true, true, true);
            SetTagAndLayer(swordFishParts.tailPrefab, true, true, false);
            SetTagAndLayer(swordFishParts.frontNosePrefab, false);
            SetTagAndLayer(swordFishParts.backNosePrefab, false);
        }
        
        private void PreparePart(SwordFishParts part)
        {
            SwordFishParts swordFishParts = part;
            
            SetTagAndLayer(swordFishParts.bodyPrefab, true);
            SetTagAndLayer(swordFishParts.headPrefab, true, true, true);
            SetTagAndLayer(swordFishParts.tailPrefab, true, true, false);
            SetTagAndLayer(swordFishParts.frontNosePrefab, false);
            SetTagAndLayer(swordFishParts.backNosePrefab, false);
        }

        public void PrepareAllParts()
        {
            for (var i = 0; i < manyParts.Length; i++)
            {
                PreparePart(manyParts[i]);
            }
        }

        public string name;
        public void TakeScreenShot()
        {
            ScreenCapture.CaptureScreenshot(name);
        }

        public void SetTagAndLayer(GameObject prefab, bool isBody, bool holdsWeapons = false, bool isFront = true)
        {
            var newObj = GameObject.Instantiate(prefab);

            var col = newObj.GetComponent<BoxCollider>();
            if(col == null)
                col = newObj.AddComponent<BoxCollider>();
            
            col.isTrigger = true;

            if (isBody)
            {
                if (newObj.GetComponent<SWBody>() == null)
                    newObj.AddComponent<SWBody>();
                
                newObj.tag = "Snake";
                newObj.layer = LayerMask.NameToLayer("Snake");
            }
            else
            {
                if (newObj.GetComponent<SWWeapon>() == null)
                    newObj.AddComponent<SWWeapon>();

                newObj.tag = "Nose";
            }

            if (holdsWeapons)
            {
                var controller = newObj.GetComponent<SWWeaponController>();
                
                if (controller == null)
                    controller = newObj.AddComponent<SWWeaponController>();
                
                controller.isFront = isFront;

                if (controller.weaponStartingPosition == null)
                {
                    if (isFront)
                    {
                        controller.weaponStartingPosition = new GameObject("NoseHolder").transform;
                        controller.weaponStartingPosition.SetParent(controller.transform);
                        var vec = new Vector3(0,0,0.97f);
                        controller.weaponStartingPosition.position = vec;
                    }
                    else
                    {
                        if (newObj.transform.childCount > 0)
                        {
                            controller.weaponStartingPosition = newObj.transform.GetChild(0);
                        }
                        else
                        {
                            controller.weaponStartingPosition = newObj.transform;
                        }
                    }
                }
            }

            newObj.transform.position = Vector3.zero;

            //PrefabUtility.ReplacePrefab(newObj, prefab, ReplacePrefabOptions.ConnectToPrefab);
        }
    }
}
