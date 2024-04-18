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
        }
        else {
            SM._.SfxPlay(SM.SFX.ClickSFX);

            //* 採掘(仕事)中なら
            if(curWS.GoblinSpotDt.IsActive && curWS.OreSpotDt.IsActive) {
                // TODO) 広告 ➝ ３０分を減る
                HM._.hui.ShowAgainAskMsg("광고시청하고 바로 완료하시겠습니까?");
                HM._.hui.OnClickAskConfirmAction = () => {
                    SM._.SfxPlay(SM.SFX.CompleteSFX);
                    curWS.MiningTime = 0;
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
