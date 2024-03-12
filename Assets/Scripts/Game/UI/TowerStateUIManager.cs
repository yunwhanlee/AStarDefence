using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class TowerSkill {
    
    [field: SerializeField] public string Name {get; set;}
    [field: SerializeField] public string[] TitleArr {get; set;} // 0: Warrior, 1: Archer, 2: Magician
    [field: SerializeField] public string[] InfoArr {get; set;} // 0: Warrior, 1: Archer, 2: Magician

    //* アクションバー左上
    [field: SerializeField] public GameObject IconObj {get; set;}
    [field: SerializeField] public Image IconImg {get; set;}
    [field: SerializeField] public Sprite[] SkillSprs {get; set;}

    //* Tower Status Info PopUp用
    [field: SerializeField] public GameObject DetailPopUpFrame {get; set;}
    [field: SerializeField] public Image DetailPopUpIconImg {get; set;}
    [field: SerializeField] public TextMeshProUGUI DetailPopUpNameTxt {get; set;}
    [field: SerializeField] public TextMeshProUGUI DetailPopUpInfoTxt {get; set;}

    public void UpdateUI(Tower tower) {
        IconObj.SetActive(true);
        IconImg.sprite = SkillSprs[(int)tower.Kind];

        DetailPopUpIconImg.sprite = SkillSprs[(int)tower.Kind];
        DetailPopUpNameTxt.text = TitleArr[(int)tower.Kind];
        DetailPopUpInfoTxt.text = InfoArr[(int)tower.Kind];
    }
}

public class TowerStateUIManager : MonoBehaviour {
    [field: SerializeField] public GameObject WindowObj {get; set;}
    [field: SerializeField] public GameObject InfoPopUp {get; set;}

    //* Skills
    [field: SerializeField] public TowerSkill[] TowerSkills;

    //* Status
    [field: SerializeField] Transform GradeLabelGroup {get; set;}
    [field: SerializeField] TextMeshProUGUI DmgTxt {get; set;}
    [field: SerializeField] TextMeshProUGUI AtkSpeedTxt {get; set;}
    [field: SerializeField] TextMeshProUGUI AtkRangeTxt {get; set;}
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
            Tower tower = GM._.tmc.HitObject.GetComponentInChildren<Tower>();

            //* レベル３から、一つずつ増やしてスキル表示
            for (int i = 0; i < TowerSkills.Length && i < tower.Lv - 2; i++) {
                TowerSkills[i].DetailPopUpFrame.SetActive(true);
            }
        }
        public void OnClickInfoPopUpCloseBtn() {
            GM._.gui.Play();
            InfoPopUp.SetActive(false);
        }
    #endregion

    #region FUNC
        public void ShowTowerStateUI(string[] states) {
            Debug.Log($"ShowTowerStateUI():: lv= {states[0]}");

            Tower tower = GM._.tmc.HitObject.GetComponentInChildren<Tower>();

            //* 非表示 初期化
            Array.ForEach(TowerSkills, Icon => {
                Icon.IconObj.SetActive(false);
                Icon.DetailPopUpFrame.SetActive(false);
            });

            //* Skill Icons 表示
            int lv = int.Parse(states[0]);
            if(tower.Type == TowerType.Random) {
                if(lv == 3) {
                    TowerSkills[0].UpdateUI(tower);
                }
                if(lv == 4) {
                    TowerSkills[0].UpdateUI(tower);
                    TowerSkills[1].UpdateUI(tower);
                }
                if(lv == 5) {
                    TowerSkills[0].UpdateUI(tower);
                    TowerSkills[1].UpdateUI(tower);
                    TowerSkills[2].UpdateUI(tower);                    
                }
                if(lv == 6) {
                    TowerSkills[0].UpdateUI(tower);
                    TowerSkills[1].UpdateUI(tower);
                    TowerSkills[2].UpdateUI(tower);
                    TowerSkills[3].UpdateUI(tower);
                }
            }

            //* GradeLabelGroup 情報表示
            for(int i = 0; i < GradeLabelGroup.childCount; i++) {
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
