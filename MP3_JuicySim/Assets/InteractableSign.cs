using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// Attach to a sign Canvas near an interactable.
/// Shows current cost and effect dynamically.
/// Assign either a GeneratorDeployer OR a PowerupPurchaser — not both.
/// </summary>
public class InteractableSign : MonoBehaviour
{
    [Header("Text")]
    public TextMeshProUGUI signText;

    [Header("Assign one of these:")]
    public GeneratorDeployer generator;
    public PowerupPurchaser powerup;

    [Header("Optional:")]
    public CooldownTimer cooldown;

    public void PlayBounce()
    {
        if (signText != null)
            StartCoroutine(BounceText());
    }

    IEnumerator BounceText()
    {
        Vector3 original = signText.transform.localScale;
        Vector3 big = original * 1.5f;
        float t = 0f;
        while (t < 1f) { t += Time.deltaTime / 0.15f; signText.transform.localScale = Vector3.Lerp(original, big, t); yield return null; }
        t = 0f;
        while (t < 1f) { t += Time.deltaTime / 0.2f; signText.transform.localScale = Vector3.Lerp(big, original, t); yield return null; }
        signText.transform.localScale = original;
    }

    void Update()
    {
        if (signText == null) return;

        string cooldownLine = "";
        if (cooldown != null && cooldown.EverUsed)
            cooldownLine = cooldown.IsReady ? "\nReady!" : $"\nReady in {Mathf.CeilToInt(cooldown.TimeRemaining)}s";

        if (generator != null)
        {
            signText.text =
                $"Generator #{generator.DeployedCount + 1}\n" +
                $"Cost: {generator.NextCost:F0} {generator.costResource}\n" +
                $"Effect: +{generator.rateBoostPerGenerator}/sec {generator.boostResource}" +
                cooldownLine;
        }
        else if (powerup != null)
        {
            signText.text =
                $"Power-up #{powerup.PurchasedCount + 1}\n" +
                $"Cost: {powerup.sunlightCost:F0} Sunlight + {powerup.moneyCost:F0} Money\n" +
                $"Effect: x{powerup.rateMultiplier} all rates" +
                cooldownLine;
        }
    }
}
