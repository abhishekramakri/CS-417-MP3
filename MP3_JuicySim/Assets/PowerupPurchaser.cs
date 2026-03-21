using UnityEngine;
using UnityEngine.XR;
using System.Collections.Generic;

/// <summary>
/// Purchasing Power-ups (4pts):
/// Attach to an XR Simple Interactable (e.g. a power-up stand/object).
/// Wire OnPurchase() to the Select Entered or Activated event in the Inspector.
/// Costs resources and applies a multiplier to ALL generators' rate contributions
/// by scaling both sunlightRate and moneyRate in GameManager.
/// </summary>
public class PowerupPurchaser : MonoBehaviour
{
    [Header("Cost")]
    public float sunlightCost = 20f;
    public float moneyCost = 20f;

    [Header("Effect")]
    [Tooltip("Multiplier applied to current rates each time a power-up is bought. e.g. 1.5 = 50% boost.")]
    public float rateMultiplier = 1.5f;

    [Header("Cooldown")]
    public CooldownTimer cooldown;

    [Header("Hide On Purchase")]
    [Tooltip("The sign GameObject to hide after purchasing.")]
    public GameObject signObject;

    [Header("Haptics")]
    [Tooltip("Duration of the haptic pulse in seconds.")]
    public float hapticDuration = 0.3f;
    [Tooltip("Amplitude of the haptic pulse (0–1).")]
    [Range(0f, 1f)]
    public float hapticAmplitude = 0.8f;

    [Header("Feedback")]
    public bool logMessages = true;

    [Tooltip("Unique key for PlayerPrefs save.")]
    public string saveKey = "powerup_default";

    private int purchasedCount = 0;

    void Start()
    {
        purchasedCount = PlayerPrefs.GetInt(saveKey + "_count", 0);
        if (purchasedCount > 0 && signObject != null) signObject.SetActive(false);
        if (purchasedCount > 0) gameObject.SetActive(false);
    }

    void OnApplicationQuit()
    {
        PlayerPrefs.SetInt(saveKey + "_count", purchasedCount);
        PlayerPrefs.Save();
    }

    public void OnPurchase()
    {
        if (cooldown != null && !cooldown.IsReady)
        {
            if (logMessages) Debug.Log("[Powerup] On cooldown.");
            return;
        }

        if (GameManager.instance.sunlight < sunlightCost || GameManager.instance.money < moneyCost)
        {
            if (logMessages) Debug.Log("[Powerup] Not enough resources. Need " + sunlightCost + " sunlight and " + moneyCost + " coins.");
            return;
        }

        GameManager.instance.sunlight -= sunlightCost;
        GameManager.instance.money -= moneyCost;

        GameManager.instance.sunlightRate *= rateMultiplier;
        GameManager.instance.moneyRate *= rateMultiplier;

        purchasedCount++;
        if (logMessages) Debug.Log("[Powerup] Power-up #" + purchasedCount + " applied! Rates multiplied by " + rateMultiplier);
        TriggerHaptics();
        if (cooldown != null) cooldown.StartCooldown();

        if (signObject != null) signObject.SetActive(false);
        gameObject.SetActive(false);
    }

    void TriggerHaptics()
    {
        var devices = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Controller, devices);
        foreach (var device in devices)
            device.SendHapticImpulse(0, hapticAmplitude, hapticDuration);
    }

    public int PurchasedCount => purchasedCount;
}
