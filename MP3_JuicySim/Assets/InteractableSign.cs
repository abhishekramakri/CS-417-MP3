using UnityEngine;
using TMPro;

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
