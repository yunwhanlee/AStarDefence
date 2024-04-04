using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public enum SKT_KEY {
    SKT_EXTRA_DMG, 
    SKT_EXTRA_SPD,
    SKT_EXTRA_RANGE, 
    SKT_EXTRA_CIRT, 
    SKT_EXTRA_CIRTDMG
}
public enum SKT_UT {
    EXTRA_MONEY, EXTRA_LIFE, TEN_KILL_1MONEY, EXTRA_EXP, TEN_KILL_2MONEY
}
public enum SKT_WR {
    EXTRA_DMG_A, EXTRA_RANGE, EXTRA_SPD, EXTRA_RAGE_TIME, EXTRA_DMG_B
}
public enum SKT_AC {
    EXTRA_DMG_A, CIRT_DMG_PER_A, CRIT_PER, CIRT_DMG_PER_B, EXTRA_DMG_B
}
public enum SKT_MG {
    EXTRA_DMG_A, EXTRA_RANGE, CRIT_PER, EXPLOSION_DMG_PER, EXTRA_DMG_B
}

public enum ItemAbilityType {
	Attack, Speed, Range, Critical, CriticalDamage, 
}


public static class Enum {
    public enum Scene {Home, Game}
    public enum Difficulty {Easy, Normal, Hard}

    public class Layer { //* Switch文で使えるため、整数化
        public const int 
            Default = 0,
            UI = 5,
            Wall = 6,
            Board = 7, 
            CCTower = 8, 
            Tower = 9,
            TowerRange = 10,
            Enemy = 11,
            SwitchTower = -1;
    }

    public enum Tag {
        Enemy
    }

    public enum Grade {
        None = -1, 
        //* 노말, 레어, 에픽, 유니크, 레전드, 신화, 태초
        Common, Rare, Epic, Unique, Legend, Myth, Prime
    }
    public enum ItemType {
        Weapon, Shoes, Accessories, Relic, Etc
    }

    public enum BossRwd {
        SuccessionTicket, 
        ChangeTypeTicket, 
        IncreaseLife, 
        IncreaseCCTowerCnt, 
        SwitchTowerPositionCnt
    }

    /// <summary> SKILLTREEのUtilityデータ情報 </summary>
    // public static enum SKT_UT {
    //     EXTRA_MONEY,
    //     EXTRA_LIFE,
    //     TEN_KILL_EXTRA_1MONEY,
    //     EXTRA_EXP,
    //     TEN_KILL_EXTRA_2MONEY,
    // }
}
