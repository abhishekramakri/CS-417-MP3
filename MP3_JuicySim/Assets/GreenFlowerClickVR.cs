using UnityEngine;

public class GreenFlowerClickVR : MonoBehaviour
{
    public float moneyPerClick = 1f;

    public void OnGreenFlowerClicked()
    {
        GameManager.instance.money += moneyPerClick;
        Debug.Log("Money: " + GameManager.instance.money);
    }
}
