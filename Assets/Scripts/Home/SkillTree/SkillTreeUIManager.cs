using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class SkillTree {
    [field:SerializeField] public bool IsLock {get; private set;}
    [field:SerializeField] public Image IconImg {get; private set;}
    [field:SerializeField] public GameObject Dim {get; private set;}
}

public enum SkillTreeCate { Warrior, Archer, Magician, Utility }

public class SkillTreeUIManager : MonoBehaviour {
    [field:SerializeField] public SkillTreeCate Cate {get; private set;}
    [field:SerializeField] public int CurIdx {get; private set;}

    [field: SerializeField] public Color[] SkillIconBgColors {get; private set;}

    //* Data
    [field:SerializeField] public SettingSkillTreeData WarriorSkillTreeSO {get; private set;}
    [field:SerializeField] public SettingSkillTreeData ArcherSkillTreeSO {get; private set;}
    [field:SerializeField] public SettingSkillTreeData MagicianSkillTreeSO {get; private set;}
    [field:SerializeField] public SettingSkillTreeData UtilitySkillTreeSO {get; private set;}

    //* Element
    [field:SerializeField] public BgPatternController BgPatternCtrl {get; private set;}
    [field:SerializeField] public SkillTree[] WarriorSkillTrees {get; private set;}
    [field:SerializeField] public SkillTree[] ArcherSkillTrees {get; private set;}
    [field:SerializeField] public SkillTree[] MagicianSkillTrees {get; private set;}
    [field:SerializeField] public SkillTree[] UtilitySkillTrees {get; private set;}

    //* UI
    [field:SerializeField] public GameObject WindowObj {get; private set;}
    [field:SerializeField] public TextMeshProUGUI MySkillPointTxt {get; private set;}
    [field:SerializeField] public Image SkillIconBgImg {get; private set;}
    [field:SerializeField] public Image SkillIconImg {get; private set;}
    [field:SerializeField] public TextMeshProUGUI SkillNameTxt {get; private set;}
    [field:SerializeField] public TextMeshProUGUI SkillInfoTxt {get; private set;}
    [field:SerializeField] public TextMeshProUGUI NeededSkillPointTxt {get; private set;}

    void Start() {
        OnClickWarriorSkillTreeBtn(0);
    }

    #region EVENT
        public void OnClickSkillTreeIconBtn() {
            WindowObj.SetActive(true);
        }
        public void OnClickClosePopUpBtn() {
            WindowObj.SetActive(false);
        }
        public void OnClickResetSkillPointBtn() {
            Debug.Log("RESET SKILL POINT");
        }
        public void OnClickLearnSkillBtn() {
            Debug.Log("LEARN SKILL");
        }

        public void OnClickWarriorSkillTreeBtn(int idx) {
            SetUI(idx, SkillTreeCate.Warrior, WarriorSkillTrees, WarriorSkillTreeSO);
        }
        public void OnClickArcherSkillTreeBtn(int idx) {
            SetUI(idx, SkillTreeCate.Archer, ArcherSkillTrees, ArcherSkillTreeSO);
        }
        public void OnClickMagicianSkillTreeBtn(int idx) {
            SetUI(idx, SkillTreeCate.Magician, MagicianSkillTrees, MagicianSkillTreeSO);
        }
        public void OnClickUtilitySkillTreeBtn(int idx) {
            SetUI(idx, SkillTreeCate.Utility, UtilitySkillTrees, UtilitySkillTreeSO);
        }
    #endregion

    #region FUNC
        private void SetUI(int idx, SkillTreeCate cate, SkillTree[] skillTrees, SettingSkillTreeData skillTreeDataSO) {
            //* 背景とアイコン背景色
            BgPatternCtrl.BgImg.color = BgPatternCtrl.SkillCateColors[(int)cate];
            SkillIconBgImg.color = SkillIconBgColors[(int)cate];

            SkillIconImg.sprite = skillTrees[idx].IconImg.sprite;

            SkillNameTxt.text = skillTreeDataSO.Datas[idx].Name;
            SkillInfoTxt.text = skillTreeDataSO.Datas[idx].Description;
            NeededSkillPointTxt.text = $"{WarriorSkillTreeSO.Datas[idx].Cost}";
        }
    #endregion
}
