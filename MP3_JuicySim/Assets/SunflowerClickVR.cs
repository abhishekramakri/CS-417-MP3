using UnityEngine;

public class SunflowerClickVR : MonoBehaviour
{
    public float sunlightPerClick = 1f;

    public void OnSunflowerClicked()
    {
        GameManager.instance.sunlight += sunlightPerClick;
        Debug.Log("Sunlight: " + GameManager.instance.sunlight);
    }
}
