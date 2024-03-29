using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum SkillTreeCate {
    Warrior, Archer, Magician, Utility
}

public class SkillTreeUIManager : MonoBehaviour {
    [field:SerializeField] public BgPatternController BgPatternCtrl {get; set;}

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
