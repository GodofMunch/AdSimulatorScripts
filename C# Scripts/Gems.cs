using TMPro;
using UnityEngine;

public class Gems : MonoBehaviour
{
    public TextMeshProUGUI gemCountString;
    private int gemAmount;
    
    public void IncreaseGems(int amount)
    {
        gemAmount += amount;
        gemCountString.text = gemAmount.ToString();
    }

    public int getGems()
    {
        return gemAmount;
    }

    public void setGems(int gems)
    {
        gemAmount = gems;
        gemCountString.text = gemAmount.ToString();
    }
}