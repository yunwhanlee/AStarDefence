using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Assets.PixelFantasy.PixelHeroes.Common.Scripts.ExampleScripts;
using Random = UnityEngine.Random;

public class ArcherTower : Tower {
    public static readonly float[] SK1_CritIncPers = new float[6] {0, 0, 0.1f, 0.15f, 0.2f, 0.25f};

    public static readonly float[] SK2_MultiShotActivePers = new float[6] {0, 0, 0, 15, 20, 25};
    public static readonly int[] SK2_MultiShotCnts = new int[6] {0, 0, 0, 2, 4, 6};

    public static readonly int[] SK3_PassShotSpans = new int[6] {0, 0, 0, 0, 10, 7};
    public static readonly float[] SK3_PassShotDmgPers = new float[6] {0, 0, 0, 0, 2.0f, 3.0f};

    public static readonly float[] SK4_ArrowRainSpans = new float[6] {0, 0, 0, 0, 0, 15};

    [field:SerializeField] public GameObject PerfectAimAuraEF {get; set;}
    [field:SerializeField] public bool IsPassArrowActive {get; set;}
    [field:SerializeField] public bool IsArrowRainActive {get; set;}

    void Start(){
        IsPassArrowActive = true;
        IsArrowRainActive = true;
    }

#region SKILLTREE_EXTRA_VALUE
    public void SetExtraDmg() {
        float extraPer = 0;

        //* SkillTree 追加ダメージ
        if(!sktDb.IsLockArcherSTs[(int)SKT_AC.EXTRA_DMG_A])
            extraPer += sktDb.GetArcherVal((int)SKT_AC.EXTRA_DMG_A);
        if(!sktDb.IsLockArcherSTs[(int)SKT_AC.EXTRA_DMG_B])
            extraPer += sktDb.GetArcherVal((int)SKT_AC.EXTRA_DMG_B);

        //* Euqip 追加ダメージ
        extraPer += DM._.DB.EquipDB.AttackPer;
        extraPer += DM._.DB.EquipDB.ArcherAttackPer;

        //* ユーザLVの追加ダメージ
        extraPer += DM._.DB.StatusDB.GetUserLvExtraDmgPercent();

        //* Infinite強化ダメージ
        extraPer += DM._.DB.InfiniteUpgradeDB.GetExtraDmgPercent();

        Debug.Log($"{this.Name} SetExtraDmg():: ExtraDmgPer= {extraPer}");

        //* DICIONARYへ追加
        if(extraPer > 0) {
            int extraDmg = Mathf.RoundToInt(TowerData.Dmg * extraPer);
            extraDmg = extraDmg == 0? 1 : extraDmg;
            ExtraDmgDic.Add($"{AbilityType.Attack}", extraDmg);
        }
    }
    public void SetExtraSpeed() {
        float extraPer = 0;

        //* SkillTree 追加ダメージ
        // なし
        //* Euqip 追加ダメージ
        extraPer += DM._.DB.EquipDB.SpeedPer;

        //* DICIONARYへ追加
        if(extraPer > 0)
            ExtraSpdDic.Add($"{AbilityType.Speed}", extraPer);
    }

    public void SetExtraRange() {
        float extraPer = 0;

        //* SkillTree 追加ダメージ
        if(!sktDb.IsLockWarriorSTs[(int)SKT_WR.EXTRA_RANGE])
            extraPer += sktDb.GetWarriorVal((int)SKT_WR.EXTRA_RANGE);
        //* Euqip 追加ダメージ
        extraPer += DM._.DB.EquipDB.RangePer;

        //* DICIONARYへ追加
        if(extraPer > 0)
            ExtraRangeDic.Add($"{AbilityType.Range}", extraPer);
    }

    public void SetExtraCrit() {
        float extraPer = 0;

        //* SkillTree 追加ダメージ
        if(!sktDb.IsLockArcherSTs[(int)SKT_AC.CRIT_PER])
            extraPer += sktDb.GetArcherVal((int)SKT_AC.CRIT_PER);
        //* Euqip 追加ダメージ
        extraPer += DM._.DB.EquipDB.CritPer;

        //* DICIONARYへ追加
        if(extraPer > 0)
            ExtraCritDic.Add($"{AbilityType.Critical}", extraPer);
    }

    public void SetExtraCritDmg() {
        float extraPer = 0;

        //* SkillTree 追加ダメージ
        if(!sktDb.IsLockArcherSTs[(int)SKT_AC.CIRT_DMG_PER_A])
            extraPer += sktDb.GetArcherVal((int)SKT_AC.CIRT_DMG_PER_A);
        if(!sktDb.IsLockArcherSTs[(int)SKT_AC.CIRT_DMG_PER_B])
            extraPer += sktDb.GetArcherVal((int)SKT_AC.CIRT_DMG_PER_B);

        //* Euqip 追加ダメージ
        extraPer += DM._.DB.EquipDB.CritDmgPer;

        //* Infinite強化ダメージ
        extraPer += DM._.DB.InfiniteUpgradeDB.GetExtraCritDmgPercent();

        //* DICIONARYへ追加
        if(extraPer > 0)
            ExtraCritDmgDic.Add($"{AbilityType.CriticalDamage}", extraPer);
    }
#endregion

    public override void CheckMergeUI() {
        Image mergeIcon = GM._.actBar.IconBtns[(int)ActionBarUIManager.ICON.Merge].GetComponent<Image>();

        //* 選んだタワーとTowerGroupの子リストと同じレベルの数を確認
        var towers = GM._.tm.ArcherGroup.GetComponentsInChildren<ArcherTower>();
        ArcherTower[] sameLvTowers = Array.FindAll(towers, tower => Lv == tower.Lv && Lv < 6);

        //* １個以上があったら、マージ可能表示
        if(sameLvTowers.Length > 1) {
            // Array.ForEach(sameLvTowers, mergableTower => mergableTower.StarIconTxtAnim.SetBool("IsTwincle", true));
            mergeIcon.sprite = GM._.actBar.MergeOnSpr;
            GM._.actBar.MergePossibleMark.SetActive(true);
        }
    }

    public override bool Merge(TowerKind kind = TowerKind.None) {
        Image mergeIcon = GM._.actBar.IconBtns[(int)ActionBarUIManager.ICON.Merge].GetComponent<Image>();

        //* マージが可能であれば
        if(mergeIcon.sprite == GM._.actBar.MergeOnSpr) {
            var towers = GM._.tm.ArcherGroup.GetComponentsInChildren<ArcherTower>();
            var another = Array.Find(towers, tower => this != tower && Lv == tower.Lv);

            //* 自分以外同じタワーを削除
            var anotherBoard = another.GetComponentInParent<Board>();
            anotherBoard.IsTowerOn = false;
            anotherBoard.transform.SetParent(GM._.tm.BoardGroup);

            DestroyImmediate(another.gameObject);

            //* 次のレベルタワーランダムで生成
            GM._.tm.CreateTower(Type, Lv++, kind);

            //* 自分を削除
            DestroyImmediate(gameObject);

            return true;
        }
        return false;
    }

    public override void Upgrade() {
        Debug.Log("Upgrade()::");
        GetComponent<CharacterControls>().SpawnAnim();
        int cardLv = GM._.tm.TowerCardUgrLvs[(int)Kind];

        //* 追加タメージDictionaryへ追加
        if(ExtraDmgDic.ContainsKey(DIC_UPGRADE)) ExtraDmgDic.Remove(DIC_UPGRADE);
        ExtraDmgDic.Add(DIC_UPGRADE, GetUpgradeCardDmg(cardLv));
    }

    private void SetMultiShot(int cnt) {
        for(int i = 0; i < cnt; i++) {
            Missile ms = GM._.mm.CreateMissile();
            int sign = (i % 2 == 0)? -1 : 1;
            int degree = 10 * (1 + i / 2);
            ms.Init(this, sign * degree);
        }
    }

    public void Skill2_MultiShot() {
        int rand = Random.Range(0, 100);
        if(rand < SK2_MultiShotActivePers[LvIdx]) {
            switch(Lv) {
                case 4: SetMultiShot(2);
                break;
                case 5: SetMultiShot(4);
                break;
                case 6: SetMultiShot(6);
                break;
            }
        }
    }

    public void Skill3_PassArrow() {
        if(IsPassArrowActive)
            StartCoroutine(CoSkill3_PassArrow());
    }
    IEnumerator CoSkill3_PassArrow() {
        IsPassArrowActive = false;
        int idx = Lv == 5? (int)MissileIdx.PassArrowRed : Lv == 6? (int)MissileIdx.PassArrowBlue : -1;
        PassArrow pa = GM._.mm.CreateMissile(idx).GetComponent<PassArrow>();
        SM._.SfxPlay(SM.SFX.PassArrowSFX);
        pa.Init(this);
        yield return new WaitForSeconds(5);
        IsPassArrowActive = true;
    }

    public void Skill4_ArrowRain() {
        if(IsArrowRainActive)
            StartCoroutine(CoSkill4_ArrowRain());
    }
    IEnumerator CoSkill4_ArrowRain() {
        const int WAIT_DESTROY_TIME = 5;

        Debug.Log("ArrowRain 開始!");
        IsArrowRainActive = false;
        SM._.SfxPlay(SM.SFX.ArrowRainSFX);

        //* 追加クリティカル
        const string ARROWRAIN = "ArrowRain";
        if(ExtraCritDic.ContainsKey(ARROWRAIN)) ExtraCritDic.Remove(ARROWRAIN);
        Debug.Log($"SIBAL= {1 - TowerData.CritPer}");
        ExtraCritDic.Add(ARROWRAIN, 1 - TowerData.CritPer);
        GM._.gui.tsm.ShowTowerStateUI(InfoState());

        ArrowRain ar = GM._.mm.CreateMissile((int)MissileIdx.ArrowRain).GetComponent<ArrowRain>();
        ar.Init(this);

        yield return new WaitForSeconds(WAIT_DESTROY_TIME);
        Debug.Log("ArrowRain 終了!");
        
        ExtraCritDic.Remove(ARROWRAIN);
        GM._.gui.tsm.ShowTowerStateUI(InfoState());
        
        yield return new WaitForSeconds(SK4_ArrowRainSpans[LvIdx] - WAIT_DESTROY_TIME);
        IsArrowRainActive = true;
    }
}
