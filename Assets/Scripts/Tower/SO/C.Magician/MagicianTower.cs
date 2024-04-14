using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Assets.PixelFantasy.PixelHeroes.Common.Scripts.ExampleScripts;
using Random = UnityEngine.Random;
// using UnityEditor.PackageManager;
// using UnityEditor;

public class MagicianTower : Tower {
    public static readonly int[] SK1_ExplosionLvActivePers = new int[6] {0, 0, 20, 25, 30, 35};
    public static readonly int[] SK2_MagicCircleActivePers = new int[6] {0, 0, 0, 10, 15, 20};
    public static readonly float[] SK2_MagicCircleDmgPers = new float[6] {0, 0, 0, 0.1f, 0.2f, 0.3f};

    public static readonly float[] SK3_LaserSpans = new float[6] {0, 0, 0, 0, 10, 8};
    public static readonly float[] SK3_LaserDmgPers = new float[6] {0, 0, 0, 0, 0.5f, 0.8f};

    public static readonly float[] SK4_BigbangSpans = new float[6] {0, 0, 0, 0, 0, 15};
    public static readonly float[] SK4_BigbangDmgs = new float[6] {0, 0, 0, 0, 0, 1.0f};

    public GameObject BigbangEF;
    [field:SerializeField] public bool IsMagicCircleOneTime {get; set;}
    [field:SerializeField] public bool IsLaserActive {get; set;}
    [field:SerializeField] public bool IsBigbangActive {get; set;}

    bool isDrawGizmos;
    Vector2 gizmosPos;

    void Start() {
        //* １回のみ生成トリガー
        IsMagicCircleOneTime = false; 
        //* 最初ONトリガー
        IsLaserActive = true; 
        IsBigbangActive = true;
    }

#region SKILLTREE_EXTRA_VALUE
    public void SetExtraDmg() {
        float extraPer = 0;

        //* SkillTree 追加ダメージ
        if(!sktDb.IsLockMagicianSTs[(int)SKT_MG.EXTRA_DMG_A])
            extraPer += sktDb.GetMagicianVal((int)SKT_MG.EXTRA_DMG_A);
        if(!sktDb.IsLockMagicianSTs[(int)SKT_MG.EXTRA_DMG_B])
            extraPer += sktDb.GetMagicianVal((int)SKT_MG.EXTRA_DMG_B);
        //* Euqip 追加ダメージ
        extraPer += DM._.DB.EquipDB.AttackPer;
        extraPer += DM._.DB.EquipDB.MagicianAttackPer;

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
        if(!sktDb.IsLockMagicianSTs[(int)SKT_MG.EXTRA_RANGE])
            extraPer += sktDb.GetMagicianVal((int)SKT_MG.EXTRA_RANGE);
        //* Euqip 追加ダメージ
        extraPer += DM._.DB.EquipDB.RangePer;

        //* DICIONARYへ追加
        if(extraPer > 0)
            ExtraRangeDic.Add($"{AbilityType.Range}", extraPer);
    }

    public void SetExtraCrit() {
        float extraPer = 0;

        //* SkillTree 追加ダメージ
        if(!sktDb.IsLockMagicianSTs[(int)SKT_MG.CRIT_PER])
            extraPer += sktDb.GetMagicianVal((int)SKT_MG.CRIT_PER);
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

        //* DICIONARYへ追加
        if(extraPer > 0)
            ExtraCritDmgDic.Add($"{AbilityType.CriticalDamage}", extraPer);
    }

    public float GetSkillTreeExtraExplosionDmgPer() {
        float extraPer = 0;

        //* SkillTree 追加ダメージ
        if(!sktDb.IsLockMagicianSTs[(int)SKT_MG.EXPLOSION_DMG_PER])
            extraPer += sktDb.GetMagicianVal((int)SKT_MG.EXPLOSION_DMG_PER);

        Debug.Log($"SetSkillTreeExtraExplosionDmgPer():: extraPer= {extraPer}");

        return extraPer;
    }
#endregion

    public override void CheckMergeUI() {
        Image mergeIcon = GM._.actBar.IconBtns[(int)ActionBarUIManager.ICON.Merge].GetComponent<Image>();

        //* 選んだタワーとTowerGroupの子リストと同じレベルの数を確認
        var towers = GM._.tm.MagicianGroup.GetComponentsInChildren<MagicianTower>();
        var sameLvTower = Array.FindAll(towers, tower => Lv == tower.Lv);

        //* １個以上があったら、マージ可能表示
        if(sameLvTower.Length > 1) {
            mergeIcon.sprite = GM._.actBar.MergeOnSpr;
            GM._.actBar.MergePossibleMark.SetActive(true);
        }
    }

    public override bool Merge(TowerKind kind = TowerKind.None) {
        Image mergeIcon = GM._.actBar.IconBtns[(int)ActionBarUIManager.ICON.Merge].GetComponent<Image>();

        //* マージが可能であれば
        if(mergeIcon.sprite == GM._.actBar.MergeOnSpr) {
            var towers = GM._.tm.MagicianGroup.GetComponentsInChildren<MagicianTower>();
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

    public void Skill1_Explosion(Enemy target) {
        const float RADIUS = 1;

        //* 発動％にならなかったら、終了
        int rand = Random.Range(0, 100);
        if(rand >= SK1_ExplosionLvActivePers[LvIdx])
            return;

        StartCoroutine(CoActiveGizmos(target.transform.position));
        var efIdx = (Lv == 3)? GameEF.ExplosionWindEF
            : (Lv == 4)? GameEF.ExplosionFireballPurpleEF
            : (Lv == 5)? GameEF.ExplosionFireballBlueEF
            : (Lv == 6)? GameEF.ExplosionFireballRedEF : GameEF.NULL;
        GM._.gef.ShowEF(efIdx, target.transform.position);
        SM._.SfxPlay(SM.SFX.FireExplosionSFX);

        int layerMask = 1 << Enum.Layer.Enemy;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(target.transform.position, RADIUS, layerMask);
        Debug.Log("アタック！ ROUND:: colliders= " + colliders.Length);
        foreach(Collider2D col in colliders) {
            Enemy enemy = col.GetComponent<Enemy>();

            //* ターゲットの敵はダメージ計算が重なるので除外
            if(target == enemy)
                continue;

            //* ダメージ５０％ (クリティカル 無し)
            float dmgPer = 0.5f + GetSkillTreeExtraExplosionDmgPer();
            Debug.Log($"Skill1_Explosion():: dmgPer= {dmgPer}");
            int dmg = Mathf.RoundToInt(Dmg * dmgPer);
            enemy.DecreaseHp(dmg);
        }
    }

    public void Skill2_MagicCircle() {
        //* 発動％にならなかったら、終了
        int rand = Random.Range(0, 100);
        if(rand >= SK2_MagicCircleActivePers[LvIdx])
            return;

        //* １キャラー当たり、１回のみ
        IsMagicCircleOneTime = true;

        int idx = Lv == 4? (int)MissileIdx.MagicCirclePurple
            : Lv == 5? (int)MissileIdx.MagicCircleBlue
            : Lv == 6? (int)MissileIdx.MagicCircleRed : -1;
        MagicCircle mc = GM._.mm.CreateMissile(idx).GetComponent<MagicCircle>();

        if(Lv == 4) SM._.SfxPlay(SM.SFX.MagicCircle1SFX);
        else if(Lv == 5) SM._.SfxPlay(SM.SFX.MagicCircle2SFX);
        else if(Lv == 6) SM._.SfxPlay(SM.SFX.MagicCircle3SFX);

        mc.Init(this);
    }

    public void Skill3_Laser() {
        if(IsLaserActive)
            StartCoroutine(CoSkill3_Laser());
    }
    IEnumerator CoSkill3_Laser() {
        const int WAIT_DESTROY_TIME = 2;

        IsLaserActive = false;
        int idx = Lv == 5? (int)MissileIdx.LaserBlue : Lv == 6? (int)MissileIdx.LaserRed : -1;

        Laser laser = GM._.mm.CreateMissile(idx).GetComponent<Laser>();
        laser.EffectObj.SetActive(false);
        laser.Init(this);
        SM._.SfxPlay(SM.SFX.LaserSFX);

        yield return new WaitForSeconds(WAIT_DESTROY_TIME);
        GM._.mm.PoolList[idx].Release(laser.gameObject);

        yield return new WaitForSeconds(SK3_LaserSpans[LvIdx] - WAIT_DESTROY_TIME);
        IsLaserActive = true;
    }

    public void Skill4_Bigbang() {
        if(IsBigbangActive)
            StartCoroutine(CoSkill4_Bigbang());
    }

    IEnumerator CoSkill4_Bigbang() {
        const int WAIT_DELAY_TIME = 2;
        const int WAIT_SPAWN_TIME = 2;
        const int WAIT_DESTROY_TIME = 2;
        IsBigbangActive = false;

        yield return new WaitForSeconds(WAIT_DELAY_TIME);

        //* スキルEF 表示
        SM._.SfxPlay(SM.SFX.BigbangSFX);
        BigbangEF.SetActive(true);

        //* 全ての敵にダメージ
        yield return new WaitForSeconds(WAIT_SPAWN_TIME);
        Transform enemyGroup = GM._.em.enemyObjGroup;
        for(int i = 0; i < enemyGroup.childCount; i++) {
            Enemy enemy = enemyGroup.GetChild(i).GetComponent<Enemy>();
            int dmg = Mathf.RoundToInt(Dmg * SK4_BigbangDmgs[LvIdx]);
            Util._.ComboAttack(enemy, dmg, hitCnt: 5, Util.Time0_15);
        }
        
        //* スキルEF 非表示
        yield return new WaitForSeconds(WAIT_DESTROY_TIME);
        BigbangEF.SetActive(false);

        //* CoolTime
        yield return new WaitForSeconds(SK4_BigbangSpans[LvIdx] - WAIT_DELAY_TIME - WAIT_SPAWN_TIME - WAIT_DESTROY_TIME);
        IsBigbangActive = true;
    }


#region GIZMOS
    IEnumerator CoActiveGizmos(Vector2 pos) {
        gizmosPos = pos;
        isDrawGizmos = true;
        yield return Util.Time1;
        isDrawGizmos = false;
    }
    
    void OnDrawGizmos() {
        if(isDrawGizmos) {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(new Vector3(gizmosPos.x, gizmosPos.y, 0), 1);
        }
    }
#endregion
}
