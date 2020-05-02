using TMPro;
using UnityEngine;

public class Score : MonoBehaviour
{
    public static int score;
    public TextMeshProUGUI textScore;

    public void IncrementScore()
    {
        score++;
        textScore.text = score.ToString();
        
        PlayerPrefs.SetInt("ScoreToUpdate", PlayerPrefs.GetInt("ScoreToUpdate", 0) + 1);
    }

    public void setScore(int score)
    {
        PlayerPrefs.SetInt("ScoreToUpdate", score);
        textScore.text = score.ToString();
    }
}