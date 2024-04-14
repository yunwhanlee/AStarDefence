using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AssetKits.ParticleImage;
using Inventory.Model;
using Inventory.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;

public class InventoryEquipUIManager : MonoBehaviour {
    public const string WEAPON_SLOT_OBJ_NAME = "WeaponInvItemUISlot";
    public const string SHOES_SLOT_OBJ_NAME = "ShoesInvItemUISlot";
    public const string RING_SLOT_OBJ_NAME = "RingInvItemUISlot";
    public const string RELIC_SLOT_OBJ_NAME = "RelicInvItemUISlot";

    [field: SerializeField] public float AttackPer {
        get => DM._.DB.EquipDB.AttackPer;
        set {
            DM._.DB.EquipDB.AttackPer = value;
            AttackPerTxt.text = $"{value * 100}%";
        }
    }
    [field: SerializeField] public float SpeedPer {
        get => DM._.DB.EquipDB.SpeedPer;
        set {
            DM._.DB.EquipDB.SpeedPer = value;
            SpeedPerTxt.text = $"{value * 100}%";
        }
    }
    [field: SerializeField] public float RangePer {
        get => DM._.DB.EquipDB.RangePer;
        set {
            DM._.DB.EquipDB.RangePer = value;
            RangePerTxt.text = $"{value * 100}%";
        }
    }
    [field: SerializeField] public float CritPer {
        get => DM._.DB.EquipDB.CritPer;
        set {
            DM._.DB.EquipDB.CritPer = value;
            CritPerTxt.text = $"{value * 100}%";
        }
    }
    [field: SerializeField] public float CritDmgPer {
        get => DM._.DB.EquipDB.CritDmgPer;
        set {
            DM._.DB.EquipDB.CritDmgPer = value;
            CritDmgPerTxt.text = $"{value * 100}%";
        }
    }
    [field: SerializeField] public float ClearEtcPer {
        get => DM._.DB.EquipDB.ClearEtcPer;
        set => DM._.DB.EquipDB.ClearEtcPer = value;
    }
    [field: SerializeField] public float ClearCoinPer {
        get => DM._.DB.EquipDB.ClearCoinPer;
        set => DM._.DB.EquipDB.ClearCoinPer = value;
    }
    [field: SerializeField] public int StartCoin {
        get => DM._.DB.EquipDB.StartMoney;
        set => DM._.DB.EquipDB.StartMoney = value;
    }
    [field: SerializeField] public int StartLife {
        get => DM._.DB.EquipDB.StartLife;
        set => DM._.DB.EquipDB.StartLife = value;
    }
    [field: SerializeField] public float ItemDropPer {
        get => DM._.DB.EquipDB.ItemDropPer;
        set => DM._.DB.EquipDB.ItemDropPer = value;
    }
    [field: SerializeField] public int BonusCoinBy10Kill {
        get => DM._.DB.EquipDB.BonusCoinBy10Kill;
        set => DM._.DB.EquipDB.BonusCoinBy10Kill = value;
    }
    [field: SerializeField] public float WarriorAttackPer {
        get => DM._.DB.EquipDB.WarriorAttackPer;
        set => DM._.DB.EquipDB.WarriorAttackPer = value;
    }
    [field: SerializeField] public float ArcherAttackPer {
        get => DM._.DB.EquipDB.ArcherAttackPer;
        set => DM._.DB.EquipDB.ArcherAttackPer = value;
    }
    [field: SerializeField] public float MagicianAttackPer {
        get => DM._.DB.EquipDB.MagicianAttackPer;
        set => DM._.DB.EquipDB.MagicianAttackPer = value;
    }
    [field: SerializeField] public float WarriorUpgCostPer {
        get => DM._.DB.EquipDB.WarriorUpgCostPer;
        set => DM._.DB.EquipDB.WarriorUpgCostPer = value;
    }
    [field: SerializeField] public float ArcherUpgCostPer {
        get => DM._.DB.EquipDB.ArcherUpgCostPer;
        set => DM._.DB.EquipDB.ArcherUpgCostPer = value;
    }
    [field: SerializeField] public float MagicianUpgCostPer {
        get => DM._.DB.EquipDB.MagicianUpgCostPer;
        set => DM._.DB.EquipDB.MagicianUpgCostPer = value;
    }
    [field: SerializeField] public int SkillPoint {
        get => DM._.DB.EquipDB.SkillPoint;
        set => DM._.DB.EquipDB.SkillPoint = value;
    }

    public InventoryUIItem[] EquipItemSlotUIs;
    public GameObject[] EmptyIconObjs;
    public ParticleImage[] EquipUIEFs;

    public TMP_Text AttackPerTxt;
    public TMP_Text SpeedPerTxt;
    public TMP_Text RangePerTxt;
    public TMP_Text CritPerTxt;
    public TMP_Text CritDmgPerTxt;

#region FUNC
    public InventoryItem FindEquipItem(Enum.ItemType type) {
        int idx = HM._.ivCtrl.FindCurEquipItemIdx(type);
        InventoryItem equipItem = HM._.ivm.GetCurItemUIFromIdx(idx);
        return equipItem;
    }

    /// <summary>
    /// 現在装置しているEquipスロットを最新化
    /// </summary>
    public void UpdateAllEquipSlots() {
        EquipItem(Enum.ItemType.Weapon, FindEquipItem(Enum.ItemType.Weapon), isEffect: false);
        EquipItem(Enum.ItemType.Shoes, FindEquipItem(Enum.ItemType.Shoes), isEffect: false);
        EquipItem(Enum.ItemType.Ring, FindEquipItem(Enum.ItemType.Ring), isEffect: false);
        EquipItem(Enum.ItemType.Relic, FindEquipItem(Enum.ItemType.Relic), isEffect: false);
    }

    private void SetEquipEmptyIcon(Enum.ItemType type, bool isActive)
        => EmptyIconObjs[(int)type].SetActive(isActive);

    public void ResetEquipSlot(Enum.ItemType type) {
        EquipItemSlotUIs[(int)type].ResetData();
        EmptyIconObjs[(int)type].SetActive(true);
        SetEquipEmptyIcon(type, true);
    }

    public void EquipItem(Enum.ItemType type, InventoryItem invItem, bool isEffect = true) {
        if(invItem.IsEmpty)
            return;

        ItemSO dt = invItem.Data;
        EquipItemSlotUIs[(int)type].SetUIData (
            dt.Type, 
            dt.Grade, 
            dt.ItemImg, 
            invItem.Quantity, 
            invItem.Lv, 
            invItem.RelicAbilities, 
            invItem.IsEquip
        );

        SetEquipEmptyIcon(type, false);

        if(isEffect)
            EquipItemSlotUIs[(int)type].PlayScaleUIEF(EquipItemSlotUIs[(int)type], dt.ItemImg);
    }

    private void ResetEquipAbilityData() {
        AttackPer = 0; 
        SpeedPer = 0; 
        RangePer = 0; 
        CritPer = 0; 
        CritDmgPer = 0;
        ClearEtcPer = 0;
        ClearCoinPer = 0;
        StartCoin = 0;
        StartLife = 0;
        ItemDropPer = 0;
        SkillPoint = 0;
        BonusCoinBy10Kill = 0;
        WarriorAttackPer = 0;
        ArcherAttackPer = 0;
        MagicianAttackPer = 0;
        WarriorUpgCostPer = 0;
        ArcherUpgCostPer = 0;
        MagicianUpgCostPer = 0;
    }

    private void SetEquipAbilityData(InventoryItem invItem) {
        if(invItem.IsEmpty) return;
        List<AbilityData> abilityDataList = new List<AbilityData>();

        //* Relicとか他のタイプに分け、Abilityデータリスト作成
        if(invItem.Data.Type == Enum.ItemType.Relic) {
            AbilityData[] abilityDb = invItem.Data.Abilities;
            Array.ForEach(invItem.RelicAbilities, relicAbtType => {
                foreach(var db in abilityDb) {
                    if(db.Type == relicAbtType) {
                        abilityDataList.Add(new AbilityData(relicAbtType, db.Val, db.UpgradeVal));
                        break;
                    }
                }
            });
        }
        else
            abilityDataList = invItem.Data.Abilities.ToList();

        //* データ設定
        int lvIdx = invItem.Lv - 1;
        foreach(var abt in abilityDataList) {
            Debug.Log($"SetEquipAbilityData():: {invItem.Data.Name}, ability= {abt.Type}");
            switch(abt.Type) {
                case AbilityType.Attack:
                    AttackPer += abt.Val + (lvIdx * abt.UpgradeVal);
                    break;
                case AbilityType.Speed:
                    SpeedPer += abt.Val + (lvIdx * abt.UpgradeVal);
                    break;
                case AbilityType.Range:
                    RangePer += abt.Val + (lvIdx * abt.UpgradeVal);
                    break;
                case AbilityType.Critical:
                    CritPer += abt.Val + (lvIdx * abt.UpgradeVal);
                    break;
                case AbilityType.CriticalDamage:
                    CritDmgPer += abt.Val + (lvIdx * abt.UpgradeVal);
                    break;
                case AbilityType.ClearEtc:
                    ClearEtcPer += abt.Val + (lvIdx * abt.UpgradeVal);
                    break;
                case AbilityType.ClearCoin:
                    ClearCoinPer += abt.Val + (lvIdx * abt.UpgradeVal);
                    break;
                case AbilityType.StartMoney:
                    StartCoin += (int)(abt.Val + (lvIdx * abt.UpgradeVal));
                    break;
                case AbilityType.StartLife:
                    StartLife += (int)(abt.Val + (lvIdx * abt.UpgradeVal));
                    break;
                case AbilityType.ItemDropPer:
                    ItemDropPer += abt.Val + (lvIdx * abt.UpgradeVal);
                    break;
                case AbilityType.SkillPoint:
                    SkillPoint += (int)(abt.Val + (lvIdx * abt.UpgradeVal));
                    break;
                case AbilityType.BonusCoinBy10Kill:
                    BonusCoinBy10Kill += (int)(abt.Val + (lvIdx * abt.UpgradeVal));
                    break;
                case AbilityType.WarriorAttack:
                    WarriorAttackPer += abt.Val + (lvIdx * abt.UpgradeVal);
                    break;
                case AbilityType.ArcherAttack:
                    ArcherAttackPer += abt.Val + (lvIdx * abt.UpgradeVal);
                    break;
                case AbilityType.MagicianAttack:
                    MagicianAttackPer += abt.Val + (lvIdx * abt.UpgradeVal);
                    break;
                case AbilityType.WarriorUpgCost:
                    WarriorUpgCostPer += abt.Val + (lvIdx * abt.UpgradeVal);
                    break;
                case AbilityType.ArcherUpgCost:
                    ArcherUpgCostPer += abt.Val + (lvIdx * abt.UpgradeVal);
                    break;
                case AbilityType.MagicianUpgCost:
                    MagicianUpgCostPer += abt.Val + (lvIdx * abt.UpgradeVal);
                    break;
            }
        }
    }

    public void UpdateAllEquipAbilityData() {
        ResetEquipAbilityData();
        SetEquipAbilityData(FindEquipItem(Enum.ItemType.Weapon));
        SetEquipAbilityData(FindEquipItem(Enum.ItemType.Shoes));
        SetEquipAbilityData(FindEquipItem(Enum.ItemType.Ring));
        SetEquipAbilityData(FindEquipItem(Enum.ItemType.Relic));
    }
#endregion
}
