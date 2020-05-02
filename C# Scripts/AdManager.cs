using UnityEngine;
using GoogleMobileAds.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using GooglePlayGames.Native.Cwrapper;
using UnityEngine.Advertisements;
using UnityEngine.UI;

public class AdManager : MonoBehaviour, IUnityAdsListener
{

    private BannerView bannerView;
    private InterstitialAd interstitialAd;
    private RewardedAd rewardedAd;

    private bool unityInterstitial;
    private bool unityRewarded;

    public bool runRewarded;

    private Gems gems;
    private Button rewardButton;

    private int rewardValue = 5;
    void Start()
    {
        DontDestroyOnLoad(this);
        string admobAppID = "ca-app-pub-8440254339399479~9979703006";
        string unityAppID = "3580197";

        bool testMode = true;
        MobileAds.Initialize(admobAppID);
        Advertisement.Initialize(unityAppID, testMode);

        Button[] allButtons = FindObjectsOfType<Button>();
        
        
        foreach(Button button in allButtons)
            if (button.name == "RewardGems")
            {
                rewardButton = button;
            }
        
        StartCoroutine (ShowBannerWhenReady ());
        Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);
        RequestBanner();
        runRewarded = false;
        gems = FindObjectOfType<Gems>();
    }
    
    IEnumerator ShowBannerWhenReady () {
        while (!Advertisement.IsReady ("unityBanner")) {
            yield return new WaitForSeconds (0.5f);
        }
        Advertisement.Banner.Show ("unityBanner");
    }
    
    private void Update()
    {
        
        if (runRewarded)
        {
            RequestRewarded();
        }

        runRewarded = false;
    }

    public void runRewardedAdd()
    {
        runRewarded = true;
    }

    private void RequestBanner()
    {
        string admobBannerID = "ca-app-pub-3940256099942544/6300978111";
        bannerView = new BannerView(admobBannerID, AdSize.Banner, AdPosition.Top);
        AdRequest request = new AdRequest.Builder().Build();
        bannerView.OnAdLoaded += HandleBannerOnAdLoaded;
        bannerView.LoadAd(request);
        bannerView.Show();
    }

    public void RequestInterstitial()
    {
        if (!unityInterstitial)
            RequestAdmobInterstitial();
        else
            RequestUnityInterstitial();
        unityInterstitial = !unityInterstitial;
    }

    public void RequestUnityInterstitial()
    {
        string unityAppId = "3580197";
        bool testMode = true;
        Advertisement.Initialize(unityAppId, false);
        Advertisement.Show();
    }
    public void RequestAdmobInterstitial()
    {
        string admobInterstitialId = "ca-app-pub-3940256099942544/1033173712";
        interstitialAd = new InterstitialAd(admobInterstitialId);
        AdRequest request = new AdRequest.Builder().Build();
        interstitialAd.OnAdLoaded += HandleInterstitialOnAdLoaded;
        interstitialAd.LoadAd(request);
    }

    public void RequestRewarded()
    {
        if (!unityRewarded) {
            RequestAdmobRewarded();
            rewardButton.onClick.AddListener(RequestUnityRewarded);
            Advertisement.AddListener(this);
        } else {
            rewardButton.onClick.RemoveListener(RequestUnityRewarded);
            //Advertisement.RemoveListener(this);
        }
        

        unityRewarded = !unityRewarded;
    }

    public void RequestUnityRewarded()
    {
        string unityAppId = "3580197";
        bool testMode = true;
        string placementId = "unityRewarded";
        
        Advertisement.Initialize(unityAppId, testMode);
        Advertisement.Show("unityRewarded");
        
    }

    public void OnUnityAdsReady(string placementId)
    {
        string myPlacementId = "unityRewarded";
        if (placementId == myPlacementId) {        
            rewardButton.interactable = true;
        }
    }

    public void OnUnityAdsDidError(string message)
    {
        Debug.Log("Unity Ad Error");
    }

    public void OnUnityAdsDidStart(string placementId)
    {
        Debug.Log("Unity Ad Started");
    }

    public void OnUnityAdsDidFinish (string placementId, ShowResult showResult) {
        if (showResult == ShowResult.Finished) 
            gems.IncreaseGems(rewardValue);
        else if (showResult == ShowResult.Skipped) 
            Debug.Log("Ah now, fair is fair, ya cant be skipping the auld ad's fella!");
        else if (showResult == ShowResult.Failed)
            Debug.LogWarning ("The ad did not finish due to an error.");
    }

    public void RequestAdmobRewarded()
    {
        string admobRewardedAd = "ca-app-pub-3940256099942544/5224354917";
        rewardedAd = new RewardedAd(admobRewardedAd);
        AdRequest request = new AdRequest.Builder().Build();
        
        rewardedAd.OnAdLoaded += HandleRewardedAdOnLoaded;
        rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
        rewardedAd.OnAdClosed += HandleRewardedAdClosed;
        rewardedAd.LoadAd(request);

        while (!rewardedAd.IsLoaded())
        {
            
        }
        rewardedAd.Show();
        gems.IncreaseGems(rewardValue);
    }
    public void HandleBannerOnAdLoaded (object sender, EventArgs args) {
        Debug.Log("HandleAdLoaded event received");
        bannerView.Show();
    }
    public void HandleInterstitialOnAdLoaded(object sender, EventArgs args) {
        Debug.Log("HandleInterstitialOnAdLoaded event received");
        interstitialAd.Show();
    }

    public void HandleRewardedAdOnLoaded(object sender, EventArgs args)
    {
        Debug.Log("HandleRewardedAdOnLoaded event received");
    }

    public void HandleUserEarnedReward(object sender, Reward args)
    {
        if (args == null)
        {
            args = new Reward();
            args.Amount = rewardValue;
            args.Type = "Gems";
        }
        string type = args.Type;
        double rewardApiAmount = args.Amount;
        Debug.Log("HandleUserEarnedReward event received for " + rewardApiAmount +
                  " " + type);
    }

    public void HandleRewardedAdClosed(object sender, EventArgs args)
    {
        Debug.Log("HandleRewardedAdClosed event received");
    }
}