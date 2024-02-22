using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceTower : Tower {
    public override string[] InfoState() {
        string[] state = new string[9];
        int i = 0;
        state[i++] = Lv.ToString(); //* Gradeラベルとして表示
        state[i++] = Dmg.ToString();
        state[i++] = AtkSpeed.ToString();
        state[i++] = AtkRange.ToString();
        state[i++] = SplashRange.ToString();
        state[i++] = SlowPer.ToString();
        state[i++] = StunSec.ToString();
        state[i++] = CritPer.ToString();
        state[i++] = CritDmgPer.ToString();

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
