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
    [field: SerializeField] public Transform SkillLvPointOnTf {get; set;}
    [field: SerializeField] public Image DetailPopUpIconImg {get; set;}
    [field: SerializeField] public TextMeshProUGUI DetailPopUpNameTxt {get; set;}
    [field: SerializeField] public TextMeshProUGUI DetailPopUpInfoTxt {get; set;}

    public void UpdateUI(TowerKind kind, int lv, int skillFrameIdx) {
        Debug.Log($"UpdateUI():: TowerKind= {kind}, lv= {lv}");
        //* Actionバー右上にアイコン表示
        
        if(IconObj) IconObj.SetActive(true);
        if(IconImg) IconImg.sprite = SkillSprs[(int)kind];

        //* Detail PopUp
        DetailPopUpIconImg.sprite = SkillSprs[(int)kind];

        DetailPopUpNameTxt.text = TitleArr[(int)kind];

        //* Skill Level Point UI
        // 初期化
        for(int i = 0; i < SkillLvPointOnTf.childCount; i++) 
            SkillLvPointOnTf.GetChild(i).gameObject.SetActive(false);
        // 適用
        switch(lv) {
            case 1:
            case 2:
                // なし
                break;
            case 3:
                if(skillFrameIdx == 0) {
                    SkillLvPointOnTf.GetChild(0).gameObject.SetActive(true);
                }
                break;
            case 4:
                if(skillFrameIdx == 0) {
                    SkillLvPointOnTf.GetChild(0).gameObject.SetActive(true);
                    SkillLvPointOnTf.GetChild(1).gameObject.SetActive(true);
                }
                if(skillFrameIdx == 1) {
                    SkillLvPointOnTf.GetChild(0).gameObject.SetActive(true);
                }
                break;
            case 5:
                if(skillFrameIdx == 0) {
                    SkillLvPointOnTf.GetChild(0).gameObject.SetActive(true);
                    SkillLvPointOnTf.GetChild(1).gameObject.SetActive(true);
                    SkillLvPointOnTf.GetChild(2).gameObject.SetActive(true);
                }
                if(skillFrameIdx == 1) {
                    SkillLvPointOnTf.GetChild(0).gameObject.SetActive(true);
                    SkillLvPointOnTf.GetChild(1).gameObject.SetActive(true);
                }
                if(skillFrameIdx == 2) {
                    SkillLvPointOnTf.GetChild(0).gameObject.SetActive(true);
                }
                break;
            case 6:
                if(skillFrameIdx == 0) {
                    SkillLvPointOnTf.GetChild(0).gameObject.SetActive(true);
                    SkillLvPointOnTf.GetChild(1).gameObject.SetActive(true);
                    SkillLvPointOnTf.GetChild(2).gameObject.SetActive(true);
                    SkillLvPointOnTf.GetChild(3).gameObject.SetActive(true);
                }
                if(skillFrameIdx == 1) {
                    SkillLvPointOnTf.GetChild(0).gameObject.SetActive(true);
                    SkillLvPointOnTf.GetChild(1).gameObject.SetActive(true);
                    SkillLvPointOnTf.GetChild(2).gameObject.SetActive(true);
                }
                if(skillFrameIdx == 2) {
                    SkillLvPointOnTf.GetChild(0).gameObject.SetActive(true);
                    SkillLvPointOnTf.GetChild(1).gameObject.SetActive(true);
                }
                if(skillFrameIdx == 3) {
                    SkillLvPointOnTf.GetChild(0).gameObject.SetActive(true);
                }
                break;
        }

        int lvIdx = lv - 1;
        string infoMsg = InfoArr[(int)kind];
        switch(kind) {
            case TowerKind.Warrior:
                if(skillFrameIdx == 0) {
                    infoMsg = infoMsg.Replace("N1", $"{WarriorTower.SK1_RageActivePers[lvIdx]}");
                    infoMsg = infoMsg.Replace("N2", $"{WarriorTower.SK1_RageDmgSpdIncPers[lvIdx] * 100}");
                    infoMsg = infoMsg.Replace("N3", $"{WarriorTower.SK1_RageTime[lvIdx]}");
                }
                else if(skillFrameIdx == 1) {
                    infoMsg = infoMsg.Replace("N1", $"{WarriorTower.SK2_WheelwindActivePers[lvIdx]}");
                    infoMsg = infoMsg.Replace("N2", $"{WarriorTower.SK2_WheelwindDmgPers[lvIdx] * 100}");
                }
                else if(skillFrameIdx == 2) {
                    infoMsg = infoMsg.Replace("N1", $"{WarriorTower.SK3_CheerUpSpans[lvIdx]}");
                    infoMsg = infoMsg.Replace("N2", $"{WarriorTower.SK3_CheerUpDmgSpdIncPers[lvIdx] * 100}");
                }
                else if(skillFrameIdx == 3) {
                    infoMsg = infoMsg.Replace("N1", $"{WarriorTower.SK4_RoarSpans[lvIdx]}");
                    infoMsg = infoMsg.Replace("N2", $"{WarriorTower.SK4_RoarDmgPers[lvIdx] * 100}");
                }
                break;
            case TowerKind.Archer:
                if(skillFrameIdx == 0) {
                    infoMsg = infoMsg.Replace("N", $"{ArcherTower.SK1_CritIncPers[lvIdx] * 100}");
                }
                else if(skillFrameIdx == 1) {
                    infoMsg = infoMsg.Replace("N1", $"{ArcherTower.SK2_MultiShotActivePers[lvIdx]}");
                    infoMsg = infoMsg.Replace("N2", $"{ArcherTower.SK2_MultiShotCnts[lvIdx]}");
                }
                else if(skillFrameIdx == 2) {
                    infoMsg = infoMsg.Replace("N1", $"{ArcherTower.SK3_PassShotSpans[lvIdx]}");
                    infoMsg = infoMsg.Replace("N2", $"{ArcherTower.SK3_PassShotDmgPers[lvIdx] * 100}");
                }
                else if(skillFrameIdx == 3) {
                    infoMsg = infoMsg.Replace("N", $"{ArcherTower.SK4_ArrowRainSpans[lvIdx]}");
                }
                break;
            case TowerKind.Magician:
                if(skillFrameIdx == 0) {
                    infoMsg = infoMsg.Replace("N", $"{MagicianTower.SK1_ExplosionLvActivePers[lvIdx]}");
                }
                else if(skillFrameIdx == 1) {
                    infoMsg = infoMsg.Replace("N1", $"{MagicianTower.SK2_MagicCircleActivePers[lvIdx]}");
                    infoMsg = infoMsg.Replace("N2", $"{MagicianTower.SK2_MagicCircleDmgPers[lvIdx] * 100}");
                }
                else if(skillFrameIdx == 2) {
                    infoMsg = infoMsg.Replace("N1", $"{MagicianTower.SK3_LaserSpans[lvIdx]}");
                    infoMsg = infoMsg.Replace("N2", $"{MagicianTower.SK3_LaserDmgPers[lvIdx] * 100}");
                }
                else if(skillFrameIdx == 3) {
                    infoMsg = infoMsg.Replace("N1", $"{MagicianTower.SK4_BigbangSpans[lvIdx]}");
                    infoMsg = infoMsg.Replace("N2", $"{MagicianTower.SK4_BigbangDmgs[lvIdx] * 100}");
                }
                break;
        }

        DetailPopUpInfoTxt.text = infoMsg;
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
            int maxSkillsToUpdate = Mathf.Min(tower.Lv - 2, TowerSkills.Length);
            for (int i = 0; i < maxSkillsToUpdate; i++) {
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
            int lv = int.Parse(states[0]);
            string type = states[1];
            TowerKind kind = (states[2] == "Warrior")? TowerKind.Warrior : (states[2] == "Archer")? TowerKind.Archer : (states[2] == "Magician")? TowerKind.Magician : TowerKind.None;
            Debug.Log($"ShowTowerStateUI():: lv= {lv}, type= {type}, kind= {kind}");

            //* 非表示 初期化
            Array.ForEach(TowerSkills, Icon => {
                Icon.IconObj.SetActive(false);
                Icon.DetailPopUpFrame.SetActive(false);
            });

            //* Skill Icons 表示 (レベル３から表示を開始)
            if(type == TowerType.Random.ToString()) {
                int maxSkillsToUpdate = Mathf.Min(lv - 2, TowerSkills.Length);
                for (int i = 0; i < maxSkillsToUpdate; i++) {
                    TowerSkills[i].UpdateUI(kind, lv, i);
                }
            }

            //* GradeLabelGroup 情報表示
            for(int i = 0; i < GradeLabelGroup.childCount; i++) {
                GradeLabelGroup.GetChild(i).gameObject.SetActive(i == (lv - 1));
            }

            //* 情報表示
            DmgTxt.text = states[3];
            AtkSpeedTxt.text = states[4];
            AtkRangeTxt.text = states[5];
            CritPerTxt.text = states[6];
            CritDmgPerTxt.text = states[7];
            SlowPerTxt.text = states[8];
            StunSecTxt.text = states[9];

            WindowObj.SetActive(true);
        }
    #endregion

}
