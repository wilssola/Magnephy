using System;
using UnityEngine;
using System.Collections;
using GoogleMobileAds.Api;
using System.Collections.Generic;

public class Admob : MonoBehaviour
{
    static public BannerView BannerViewObject;
    static public RewardBasedVideoAd RewardBasedVideo;

    public string AndroidID = "ca-app-pub-3940256099942544~3347511713";
    public string IphoneID = "ca-app-pub-3940256099942544~1458002511";

    public string AndroidBannerUnitID = "ca-app-pub-3940256099942544/6300978111";
    public string IphoneBannerUnitID = "ca-app-pub-3940256099942544/2934735716";

    public string AndroidRewardBasedVideoUnitID = "ca-app-pub-3940256099942544/5224354917";
    public string IphoneRewardBasedVideoUnitID = "ca-app-pub-3940256099942544/1712485313";

    // Interface de Morte.
    public GameObject DeathUI;

    public void Start()
    {
        #if UNITY_ANDROID
            string AppID = AndroidID;
        #elif UNITY_IPHONE
            string AppID = IphoneID;
        #else
            string AppID = "unexpected_platform";
        #endif

        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize(AppID);

        // Get singleton reward based video ad reference.
        RewardBasedVideo = RewardBasedVideoAd.Instance;

        RewardBasedVideo.OnAdClosed += HandleRewardBasedVideoClosed;
        RewardBasedVideo.OnAdRewarded += HandleRewardBasedVideoRewarded;

        RequestBanner();
        RequestRewardBasedVideo();
    }

    private void RequestBanner()
    {
        #if UNITY_ANDROID
            string AdUnitID = AndroidBannerUnitID;
        #elif UNITY_IPHONE
            string AdUnitID = IphoneBannerUnitID;
        #else
            string AdUnitID = "unexpected_platform";
        #endif

        AdSize CustomAdSize = new AdSize(250, 50);

        // Create a smart banner at the bottom of the screen.
        BannerViewObject = new BannerView(AdUnitID, CustomAdSize, AdPosition.Bottom);

        // Create an empty ad request.
        AdRequest Request = new AdRequest.Builder().Build();

        // Load the banner with the request.
        BannerViewObject.LoadAd(Request);
    }

    private void RequestRewardBasedVideo()
    {
        #if UNITY_ANDROID
            string AdUnitID = AndroidRewardBasedVideoUnitID;
        #elif UNITY_IPHONE
            string AdUnitID = IphoneRewardBasedVideoUnitID;
        #else
            string AdUnitID = "unexpected_platform";
        #endif

        // Create an empty ad request.
        AdRequest Request = new AdRequest.Builder().Build();

        // Load the rewarded video ad with the request.
        RewardBasedVideo.LoadAd(Request, AdUnitID);
    }

    public void HandleRewardBasedVideoClosed(object Sender, EventArgs Args)
    {
        RequestRewardBasedVideo();
    }

    public void HandleRewardBasedVideoRewarded(object Sender, EventArgs Args)
    {
        DeathUI.SetActive(false);
        Player.Life = 100;
        Player.CallInvincibility = true;
        Player.GameOver = false;
    }
}