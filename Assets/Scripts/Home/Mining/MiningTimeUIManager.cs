using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class MiningTimeUIManager : MonoBehaviour {
    [field: SerializeField] public Slider Slider {get; set;}
    [field: SerializeField] public TextMeshProUGUI SliderTxt {get; set;}
    [field: SerializeField] public TextMeshProUGUI MiningSpdTxt {get; set;}
    [field: SerializeField] public Image IconFrameImg {get; set;}
    [field: SerializeField] public GameObject RewardAuraEF {get; set;}

    [field: SerializeField] public bool IsFinish; // リワード収集できる

#region EVENT
    public void OnClickTimerSliderBtn() {
        if(IsFinish) {
            Debug.Log("ACCEPT MINING REWARD!!");
            //* 初期化
            IsFinish = false;
            HM._.mtm.RewardAuraEF.SetActive(false);
            HM._.wsm.GoblinChrCtrl.GoblinStopAnim();
            
            // スライダー UI
            SetTimer(isOn: false);
            SetTimerSlider("광석 등록필요", 0);

            // 鉱石スポット Off
            HM._.wsm.CurWorkSpace.OreSpotDt.Init();
            HM._.wsm.OreSpot.Show(isActive: false);

            // 鉱石カード
            Array.ForEach(HM._.mnm.OreCards, card => card.InitCheck());

            //* リワードPopUp表示
            HM._.rwm.RewardListPopUp.SetActive(true);
        }
        else {
            // TODO) 広告 ➝ ３０分を減る
        }
    }
#endregion

#region FUNC
    public void SetTimer(bool isOn = false) {
        //* ゴブリン採掘速度％ UI 表示
        int lvIdx = HM._.wsm.CurWorkSpace.GoblinSpotDt.LvIdx;
        IconFrameImg.color = isOn? HM._.RedOrangeColor : HM._.SkyBlueColor;
        MiningSpdTxt.text = isOn? $"{HM._.mnm.GoblinDataSO.Datas[lvIdx].SpeedPer * 100}%" : "";
    }

    public void SetTimerSlider(string timeTxt, float value) {
        SliderTxt.text = $"{timeTxt}";
        Slider.value = value;
    }
#endregion
}
