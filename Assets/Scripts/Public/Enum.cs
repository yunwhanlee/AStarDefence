using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;


#region SKILL TREE
    public enum SKT_KEY {
        SKT_EXTRA_DMG, SKT_EXTRA_SPD, SKT_EXTRA_RANGE, SKT_EXTRA_CIRT, SKT_EXTRA_CIRTDMG
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
#endregion

public enum AbilityType {
	Attack, 
    Speed, 
    Range, 
    Critical, 
    CriticalDamage,
    ClearEtc,
    ClearCoin,
    StartCoin,
    StartLife,
    ItemDropPer,
    SkillPoint,
    BonusCoinBy10Kill,
    WarriorAttack,
    ArcherAttack,
    MagicianAttack,
    WarriorUpgCost,
    ArcherUpgCost,
    MagicianUpgCost,
}

public class Etc {
    public enum NoshowInvItem {
        Goblin = -2, Ore = -1,
        Coin, Diamond, Exp, 
        Goblin0, Goblin1, Goblin2, Goblin3, Goblin4, Goblin5, Goblin6,
        Ore0, Ore1, Ore2, Ore3, Ore4, Ore5, Ore6, Ore7, Ore8,
        SkillPoint
    }
    public static string GetNoshowInvItemNameStr(NoshowInvItem noshowInvItemEnum) {
        return noshowInvItemEnum.ToString();
    }
    public enum ConsumableItem {
        BizzardScroll, 
        ChestCommon, ChestDiamond, ChestEquipment, ChestGoldKey, ChestPremium, 
        Clover, GoldClover, 
        GoldKey, LightningScroll, MagicStone, 
        Present0, Present1, Present2, 
        SoulStone, SteamPack0, SteamPack1
    }
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
        Common, Rare, Epic, Unique, Legend, Myth, Prime
    }
    public static string GetGradeName(Grade grade) {
        return (grade == Grade.Common)? "일반" 
            : (grade == Grade.Rare)? "레어"
            : (grade == Grade.Epic)? "에픽"
            : (grade == Grade.Unique)? "유니크"
            : (grade == Grade.Legend)? "레전드"
            : (grade == Grade.Myth)? "신화"
            : (grade == Grade.Prime)? "태초"
            : "기타";
    }
    public enum ItemType {
        Weapon, Shoes, Ring, Relic, Etc
    }
    public static string GetItemTypeName(ItemType itemType) {
        return (itemType == ItemType.Weapon)? "무기" 
            : (itemType == ItemType.Shoes)? "신발"
            : (itemType == ItemType.Ring)? "반지"
            : (itemType == ItemType.Relic)? "유물"
            : "기타";
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
