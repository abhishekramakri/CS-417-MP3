using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public float sunlight;
    public float money;

    public float sunlightRate;
    public float moneyRate;

    [HideInInspector] public float lastIdleSunlight;
    [HideInInspector] public float lastIdleMoney;

    void Awake()
    {
        instance = this;
        Load();
    }

    void Update()
    {
        // Euler integration
        sunlight += sunlightRate * Time.deltaTime;
        money += moneyRate * Time.deltaTime;
    }

    void OnApplicationQuit()
    {
        Save();
    }

    public void Save()
    {
        PlayerPrefs.SetFloat("sunlight", sunlight);
        PlayerPrefs.SetFloat("money", money);
        PlayerPrefs.SetFloat("sunlightRate", sunlightRate);
        PlayerPrefs.SetFloat("moneyRate", moneyRate);
        PlayerPrefs.SetString("quitTime", System.DateTime.UtcNow.ToBinary().ToString());
        PlayerPrefs.Save();
    }

    public void Load()
    {
        sunlight = PlayerPrefs.GetFloat("sunlight", sunlight);
        money = PlayerPrefs.GetFloat("money", money);
        sunlightRate = PlayerPrefs.GetFloat("sunlightRate", sunlightRate);
        moneyRate = PlayerPrefs.GetFloat("moneyRate", moneyRate);

        if (PlayerPrefs.HasKey("quitTime"))
        {
            var quitTime = System.DateTime.FromBinary(System.Convert.ToInt64(PlayerPrefs.GetString("quitTime")));
            float elapsed = (float)(System.DateTime.UtcNow - quitTime).TotalSeconds;
            lastIdleSunlight = sunlightRate * elapsed;
            lastIdleMoney = moneyRate * elapsed;
            sunlight += lastIdleSunlight;
            money += lastIdleMoney;
        }
    }
}