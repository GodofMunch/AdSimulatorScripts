using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;

public class AdSim2020Authenticator : MonoBehaviour
{
    public static PlayGamesPlatform platform;
    void Start()
    {
        if (platform == null)
        {
            PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().Build();
            PlayGamesPlatform.InitializeInstance(config);
            PlayGamesPlatform.DebugLogEnabled = true;

            platform = PlayGamesPlatform.Activate();
        }
        
        Social.Active.localUser.Authenticate(success =>
        {
            if (success)
            {
                Debug.Log("Login Succeeded");
            }
            else
            {
                Debug.Log("Login Failed");
            }
        });
    }
}