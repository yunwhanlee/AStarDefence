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
    [field: SerializeField] public Canvas TopNavCanvas {get; set;}
    [field: SerializeField] public TextMeshProUGUI TopGoldKeyTxt {get; set;}
    [field: SerializeField] public TextMeshProUGUI TopCoinTxt {get; set;}
    [field: SerializeField] public TextMeshProUGUI TopDiamondTxt {get; set;}
    [field: SerializeField] public TextMeshProUGUI LvTxt {get; set;}
    [field: SerializeField] public TextMeshProUGUI ExpTxt {get; set;}
    [field: SerializeField] public TextMeshProUGUI FameTxt {get; set;}
    [field: SerializeField] public Slider ExpSlider {get; set;}
    [field: SerializeField] public GameObject RemoveAdIcon {get; set;}
    [field: SerializeField] public GameObject CloverActiveIcon {get; set;}
    [field: SerializeField] public GameObject GoldCloverActiveIcon {get; set;}
    [field: SerializeField] public GameObject SpeedUpAdBtnOffObj {get; set;}
    [field: SerializeField] public GameObject SpeedUpAdBtnOnObj {get; set;}
    [field: SerializeField] public bool IsActivePopUp {get; set;}

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
        TopGoldKeyTxt.text = $"{HM._.GoldKey}";
        TopCoinTxt.text = $"{HM._.Coin}";
        TopDiamondTxt.text = $"{HM._.Diamond}";
        FameTxt.text = $"{DM._.DB.StatusDB.Fame}";

        //* クロバーEXPアイテム活性化 表示
        RemoveAdIcon.SetActive(DM._.DB.IsRemoveAd);
        HM._.shopMg.RemoveAdDim.SetActive(DM._.DB.IsRemoveAd);
        CloverActiveIcon.SetActive(DM._.DB.IsCloverActive);
        GoldCloverActiveIcon.SetActive(DM._.DB.IsGoldCloverActive);

        //* SpeedUpアイコン活性化 表示
        DM._.IsActiveSpeedUp = DM._.DB.IsRemoveAd;
        SpeedUpAdBtnOffObj.SetActive(!DM._.IsActiveSpeedUp);
        SpeedUpAdBtnOnObj.SetActive(DM._.IsActiveSpeedUp);
    }

#region EVENT
    public void OnClickSpeedUpAdBtnOff() {
        SM._.SfxPlay(SM.SFX.ClickSFX);
        ShowAgainAskMsg("광고를 시청하고 게임배속 2.5배를 추가하시겠습니까?\n<color=grey>(원래 1배, 2배속만 가능)</color>");
        OnClickAskConfirmAction = () => {
            AdmobManager._.ProcessRewardAd(() => {
                SM._.SfxPlay(SM.SFX.CompleteSFX);
                SpeedUpAdBtnOffObj.SetActive(false);
                SpeedUpAdBtnOnObj.SetActive(true);
                DM._.IsActiveSpeedUp = true;
            });
        };
    }
    public void OnClickPlayBtn() {
        SM._.SfxPlay(SM.SFX.ClickSFX);
        HM._.stgm.StageGroup.SetActive(true);
        int i = 0;
        Array.ForEach(HM._.stgm.StagePopUps, popUp => popUp.SetActive(HM._.SelectedStage == i++));
    }
    // public void OnClickMenuBtn() {
    //     const string NOTION_URL = "https://www.notion.so/A-Defence-2a40adca8a77420c80a6db623a89083f?pvs=4";
    //     Application.OpenURL(NOTION_URL);
    // }
    public void OnClickTopNavGoldKeyPlusBtn() {
        //* 広告閲覧数が残ったら
        if(DM._.DB.GoldkeyFreeAdCnt > 0) {
            ShowAgainAskMsg(
                "쉿! <color=yellow>황금열쇠</color>가 필요하신가요?"
                + "\n<sprite name=Ad>광고시청 후 <color=yellow>무료 3개</color> 지급!"
                + $"\n(현재 {DM._.DB.GoldkeyFreeAdCnt}회 가능)"
                + "\n<color=red><size=60%>*상점에서도 구매가 가능합니다.</size></color>"
            );

            //* 確認ボタン イベント登録
            OnClickAskConfirmAction = () => {
                //* リワード広告
                AdmobManager._.ProcessRewardAd(() => {
                    HM._.rwlm.ShowReward(new List<RewardItem>() {
                        new (HM._.rwlm.RwdItemDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.GoldKey], 3)
                    });
                });
                DM._.DB.GoldkeyFreeAdCnt--;
            };
        }
        //* 一日広告を全部見たら
        else {
            ShowAgainAskMsg("1일치 무료광고를 다봤습니다.\n상점구매로 이동하시겠습니까?");

            //* 確認ボタン イベント登録
            OnClickAskConfirmAction = () => {
                //* SHOPに移動
                HM._.shopMg.InitUI();
                HM._.shopMg.OnClickShopIconBtnAtHome();
                HM._.shopMg.OnClickTapBtn(ShopManager.ETC_CHEST);
            };
        }
    }
    public void OnClickTopNavDiamondPlusBtn() {
        HM._.shopMg.InitUI();
        HM._.shopMg.OnClickShopIconBtnAtHome();
        HM._.shopMg.OnClickTapBtn(ShopManager.TAPBTN_RSC);
    }
    public void OnClickTopNavCoinPlusBtn() {
        HM._.shopMg.InitUI();
        HM._.shopMg.OnClickShopIconBtnAtHome();
        HM._.shopMg.OnClickTapBtn(ShopManager.TAPBTN_RSC);
    }
#endregion

#region FUNC
    /// <summary>
    /// トップ右の財貨NavBarのレイアー変更
    /// </summary>
    /// <param name="isLocateFront"></param> <summary>
    public void SetTopNavOrderInLayer(bool isLocateFront) {
        TopNavCanvas.sortingOrder = isLocateFront? 101 : 99;
    }
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
