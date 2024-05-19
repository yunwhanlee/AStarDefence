using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class MiningTimeUIManager : MonoBehaviour {
    const int PRICE_UNIT = 10;

    [field: SerializeField] public GameObject SliderBtn {get; set;}
    [field: SerializeField] public Slider Slider {get; set;}
    [field: SerializeField] public Animator SliderBtnAnim {get; set;}
    [field: SerializeField] public TextMeshProUGUI SliderTxt {get; set;}
    [field: SerializeField] public TextMeshProUGUI MiningSpdTxt {get; set;}
    [field: SerializeField] public Image IconFrameImg {get; set;}
    [field: SerializeField] public GameObject RewardAuraEF {get; set;}

    [field: Header("Finish Ask PopUp")]
    [field: SerializeField] int FinishPrice {get; set;}
    [field: SerializeField] public GameObject MiningFinishAskPopUp {get; set;}
    [field: SerializeField] public TMP_Text FinishPriceTxt {get; set;}
    [field: SerializeField] public TMP_Text AdLimitCntTxt {get; set;}

#region EVENT
    /// <summary>
    /// 時間がたったら、Miningリワード習得
    /// </summary>
    public void OnClickTimerSliderBtn() {
        FinishPrice = 0; //* 初期化
        WorkSpace curWS = HM._.wsm.CurWorkSpace;

        //* 採掘(仕事)が終わったら
        if(curWS.IsFinishWork) {
            HM._.wsm.AcceptReward();
            DM._.DB.DailyMissionDB.ClearMiningVal++;
        }
        else {
            //* 採掘(仕事)中なら
            if(curWS.GoblinSpotDt.IsActive && curWS.OreSpotDt.IsActive) {
                SM._.SfxPlay(SM.SFX.ClickSFX);
                MiningFinishAskPopUp.SetActive(true);

                int divideToMiniteCnt = curWS.MiningTime / 60;
                FinishPrice = divideToMiniteCnt * PRICE_UNIT;
                FinishPriceTxt.text = $"<sprite name=Diamond>{FinishPrice}";
                AdLimitCntTxt.text = $"하루 남은 횟수 : {DM._.DB.MiningFreeAdCnt}";
            }
            //* 仕事していない場合
            else { 
                HM._.wsm.OnClickGoblinLeftSpotBtn();
            }
        }
    }

    public void OnClickFinishWorkBtn() {
        if(HM._.Diamond < FinishPrice) {
            HM._.hui.ShowMsgError("다이아몬드가 부족합니다.");
            return;
        }
        WorkSpace curWS = HM._.wsm.CurWorkSpace;

        //* 採掘完了
        MiningFinishAskPopUp.SetActive(false);
        SM._.SfxPlay(SM.SFX.CompleteSFX);
        HM._.Diamond -= FinishPrice;
        curWS.MiningTime = 0;
    }

    public void OnClickCooldownAdBtn() {
        if(DM._.DB.MiningFreeAdCnt <= 0) {
            HM._.hui.ShowMsgError("1일 광고횟수를 다 사용했습니다.");
            return;
        }

        DM._.DB.MiningFreeAdCnt--;

        //* AD
        WorkSpace curWS = HM._.wsm.CurWorkSpace;
        AdmobManager._.ProcessRewardAd(() => {
            const int HALF_HOUR = 1800;
            //* 採掘の時間減る
            if(curWS.MiningTime > HALF_HOUR) {
                SM._.SfxPlay(SM.SFX.LevelUpSFX);
                curWS.MiningTime -= HALF_HOUR;
            }
            else {
                SM._.SfxPlay(SM.SFX.CompleteSFX);
                curWS.MiningTime = 0;
            }

            MiningFinishAskPopUp.SetActive(false);
        });
    }
#endregion

#region FUNC
    public void InitSlider() {
        WorkSpace curWS = HM._.wsm.CurWorkSpace;
        SetTimer(isOn: false);
        string notice = curWS.OreSpotDt.IsActive? "고블린등록 필요" : "광석등록 필요";
        SetTimerSlider(notice, 0);
    }

    public void SetTimer(bool isOn = false) {
        //* ゴブリン採掘速度％ UI 表示
        int lvIdx = HM._.wsm.CurWorkSpace.GoblinSpotDt.LvIdx;
        IconFrameImg.color = isOn? HM._.RedOrangeColor : HM._.SkyBlueColor;
        MiningSpdTxt.text = isOn? $"{HM._.mnm.GoblinDataSO.Datas[lvIdx].SpeedPer * 100}%" : "";
    }

    public void SetTimerSlider(string timeTxt, float value) {
        Debug.Log($"SetTimerSlider({timeTxt}, {value}):: ");
        SliderTxt.text = $"{timeTxt}";
        Slider.value = value;
    }
#endregion
}
