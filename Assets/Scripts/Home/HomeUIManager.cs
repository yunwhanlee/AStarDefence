using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Lobby
/// </summary>
public class HomeUIManager : MonoBehaviour {
    Coroutine CorMsgNoticeID;

    public Button PlayBtn {get; set;}
    [Header("ERROR MSG POPUP")]
    public GameObject TopMsgError;
    public TextMeshProUGUI MsgErrorTxt;

    [Header("INFO MSG POPUP")]
    public GameObject TopMsgInfo;
    public TextMeshProUGUI MsgInfoTxt;

    [Header("NOTICE MSG POPUP")]
    public GameObject BottomMsgNotice;
    public TextMeshProUGUI MsgNoticeTxt;

    void Start() {
        CorMsgNoticeID = null;
    }

#region EVENT
    public void OnClickPlayBtn() {
        SM._.SfxPlay(SM.SFX.ClickSFX);
        HM._.stgm.StageGroup.SetActive(true);
        int i = 0;
        Array.ForEach(HM._.stgm.StagePopUps, popUp => popUp.SetActive(HM._.SelectedStage == i++));
    }
    public void OnClickMenuBtn() {
        const string NOTION_URL = "https://www.notion.so/A-Defence-2a40adca8a77420c80a6db623a89083f?pvs=4";
        Application.OpenURL(NOTION_URL);
    }
#endregion

#region FUNC
    /// <summary> 上にへエラーメッセージバー表示（自動OFF）</summary>
    public void ShowMsgError(string msg) {
        SM._.SfxPlay(SM.SFX.ErrorSFX);
        StartCoroutine(CoShowMsgError(msg));
    }
    IEnumerator CoShowMsgError(string msg) {
        TopMsgError.SetActive(true);
        MsgErrorTxt.text = msg;
        yield return Util.RealTime1;
        TopMsgError.SetActive(false);
    }
    public void ShowErrMsg_PlsClearPreviousDifficulty() 
        => ShowMsgError("이전 난이도를 클리어 해주세요!");

    /// <summary> 上にへ情報メッセージバー表示（ON、OFF形式）</summary>
    public void ShowMsgInfo(bool isActive, string msg = "") {
        MsgInfoTxt.text = isActive? msg : "";
        TopMsgInfo.SetActive(isActive);
    }

    /// <summary> 下にお知らせメッセージ表示（自動OFF）</summary>
    public void ShowMsgNotice(string msg, int y = 350) {
        if(CorMsgNoticeID != null) StopCoroutine(CorMsgNoticeID);
        CorMsgNoticeID = StartCoroutine(CoShowMsgNotice(msg, y));
    }
    IEnumerator CoShowMsgNotice(string msg, int y) {
        BottomMsgNotice.SetActive(true);
        BottomMsgNotice.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, y);
        MsgNoticeTxt.text = msg;
        yield return Util.RealTime1;
        BottomMsgNotice.SetActive(false);
    }
#endregion
}
