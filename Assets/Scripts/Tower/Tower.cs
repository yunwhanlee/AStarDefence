using System.Collections;
using System.Collections.Generic;
using Assets.PixelFantasy.PixelHeroes.Common.Scripts.CharacterScripts;
using Microsoft.Unity.VisualStudio.Editor;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public enum TowerType {Random, Board, CC_IceTower, CC_StunTower}
public enum TowerKind {None = -1, Warrior, Archer, Magician}
public enum AttackType {Target, Round}

public abstract class Tower : MonoBehaviour {
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
    public int Dmg;
    public float AtkSpeed;
    [Range(0, 10)] public float AtkRange;
    [Range(0, 10)] public float SplashRange;
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
                                enemy.DecreaseHp(Dmg);
                                GM._.gef.ShowDmgTxtEF(enemy.transform.position, Dmg);
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
                            enemy.DecreaseHp(Dmg);
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
        Missile missile = GM._.mm.CreateMissile();
        missile.Init(this);
    }

    public virtual void StateUpdate() {
        Lv = TowerData.Lv;
        Name = TowerData.name;
        Dmg = TowerData.Dmg;
        AtkSpeed = TowerData.AtkSpeed;
        AtkRange = TowerData.AtkRange;
        SplashRange = TowerData.SplashRange;
        SlowSec = TowerData.SlowSec;
        StunSec = TowerData.StunSec;
        Debug.Log($"<color=yellow>Tower:: StateUpdate()::Lv= {Lv}, Name= {Name}, Dmg= {Dmg}, AtkSpeed= {AtkSpeed}, AtkRange= {AtkRange}, SplashRange= {SplashRange}</color>");
    }

    public virtual string[] InfoState() {
        Debug.Log($"Tower:: InfoState():: Name={Name}, Lv= {Lv}");
        const TowerKind W = TowerKind.Warrior;
        const TowerKind A = TowerKind.Archer;
        // const TowerKind M = TowerKind.Magician;
        int W_DMG = TowerManager.WARRIOR_CARD_DMG_UP;
        int A_DMG = TowerManager.ARCHER_CARD_DMG_UP;
        int M_DMG = TowerManager.MAGICIAN_CARD_DMG_UP;

        //* カードアップグレードデータ反映
        string extraDmg = ExtraTxt((Kind == W)? W_DMG : (Kind == A)? A_DMG : M_DMG);

        string[] states = new string[9];
        int i = 0;
        states[i++] = Lv.ToString(); //* Gradeラベルとして表示
        states[i++] = $"{TowerData.Dmg}{extraDmg}";
        states[i++] = $"{TowerData.AtkSpeed}";
        states[i++] = $"{TowerData.AtkRange}";
        states[i++] = $"{TowerData.SplashRange}";
        states[i++] = $"{TowerData.CritPer}";
        states[i++] = $"{TowerData.CritDmgPer}";
        states[i++] = $"{TowerData.SlowSec}";
        states[i++] = $"{TowerData.StunSec}";
        return states;
    }
#endregion
    private string ExtraTxt(int unit) {
        int cardLv = GM._.tm.TowerCardUgrLvs[(int)Kind];
        string txt = $"<color=green>(+{Lv * unit * cardLv})</color>";
        return Type == TowerType.Random? (cardLv > 0? txt : "") : "";
    }

    private void OnDrawGizmos() {
        var pos = transform.position;
        Gizmos.color = trc.CurTarget? Color.red : Color.yellow;
        Gizmos.DrawWireSphere(new Vector3(pos.x, pos.y - 0.15f, pos.z), AtkRange * 1.575f);
    }
}
