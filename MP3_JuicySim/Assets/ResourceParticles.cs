using UnityEngine;

/// <summary>
/// Attach to a GameObject with a Particle System.
/// Emission rate scales with the corresponding resource's generation rate.
/// </summary>
public class ResourceParticles : MonoBehaviour
{
    public enum ResourceType { Sunlight, Money }

    [Header("Settings")]
    public ResourceType resourceType = ResourceType.Sunlight;

    [Tooltip("Emission rate per unit of resource rate. e.g. 5 = 5 particles/sec per 1/sec rate.")]
    public float particlesPerRateUnit = 5f;

    [Tooltip("Maximum emission rate cap.")]
    public float maxEmissionRate = 100f;

    private ParticleSystem ps;

    void Start()
    {
        ps = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        if (ps == null || GameManager.instance == null) return;

        float rate = resourceType == ResourceType.Sunlight
            ? GameManager.instance.sunlightRate
            : GameManager.instance.moneyRate;

        var emission = ps.emission;
        emission.rateOverTime = Mathf.Min(rate * particlesPerRateUnit, maxEmissionRate);
    }
}
