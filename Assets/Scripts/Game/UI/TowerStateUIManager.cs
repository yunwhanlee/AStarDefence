using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TowerStateUIManager : MonoBehaviour {
    [field: SerializeField] public GameObject WindowObj {get; set;}
    [field: SerializeField] public GameObject InfoPopUp {get; set;}


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

    #region EVENT
        public void OnClickInfoIcon() {
            GM._.gui.Pause();
            InfoPopUp.SetActive(true);
        }
        public void OnClickInfoPopUpCloseBtn() {
            GM._.gui.Play();
            InfoPopUp.SetActive(false);
        }
    #endregion

    #region FUNC
        public void ShowTowerStateUI(string[] states) {
            Debug.Log($"ShowTowerStateUI():: lv= {states[0]}");

            //* GradeLabelGroup 情報表示
            for(int i = 0; i < GradeLabelGroup.childCount; i++) {
                int lv = int.Parse(states[0]);
                GradeLabelGroup.GetChild(i).gameObject.SetActive(i == (lv - 1));
            }

            //* 情報表示
            DmgTxt.text = states[1];
            AtkSpeedTxt.text = states[2];
            AtkRangeTxt.text = states[3];
            CritPerTxt.text = states[4];
            CritDmgPerTxt.text = states[5];
            SlowPerTxt.text = states[6];
            StunSecTxt.text = states[7];

            WindowObj.SetActive(true);
        }
    #endregion

}
