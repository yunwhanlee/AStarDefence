using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum SkillTreeCate { Warrior, Archer, Magician, Utility }

public class SkillTreeUIManager : MonoBehaviour {
    [field:SerializeField] public BgPatternController BgPatternCtrl {get; private set;}

    [field:SerializeField] public SettingSkillTreeData WarriorSkillTreeSO {get; private set;}
    [field:SerializeField] public SettingSkillTreeData ArcherSkillTreeSO {get; private set;}
    [field:SerializeField] public SettingSkillTreeData MagicianSkillTreeSO {get; private set;}
    [field:SerializeField] public SettingSkillTreeData UtilitySkillTreeSO {get; private set;}

    int i = 0;

    void Update() {
        if(Input.GetKeyDown(KeyCode.V)) {
            i++;
            if(i > 3) i = 0;
            BgPatternCtrl.BgImg.color = BgPatternCtrl.SkillCateColors[i];
        }
    }

    #region EVENT
        public void OnClickSkillIcon(string type_Idx) {

        }
    #endregion
}
