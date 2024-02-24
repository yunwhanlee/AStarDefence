using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TowerType {
    Random, Board, CC_IceTower, CC_StunTower
}
public enum TowerKind {
    None,
    //* Random
    Warrior, Archor, Magician,
}

public abstract class Tower : MonoBehaviour {
    public SettingTowerData TowerData;
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

    void Start() {
        StateUpdate(); //* Init
    }

    #region Func
        public virtual string[] InfoState() {
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

        public virtual void StateUpdate() {
            Debug.Log("Tower:: StateUpdate()::");
            Lv = TowerData.Lv;
            Dmg = TowerData.Dmg;
            AtkSpeed = TowerData.AtkSpeed;
            AtkRange = TowerData.AtkRange;
            SplashRange = TowerData.SplashRange;
            SlowPer = TowerData.SlowPer;
            StunSec = TowerData.StunSec;
        }
    #endregion
}
