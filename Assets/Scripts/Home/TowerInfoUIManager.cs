using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public struct TowerCardStatusUI {
    [field:SerializeField] public GameObject Obj {get; set;}
    [field:HideInInspector] public TMP_Text DmgTxt {get; set;}
    [field:HideInInspector] public TMP_Text SpdTxt {get; set;}
    [field:HideInInspector] public TMP_Text RangeTxt {get; set;}

    public void InitObj() {
        if(Obj == null) return;
        //* Status Text UI 初期化
        Transform tf = Obj.transform;
        Transform lastTf = tf.GetChild(tf.childCount - 1);
        Transform lastTf1 = tf.GetChild(tf.childCount - 2);
        Transform lastTf2 = tf.GetChild(tf.childCount - 3);
        Debug.Log($"InitObj():: lastTf= {lastTf.name}, lastTf1= {lastTf1.name}, lastTf2= {lastTf2.name}");
        RangeTxt = lastTf.GetComponentInChildren<TMP_Text>();
        SpdTxt = lastTf1.GetComponentInChildren<TMP_Text>();
        DmgTxt = lastTf2.GetComponentInChildren<TMP_Text>();
    }
    public void SetUI(SettingTowerData towerDt) {
        DmgTxt.text = towerDt.Dmg.ToString();
        SpdTxt.text = towerDt.AtkSpeed.ToString();
        RangeTxt.text = towerDt.AtkRange.ToString();
    }
}

public class TowerInfoUIManager : MonoBehaviour {
    [field:Header("TOWER DATA SO")]
    [field:SerializeField] public SettingTowerData[] WarriorTowerDts {get; private set;}
    [field:SerializeField] public SettingTowerData[] ArcherTowerDts {get; private set;}
    [field:SerializeField] public SettingTowerData[] MagicianTowerDts {get; private set;}

    [field:Header("UI ELEMENT")]
    [field:SerializeField] public GameObject WindowObj {get; private set;}
    [field:SerializeField] public TowerCardStatusUI[] WarriorCardStatusUIs {get; private set;}
    [field:SerializeField] public TowerCardStatusUI[] ArcherCardStatusUIs {get; private set;}
    [field:SerializeField] public TowerCardStatusUI[] MagicianCardStatusUIs {get; private set;}

    [field:Header("TOWER SKILL DETAIL INFO")]
    [field: SerializeField] public TowerSkill[] TowerSkills;
    [field: SerializeField] public GameObject InfoPopUp {get; set;}
    
    void Start() {
        int i = 0;
        Array.ForEach(WarriorCardStatusUIs, towerCardSttUI => {
            towerCardSttUI.InitObj();
            towerCardSttUI.SetUI(WarriorTowerDts[i++]);
        });
        i = 0;
        Array.ForEach(ArcherCardStatusUIs, towerCardSttUI => {
            towerCardSttUI.InitObj();
            towerCardSttUI.SetUI(ArcherTowerDts[i++]);
        });
        i = 0;
        Array.ForEach(MagicianCardStatusUIs, towerCardSttUI => {
            towerCardSttUI.InitObj();
            towerCardSttUI.SetUI(MagicianTowerDts[i++]);
        });

        //*  初期化：最初にスキルフレーム 非表示
        Array.ForEach(TowerSkills, Icon => Icon.DetailPopUpFrame.SetActive(false));
    }

    #region EVENT
        public void OnClickTowerInfoIconBtnAtHome() {
            SM._.SfxPlay(SM.SFX.ClickSFX);
            WindowObj.SetActive(true);
        }
        public void OnClickBackBtn() {
            SM._.SfxPlay(SM.SFX.ClickSFX);
            WindowObj.SetActive(false);
        }


        public void OnClickWarriorTowerInfoCard(int lv) {
            Array.ForEach(TowerSkills, Icon => Icon.DetailPopUpFrame.SetActive(false));
            InfoPopUp.SetActive(true);
            //* レベル３から、一つずつ増やしてスキル表示
            int maxSkillsToUpdate = Mathf.Min(lv - 2, TowerSkills.Length);
            for(int i = 0; i < maxSkillsToUpdate; i++) {
                TowerSkills[i].DetailPopUpFrame.SetActive(true);
                TowerSkills[i].UpdateUI(TowerKind.Warrior, lv, i++);
            }
        }
        public void OnClickArcherTowerInfoCard(int lv) {
            Array.ForEach(TowerSkills, Icon => Icon.DetailPopUpFrame.SetActive(false));
            InfoPopUp.SetActive(true);
            //* レベル３から、一つずつ増やしてスキル表示
            int maxSkillsToUpdate = Mathf.Min(lv - 2, TowerSkills.Length);
            for(int i = 0; i < maxSkillsToUpdate; i++) {
                TowerSkills[i].DetailPopUpFrame.SetActive(true);
                TowerSkills[i].UpdateUI(TowerKind.Archer, lv, i++);
            }
        }
        public void OnClickMagicianTowerInfoCard(int lv) {
            Array.ForEach(TowerSkills, Icon => Icon.DetailPopUpFrame.SetActive(false));
            InfoPopUp.SetActive(true);
            //* レベル３から、一つずつ増やしてスキル表示
            int maxSkillsToUpdate = Mathf.Min(lv - 2, TowerSkills.Length);
            for(int i = 0; i < maxSkillsToUpdate; i++) {
                TowerSkills[i].DetailPopUpFrame.SetActive(true);
                TowerSkills[i].UpdateUI(TowerKind.Magician, lv, i++);
            }
        }
        public void OnClickDetailSkillInfoCloseBtn() {
            SM._.SfxPlay(SM.SFX.ClickSFX);            
            InfoPopUp.SetActive(false);
        }
    #endregion
    #region FUNC
    #endregion
}