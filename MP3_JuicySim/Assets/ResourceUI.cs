using UnityEngine;
using TMPro;

public class ResourceUI : MonoBehaviour
{
    public TextMeshProUGUI sunlightText;
    public TextMeshProUGUI moneyText;

    void Update()
    {
        sunlightText.text = "Sunlight: " + GameManager.instance.sunlight.ToString("F0");
        moneyText.text = "Money: " + GameManager.instance.money.ToString("F0");
    }
}
