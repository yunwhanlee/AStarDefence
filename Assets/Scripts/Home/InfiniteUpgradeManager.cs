using System.Collections;
using System.Collections.Generic;
using AssetKits.ParticleImage;
using DG.Tweening;
using TMPro;
using UnityEngine;

[System.Serializable]
public class InfiniteUpgBtn {
    const int START_UPG_SUCCESS_PER_MAX = 90; // 0 ~ 100%として
    [field: SerializeField] public int Id {get; set;}
    [field: SerializeField] public int Lv {
        get {
            return Id == 0? DM._.DB.InfiniteUpgradeDB.DmgUpgLv
                : Id == 1? DM._.DB.InfiniteUpgradeDB.CritDmgUpgLv
                : DM._.DB.InfiniteUpgradeDB.BossDmgUpgLv;
        }
        set {
            switch (Id) {
                case 0: DM._.DB.InfiniteUpgradeDB.DmgUpgLv = value;         break;
                case 1: DM._.DB.InfiniteUpgradeDB.CritDmgUpgLv = value;     break;
                case 2: DM._.DB.InfiniteUpgradeDB.BossDmgUpgLv = value;     break;
            }
            SetDataUI();
        }
    }
    [field: SerializeField] public float UpgUnit {get; set;}
    [field: SerializeField] public int NeedCrackCnt {get; set;}
    [field: SerializeField] public int SuccessPer {get; set;}

    [field: SerializeField] public TMP_Text LvTxt;
    [field: SerializeField] public TMP_Text ValTxt;
    [field: SerializeField] public TMP_Text NeedCrackCntTxt;
    [field: SerializeField] public TMP_Text SuccessPerTxt;
    [field: SerializeField] public ParticleImage UpgradeAuraUIEF;

    public void SetDataUI() {
        LvTxt.text = $"LV {Lv}";

        float upgVal = Lv * UpgUnit * 100;
        ValTxt.text = (Id == 0)? $"공격력 <color=green>{upgVal}</color>% 증가"
                :(Id == 1)? $"치명타피해 <color=green>{upgVal}</color>% 증가"
                :$"보스추가피해 <color=green>{upgVal}</color>% 증가";

        NeedCrackCnt = Lv + 1;
        NeedCrackCntTxt.text = $"{NeedCrackCnt}";

        SuccessPer = START_UPG_SUCCESS_PER_MAX - (START_UPG_SUCCESS_PER_MAX * Lv / (Lv + 50));
        SuccessPerTxt.text = $"성공확률 {SuccessPer}%";
    }
}

public class InfiniteUpgradeManager : MonoBehaviour {
    public const int ATK = 0, CRITDMG = 1, BOSSDMG = 2;
    [field: SerializeField] public GameObject WindowObj {get; set;}
    [field: SerializeField] public TMP_Text CurCrackTxt;
    [field: SerializeField] public DOTweenAnimation BgImgDOTAnim;

    [field:Header("ID[0]: DMG, ID[1]: CRITDMG, ID[2]: BOSSDMG")]
    [field: SerializeField] public InfiniteUpgBtn[] InfiniteUpgBtns;

    void Start() {
        CurCrackTxt.text = $"{HM._.Crack}";
        foreach(var upgradeBtn in InfiniteUpgBtns) {
            upgradeBtn.SetDataUI();
        }
    }
    #region EVENT
        public void OnClickCrackUpgradeIcon() {
            SM._.SfxPlay(SM.SFX.ClickSFX);
            WindowObj.SetActive(true);
        }
        public void OnClickBackBtn() {
            SM._.SfxPlay(SM.SFX.ClickSFX);
            WindowObj.SetActive(false);
        }
        public void OnClickMoveToDungeonEntrance() {
            SM._.SfxPlay(SM.SFX.ClickSFX);
            WindowObj.SetActive(false);
            HM._.stgm.InfiniteDungeonWindow.SetActive(true);
        }
        public void OnClickUpgradeBtn(int idx) {
            if(HM._.Crack < InfiniteUpgBtns[idx].NeedCrackCnt) {
                SM._.SfxPlay(SM.SFX.ErrorSFX);
                HM._.hui.ShowMsgNotice("균열조각이 부족합니다!", y: 650);
                return;
            }

            HM._.Crack -= InfiniteUpgBtns[idx].NeedCrackCnt;

            int randPer = Random.Range(0, 100);
            if(randPer < InfiniteUpgBtns[idx].SuccessPer) {
                SM._.SfxPlay(SM.SFX.RoarASFX);
                HM._.hui.ShowMsgNotice("업그레이드 성공!", y: 650);
                InfiniteUpgBtns[idx].UpgradeAuraUIEF.Play();
                InfiniteUpgBtns[idx].Lv++;
            }
            else {
                HM._.hui.ShowMsgNotice("업그레이드 실패!", y: 650);
                SM._.SfxPlay(SM.SFX.EnemyDeadSFX);
                BgImgDOTAnim.DORestart();
            }
        }
    #endregion

    #region FUNC
        
    #endregion
}
