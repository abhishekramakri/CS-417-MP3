using UnityEngine;

/// <summary>
/// Attach to any interactable object alongside its action script (GeneratorDeployer, PowerupPurchaser, etc.).
/// Call StartCooldown() after a successful interaction to enforce a pause.
/// Other scripts check IsReady before allowing the interaction.
/// Assign timerText to a TextMeshProUGUI in the world to show the countdown.
/// </summary>
public class CooldownTimer : MonoBehaviour
{
    [Tooltip("Seconds the player must wait between interactions.")]
    public float cooldownDuration = 3f;

    private float timeRemaining = 0f;
    private bool everUsed = false;

    public bool IsReady => timeRemaining <= 0f;
    public float TimeRemaining => timeRemaining;
    public bool EverUsed => everUsed;

    void Update()
    {
        if (timeRemaining > 0f)
        {
            timeRemaining -= Time.deltaTime;
            if (timeRemaining < 0f) timeRemaining = 0f;
        }
    }

    public void StartCooldown()
    {
        timeRemaining = cooldownDuration;
        everUsed = true;
    }
}
