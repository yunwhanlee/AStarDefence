using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.UI;

public enum TowerType {Random, Board, CC_IceTower, CC_StunTower}
public enum TowerKind {None, Warrior, Archer, Magician}
public enum AttackType {Target, Round}

public abstract class Tower : MonoBehaviour {
    //* 外部
    public SettingTowerData TowerData;
    public TowerRangeController trc;

    //* Value
    private Coroutine CorAttack;
    public TowerType Type;
    public TowerKind Kind;
    [Tooltip("AttackType : Target：ターゲット型、Round：自分の原点から矩形の爆発（Splash ON）")]
    public AttackType AtkType;
    public string Name;
    [Range(1, 7)] public int Lv;
    public int Dmg;
    public float AtkSpeed;
    [Range(0, 10)] public float AtkRange;
    [Range(0, 10)] public float SplashRange;
    [Range(0.00f, 1.00f)] public float CritPer;
    public float CritDmgPer;
    //* CC
    [Range(0.00f, 1.00f)] public float SlowPer;
    [Range(0.0f, 5.0f)] public float StunSec;

    void Awake() {
        trc = GetComponentInChildren<TowerRangeController>();
        StateUpdate(); //* Init
    }

    void Update() {
        if(GM._.state == GM.State.Ready) {

        }
        else if(GM._.state == GM.State.Play) {
            if(trc.CurTarget && CorAttack == null) {
                Debug.Log("ATTACK START!");
                CorAttack = StartCoroutine(CoAttack());
            }
            // else {
            //     StopCoroutine(CorAttack);
            //     CorAttack = null;
            //     Debug.Log("STOP!");
            // }
        }
    }

#region ABSTRACT FUNC
    public abstract void CheckMergeUI();
    public abstract bool Merge();
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
                        Debug.Log("アタック！ TARGET");
                        //TODO ミサイル発射
                        break;
                    }
                    case AttackType.Round: {
                        var layerMask = 1 << Enum.Layer.Enemy;
                        Collider2D[] colliders = Physics2D.OverlapCircleAll(trc.transform.position, AtkRange, layerMask);
                        Debug.Log("アタック！ ROUND:: colliders= " + colliders.Length);
                        foreach(Collider2D col in colliders) {
                            Monster enemy = col.GetComponent<Monster>();
                            switch(Type) {
                                case TowerType.CC_IceTower:
                                    enemy.Slow(SlowPer);
                                    break;
                                case TowerType.CC_StunTower:
                                    enemy.Stun(StunSec);
                                    break;
                            }
                            enemy.DecreaseHp(2);
                            Debug.Log($"アタック！ {name}:: -> {col.name}");
                        }
                        break;
                    }
                }
            }

            yield return new WaitForSeconds(AtkSpeed);
        }
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, AtkRange);
    }

    public virtual void StateUpdate() {
        Debug.Log("<color=yellow>Tower:: StateUpdate()::</color>");
        Lv = TowerData.Lv;
        Name = TowerData.name;
        Dmg = TowerData.Dmg;
        AtkSpeed = TowerData.AtkSpeed;
        AtkRange = TowerData.AtkRange;
        SplashRange = TowerData.SplashRange;
        SlowPer = TowerData.SlowPer;
        StunSec = TowerData.StunSec;
        Debug.Log($"<color=yellow>Lv= {Lv}, Name= {Name}, Dmg= {Dmg}</color>");
    }

    public virtual string[] InfoState() {
        Debug.Log($"Tower:: InfoState():: Name={Name}, Lv= {Lv}");
        string[] states = new string[9];
        int i = 0;
        states[i++] = Lv.ToString(); //* Gradeラベルとして表示
        states[i++] = Dmg.ToString();
        states[i++] = AtkSpeed.ToString();
        states[i++] = AtkRange.ToString();
        states[i++] = SplashRange.ToString();
        states[i++] = CritPer.ToString();
        states[i++] = CritDmgPer.ToString();
        states[i++] = SlowPer.ToString();
        states[i++] = StunSec.ToString();
        return states;
    }

#endregion
}
