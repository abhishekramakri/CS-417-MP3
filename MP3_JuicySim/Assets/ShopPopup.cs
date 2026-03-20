using UnityEngine;
using MobileVRInventory;

/// <summary>
/// Shop UI popup: buy Watering Can (5 sunlight), Fertilizer (5 coins), or Powerup (15 sunlight + 15 coins).
/// Assign this to the popup GameObject and hook each button's OnClick() to the corresponding Buy method.
/// Ensure "Watering Can", "Fertilizer", and "Powerup" exist in your Inventory Item Database.
/// </summary>
public class ShopPopup : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Leave empty to find at runtime.")]
    public VRInventory vrInventory;

    [Header("Watering Can")]
    public float wateringCanSunlightCost = 5f;
    public string wateringCanItemName = "Watering Can";

    [Header("Fertilizer")]
    public float fertilizerCoinCost = 5f;
    public string fertilizerItemName = "Fertilizer";

    [Header("Powerup")]
    public float powerupSunlightCost = 15f;
    public float powerupCoinCost = 15f;
    public string powerupItemName = "Powerup";

    [Header("Optional feedback")]
    [Tooltip("Show in console or hook up to your own UI message.")]
    public bool logMessages = true;

    void Awake()
    {
        if (vrInventory == null)
            vrInventory = UnityEngine.Object.FindAnyObjectByType<VRInventory>();
    }

    /// <summary>Show the shop UI canvas. Call from medieval shop click. (Only this Canvas is toggled; the 3D shop object stays visible.)</summary>
    public void ShowPopup()
    {
        gameObject.SetActive(true);
    }

    /// <summary>Hide the shop UI canvas. Assign to your Close button's OnClick.</summary>
    public void HidePopup()
    {
        gameObject.SetActive(false);
    }

    /// <summary>Call from Button OnClick: buy Watering Can for sunlight.</summary>
    public void BuyWateringCan()
    {
        if (!CanAfford(wateringCanSunlightCost, 0f))
        {
            Notify("Not enough sunlight. Need " + wateringCanSunlightCost + " sunlight.");
            return;
        }
        if (vrInventory == null)
        {
            Notify("No inventory found.");
            return;
        }

        GameManager.instance.sunlight -= wateringCanSunlightCost;
        vrInventory.AddItem(wateringCanItemName, 1);
        Notify("Bought " + wateringCanItemName + "!");
    }

    /// <summary>Call from Button OnClick: buy Fertilizer for coins.</summary>
    public void BuyFertilizer()
    {
        if (!CanAfford(0f, fertilizerCoinCost))
        {
            Notify("Not enough coins. Need " + fertilizerCoinCost + " coins.");
            return;
        }
        if (vrInventory == null)
        {
            Notify("No inventory found.");
            return;
        }

        GameManager.instance.money -= fertilizerCoinCost;
        vrInventory.AddItem(fertilizerItemName, 1);
        Notify("Bought " + fertilizerItemName + "!");
    }

    /// <summary>Call from Button OnClick: buy Powerup for sunlight and coins.</summary>
    public void BuyPowerup()
    {
        if (!CanAfford(powerupSunlightCost, powerupCoinCost))
        {
            Notify("Not enough resources. Need " + powerupSunlightCost + " sunlight and " + powerupCoinCost + " coins.");
            return;
        }
        if (vrInventory == null)
        {
            Notify("No inventory found.");
            return;
        }

        GameManager.instance.sunlight -= powerupSunlightCost;
        GameManager.instance.money -= powerupCoinCost;
        vrInventory.AddItem(powerupItemName, 1);
        Notify("Bought " + powerupItemName + "!");
    }

    bool CanAfford(float sunlightRequired, float coinsRequired)
    {
        if (GameManager.instance == null) return false;
        return GameManager.instance.sunlight >= sunlightRequired && GameManager.instance.money >= coinsRequired;
    }

    void Notify(string message)
    {
        if (logMessages) Debug.Log("[Shop] " + message);
    }
}
