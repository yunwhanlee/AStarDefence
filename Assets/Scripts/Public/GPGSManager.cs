using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames;
using TMPro;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;

public class GPGSManager : MonoBehaviour {
    public static GPGSManager _;

    bool isLogin = false;

    void Start() {
        _ = this;
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
        PlayGamesPlatform.Instance.ShowLeaderboardUI(GPGSIds.leaderboard);
    }

    /// <summary>
    /// 名声データをGoogleリーダボードへアップデート
    /// </summary>
    public void UpdateFameToLeaderBoard() {
        PlayGamesPlatform.Instance.ReportScore(DM._.DB.StatusDB.Fame, GPGSIds.leaderboard, (bool success) => {});
    }
#endregion
}
