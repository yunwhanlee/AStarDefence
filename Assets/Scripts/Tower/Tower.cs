using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.UI;

public enum TowerType {
    Random, Board, CC_IceTower, CC_StunTower
}
public enum TowerKind {
    None,
    //* Random
    Warrior, Archer, Magician,
}

public abstract class Tower : MonoBehaviour {
    public SettingTowerData TowerData;
    public TowerType Type;
    public TowerKind Kind;
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
        StateUpdate(); //* Init
    }

#region Abstract Func
    public abstract void CheckMergeUI();
    public abstract bool Merge();
    public abstract void Upgrade();
#endregion  

#region Func
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
