using System;
using System.Collections;
using System.Collections.Generic;
using Assets.PixelFantasy.PixelHeroes.Common.Scripts.CharacterScripts;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public enum TowerType {Random, Board, CC_IceTower, CC_StunTower}
public enum TowerKind {None = -1, Warrior, Archer, Magician}
public enum AttackType {Target, Round}

public abstract class Tower : MonoBehaviour {
    public const string DIC_UPGRADE = "Upgrade";

    //* 外部
    public SettingTowerData TowerData;
    public TowerRangeController trc;
    public Character chara;

    //* Value
    private Coroutine CorAttack;
    public TowerType Type;
    public TowerKind Kind;
    [Tooltip("AttackType : Target：ターゲット型、Round：自分の原点から矩形の爆発（Splash ON）")]
    public AttackType AtkType;
    public SpriteRenderer BodySprRdr;
    public Sprite MissileSpr;
    public string Name;
    [Range(1, 7)] public int Lv;

    public Dictionary<string, int> ExtraDmgDic = new Dictionary<string, int>();
    public int Dmg {
        get {
            //* 追加ダメージ
            int extraDmg = 0;
            foreach(var dic in ExtraDmgDic)
                extraDmg += dic.Value;

            return TowerData.Dmg + extraDmg;
        }
    }

    public Dictionary<string, float> ExtraSpdDic = new Dictionary<string, float>();
    public float AtkSpeed {
        get {
            float extraSpd = 0;
            foreach(var dic in ExtraSpdDic)
                extraSpd += dic.Value;
            return TowerData.AtkSpeed - extraSpd;
        }
    }
    [Range(0, 10)] public float AtkRange;
    [Range(0.00f, 1.00f)] public float CritPer;
    public float CritDmgPer;
    //* CC
    [Range(0.0f, 5.00f)] public float SlowSec;
    [Range(0.0f, 5.0f)] public float StunSec;

    void Awake() {
        trc = GetComponentInChildren<TowerRangeController>();
        chara = GetComponentInChildren<Character>();
        StateUpdate(); //* Init
    }

    void Update() {
        if(GM._.State == GameState.Ready) {

        }
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
    private IEnumerator CoAttack() {
        while(true) {
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
                                chara.Animator.SetTrigger("Slash");
                                //* クリティカル
                                bool isCritical = Util.CheckCriticalDmg(this);
                                int totalDmg = Dmg * (isCritical? 2 : 1);
                                enemy.DecreaseHp(totalDmg, isCritical);
                                var warrior = this as WarriorTower;
                                warrior.Skill1_Rage();
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
                        var layerMask = 1 << Enum.Layer.Enemy;
                        Collider2D[] colliders = Physics2D.OverlapCircleAll(trc.transform.position, AtkRange, layerMask);
                        Debug.Log("アタック！ ROUND:: colliders= " + colliders.Length);
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
            yield return new WaitForSeconds(AtkSpeed);
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
            var archer = this as ArcherTower;
            if(Lv > 3)
                archer.Skill2_MultiShot();
            if(Lv > 4)
                archer.Skill3_PassArrow();

        }
    }
    public virtual void StateUpdate() {
        Lv = TowerData.Lv;
        Name = TowerData.name;
        // Dmg = TowerData.Dmg;
        // AtkSpeed = TowerData.AtkSpeed;
        AtkRange = TowerData.AtkRange;
        CritPer = TowerData.CritPer;
        CritDmgPer = TowerData.CritDmgPer;
        SlowSec = TowerData.SlowSec;
        StunSec = TowerData.StunSec;
        Debug.Log($"<color=yellow>Tower:: StateUpdate()::Lv= {Lv}, Name= {Name}, Dmg= {Dmg}, AtkSpeed= {AtkSpeed}, AtkRange= {AtkRange}</color>");
    }

    public virtual string[] InfoState() {
        Debug.Log($"Tower:: InfoState():: Name={Name}, Lv= {Lv}");

        //* 追加ダメージ
        int extraDmgVal = Dmg - TowerData.Dmg;
        string extraDmgStr = extraDmgVal == 0? "" : $"<color=green>+{extraDmgVal}";
        //* 追加スピード
        float extraSpdVal = AtkSpeed - TowerData.AtkSpeed;
        string extraSpdStr = extraSpdVal == 0? "" : $"<color=green>{extraSpdVal})";

        string[] states = new string[10];
        states[0] = Lv.ToString(); //* Gradeラベルとして表示
        states[1] = Type.ToString(); //* タイプ
        states[2] = Kind.ToString(); //* 種類
        states[3] = $"{TowerData.Dmg}{extraDmgStr}";
        states[4] = $"{TowerData.AtkSpeed}{extraSpdStr}";
        states[5] = $"{TowerData.AtkRange}";
        states[6] = $"{TowerData.CritPer * 100}%";
        states[7] = $"{TowerData.CritDmgPer * 100}%";
        states[8] = $"{TowerData.SlowSec}";
        states[9] = $"{TowerData.StunSec}";
        return states;
    }
#endregion

    private void OnDrawGizmos() {
        var pos = transform.position;
        Gizmos.color = trc.CurTarget? Color.red : Color.yellow;
        Gizmos.DrawWireSphere(new Vector3(pos.x, pos.y - 0.15f, pos.z), AtkRange * 1.575f);
    }
}
