using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using System;

//* TEST REWARD AD ID
// Android : ca-app-pub-3940256099942544/5224354917
// iOS : ca-app-pub-3940256099942544/1712485313

public class AdmobManager : MonoBehaviour {
    public static AdmobManager _;
    public Action OnGetRewardAd = () => {};
    public const int FREE_LUCKYSPIN = 0;
    const string RewardAdTestID = "ca-app-pub-3940256099942544/5224354917";
    const string RewardAdID = "ca-app-pub-2459080562974506/4228000878";
    [SerializeField] bool isTestMode;
    RewardedAd RewardAd;

    void Awake() {
        Singleton();
        MobileAds.Initialize((InitializationStatus initStatus) => 
            LoadRewardedAd()
        );
    }

    private void Singleton(){
        if(_ == null) {
            _ = this;
            DontDestroyOnLoad(_);
        }
        else
            Destroy(gameObject);
    }

    /// <summary>
    /// リワードロード
    /// </summary>
    public void LoadRewardedAd() {
        //* Clean old ad
        if (RewardAd != null) {
                RewardAd.Destroy();
                RewardAd = null;
        }
        //* 広告要請
        var adRequest = new AdRequest();
        //* Unit ID
        string unitId = isTestMode? RewardAdTestID : RewardAdID;

        //* リワードロード
        RewardedAd.Load(unitId, adRequest, (RewardedAd ad, LoadAdError error) => {
            //* Fail
            if (error != null || ad == null) {
                Debug.LogError("Rewarded ad failed to load an ad " +　"with error : " + error);
                return;
            }

            Debug.Log("Rewarded ad loaded with response : " + ad.GetResponseInfo());

            RewardAd = ad;

            //* イベントCALLBACK処理 購読（広告を全部見てからアプリを閉じるときに行う）
            // RegisterEventHandlers(RewardAd);
        });
    }

    /// <summary>
    /// 広告再生
    /// </summary>
    public void ShowRewardedAd() {
        //* 広告表示
        if (RewardAd != null && RewardAd.CanShowAd())
            RewardAd.Show((Reward reward) => {
                Debug.Log($"SHOW REWARD!");
                StartCoroutine(CoShowRewardAfterWaiting());
            });
        else
            LoadRewardedAd();
    }

    IEnumerator CoShowRewardAfterWaiting() {
        //! (BUG) 広告終了してから、UI Particle Imageプラグインがメインスレッド問題で瞬間NULLなるため、少し待機してエラー防止
        yield return Util.Time0_3;
        OnGetRewardAd?.Invoke();
        OnGetRewardAd = null;
        LoadRewardedAd();
    }

#region FUNC
/// <summary>
/// 広告閲覧
/// </summary>
/// <param name="rewardCallbackPrecess">リワード処理の中身</param>
    public void ProcessRewardAd(Action rewardCallbackPrecess) {
        OnGetRewardAd = rewardCallbackPrecess;
        //* 広告削除が有ったら
        if(DM._.DB.IsRemoveAd) {
            OnGetRewardAd?.Invoke();
            OnGetRewardAd = null;
        }
        //* 広告再生
        else {
            ShowRewardedAd();
        }
    }
#endregion

    /// <summary>
    /// 広告についたCallbackイベント
    /// </summary>
    // private void RegisterEventHandlers(RewardedAd ad) {
    //     // Raised when the ad is estimated to have earned money.
    //     ad.OnAdPaid += (AdValue adValue) => Debug.Log(String.Format("Rewarded ad paid {0} {1}.", adValue.Value, adValue.CurrencyCode));
    //     // Raised when an impression is recorded for an ad.
    //     ad.OnAdImpressionRecorded += () => Debug.Log("Rewarded ad recorded an impression.");
    //     // Raised when a click is recorded for an ad.
    //     ad.OnAdClicked += () => Debug.Log("Rewarded ad was clicked.");
    //     // Raised when an ad opened full screen content.
    //     ad.OnAdFullScreenContentOpened += () => Debug.Log("Rewarded ad full screen content opened.");

    //     //* ★広告全部見てから、アプリ閉じる。(Raised when the ad closed full screen content.)
    //     ad.OnAdFullScreenContentClosed += () => {
    //         Debug.Log("Rewarded Ad full screen content closed.");
    //         // Reload the ad so that we can show another as soon as possible.
    //         // OnGetRewardAd?.Invoke();
    //         // OnGetRewardAd = null;
    //     };

    //     // Raised when the ad failed to open full screen content.
    //     ad.OnAdFullScreenContentFailed += (AdError error) => {
    //         Debug.LogError("Rewarded ad failed to open full screen content " + "with error : " + error);
    //     };
    // }
}
