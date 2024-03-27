using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MiningTimeUIManager : MonoBehaviour {
    [field: SerializeField] public Slider Slider {get; set;}
    [field: SerializeField] public TextMeshProUGUI SliderTxt {get; set;}
    [field: SerializeField] public TextMeshProUGUI MiningSpdTxt {get; set;}
    [field: SerializeField] public Image IconFrameImg {get; set;}

    [field: SerializeField] public bool IsFinish; // リワード収集できる

#region EVENT
    public void OnClickTimerSliderBtn() {
        if(IsFinish) {
            IsFinish = false;
            Debug.Log("ACCEPT MINING REWARD!!");
            SetTimer(isOn: false);
        }
        else {
            // TODO) 広告 ➝ ３０分を減る
        }
    }
#endregion

#region FUNC
    public void SetTimer(bool isOn = false) {
        int lvIdx = HM._.wsm.CurWorkSpace.OreSpotDt.LvIdx;

        IconFrameImg.color = isOn? HM._.RedOrangeColor : HM._.SkyBlueColor;
        HM._.mtm.MiningSpdTxt.text = isOn? $"{HM._.mnm.GoblinDataSO.Datas[lvIdx].SpeedPer * 100}%" : "";
    }
#endregion
}
