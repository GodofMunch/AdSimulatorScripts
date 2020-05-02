using UnityEngine;
using GooglePlayGames;
public class AdSimulatorAchievements : MonoBehaviour
{
    public void OpenAchievmentsPanel()
    {
        Social.ShowAchievementsUI();
    }

    public void UpdateIncremental()
    {
        PlayGamesPlatform.Instance.IncrementAchievement(GPGSIds.achievement_tap_master, 1, null);
    }

    public void UnlockRegular()
    {
        Social.ReportProgress(GPGSIds.achievement_chicken_dinner, 100f, null);
    }
}