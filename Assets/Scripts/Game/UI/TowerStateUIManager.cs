using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TowerStateUIManager : MonoBehaviour {
    [field: SerializeField] public GameObject WindowObj {get; set;}
    [field: SerializeField] Transform GradeLabelGroup {get; set;}
    [field: SerializeField] TextMeshProUGUI DmgTxt {get; set;}
    [field: SerializeField] TextMeshProUGUI AtkSpeedTxt {get; set;}
    [field: SerializeField] TextMeshProUGUI AtkRangeTxt {get; set;}
    [field: SerializeField] TextMeshProUGUI SplashRangeTxt {get; set;}
    [field: SerializeField] TextMeshProUGUI CritPerTxt {get; set;}
    [field: SerializeField] TextMeshProUGUI CritDmgPerTxt {get; set;}
    [field: SerializeField] TextMeshProUGUI SlowPerTxt {get; set;}
    [field: SerializeField] TextMeshProUGUI StunSecTxt {get; set;}

    void Start() {
        WindowObj.SetActive(false);
    }

    public void ShowTowerStateUI(string[] states) {
        Debug.Log($"ShowTowerStateUI():: lv= {states[0]}");
        //* GradeLabelGroup
        for(int i = 0; i < GradeLabelGroup.childCount; i++) {
            int lv = int.Parse(states[0]);
            GradeLabelGroup.GetChild(i).gameObject.SetActive(i == (lv - 1));
        }

        //* 情報表示
        DmgTxt.text = states[1];
        AtkSpeedTxt.text = states[2];
        AtkRangeTxt.text = states[3];
        SplashRangeTxt.text = states[4];
        CritPerTxt.text = states[5];
        CritDmgPerTxt.text = states[6];
        SlowPerTxt.text = states[7];
        StunSecTxt.text = states[8];

        WindowObj.SetActive(true);
    }
}
