using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class MiningTimeUIManager : MonoBehaviour {
    [field: SerializeField] public GameObject SliderBtn {get; set;}
    [field: SerializeField] public Slider Slider {get; set;}
    [field: SerializeField] public Animator SliderBtnAnim {get; set;}
    [field: SerializeField] public TextMeshProUGUI SliderTxt {get; set;}
    [field: SerializeField] public TextMeshProUGUI MiningSpdTxt {get; set;}
    [field: SerializeField] public Image IconFrameImg {get; set;}
    [field: SerializeField] public GameObject RewardAuraEF {get; set;}

#region EVENT
    /// <summary>
    /// 時間がたったら、Miningリワード習得
    /// </summary>
    public void OnClickTimerSliderBtn() {
        WorkSpace curWS = HM._.wsm.CurWorkSpace;

        //* 採掘(仕事)が終わったら
        if(curWS.IsFinishWork) {
            HM._.wsm.AcceptReward();
            DM._.DB.DailyMissionDB.ClearMiningVal++;
        }
        else {
            SM._.SfxPlay(SM.SFX.ClickSFX);

            //* 採掘(仕事)中なら
            if(curWS.GoblinSpotDt.IsActive && curWS.OreSpotDt.IsActive) {
                HM._.hui.ShowAgainAskMsg("광고를 시청하고 남은시간의 절반을 줄이겠습니까?\n<color=blue><size=70%>시간이 20분 미만으로 남았다면 바로 완료합니다!");
                HM._.hui.OnClickAskConfirmAction = () => {
                    AdmobManager._.ProcessRewardAd(() => {
                        SM._.SfxPlay(SM.SFX.CompleteSFX);
                        const int TWENTY_MINUTES = 6000;

                        //* 採掘の時間減る
                        if(curWS.MiningTime > TWENTY_MINUTES)
                            curWS.MiningTime /= 2;
                        else
                            curWS.MiningTime = 0;
                    });
                };
            }
            //* 仕事していない場合
            else { 
                HM._.wsm.OnClickOreSpotBtn();
            }
        }
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
