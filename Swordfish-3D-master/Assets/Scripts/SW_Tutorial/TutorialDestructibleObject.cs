using Managers;
using Particles;
using UnityEngine;

namespace SW_Tutorial
{
    public class TutorialDestructibleObject : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Nose"))
            {
                TutorialManager.instance.DestroyedObject();
            }
            
            ParticleManager.Instance.PlayParticle(this.transform.position, ParticleType.Destruction);
            
            GameManager.Instance.ThrowCoins(transform.position);
            
            GameObject.Destroy(this.gameObject);
        }
    }
}
