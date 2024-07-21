using Google.Play.AppUpdate;
using Google.Play.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames;
using TMPro;
using GooglePlayGames.BasicApi;

public class GPGSManager : MonoBehaviour {
    public static GPGSManager _;
    bool isLogin = false;
    AppUpdateManager appUpdateManager;

    void Awake() {
        _ = this;

    #if UNITY_EDITOR

    #else 
        StartCoroutine(CheckForUpdate());
    #endif
    }



#region EVENT
    public void OnClickRankIconBtnAtHome() {
        if(!isLogin)
            GPGSLogin();
        else
            ShowLeaderBoardUI();
    }
#endregion

#region FUNC
    IEnumerator CheckForUpdate() {
        Debug.Log("★Starting CheckForUpdate");
        appUpdateManager = new AppUpdateManager();

        PlayAsyncOperation<AppUpdateInfo, AppUpdateErrorCode> appUpdateInfoOperation =
            appUpdateManager.GetAppUpdateInfo();

        // Wait until the asynchronous operation completes.
        yield return appUpdateInfoOperation;

        if (appUpdateInfoOperation.IsSuccessful)
        {
            var appUpdateInfoResult = appUpdateInfoOperation.GetResult();
            
            //* アップデート可能なのかを確認
            if(appUpdateInfoResult.UpdateAvailability == UpdateAvailability.UpdateAvailable) {
                var appUpdateOptions = AppUpdateOptions.ImmediateAppUpdateOptions();
                var startUpdateRequest = appUpdateManager.StartUpdate(appUpdateInfoResult, appUpdateOptions);

                while(!startUpdateRequest.IsDone) {
                    if(startUpdateRequest.Status == AppUpdateStatus.Downloading) {
                        Debug.Log("CheckForUpdate():: 업데이트 다운로드 진행 중");
                    }
                    else if(startUpdateRequest.Status == AppUpdateStatus.Downloaded) {
                        Debug.Log("CheckForUpdate():: 다운로드 완료");
                    }
                    yield return null;
                }

                var result = appUpdateManager.CompleteUpdate();

                //* 完了できたら、最後に確認
                while(!result.IsDone) {
                    yield return new WaitForEndOfFrame();
                }

                yield return (int)startUpdateRequest.Status;
            }
            else if(appUpdateInfoResult.UpdateAvailability == UpdateAvailability.UpdateNotAvailable){
                Debug.Log("CheckForUpdate():: 업데이트가 없음");
                yield return (int)UpdateAvailability.UpdateNotAvailable;
            }
            else {
                Debug.Log("CheckForUpdate():: 업데이트 가능여부 알수 없음");
                yield return (int)UpdateAvailability.Unknown;
            }
        }
        else {
            Debug.Log("CheckForUpdate():: 업데이트 오류");
        }
    }

    public void GPGSLogin() {
        PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
    }

    internal void ProcessAuthentication(SignInStatus status) {
        if (status == SignInStatus.Success) {
            isLogin = true;
            // string displayName = PlayGamesPlatform.Instance.GetUserDisplayName();
            // string userID = PlayGamesPlatform.Instance.GetUserId();
            HM._.hui.ShowMsgNotice($"로그인 성공: 구글 리더보드를 불러옵니다.");
            ShowLeaderBoardUI();
        } else {
            HM._.hui.ShowMsgNotice("로그인 실패");
        }
    }

    /// <summary>
    /// リーダボード閲覧
    /// </summary>
    public void ShowLeaderBoardUI() {
        UpdateFameToLeaderBoard();
        UpdateCrackDungeonBestWaveToLeaderBoard();
        PlayGamesPlatform.Instance.ShowLeaderboardUI();
    }

    /// <summary>
    /// 「名声」データをGoogleリーダボードへアップデート
    /// </summary>
    public void UpdateFameToLeaderBoard() {
        PlayGamesPlatform.Instance.ReportScore(
            DM._.DB.StatusDB.Fame, GPGSIds.leaderboard, (bool success) => {}
        );
    }
    /// <summary>
    /// 「きれつダンジョン突破」データをGoogleリーダボードへアップデート
    /// </summary>
    public void UpdateCrackDungeonBestWaveToLeaderBoard() {
        PlayGamesPlatform.Instance.ReportScore(
            DM._.DB.InfiniteUpgradeDB.MyBestInfiniteFloor, GPGSIds.leaderboard_2, (bool success) => {}
        );
    }
#endregion
}
