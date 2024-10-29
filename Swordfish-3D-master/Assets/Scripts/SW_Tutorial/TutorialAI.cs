using Particles;
using UnityEngine;

namespace SW_Tutorial
{
    public class TutorialAI : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            Debug.Log(other.tag);
            if (other.CompareTag("Snake"))
            {
                TutorialManager.instance.GotKill();
            }
            
            ParticleManager.Instance.PlayParticle(this.transform.position, ParticleType.Destruction);
            
            GameObject.Destroy(this.transform.parent.gameObject);
        }
    }
}
