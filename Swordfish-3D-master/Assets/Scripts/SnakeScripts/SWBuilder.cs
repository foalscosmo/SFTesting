using System.Collections.Generic;
using Car;
using UnityEngine;

namespace SnakeScripts
{
    public class SWBuilder
    {
        public static void BuildSnake(SwordFishParts swordFishParts, SWController swController)
        {
            var head = GameObject.Instantiate(swordFishParts.headPrefab,swController.transform).GetComponent<SWWeaponController>();
            head.isFront = true;
            head.Initialize(swordFishParts.frontNosePrefab,swController,0);
            swController.bodyParts.Add(head.transform);
            
            for (var i = 0; i < swordFishParts.bodyCount; i++)
            {
                swController.bodyParts.Add(GameObject.Instantiate(swordFishParts.bodyPrefab,swController.bodyHolder).transform);
            }
            
            var tail = GameObject.Instantiate(swordFishParts.tailPrefab,swController.bodyHolder).GetComponent<SWWeaponController>();
            tail.isFront = false;
            tail.Initialize(swordFishParts.backNosePrefab,swController,0);
            swController.bodyParts.Add(tail.transform);
        }
        
        public static void BuildTutorialSnake(SwordFishParts swordFishParts, SWController swController)
        {
            var head = GameObject.Instantiate(swordFishParts.headPrefab,swController.transform).GetComponent<SWWeaponController>();
            head.isFront = true;
            head.Initialize(swordFishParts.frontNosePrefab,swController,0);
            swController.bodyParts.Add(head.transform);
            
            for (var i = 0; i < swordFishParts.bodyCount; i++)
            {
                swController.bodyParts.Add(GameObject.Instantiate(swordFishParts.bodyPrefab,swController.bodyHolder).transform);
            }
            
            var tail = GameObject.Instantiate(swordFishParts.tailPrefab,swController.bodyHolder).GetComponent<SWWeaponController>();
            tail.isFront = false;
            tail.Initialize(swordFishParts.backNosePrefab,swController,0);
            swController.bodyParts.Add(tail.transform);
        }
        
        public static void BuildSnakeForShop(Transform parent, SwordFishParts swordFishParts)
        {
            var bodyPartMeshes = new List<MeshRenderer>();
            var _distanceBetweenPart = new List<float>();
            
            var head = GameObject.Instantiate(swordFishParts.headPrefab, parent).GetComponentsInChildren<SWWeaponController>();
            for (var i = 0; i < head.Length; i++)
            {
                head[i].Initialize(swordFishParts.frontNosePrefab,null,3);
            }
            
            bodyPartMeshes.Add(head[0].GetComponentInChildren<MeshRenderer>());
            
            for (var i = 0; i < swordFishParts.bodyCount; i++)
            {
                bodyPartMeshes.Add(GameObject.Instantiate(swordFishParts.bodyPrefab,parent).GetComponentInChildren<MeshRenderer>());
            }
            
            var tail = GameObject.Instantiate(swordFishParts.tailPrefab,parent).GetComponent<SWWeaponController>();
            tail.Initialize(swordFishParts.backNosePrefab,null,3);
            bodyPartMeshes.Add(tail.transform.GetComponentInChildren<MeshRenderer>());
            
            
            var multiplier = 0.8f;
            for (var i = 1; i < bodyPartMeshes.Count; i++)
            {
                if (i == bodyPartMeshes.Count - 1)
                    multiplier = 0.2f;

                if (i == 1) //is head
                {
                    _distanceBetweenPart.Add(bodyPartMeshes[0].bounds.size.z * 0.4f);
                }
                else //is body
                {
                    _distanceBetweenPart.Add(bodyPartMeshes[i].bounds.size.z * multiplier);
                }
            }

            var offset = 0.0f;
            for (var i = 1; i < bodyPartMeshes.Count; i++)
            {
                offset -= _distanceBetweenPart[i - 1];
                var pos = bodyPartMeshes[i].transform.position;
                pos.z += offset;
                bodyPartMeshes[i].transform.position = pos;
            }

            var allGameOjects = parent.GetComponentsInChildren<MeshRenderer>();
            foreach (var renderer in allGameOjects)
            {
                renderer.gameObject.layer = LayerMask.NameToLayer("RenderTexture");
            }
        }
    }
}
