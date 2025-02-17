using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Assets.PixelFantasy.PixelHeroes.Common.Scripts.ExampleScripts;
using Random = UnityEngine.Random;
using UnityEngine.Analytics;

public class WarriorTower : Tower {
    public static readonly int[] SK1_RageActivePers = new int[6] {0, 0, 5, 10, 13, 15};
    public static readonly float[] SK1_RageDmgSpdIncPers = new float[6] {0, 0, 0.1f, 0.15f, 0.175f, 0.2f};
    public static readonly float[] SK1_RageTime = new float[6] {0, 0, 2.25f, 2.5f, 2.75f, 3f};

    public static readonly float[] SK2_WheelwindActivePers = new float[6] {0, 0, 0, 10, 12, 15};
    public static readonly float[] SK2_WheelwindDmgPers = new float[6] {0, 0, 0, 0.3f, 0.45f, 0.6f};

    public static readonly float[] SK3_CheerUpSpans = new float[6] {0, 0, 0, 0, 15, 12};
    public static readonly float[] SK3_CheerUpDmgSpdIncPers = new float[6] {0, 0, 0, 0, 0.1f, 0.15f};

    public static readonly float[] SK4_RoarSpans = new float[6] {0, 0, 0, 0, 0, 15};
    public static readonly float[] SK4_RoarDmgPers = new float[6] {0, 0, 0, 0, 0, 0.8f};

    public GameObject RageAuraEF;
    public GameObject WheelwindEF;
    public GameObject CheerUpEF;
    public GameObject RoarEF;
    
    public Coroutine CorSkill1ID;
    [field:SerializeField] public int RageDmgUp {get; private set;}
    [field:SerializeField] public float RageSpdUp {get; private set;}

    [field:SerializeField] public bool IsCheerUpActive {get; set;}
    [field:SerializeField] public int CheerUpDmgUp {get; private set;}
    [field:SerializeField] public float CheerUpSpdUp {get; private set;}

    [field:SerializeField] public bool IsRoarActive {get; set;}

    [field:SerializeField] public List<Tower> allTowerList {get; set;}

    bool isDrawGizmos;
    Vector2 gizmosPos;

    void Start() {
        if(Lv >= 5) IsCheerUpActive = true; //* レベル５以上
        if(Lv >= 6) IsRoarActive = true; //* レベル５以上
    }

#region EXTRA_VALUE
    public void SetExtraDmg() {
        float extraPer = 0;

        //* SkillTree 追加ダメージ
        if(!sktDb.IsLockWarriorSTs[(int)SKT_WR.EXTRA_DMG_A]) {
            extraPer += sktDb.GetWarriorVal((int)SKT_WR.EXTRA_DMG_A);
            Debug.Log($"{this.Name} SetExtraDmg():: SKT_WR.EXTRA_DMG_A= {sktDb.GetWarriorVal((int)SKT_WR.EXTRA_DMG_A)}");
        }
        if(!sktDb.IsLockWarriorSTs[(int)SKT_WR.EXTRA_DMG_B]) {
            extraPer += sktDb.GetWarriorVal((int)SKT_WR.EXTRA_DMG_B);
            Debug.Log($"{this.Name} SetExtraDmg():: SKT_WR.EXTRA_DMG_A= {sktDb.GetWarriorVal((int)SKT_WR.EXTRA_DMG_B)}");
        }

        //* Euqip 追加ダメージ
        extraPer += DM._.DB.EquipDB.AttackPer;
        Debug.Log($"{this.Name} SetExtraDmg():: AttackPer= {DM._.DB.EquipDB.AttackPer}");
        extraPer += DM._.DB.EquipDB.WarriorAttackPer;

        //* ユーザLVの追加ダメージ
        extraPer += DM._.DB.StatusDB.GetUserLvExtraDmgPercent();
        Debug.Log($"{this.Name} SetExtraDmg():: UserLVExtraDmgPer= {DM._.DB.StatusDB.GetUserLvExtraDmgPercent()}");

        //* Infinite強化ダメージ
        extraPer += DM._.DB.InfiniteUpgradeDB.GetExtraDmgPercent();
        Debug.Log($"{this.Name} SetExtraDmg():: InfiniteUpgradeDB= {DM._.DB.InfiniteUpgradeDB.GetExtraDmgPercent()}");

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
        if(!sktDb.IsLockWarriorSTs[(int)SKT_WR.EXTRA_SPD])
            extraPer += sktDb.GetWarriorVal((int)SKT_WR.EXTRA_SPD);
        //* Euqip 追加ダメージ
        extraPer += DM._.DB.EquipDB.SpeedPer;

        //* DICIONARYへ追加
        if(extraPer > 0)
            ExtraSpdDic.Add($"{AbilityType.Speed}", extraPer);
    }

    public void SetExtraRange() {
        float extraPer = 0;

        //* SkillTree 追加ダメージ
        // なし
        //* Euqip 追加ダメージ
        extraPer += DM._.DB.EquipDB.RangePer;

        //* DICIONARYへ追加
        if(extraPer > 0)
            ExtraRangeDic.Add($"{AbilityType.Range}", extraPer);
    }

    public void SetExtraCrit() {
        float extraPer = 0;

        //* SkillTree 追加ダメージ
        // なし
        //* Euqip 追加ダメージ
        extraPer += DM._.DB.EquipDB.CritPer;

        //* DICIONARYへ追加
        if(extraPer > 0)
            ExtraCritDic.Add($"{AbilityType.Critical}", extraPer);
    }

    public void SetExtraCritDmg() {
        float extraPer = 0;

        //* SkillTree 追加ダメージ
        // なし

        //* Euqip 追加ダメージ
        extraPer += DM._.DB.EquipDB.CritDmgPer;

        //* Infinite強化ダメージ
        extraPer += DM._.DB.InfiniteUpgradeDB.GetExtraCritDmgPercent();

        //* DICIONARYへ追加
        if(extraPer > 0)
            ExtraCritDmgDic.Add($"{AbilityType.CriticalDamage}", extraPer);
    }

    public WaitForSeconds GetSkillTreeExtraRageTime() {
        float extraTime = 0;

        //* SkillTree 追加ダメージ
        if(!sktDb.IsLockWarriorSTs[(int)SKT_WR.EXTRA_RAGE_TIME])
            extraTime += sktDb.GetWarriorVal((int)SKT_WR.EXTRA_RAGE_TIME);

        Debug.Log($"GetSkillTreeExtraRageTime():: extraRageTime= {extraTime}");

        if(extraTime == 1)
            return Util.Time1;
        else 
            return null;
    }
#endregion

    public override void CheckMergeUI() {
        Image mergeIcon = GM._.actBar.IconBtns[(int)ActionBarUIManager.ICON.Merge].GetComponent<Image>();

        //* 選んだタワーとTowerGroupの子リストと同じレベルの数を確認
        var towers = GM._.tm.WarriorGroup.GetComponentsInChildren<WarriorTower>();
        WarriorTower[] sameLvTowers = Array.FindAll(towers, tower => Lv == tower.Lv && Lv < 6);
        // Array.ForEach(sameLvTowers, mergableTower => mergableTower.StarIconTxtAnim.SetBool("IsTwincle", false));

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
            var towers = GM._.tm.WarriorGroup.GetComponentsInChildren<WarriorTower>();
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

    public void SlashEffect(Transform tgTf) {
        GameEF idx = (Lv == 1 || Lv == 2)? GameEF.SwordSlashWhiteEF
            : (Lv == 3)? GameEF.SwordSlashYellowEF
            : (Lv == 4)? GameEF.SwordSlashRedEF
            : (Lv == 5)? GameEF.SwordSlashBlueEF
            : GameEF.SwordSlashPurpleBlackEF;
        //* エフェクト
        GM._.gef.ShowEF(idx, tgTf.position);
    }

#region SKILL1 RAGE
    public void Skill1_Rage() {
        if(CorSkill1ID != null) return;
        Debug.Log($"Skill1_Rage():: SK1_RageActivePers[{LvIdx}]= {SK1_RageActivePers[LvIdx]}");

        int rand = Random.Range(0, 100);
        if(rand < SK1_RageActivePers[LvIdx]) {
            CorSkill1ID = StartCoroutine(CoSkill1_Rage());
        }
    }

    IEnumerator CoSkill1_Rage() {
        Debug.Log($"CoSkill1_Rage():: DmgSpdIncPers[{LvIdx}]= {SK1_RageDmgSpdIncPers[LvIdx]}, RageDmgSpdIncPers[{LvIdx}]= {SK1_RageDmgSpdIncPers[LvIdx]}");
        const string RAGE = "RAGE";

        RageDmgUp = (int)(TowerData.Dmg * SK1_RageDmgSpdIncPers[LvIdx]);
        RageSpdUp = (float)(TowerData.AtkSpeed * SK1_RageDmgSpdIncPers[LvIdx]);

        //* 追加タメージ
        if(ExtraDmgDic.ContainsKey(RAGE)) ExtraDmgDic.Remove(RAGE);
        ExtraDmgDic.Add(RAGE, RageDmgUp);
        //* 追加スピード
        if(ExtraSpdDic.ContainsKey(RAGE)) ExtraSpdDic.Remove(RAGE);
        ExtraSpdDic.Add(RAGE, RageSpdUp);

        RageAuraEF.SetActive(true);
        SM._.SfxPlay(SM.SFX.RageSFX);

        //* 現在選択したBoardと今スキルを発動したWarriorの親Boardが同じいときのみ、タワーステータスUI最新化
        if(GM._.tmc.HitObject == transform.parent.gameObject)
            GM._.gui.tsm.ShowTowerStateUI(InfoState());

        yield return Util.Time2;
        yield return GetSkillTreeExtraRageTime();

        RageAuraEF.SetActive(false);
        CorSkill1ID = null;
        ExtraDmgDic.Remove(RAGE);
        ExtraSpdDic.Remove(RAGE);

        //* 現在選択したBoardと今スキルを発動したWarriorの親Boardが同じいときのみ、タワーステータスUI最新化
        if(GM._.tmc.HitObject == transform.parent.gameObject)
            GM._.gui.tsm.ShowTowerStateUI(InfoState());
    }
#endregion

#region SKILL2 WHEELWIND
    public void Skill2_Wheelwind() {
        //* 発動％にならなかったら、終了
        int rand = Random.Range(0, 100);
        if(rand >= SK2_WheelwindActivePers[LvIdx])
            return;

        StartCoroutine(CoActiveGizmos(transform.position));

        int layerMask = 1 << Enum.Layer.Enemy;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, AtkRange * 1.425f, layerMask);
        Debug.Log("アタック！ ROUND:: colliders= " + colliders.Length);
        
        StartCoroutine(CoShowWheelwindEF());
        SM._.SfxPlay(SM.SFX.WheelWindSFX);

        //* ホイールウィンド！
        for(int i = 0; i < colliders.Length; i++) {
            Enemy enemy = colliders[i].GetComponent<Enemy>();

            //* ダメージ
            enemy.DecreaseHp(Mathf.RoundToInt(Dmg * SK2_WheelwindDmgPers[LvIdx]));

            //* 気絶(きぜつ)
            if(enemy.gameObject.activeSelf)
                enemy.Stun(0.5f);
        }
    }

    IEnumerator CoShowWheelwindEF() {
        WheelwindEF.SetActive(true);
        yield return Util.Time1_5;
        WheelwindEF.SetActive(false);
    }
#endregion

#region SKILL3 CHEER UP
    public void Skill3_CheerUp() {
        if(IsCheerUpActive)
            StartCoroutine(CoSkill3_CheerUp());
    }
    IEnumerator CoSkill3_CheerUp() {
        const string CHEERUP = "CHEERUP";
        Debug.Log("CheerUp開始");
        GetComponent<CharacterControls>().SpawnAnim();
        IsCheerUpActive = false;
        SM._.SfxPlay(SM.SFX.CheerUpSFX);
        CheerUpEF.SetActive(true);

        bool isRandomTower = GM._.tmc.HitObject?.GetComponentInChildren<Tower>();

        CheerUpDmgUp = (int)(TowerData.Dmg * SK3_CheerUpDmgSpdIncPers[LvIdx]);
        CheerUpSpdUp = (float)(TowerData.AtkSpeed * SK3_CheerUpDmgSpdIncPers[LvIdx]);

        //* 全てタワーを探す
        // List<Tower> allTowerList = GM._.tm.GetAllTower();
        allTowerList = GM._.tm.GetAllTower();

        //* 全てタワー
        allTowerList.ForEach(tower => {
            //* スタイル変更
            if(tower)
                Util._.SetRedMt(tower.BodySprRdr);

            //* 追加タメージ
            int extraDmg = (int)(tower.TowerData.Dmg * SK3_CheerUpDmgSpdIncPers[LvIdx]);
            if(tower.ExtraDmgDic.ContainsKey(CHEERUP)) tower.ExtraDmgDic.Remove(CHEERUP);
            tower.ExtraDmgDic.Add(CHEERUP, extraDmg);
            //* 追加スピード
            float extraSpd = (float)(tower.TowerData.AtkSpeed * SK3_CheerUpDmgSpdIncPers[LvIdx]);
            if(tower.ExtraSpdDic.ContainsKey(CHEERUP)) tower.ExtraSpdDic.Remove(CHEERUP);
            tower.ExtraSpdDic.Add(CHEERUP, extraSpd);
        });

        
        if(GM._.tmc.HitObject != null) {
            if(isRandomTower)
                GM._.gui.tsm.ShowTowerStateUI(GM._.tmc.HitObject.GetComponentInChildren<Tower>().InfoState());
        }

        yield return Util.Time5;
        Debug.Log("CheerUp終了");
        CheerUpEF.SetActive(false);

        //* 全てタワー
        allTowerList.ForEach(tower => {
            Debug.Log($"CoSkill3_CheerUp():: tower= {tower}, Spr= {tower.BodySprRdr}");
            //* 途中で合成とか削除などで消えたタワーはリストから処理しない
            if(tower != null) {
                //* スタイル戻す
                Util._.SetDefMt(tower.BodySprRdr);
                //* ダメージと速度戻す
                tower.ExtraDmgDic.Remove(CHEERUP);
                tower.ExtraSpdDic.Remove(CHEERUP);
            }
        });

        if(GM._.tmc.HitObject != null) {
            Debug.Log($"CoSkill3_CheerUp():: GM._.tmc.HitObject= {GM._.tmc.HitObject.name}");
            var board = GM._.tmc.HitObject.GetComponent<Board>();
            if(isRandomTower && board.IsTowerOn)
                GM._.gui.tsm.ShowTowerStateUI(GM._.tmc.HitObject.GetComponentInChildren<Tower>().InfoState());
        }

        yield return Util.Time10;
        IsCheerUpActive = true;
    }
#endregion

#region SKILL4 ROAR
    public void Skill4_Roar() {
        if(IsRoarActive)
            StartCoroutine(CoSkill4_Roar());
    }
    IEnumerator CoSkill4_Roar() {
        const int WAIT_DELAY_TIME = 2;
        const int WAIT_SPAWN_TIME = 1;
        const int WAIT_DESTROY_TIME = 2;
        IsRoarActive = false;

        yield return new WaitForSeconds(WAIT_DELAY_TIME);

        //* スキルEF 表示
        GetComponent<CharacterControls>().SpawnAnim();
        RoarEF.SetActive(true);
        SM._.SfxPlay(SM.SFX.RoarASFX);

        //* 全ての敵にダメージ
        yield return new WaitForSeconds(WAIT_SPAWN_TIME);
        SM._.SfxPlay(SM.SFX.RoarBSFX);
        Transform enemyGroup = GM._.em.enemyObjGroup;
        for(int i = 0; i < enemyGroup.childCount; i++) {
            Enemy enemy = enemyGroup.GetChild(i).GetComponent<Enemy>();
            int dmg = Mathf.RoundToInt(Dmg * SK4_RoarDmgPers[LvIdx]);
            enemy.DecreaseHp(dmg);
            if(enemy != null && enemy.gameObject.activeSelf)
                enemy.Stun(1);
        }

        //* スキルEF 非表示
        yield return new WaitForSeconds(WAIT_DESTROY_TIME);
        RoarEF.SetActive(false);

        //* CoolTime
        yield return new WaitForSeconds(SK4_RoarSpans[LvIdx] - WAIT_DELAY_TIME - WAIT_SPAWN_TIME - WAIT_DESTROY_TIME);
        IsRoarActive = true;
    }
#endregion

    IEnumerator CoActiveGizmos(Vector2 pos) {
        gizmosPos = pos;
        isDrawGizmos = true;
        yield return Util.Time1;
        isDrawGizmos = false;
    }
    
    void OnDrawGizmos() {
        if(isDrawGizmos) {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(new Vector3(gizmosPos.x, gizmosPos.y, 0), AtkRange * 1.425f);
        }
    }
}
