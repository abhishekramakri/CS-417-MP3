using UnityEngine;
using System.Collections;
using DG.Tweening;

/// <summary>
/// Planting Generators (3pts):
/// Attach to an XR Simple Interactable (e.g. a seed packet or pot).
/// Wire OnDeploy() to the Select Entered or Activated event in the Inspector.
/// Each activation costs resources and permanently adds a flat rate boost to GameManager.
/// Also covers Exponential Costs (1pt bonus): each new generator costs more than the last.
/// </summary>
public class GeneratorDeployer : MonoBehaviour
{
    public enum ResourceType { Sunlight, Money }

    [Header("Generator Settings")]
    public ResourceType costResource = ResourceType.Sunlight;
    public float baseCost = 10f;

    public ResourceType boostResource = ResourceType.Sunlight;
    public float rateBoostPerGenerator = 1f;

    [Header("Cooldown")]
    public CooldownTimer cooldown;

    [Header("Ease Animation")]
    [Tooltip("The visual object to animate on deploy (can be this object or a child mesh).")]
    public Transform animTarget;
    [Tooltip("The sign next to this generator — its text will bounce on deploy.")]
    public InteractableSign sign;

    [Header("Feedback")]
    public bool logMessages = true;

    [Tooltip("Unique key for PlayerPrefs save (must be different for each generator in the scene).")]
    public string saveKey = "generator_default";

    private int deployedCount = 0;

    void Start()
    {
        deployedCount = PlayerPrefs.GetInt(saveKey + "_count", 0);
    }

    void OnApplicationQuit()
    {
        PlayerPrefs.SetInt(saveKey + "_count", deployedCount);
        PlayerPrefs.Save();
    }

    public void OnDeploy()
    {
        if (cooldown != null && !cooldown.IsReady)
        {
            if (logMessages) Debug.Log("[Generator] On cooldown.");
            return;
        }

        // Exponential Costs: cost multiplies by 1.5 each time
        float actualCost = baseCost * Mathf.Pow(1.5f, deployedCount);

        float current = costResource == ResourceType.Sunlight
            ? GameManager.instance.sunlight
            : GameManager.instance.money;

        if (current < actualCost)
        {
            if (logMessages) Debug.Log("[Generator] Not enough " + costResource + ". Need " + actualCost.ToString("F0"));
            return;
        }

        if (costResource == ResourceType.Sunlight)
            GameManager.instance.sunlight -= actualCost;
        else
            GameManager.instance.money -= actualCost;

        if (boostResource == ResourceType.Sunlight)
            GameManager.instance.sunlightRate += rateBoostPerGenerator;
        else
            GameManager.instance.moneyRate += rateBoostPerGenerator;

        deployedCount++;
        if (logMessages) Debug.Log("[Generator] Deployed #" + deployedCount + ". Next cost: " + (baseCost * Mathf.Pow(1.5f, deployedCount)).ToString("F0"));
        if (cooldown != null) cooldown.StartCooldown();
        PlayDeployAnimation();
        if (sign != null) sign.PlayBounce();
    }

    void PlayDeployAnimation()
    {
        Transform target = animTarget != null ? animTarget : transform;
        Debug.Log("[Generator] Playing animation on: " + target.name + " scale: " + target.localScale);
        StartCoroutine(ScaleBounce(target));
    }

    IEnumerator ScaleBounce(Transform target)
    {
        Vector3 original = target.localScale;
        Vector3 big = original * 2f;
        float duration = 0.4f;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            target.localScale = Vector3.Lerp(original, big, t);
            Debug.Log("[Anim] t=" + t.ToString("F2") + " scale=" + target.localScale);
            yield return null;
        }

        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / 0.3f;
            target.localScale = Vector3.Lerp(big, original, t);
            yield return null;
        }

        target.localScale = original;
    }

    public int DeployedCount => deployedCount;
    public float NextCost => baseCost * Mathf.Pow(1.5f, deployedCount);
}
