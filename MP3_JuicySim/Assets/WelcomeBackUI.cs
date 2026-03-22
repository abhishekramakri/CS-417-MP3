using UnityEngine;
using TMPro;
using System.Collections;

/// <summary>
/// Inter-session Saves (2pts):
/// Attach to RespawnCanvas. Assign the TextMeshProUGUI on the canvas.
/// On load, if idle gains > 0, pops in with a scale overshoot then types out the message.
/// </summary>
public class WelcomeBackUI : MonoBehaviour
{
    public TextMeshProUGUI messageText;
    public float displayDuration = 4f;
    public float typewriterDelay = 0.04f;

    void Start()
    {
        gameObject.SetActive(false);

        if (GameManager.instance == null) return;

        float sun = GameManager.instance.lastIdleSunlight;
        float money = GameManager.instance.lastIdleMoney;

        if (sun <= 0f && money <= 0f) return;

        string fullMessage = $"Welcome back!\n+{sun:F0} Sunlight\n+{money:F0} Money";
        gameObject.SetActive(true);
        StartCoroutine(ShowSequence(fullMessage));
    }

    IEnumerator ShowSequence(string fullMessage)
    {
        // Scale overshoot punch-in
        transform.localScale = Vector3.zero;
        messageText.text = "";

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / 0.2f;
            float scale = OvershootCurve(t);
            transform.localScale = Vector3.one * scale;
            yield return null;
        }
        transform.localScale = Vector3.one;

        // Typewriter
        for (int i = 0; i <= fullMessage.Length; i++)
        {
            messageText.text = fullMessage.Substring(0, i);
            yield return new WaitForSeconds(typewriterDelay);
        }

        yield return new WaitForSeconds(displayDuration);
        gameObject.SetActive(false);
    }

    // Overshoot: goes past 1.0 then settles
    float OvershootCurve(float t)
    {
        t = Mathf.Clamp01(t);
        return 1f + Mathf.Sin(t * Mathf.PI) * 0.3f * (1f - t);
    }
}
