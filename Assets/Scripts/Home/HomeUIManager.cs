using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using JetBrains.Annotations;
using Inventory.Model;
using System.Linq;
using Random = UnityEngine.Random;

/// <summary>
/// Lobby
/// </summary>
public class HomeUIManager : MonoBehaviour {
    Coroutine CorMsgNoticeID;
    Coroutine CorMsgErrorID;
    public Action OnClickAskConfirmAction;
    public Action OnClickAskCloseExtraAction;

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
    [field: SerializeField] public GameObject[] CloverActiveIconArr {get; set;}
    [field: SerializeField] public GameObject[] GoldCloverActiveIconArr {get; set;}
    [field: SerializeField] public bool IsActivePopUp {get; set;}

    public Button PlayBtn {get; set;}

    [Header("ERROR MSG POPUP")]
    public GameObject TitlePopUp;

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
    public GameObject YesBtnObj;
    public GameObject NoBtnObj;

    [Header("THANKS FOR PLAYING POPUP")]
    public GameObject ThanksForPlayingPopUp;

    [Header("RECOVER INV DATA POPUP")]
    public GameObject RecoverInvDataNoticePopUp;
    public TextMeshProUGUI RecoverInvDataMsgTxt;

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

        const int OFF = 0, ON = 1;
        CloverActiveIconArr[OFF].SetActive(!DM._.DB.IsCloverActive);
        CloverActiveIconArr[ON].SetActive(DM._.DB.IsCloverActive);
        GoldCloverActiveIconArr[OFF].SetActive(!DM._.DB.IsGoldCloverActive);
        GoldCloverActiveIconArr[ON].SetActive(DM._.DB.IsGoldCloverActive);

        //* アプリのコメント要求
        if(HM._.Fame >= 3 && !DM._.DB.IsThanksForPlaying) {
            DM._.DB.IsThanksForPlaying = true;
            ThanksForPlayingPopUp.SetActive(true);
        }

    }

#region EVENT
    public void OnClickTitleGamePlayBtn() {
        SM._.SfxPlay(SM.SFX.Merge1SFX);
        TitlePopUp.gameObject.SetActive(false);
    }
    public void OnClickThanksForPlayingConfirmBtn() {
        const string NOTION_URL = "https://play.google.com/store/apps/details?id=com.frontooth.AStarDefence";
        Application.OpenURL(NOTION_URL);
        ThanksForPlayingPopUp.SetActive(false);

        //* Reward Diamond
        var rewardList = new List<RewardItem> {
            new (HM._.rwlm.RwdItemDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Diamond], 150)
        };

        HM._.rwlm.ShowReward(rewardList);
    }

    public void OnClickCloverToggleIconBtn() {
        InventorySO invDt = HM._.ivCtrl.InventoryData;
        int findIndex = Array.FindIndex(invDt.InvArr, item => item.Data?.name == Etc.ConsumableItem.Clover.ToString());
        Debug.Log($"OnClickCloverToggleIconBtn():: find Index= {findIndex}");

        //* クロバー活性化
        if(findIndex != -1) {
            HM._.ivm.ConsumePopUp.SetActive(true);
            // InventoryItem invItem = invDt.GetItemAt(findIndex);
            // ItemSO item = invItem.Data;
            HM._.ivm.OnDescriptionRequested?.Invoke(findIndex); // HM._.ivm.UpdateDescription(findIndex, item, invItem.Quantity, invItem.Lv, invItem.RelicAbilities, invItem.IsEquip);
        }
        else {
            ShowAgainAskMsg("클로버가 없습니다.\n일반 및 황금상자에서 획득합니다.\n<color=blue>상점으로 이동하시겠습니까?");

            //* 確認ボタン イベント登録
            OnClickAskConfirmAction = () => {
                //* SHOPに移動
                HM._.shopMg.ShowShopAtTapBtn(ShopManager.TAPBTN_CHEST);
            };
        }
    }
    public void OnClickGoldCloverToggleIconBtn() {
        InventorySO invDt = HM._.ivCtrl.InventoryData;
        int findIndex = Array.FindIndex(invDt.InvArr, item => item.Data?.name == Etc.ConsumableItem.GoldClover.ToString());
        Debug.Log($"OnClickGoldCloverToggleIconBtn():: find Index= {findIndex}");

        //* クロバー活性化
        if(findIndex != -1) {
            HM._.ivm.ConsumePopUp.SetActive(true);
            // InventoryItem invItem = invDt.GetItemAt(findIndex);
            // ItemSO item = invItem.Data;
            HM._.ivm.OnDescriptionRequested?.Invoke(findIndex); // HM._.ivm.UpdateDescription(findIndex, item, invItem.Quantity, invItem.Lv, invItem.RelicAbilities, invItem.IsEquip);
        }
        else {
            ShowAgainAskMsg("골드클로버가 없습니다.\n일반 및 황금상자에서 획득합니다.\n<color=blue>상점으로 이동하시겠습니까?");

            //* 確認ボタン イベント登録
            OnClickAskConfirmAction = () => {
                //* SHOPに移動
                HM._.shopMg.ShowShopAtTapBtn(ShopManager.TAPBTN_CHEST);
            };
        }
    }

    public void OnClickPlayBtn() {
        SM._.SfxPlay(SM.SFX.ClickSFX);
        HM._.stgm.StageGroup.SetActive(true);
        for(int i = 0; i < HM._.stgm.StagePopUps.Length; i++) {
            HM._.stgm.StagePopUps[i].SetActive(HM._.SelectedStageIdx == i);
        }

        //* StageSignGroup表示
        HM._.stgm.UpdateStageUnlockSignGroup();

        //* Tip Message
        int randTipIdx = Random.Range(0, HM._.stgm.TipMsgArr.Length);
        for(int i = 0; i < HM._.stgm.TipMsgArr.Length; i++) {
            HM._.stgm.TipMsgArr[i].SetActive(i == randTipIdx);
        }
        

        //! ステージPopUpが表示しないバグあり対応
        //* 全てのステージが非表示なら
        if(Array.TrueForAll(HM._.stgm.StagePopUps, popUp => popUp.activeSelf == false)) {
            //* 最初のステージを表示
            HM._.SelectedStageIdx = 0;
            HM._.stgm.StagePopUps[0].SetActive(true);
        }

        //* 以前にプレイしたステージデータ ロード
        if(DM._.DB.StageTileMapSaveDt.IsSaved) {
            var saveDt = DM._.DB.StageTileMapSaveDt;

            string stageName = (saveDt.Stage == Config.Stage.STG1_FOREST)? "초원"
                : (saveDt.Stage == Config.Stage.STG2_DESERT)? "사막"
                : (saveDt.Stage == Config.Stage.STG3_SEA)? "바다"
                : (saveDt.Stage == Config.Stage.STG4_UNDEAD)? "언데드"
                : (saveDt.Stage == Config.Stage.STG5_HELL)? "지옥"
                : "스테이지";

            string msg = "게임을 이어서 하시겠습니까?"
                + $"\n<size=85%>( {stageName}스테이지 {saveDt.Stage + 1}-{(int)saveDt.StageNum + 1}, 웨이브{Mathf.Max(1, saveDt.Wave)} )</size>";
            HM._.hui.ShowAgainAskMsg(msg, isActiveNoBtn: true);

            //* YES
            HM._.hui.OnClickAskConfirmAction = () => {
                DM._.DB.StageTileMapSaveDt.LoadStageValDt();
                HM._.stgm.OnClickPlayBtn(isLoadSaveDt: true);
            };
        }
    }

    public void OnClickTopNavGoldKeyPlusBtn() {
        //* 広告閲覧数が残ったら
        if(DM._.DB.GoldkeyFreeAdCnt > 0) {
            ShowAgainAskMsg(
                "<color=yellow>황금열쇠</color>가 필요하신가요?"
                + "\n<sprite name=Ad>광고시청 후 <color=yellow>무료 3개</color> 지급!"
                + $"\n<color=green>(하루 남은 횟수 : {DM._.DB.GoldkeyFreeAdCnt})</color>"
                + "\n<color=blue><size=65%>*상점에서도 구매가 가능합니다.</size></color>"
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
                HM._.shopMg.ShowShopAtTapBtn(ShopManager.TAPBTN_ETC);
            };
        }
    }
    public void OnClickTopNavDiamondPlusBtn() {
        HM._.shopMg.ShowShopAtTapBtn(ShopManager.TAPBTN_RSC);
    }
    public void OnClickTopNavCoinPlusBtn() {
        HM._.shopMg.ShowShopAtTapBtn(ShopManager.TAPBTN_RSC);
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
        Debug.Log($"<color=red>ShowMsgError():: msg= {msg}</color>");
        if(CorMsgErrorID != null) 
            StopCoroutine(CorMsgErrorID);
        SM._.SfxPlay(SM.SFX.ErrorSFX);
        CorMsgErrorID = StartCoroutine(CoShowMsgError(msg));
    }
    IEnumerator CoShowMsgError(string msg) {
        TopMsgError.SetActive(true);
        MsgErrorTxt.text = msg;
        yield return Util.Time2;
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
    public void ShowAgainAskMsg(string msg = "", bool isActiveNoBtn = false, string noBtnTxt = "아니오") {
        NoBtnObj.SetActive(isActiveNoBtn);
        NoBtnObj.GetComponentInChildren<TMP_Text>().text = noBtnTxt;
        AgainAskPopUp.SetActive(true);
        AgainAskMsgTxt.text = msg;
    }
    public void OnClickAgainAskPopUpConfirmBtn() {
        AgainAskPopUp.SetActive(false);
        OnClickAskConfirmAction?.Invoke();
    }
    public void OnClickAgainAskPopUpCloseBtn_ExtraAction() {
        AgainAskPopUp.SetActive(false);
        OnClickAskCloseExtraAction?.Invoke();
    }
#endregion
}
