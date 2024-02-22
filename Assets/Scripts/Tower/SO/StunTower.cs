using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunTower : Tower {
    public override string[] InfoState() {
        string[] state = new string[8];
        state[0] = Name;
        state[1] = Lv.ToString();
        state[2] = Dmg.ToString();
        state[3] = AtkSpeed.ToString();
        state[4] = AtkRange.ToString();
        state[5] = SplashRange.ToString();
        state[6] = SlowPer.ToString();
        state[7] = StunSec.ToString();

        return state;
    }

    public override void StateUpdate(SettingTowerData towerData) {
        Lv = towerData.Lv;
        Dmg = towerData.Dmg;
        AtkSpeed = towerData.AtkSpeed;
        AtkRange = towerData.AtkRange;
        SplashRange = towerData.SplashRange;
        SlowPer = towerData.SlowPer;
        StunSec = towerData.StunSec;
    }
}
