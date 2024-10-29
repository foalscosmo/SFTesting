using System.Collections;
using CandyCoded.HapticFeedback;
using Car;
using Managers;
using Particles;
using UnityEngine;

public class DestructibleObject : MonoBehaviour
{
    [Header("Prize")]
    public GameObject ObjectToSpawn;
    public int prizeCount;
    public ParticleType particleType = ParticleType.Destruction;
    public float respawnTime = 10f;
    
    private Collider _collider;
    private MeshRenderer _renderer;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
        _renderer = GetComponent<MeshRenderer>();
        this.tag = "Destructible";
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Nose"))
        {
            OnGetDestroyed();
        }
    }

    private void OnGetDestroyed()
    {
        ParticleManager.Instance.PlayParticle(this.transform.position,particleType);
        var percent = Random.Range(0, 100);

        if (percent < 40)
            GameManager.Instance.ThrowCoins(transform.position);

        StartCoroutine(StartReSpawn());
    }

    private IEnumerator StartReSpawn()
    {
        ActivateObject(false);
        yield return new WaitForSeconds(20);
        ActivateObject(true);
    }

    private void ActivateObject(bool activate)
    {
        _collider.enabled = _renderer.enabled = activate;
    }
}
