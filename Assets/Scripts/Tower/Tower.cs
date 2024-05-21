using System;
using System.Collections;
using System.Collections.Generic;
using Assets.PixelFantasy.PixelHeroes.Common.Scripts.CharacterScripts;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public enum TowerType {Random, Board, CC_IceTower, CC_StunTower}
public enum TowerKind {None = -1, Warrior, Archer, Magician}
public enum AttackType {Target, Round}

public abstract class Tower : MonoBehaviour {
    public const string DIC_UPGRADE = "Upgrade";
    const float CC_ATK_DELAY = 0.5f; // sec
    const float CC_RANGE_SCALE_OFFSET = 1.575f;
    readonly float[] CARD_UPG_DMGUP_PERS = {
        0.4f, // Tower Lv1
        0.2f, // Tower Lv2
        0.15f, // Tower Lv3
        0.12f, // Tower Lv4
        0.11f, // Tower Lv5
        0.1f// Tower Lv6
    };

    //* 外部
    public SettingTowerData TowerData;
    public TowerRangeController trc;
    public Character chara;

    //* Value------------------------------
    private Coroutine CorAttack;
    public TowerType Type;
    public TowerKind Kind;
    [Tooltip("AttackType : Target：ターゲット型、Round：自分の原点から矩形の爆発（Splash ON）")]
    public AttackType AtkType;
    public SpriteRenderer BodySprRdr;
    public Sprite MissileSpr;
    public DOTweenAnimation StarIconDTAnim;
    public string Name;
    [Range(1, 7)] public int Lv;
    public int LvIdx { get => Lv - 1;}
    protected SkillTreeDB sktDb;

    //* ダメージ
    public Dictionary<string, int> ExtraDmgDic = new Dictionary<string, int>();
    public virtual int Dmg {
        get {
            //* SkillTree 追加タメージ
            SetExtraDmgDic();

            //* 追加ダメージ 
            int extraDmg = 0;
            foreach(var dic in ExtraDmgDic)
                extraDmg += dic.Value;
            //* 合計
            return TowerData.Dmg + extraDmg;
        }
    }

    //* 速度
    public Dictionary<string, float> ExtraSpdDic = new Dictionary<string, float>();
    public float AtkSpeed {
        get {
            //* SkillTree 追加速度
            SetExtraSpdDic();

            //* 追加速度
            float extraSpd = 0;
            foreach(var dic in ExtraSpdDic)
                extraSpd += dic.Value;
            //* 合計
            return TowerData.AtkSpeed + extraSpd;
        }
    }

    //* 範囲
    public Dictionary<string, float> ExtraRangeDic = new Dictionary<string, float>();
    [field:Range(0, 10)] public float AtkRange {
        get {
            //* SkillTree 追加範囲
            SetExtraRangeDic();

            //* 追加範囲
            float extraRange = 0;
            foreach(var dic in ExtraRangeDic)
                extraRange += dic.Value;
            //* 合計
            return TowerData.AtkRange + extraRange;
        }
    }

    //* クリティカル
    public Dictionary<string, float> ExtraCritDic = new Dictionary<string, float>();
    public float CritPer {
        get {
            //* SkillTree 追加クリティカル
            SetExtraCritDic();

            //* 追加クリティカル
            float extraCrit = 0;
            foreach(var dic in ExtraCritDic)
                extraCrit += dic.Value;
            //* 合計
            return TowerData.CritPer + extraCrit;
        }
    }

    //* クリティカルダメージ
    public Dictionary<string, float> ExtraCritDmgDic = new Dictionary<string, float>();
    public float CritDmgPer {
        get {
            //* SkillTree 追加クリティカルダメージ
            SetExtraCritDmgDic();

            //* 追加クリティカルダメージ
            float extraCritDmgPer = 0;
            foreach(var dic in ExtraCritDmgDic)
                extraCritDmgPer += dic.Value;
            //* 合計
            return TowerData.CritDmgPer + extraCritDmgPer;
        }
    }

    //* CC
    [Range(0.0f, 5.00f)] public float SlowSec;
    [Range(0.0f, 5.0f)] public float StunSec;

    void Awake() {
        sktDb = DM._.DB.SkillTreeDB;
        StarIconDTAnim = GetComponentInChildren<DOTweenAnimation>();
        trc = GetComponentInChildren<TowerRangeController>();
        chara = GetComponentInChildren<Character>();
        StateUpdate(); //* Init
    }

    void Update() {
        if(GM._.State == GameState.Pause) return;
        if(GM._.State == GameState.Ready) return;
        if(GM._.State == GameState.Gameover) return;

        else if(GM._.State == GameState.Play) {
            if(trc.CurTarget && CorAttack == null) {
                var enemy = trc.CurTarget.GetComponent<Enemy>();
                Debug.Log($"ATTACK START! enemy.Hp= {enemy.Hp}");
                CorAttack = StartCoroutine(CoAttack());
            }
            //TODO ステージ終わったら、STOP処理
            // else {
            //     StopCoroutine(CorAttack);
            //     CorAttack = null;
            //     Debug.Log("STOP!");
            // }
        }
    }

#region ABSTRACT FUNC
    public abstract void CheckMergeUI();
    public abstract bool Merge(TowerKind kind = TowerKind.None);
    public abstract void Upgrade();
#endregion

#region FUNC
    /// <summary>
    /// 追加ダメージ：まとめて実行
    /// </summary>
    private void SetExtraDmgDic() {
        if(ExtraDmgDic.ContainsKey($"{AbilityType.Attack}"))
            ExtraDmgDic.Remove($"{AbilityType.Attack}");
        switch(Kind) {
            case TowerKind.Warrior:
                WarriorTower wr = this as WarriorTower;
                wr.SetExtraDmg();
                break;
            case TowerKind.Archer:
                ArcherTower ac = this as ArcherTower;
                ac.SetExtraDmg();
                break;
            case TowerKind.Magician:
                MagicianTower mg = this as MagicianTower;
                mg.SetExtraDmg();
                break;
        }
    }

    /// <summary>
    /// 追加速度：まとめて実行
    /// </summary>
    private void SetExtraSpdDic() {
        if(ExtraSpdDic.ContainsKey($"{AbilityType.Speed}"))
            ExtraSpdDic.Remove($"{AbilityType.Speed}");
        switch(Kind) {
            case TowerKind.Warrior:
                WarriorTower wr = this as WarriorTower;
                wr.SetExtraSpeed();
                break;
            case TowerKind.Archer:
                ArcherTower ac = this as ArcherTower;
                ac.SetExtraSpeed();
                break;
            case TowerKind.Magician:
                MagicianTower mg = this as MagicianTower;
                mg.SetExtraSpeed();
                break;
        }
    }

    /// <summary>
    /// 追加範囲：まとめて実行
    /// </summary>
    private void SetExtraRangeDic() {
        if(ExtraRangeDic.ContainsKey($"{AbilityType.Range}"))
            ExtraRangeDic.Remove($"{AbilityType.Range}");
        switch(Kind) {
            case TowerKind.Warrior:
                WarriorTower wr = this as WarriorTower;
                wr.SetExtraRange();
                break;
            case TowerKind.Archer:
                ArcherTower ac = this as ArcherTower;
                ac.SetExtraRange();
                break;
            case TowerKind.Magician: 
                MagicianTower mg = this as MagicianTower;
                mg.SetExtraRange();
                break;
        }
    }

    /// <summary>
    /// 追加クリティカル：まとめて実行
    /// </summary>
    private void SetExtraCritDic() {
        if(ExtraCritDic.ContainsKey($"{AbilityType.Critical}"))
            ExtraCritDic.Remove($"{AbilityType.Critical}");
        switch(Kind) {
            case TowerKind.Warrior:
                WarriorTower wr = this as WarriorTower;
                wr.SetExtraCrit();
                break;
            case TowerKind.Archer:
                ArcherTower ac = this as ArcherTower;
                ac.SetExtraCrit();
                break;
            case TowerKind.Magician:
                MagicianTower mg = this as MagicianTower;
                mg.SetExtraCrit();
                break;
        }
    }

    /// <summary>
    /// 追加クリティカルダメージ：まとめて実行
    /// </summary>
    private void SetExtraCritDmgDic() {
        if(ExtraCritDmgDic.ContainsKey($"{AbilityType.CriticalDamage}"))
            ExtraCritDmgDic.Remove($"{AbilityType.CriticalDamage}");
        switch(Kind) {
            case TowerKind.Warrior:
                WarriorTower wr = this as WarriorTower;
                wr.SetExtraCritDmg();
                break;
            case TowerKind.Archer:
                ArcherTower ac = this as ArcherTower;
                ac.SetExtraCritDmg();
                break;
            case TowerKind.Magician:
                MagicianTower mg = this as MagicianTower;
                mg.SetExtraCritDmg();
                break;
        }
    }

    /// <summary>
    /// カードでアップグレードした追加ダメージを返す
    /// </summary>
    protected int GetUpgradeCardDmg(int cardLv) 
        => (cardLv >= 1)? Mathf.RoundToInt(cardLv * TowerData.Dmg * CARD_UPG_DMGUP_PERS[LvIdx]) : 0;

    /// <summary>
    /// 攻撃
    /// </summary>
    private IEnumerator CoAttack() {
        while(true) {
            if(GM._.State == GameState.Gameover)
                yield break;

            if(trc.CurTarget == null) {
                Debug.Log("アタック終了");
                StopCoroutine(CorAttack);
                CorAttack = null;
                break;
            }
            else {
                switch(AtkType) {
                    case AttackType.Target: {
                        Debug.Log("ATTACK TARGET!");
                        LookAtTarget(trc.CurTarget);
                        var enemy = trc.CurTarget.GetComponent<Enemy>();

                        if(!enemy.gameObject.activeSelf) {
                            Debug.Log("Stop Attack Process, because Enemy is Already Dead => enemy.activeSelf= " + enemy.gameObject.activeSelf);
                            break;
                        }

                        switch(Kind) {
                            case TowerKind.Warrior:
                                //TODO EFFECT
                                SM._.SfxPlay(SM.SFX.SwordSFX);
                                chara.Animator.SetTrigger("Slash");

                                //* クリティカル
                                bool isCritical = Util._.CheckCriticalDmg(this);
                                float critDmgPer = 2 + (CritDmgPer - 1);
                                Debug.Log($"Attack:: MyTower.CritDmgPer= {CritDmgPer}, critDmgPer= {critDmgPer}");
                                int totalDmg = Mathf.RoundToInt(Dmg * (isCritical? critDmgPer : 1));

                                enemy.DecreaseHp(totalDmg, isCritical);
                                var warrior = this as WarriorTower;
                                warrior.SlashEffect(enemy.transform);
                                warrior.Skill1_Rage();
                                warrior.Skill2_Wheelwind();
                                warrior.Skill3_CheerUp();
                                warrior.Skill4_Roar();
                                break;

                            case TowerKind.Archer:
                            case TowerKind.Magician:
                                chara.Animator.SetTrigger(Kind == TowerKind.Archer? "Shot" : "Jab");
                                Shoot();
                                break;
                        }
                        break;
                    }
                    case AttackType.Round: {
                        //* 少し待機して攻撃（すぐやっちゃうと当たらない）
                        yield return new WaitForSeconds(CC_ATK_DELAY);
                        
                        //* 丸型衝突判定
                        var layerMask = 1 << Enum.Layer.Enemy;
                        Collider2D[] colliders = Physics2D.OverlapCircleAll(trc.transform.position, AtkRange * CC_RANGE_SCALE_OFFSET, layerMask);
                        Debug.Log("アタック！ ROUND:: colliders= " + colliders.Length);

                        //* Nova EF
                        GameEF idx = GameEF.NULL;
                        if(Type == TowerType.CC_IceTower) {
                            SM._.SfxPlay(SM.SFX.CCFrostSFX);
                            switch(Lv) {
                                case 1: idx = GameEF.NovaFrostLv1EF; break;
                                case 2: idx = GameEF.NovaFrostLv2EF; break;
                                case 3: idx = GameEF.NovaFrostLv3EF; break;
                            }
                        }
                        else if(Type == TowerType.CC_StunTower) {
                            SM._.SfxPlay(SM.SFX.CCLightningSFX);
                            switch(Lv) {
                                case 1: idx = GameEF.NovaLightningLv1EF; break;
                                case 2: idx = GameEF.NovaLightningLv2EF; break;
                                case 3: idx = GameEF.NovaLightningLv3EF; break;
                            }
                        }

                        //* エフェクト
                        GM._.gef.ShowEF(idx, transform.position, Util.Time2);

                        foreach(Collider2D col in colliders) {
                            Enemy enemy = col.GetComponent<Enemy>();
                            switch(Type) {
                                case TowerType.CC_IceTower:
                                    enemy.Slow(SlowSec);
                                    break;
                                case TowerType.CC_StunTower:
                                    enemy.Stun(StunSec);
                                    break;
                            }
                            // enemy.DecreaseHp(Dmg);
                            Debug.Log($"アタック！ {name}:: -> {col.name}");
                        }
                        break;
                    }
                }
            }

            float calcSec = 1 / AtkSpeed;
            Debug.Log($"CoAttack():: AtkSpeed= {AtkSpeed}, calcSec= {calcSec}");
            yield return new WaitForSeconds(calcSec);
        }
    }

    private void LookAtTarget(Transform target) {
        Vector3 dir = target.position - transform.position;
        dir = dir.normalized;
        BodySprRdr.flipX = dir.x < 0;
    }

    private void Shoot() {
        Missile ms = GM._.mm.CreateMissile();
        ms.Init(this);

        if(Kind == TowerKind.Archer) {
            SM._.SfxPlay(SM.SFX.ArrowSFX);
            var ac = this as ArcherTower;
            if(Lv >= 4)
                ac.Skill2_MultiShot();
            if(Lv >= 5)
                ac.Skill3_PassArrow();
            if(Lv >= 6)
                ac.Skill4_ArrowRain();
        }
        else if(Kind == TowerKind.Magician) {
            Debug.Log("MAGICIAN LV= " + Lv);
            if(Lv == 1) SM._.SfxPlay(SM.SFX.Magic1SFX);
            else if(Lv == 2) SM._.SfxPlay(SM.SFX.Magic2SFX);
            else if(Lv == 3) SM._.SfxPlay(SM.SFX.Magic3SFX);
            else if(Lv == 4) SM._.SfxPlay(SM.SFX.Magic4SFX);
            else if(Lv == 5) SM._.SfxPlay(SM.SFX.Magic5SFX);
            else SM._.SfxPlay(SM.SFX.Magic6SFX);
            
            var mg = this as MagicianTower;
            if(Lv >= 4 && !mg.IsMagicCircleOneTime)
                mg.Skill2_MagicCircle();
            if(Lv >= 5)
                mg.Skill3_Laser();
            if(Lv >= 6)
                mg.Skill4_Bigbang();
        }
    }
    public virtual void StateUpdate() {
        Lv = TowerData.Lv;
        Name = TowerData.name;
        // Dmg = TowerData.Dmg;
        // AtkSpeed = TowerData.AtkSpeed;
        // AtkRange = TowerData.AtkRange;
        // CritPer = TowerData.CritPer;
        // CritDmgPer = TowerData.CritDmgPer;
        SlowSec = TowerData.SlowSec;
        StunSec = TowerData.StunSec;
        Debug.Log($"<color=yellow>Tower:: StateUpdate()::Lv= {Lv}, Name= {Name}, Dmg= {Dmg}, AtkSpeed= {AtkSpeed}, AtkRange= {AtkRange}</color>");
    }

    public virtual string[] InfoState() {
        Debug.Log($"Tower:: InfoState():: Name={Name}, Lv= {Lv}");
        const string TAG_OPT = "<size=80%><color=green>";

        //* 追加ダメージ
        int extraDmg = Dmg - TowerData.Dmg;
        string extraDmgStr = extraDmg == 0? "" : $"{TAG_OPT}+{extraDmg}";
        //* 追加スピード
        float extraSpd = Util.RoundDecimal(AtkSpeed - TowerData.AtkSpeed);
        string extraSpdStr = extraSpd == 0? "" : $"{TAG_OPT}+{extraSpd}";
        //* 追加範囲
        float extraRange = Util.RoundDecimal(AtkRange - TowerData.AtkRange);
        string extraRangeStr = extraRange == 0? "" : $"{TAG_OPT}+{extraRange}";
        //* 追加クリティカル
        float extraCirtPer = CritPer - TowerData.CritPer;
        string extraCritStr = extraCirtPer == 0? "" : $"{TAG_OPT}+{extraCirtPer * 100}";
        //* 追加クリティカルダメージ
        float extraCritDmgPer = CritDmgPer - TowerData.CritDmgPer;
        string extraCritDmgPerStr = extraCritDmgPer == 0? "" : $"{TAG_OPT}+{extraCritDmgPer * 100}";

        string[] states = new string[10];
        states[0] = Lv.ToString(); //* Gradeラベルとして表示
        states[1] = Type.ToString(); //* タイプ
        states[2] = Kind.ToString(); //* 種類
        states[3] = $"{TowerData.Dmg}{extraDmgStr}";
        states[4] = $"{TowerData.AtkSpeed}{extraSpdStr}";
        states[5] = $"{TowerData.AtkRange}{extraRangeStr}";
        states[6] = $"{TowerData.CritPer * 100}{extraCritStr}%";
        states[7] = $"{TowerData.CritDmgPer * 100}{extraCritDmgPerStr}%";
        states[8] = $"{TowerData.SlowSec}";
        states[9] = $"{TowerData.StunSec}";
        return states;
    }
#endregion

    private void OnDrawGizmos() {
        var pos = transform.position;
        Gizmos.color = trc.CurTarget? Color.red : Color.yellow;
        Gizmos.DrawWireSphere(new Vector3(pos.x, pos.y, pos.z), AtkRange * CC_RANGE_SCALE_OFFSET);
    }
}
