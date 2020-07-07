using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdManager : MonoBehaviour, IUnityAdsListener
{
    private string googlePlayID = "3539961";

    bool isTestMode = false;
    string myPlacementId = "rewardedVideo";
    bool respawnCalled = false;
    private static bool isInitialized = false;
    void Start()
    {
        if (!isInitialized)
        {
            Debug.Log("AdManager started, addlistener initialized");
            isInitialized = true;
            Advertisement.AddListener(this);
            Advertisement.Initialize(googlePlayID, isTestMode);
        }
    }

    public void ShowAd()
    {
        Advertisement.Show();
        respawnCalled = false;
    }

    public void ShowRewardingAd()
    {
        Advertisement.Show(myPlacementId);
        respawnCalled = false;
    }

    // Implement IUnityAdsListener interface methods:
    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        // Define conditional logic for each ad completion status:
        if (showResult == ShowResult.Finished)
        {
            Debug.Log("OnUnityAdsDidFinish runs");

            // Reward the user for watching the ad to completion.
            if (placementId == "rewardedVideo")
            {
                Debug.Log("rewarded for watching ad");
                CallRespawn(true);
            }
            else
            {
                Debug.Log("placementID for non rewarding string is: " + placementId);
                Debug.Log("watching basic ad");
                CallRespawn(false);
            }

        }
        else if (showResult == ShowResult.Skipped)
        {
            Debug.Log("ShowResult.Skipped");
        }
        else if (showResult == ShowResult.Failed)
        {
            Debug.Log("ShowResult.Failed");
        }
    }

    public void OnUnityAdsReady(string placementId)
    {
        if (placementId == myPlacementId)
        {
            // OnUnityAdsReady and its loaded
        }
    }

    public void OnUnityAdsDidError(string message)
    {
        // Log the error.
    }

    public void OnUnityAdsDidStart(string placementId)
    {
        // Optional actions to take when the end-users triggers an ad.
    }

    // Call only once with prameter: should we reward the player?
    private void CallRespawn(bool isRewarded)
    {
        Debug.Log("Ads are ready n calling respawn");
        if (!respawnCalled)
        {
            respawnCalled = true;
            if (isRewarded)
            {
                GameManager.GetInstance().TripleScore();
            }
        }

    }

}
