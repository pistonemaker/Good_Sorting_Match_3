using System;
using UnityEngine;
using GoogleMobileAds.Api;

namespace GoogleMobileAds.Sample
{
    public class RewardedInterstitialAdController : MonoBehaviour
    {
#if UNITY_ANDROID
        private const string _adUnitId = "ca-app-pub-3940256099942544/5354046379";
#elif UNITY_IPHONE
        private const string _adUnitId = "ca-app-pub-3940256099942544/6978759866";
#else
        private const string _adUnitId = "unused";
#endif

        private RewardedInterstitialAd _rewardedInterstitialAd;
        public Action OnAdsClose;
        public bool isLoaded = false;
        public bool isSucess = false;

        // Loads the ad.
        public void LoadAd()
        {
            // Clean up the old ad before loading a new one.
            if (_rewardedInterstitialAd != null)
            {
                DestroyAd();
            }

            Debug.Log("Loading rewarded interstitial ad.");

            // Create our request used to load the ad.
            var adRequest = new AdRequest();

            // Send the request to load the ad.
            RewardedInterstitialAd.Load(_adUnitId, adRequest,
                (RewardedInterstitialAd ad, LoadAdError error) =>
                {
                    // If the operation failed with a reason.
                    if (error != null)
                    {
                        isLoaded = false;
                        Debug.LogError("Rewarded interstitial ad failed to load an ad with error : "
                                        + error);
                        return;
                    }
                    
                    // If the operation failed for unknown reasons.
                    // This is an unexpexted error, please report this bug if it happens.
                    if (ad == null)
                    {
                        isLoaded = false;
                        Debug.LogError("Unexpected error: Rewarded interstitial load event fired with null ad and null error.");
                        return;
                    }

                    // The operation completed successfully.
                    Debug.Log("Rewarded interstitial ad loaded with response : "
                        + ad.GetResponseInfo());
                    _rewardedInterstitialAd = ad;
                    isLoaded = true;

                    // Register to ad events to extend functionality.
                    RegisterEventHandlers(ad);
                });
        }

        // Shows the ad.
        public void ShowAd()
        {
            if (_rewardedInterstitialAd != null && _rewardedInterstitialAd.CanShowAd())
            {
                isSucess = true;
                _rewardedInterstitialAd.Show((Reward reward) =>
                {
                    Debug.Log("Rewarded interstitial ad rewarded : " + reward.Amount);
                });
            }
            else
            {
                isSucess = false;
                Debug.LogError("Rewarded interstitial ad is not ready yet.");
            }
        }

        // Destroys the ad.
        public void DestroyAd()
        {
            if (_rewardedInterstitialAd != null)
            {
                isLoaded = false;
                isSucess = false;
                Debug.Log("Destroying rewarded interstitial ad.");
                _rewardedInterstitialAd.Destroy();
                _rewardedInterstitialAd = null;
            }
        }

        // Logs the ResponseInfo.
        public void LogResponseInfo()
        {
            if (_rewardedInterstitialAd != null)
            {
                var responseInfo = _rewardedInterstitialAd.GetResponseInfo();
                Debug.Log(responseInfo);
            }
        }

        protected void RegisterEventHandlers(RewardedInterstitialAd ad)
        {
            // Raised when the ad is estimated to have earned money.
            ad.OnAdPaid += (AdValue adValue) =>
            {
                Debug.Log(String.Format("Rewarded interstitial ad paid {0} {1}.",
                    adValue.Value,
                    adValue.CurrencyCode));
            };
            // Raised when an impression is recorded for an ad.
            ad.OnAdImpressionRecorded += () =>
            {
                Debug.Log("Rewarded interstitial ad recorded an impression.");
            };
            // Raised when a click is recorded for an ad.
            ad.OnAdClicked += () =>
            {
                Debug.Log("Rewarded interstitial ad was clicked.");
            };
            // Raised when an ad opened full screen content.
            ad.OnAdFullScreenContentOpened += () =>
            {
                Debug.Log("Rewarded interstitial ad full screen content opened.");
            };
            // Raised when the ad closed full screen content.
            ad.OnAdFullScreenContentClosed += () =>
            {
                Debug.Log("Rewarded interstitial ad full screen content closed.");
            };
            // Raised when the ad failed to open full screen content.
            ad.OnAdFullScreenContentFailed += (AdError error) =>
            {
                Debug.LogError("Rewarded interstitial ad failed to open full screen content" +
                               " with error : " + error);
            };
        }
    }
}
