using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TowerType {Random, Board, CC_IceTower, CC_StunTower}
public enum TowerKind {
    //* Random
    Warrior, Archor, Magician,
}

public abstract class Tower : MonoBehaviour {
    public TowerType Type;
    public TowerKind Kind;
    public string Name;
    [Range(0, 7)] public int Lv;
    public int Dmg;
    public float AtkSpeed;
    [Range(0, 10)] public int AtkRange;
    [Range(0, 10)] public int SplashRange;
    [Range(0.00f, 1.00f)] public float CritPer;
    public float CritDmgPer;
    //* CC
    [Range(0.00f, 1.00f)] public float SlowPer;
    [Range(0.0f, 5.0f)] public int StunSec;

    #region Func
        public abstract string[] InfoState();
        public abstract void StateUpdate(SettingTowerData towerData);
    #endregion
}
