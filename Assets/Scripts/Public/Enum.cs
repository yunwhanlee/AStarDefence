using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
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
    //* Relic Abilityとして適用することはここから
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
        GoldKey, Coin, Diamond, Exp, 
        Goblin0, Goblin1, Goblin2, Goblin3, Goblin4, Goblin5, Goblin6,
        Ore0, Ore1, Ore2, Ore3, Ore4, Ore5, Ore6, Ore7, Ore8,
        SkillPoint
    }
    public static string GetNoshowInvItemNameStr(NoshowInvItem noshowInvItemEnum) {
        return noshowInvItemEnum.ToString();
    }
    public static NoshowInvItem GetGoblinInvItem(int idx) {
        switch(idx) {
            case 0: return NoshowInvItem.Goblin0;
            case 1: return NoshowInvItem.Goblin1;
            case 2: return NoshowInvItem.Goblin2;
            case 3: return NoshowInvItem.Goblin3;
            case 4: return NoshowInvItem.Goblin4;
            case 5: return NoshowInvItem.Goblin5;
            case 6: return NoshowInvItem.Goblin6;
        }
        return NoshowInvItem.Goblin; //* 何も該当することがなかったら、エラーとして表示
    }
    public static NoshowInvItem GetOreInvItem(int idx) {
        switch(idx) {
            case 0: return NoshowInvItem.Ore0;
            case 1: return NoshowInvItem.Ore1;
            case 2: return NoshowInvItem.Ore2;
            case 3: return NoshowInvItem.Ore3;
            case 4: return NoshowInvItem.Ore4;
            case 5: return NoshowInvItem.Ore5;
            case 6: return NoshowInvItem.Ore6;
            case 7: return NoshowInvItem.Ore7;
            case 8: return NoshowInvItem.Ore8;
        }
        return NoshowInvItem.Ore; //* 何も該当することがなかったら、エラーとして表示
    }
    public enum ConsumableItem {
        BizzardScroll, LightningScroll, SteamPack0, SteamPack1,
        ChestCommon, ChestDiamond, ChestEquipment, ChestGold, ChestPremium,
        Clover, GoldClover,
        Present0, Present1, Present2,
        SoulStone, MagicStone,
    }

    public static string GetChestName(ConsumableItem enumChest) {
        string name = "";
        switch(enumChest) {
            case ConsumableItem.ChestCommon: name = "일반상자"; break;
            case ConsumableItem.ChestDiamond: name = "다이아상자"; break;
            case ConsumableItem.ChestEquipment: name = "장비상자"; break;
            case ConsumableItem.ChestGold: name = "황금상자"; break;
            case ConsumableItem.ChestPremium: name = "프리미엄상자"; break;
        }
        return name;
    }

    public static ConsumableItem GetConsumableItem(int idx) {
        switch(idx) {
            case 0: return ConsumableItem.BizzardScroll;
            case 1: return ConsumableItem.LightningScroll;
            case 2: return ConsumableItem.SteamPack0;
            case 3: return ConsumableItem.SteamPack1;
        }
        return ConsumableItem.BizzardScroll;
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
}
