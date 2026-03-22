using UnityEngine;
using UnityEngine.XR;
using System.Collections;
using System.Collections.Generic;

public class SunflowerClickVR : MonoBehaviour
{
    public float sunlightPerClick = 1f;

    [Header("Haptics")]
    [Range(0f, 1f)] public float hapticAmplitude = 0.6f;
    public float hapticDuration = 0.1f;

    private Vector3 originalScale;

    void Start() { originalScale = transform.localScale; }

    public void OnSunflowerClicked()
    {
        GameManager.instance.sunlight += sunlightPerClick;
        TriggerHaptics();
        StartCoroutine(ClickBounce());
    }

    void TriggerHaptics()
    {
        var devices = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Controller, devices);
        foreach (var device in devices)
            device.SendHapticImpulse(0, hapticAmplitude, hapticDuration);
    }

    IEnumerator ClickBounce()
    {
        Vector3 big = originalScale * 1.3f;
        float t = 0f;
        while (t < 1f) { t += Time.deltaTime / 0.08f; transform.localScale = Vector3.Lerp(originalScale, big, t); yield return null; }
        t = 0f;
        while (t < 1f) { t += Time.deltaTime / 0.12f; transform.localScale = Vector3.Lerp(big, originalScale, t); yield return null; }
        transform.localScale = originalScale;
    }
}
