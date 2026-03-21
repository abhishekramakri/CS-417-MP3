using UnityEngine;
using TMPro;

/// <summary>
/// Unlockable UI (4pts):
/// Attach to the GameObject that contains the second resource's display and interactions.
/// That GameObject (and its children) starts INACTIVE (hidden).
/// Wire OnTryUnlock() to an XR Simple Interactable (e.g. a locked gate/door/sign object).
/// When the player has enough of the primary resource, the UI unlocks and the GameObject activates.
/// </summary>
public class UnlockableUI : MonoBehaviour
{
    [Header("Unlock Settings")]
    [Tooltip("How much of the primary resource (sunlight) it costs to unlock.")]
    public float unlockCost = 50f;

    [Tooltip("The GameObject to show when unlocked. Assign the panel/display for the second resource.")]
    public GameObject lockedContent;

    [Tooltip("Optional: the lock object to hide after unlocking (e.g. a padlock mesh).")]
    public GameObject lockVisual;

    [Tooltip("Optional: text to update with unlock status / cost hint.")]
    public TextMeshProUGUI hintText;

    [Tooltip("Optional: the trigger object itself to hide after unlocking (e.g. the gate cube/button).")]
    public GameObject triggerObject;

    [Tooltip("Optional: a sign GameObject to hide after unlocking.")]
    public GameObject signObject;

    [Header("Sound")]
    [Tooltip("AudioSource on or near the gate object for spatialized playback.")]
    public AudioSource audioSource;
    [Tooltip("Sound to play when the gate is unlocked.")]
    public AudioClip unlockSound;

    [Header("Feedback")]
    public bool logMessages = true;

    private bool unlocked = false;

    void Start()
    {
        unlocked = PlayerPrefs.GetInt("unlockableUI_unlocked", 0) == 1;

        if (lockedContent != null)
            lockedContent.SetActive(unlocked);

        if (unlocked)
        {
            if (lockVisual != null) lockVisual.SetActive(false);
            if (triggerObject != null) triggerObject.SetActive(false);
            if (signObject != null) signObject.SetActive(false);
        }

        UpdateHint();
    }

    public void OnTryUnlock()
    {
        if (unlocked) return;

        if (GameManager.instance.sunlight < unlockCost)
        {
            if (logMessages) Debug.Log("[UnlockableUI] Need " + unlockCost + " sunlight to unlock. Currently have: " + GameManager.instance.sunlight.ToString("F0"));
            UpdateHint();
            return;
        }

        GameManager.instance.sunlight -= unlockCost;
        unlocked = true;

        if (audioSource != null && unlockSound != null)
        {
            Debug.Log("[UnlockableUI] Playing unlock sound on: " + audioSource.gameObject.name);
            audioSource.PlayOneShot(unlockSound);
        }
        else
        {
            Debug.LogWarning("[UnlockableUI] Sound skipped — audioSource: " + audioSource + " unlockSound: " + unlockSound);
        }

        if (lockedContent != null)
            lockedContent.SetActive(true);

        if (lockVisual != null)
            lockVisual.SetActive(false);

        if (triggerObject != null) triggerObject.SetActive(false);
        if (signObject != null) signObject.SetActive(false);

        PlayerPrefs.SetInt("unlockableUI_unlocked", 1);
        PlayerPrefs.Save();
        if (logMessages) Debug.Log("[UnlockableUI] Second resource area unlocked!");
    }

    void UpdateHint()
    {
        if (hintText != null && !unlocked)
            hintText.text = "Unlock: " + unlockCost + " Sunlight";
    }

    public bool IsUnlocked => unlocked;
}
