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
    Coroutine CorMsgErrorID;
    public Action OnClickAskConfirmAction;

    [field: Header("STATUS")]
    [field: SerializeField] public TextMeshProUGUI TopGoblinKeyTxt {get; set;}
    [field: SerializeField] public TextMeshProUGUI TopCoinTxt {get; set;}
    [field: SerializeField] public TextMeshProUGUI TopDiamondTxt {get; set;}
    [field: SerializeField] public TextMeshProUGUI LvTxt {get; set;}
    [field: SerializeField] public TextMeshProUGUI ExpTxt {get; set;}

    [field: SerializeField] public GameObject CloverActiveIcon {get; set;}
    [field: SerializeField] public GameObject GoldCloverActiveIcon {get; set;}

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

    [Header("AGAIN ASK POPUP")]
    public GameObject AgainAskPopUp;
    public TextMeshProUGUI AgainAskMsgTxt;

    void Start() {
        CorMsgNoticeID = null;
        CorMsgErrorID = null;

        //* Init Status Text UI
        TopGoblinKeyTxt.text = $"{HM._.GoblinKey}/{Config.MAX_GOBLINKEY}";
        TopCoinTxt.text = $"{HM._.Coin}";
        TopDiamondTxt.text = $"{HM._.Diamond}";
        LvTxt.text = $"{HM._.Lv}";
        ExpTxt.text = $"{HM._.Exp}";

        //* クロバーEXPアイテム活性化 表示
        CloverActiveIcon.SetActive(DM._.DB.IsCloverActive);
        GoldCloverActiveIcon.SetActive(DM._.DB.IsGoldCloverActive);
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
        if(CorMsgErrorID != null) 
            StopCoroutine(CorMsgErrorID);
        SM._.SfxPlay(SM.SFX.ErrorSFX);
        CorMsgErrorID = StartCoroutine(CoShowMsgError(msg));
    }
    IEnumerator CoShowMsgError(string msg) {
        TopMsgError.SetActive(true);
        MsgErrorTxt.text = msg;
        yield return Util.Time1;
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
        if(CorMsgNoticeID != null)
            StopCoroutine(CorMsgNoticeID);
        CorMsgNoticeID = StartCoroutine(CoShowMsgNotice(msg, y, Util.Time2));
    }
    public void ShowMsgNotice(string msg, WaitForSeconds time) {
        if(CorMsgNoticeID != null)
            StopCoroutine(CorMsgNoticeID);
        CorMsgNoticeID = StartCoroutine(CoShowMsgNotice(msg, 350, time));
    }
    IEnumerator CoShowMsgNotice(string msg, int y, WaitForSeconds waitTime) {
        BottomMsgNotice.SetActive(true);
        BottomMsgNotice.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, y);
        MsgNoticeTxt.text = msg;
        yield return waitTime;
        BottomMsgNotice.SetActive(false);
    }
    /// <summary> もう一度確認するPOPUP：★OnClickAskConfirmActionへ確認ボタン押してから、処理するメソッドを購読すること！</summary>
    public void ShowAgainAskMsg(string msg = "") {
        AgainAskPopUp.SetActive(true);
        AgainAskMsgTxt.text = msg;
    }
    public void OnClickAgainAskPopUpConfirmBtn() {
        AgainAskPopUp.SetActive(false);
        OnClickAskConfirmAction?.Invoke();
    }
#endregion
}
