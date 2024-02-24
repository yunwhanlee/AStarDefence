using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarriorTower : Tower {
    public override string[] InfoState() {
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

    public override void StateUpdate() {
        Lv = TowerData.Lv;
        Dmg = TowerData.Dmg;
        AtkSpeed = TowerData.AtkSpeed;
        AtkRange = TowerData.AtkRange;
        SplashRange = TowerData.SplashRange;
        SlowPer = TowerData.SlowPer;
        StunSec = TowerData.StunSec;
    }
}
