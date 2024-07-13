using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AssetKits.ParticleImage;
using Inventory.Model;
using Inventory.UI;
using TMPro;
using UnityEngine;

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
        get => DM._.DB.EquipDB.ClearExpPer;
        set => DM._.DB.EquipDB.ClearExpPer = value;
    }
    [field: SerializeField] public float ClearCoinPer {
        get => DM._.DB.EquipDB.ClearCoinPer;
        set => DM._.DB.EquipDB.ClearCoinPer = value;
    }
    [field: SerializeField] public int StartMoney {
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
    // [field: SerializeField] public int SkillPoint {
    //     get => DM._.DB.EquipDB.SkillPoint;
    //     set => DM._.DB.EquipDB.SkillPoint = value;
    // }

    public InventoryUIItem[] EquipItemSlotUIs;
    public GameObject[] EmptyIconObjs;
    public ParticleImage[] EquipUIEFs;

    public TMP_Text AttackPerTxt;
    public TMP_Text SpeedPerTxt;
    public TMP_Text RangePerTxt;
    public TMP_Text CritPerTxt;
    public TMP_Text CritDmgPerTxt;

#region FUNC
    public InventoryItem FindEquipSlotItem(Enum.ItemType type) {
        int idx = HM._.ivCtrl.FindCurrentEquipItemIdx(type);
        InventoryItem equipItem = HM._.ivm.GetCurItemUIFromIdx(idx);
        return equipItem;
    }

    /// <summary>
    /// 現在装置しているEquipスロットを最新化
    /// </summary>
    public void UpdateAllEquipSlots() {
        var invDtArr = HM._.ivCtrl.InventoryData.InvArr;

        // 現在着用した装置INDEX ( 空なら -1 )
        int weaponIdx = Array.FindIndex(invDtArr, item => item.Data.Type == Enum.ItemType.Weapon && item.IsEquip);
        int shoesIdx = Array.FindIndex(invDtArr, item => item.Data.Type == Enum.ItemType.Shoes && item.IsEquip);
        int ringIdx = Array.FindIndex(invDtArr, item => item.Data.Type == Enum.ItemType.Ring && item.IsEquip);
        int relicIdx = Array.FindIndex(invDtArr, item => item.Data.Type == Enum.ItemType.Relic && item.IsEquip);

        if(weaponIdx == -1) EquipItemSlotUIs[(int)Enum.ItemType.Weapon].IsEmpty = true;
        else                EquipItem(Enum.ItemType.Weapon, invDtArr[weaponIdx], isEffect: false);

        if(shoesIdx == -1) EquipItemSlotUIs[(int)Enum.ItemType.Shoes].IsEmpty = true;
        else                EquipItem(Enum.ItemType.Shoes, invDtArr[shoesIdx], isEffect: false);

        if(ringIdx == -1) EquipItemSlotUIs[(int)Enum.ItemType.Ring].IsEmpty = true;
        else                EquipItem(Enum.ItemType.Ring, invDtArr[ringIdx], isEffect: false);

        if(relicIdx == -1) EquipItemSlotUIs[(int)Enum.ItemType.Relic].IsEmpty = true;
        else                EquipItem(Enum.ItemType.Relic, invDtArr[relicIdx], isEffect: false);
    }

    public void ResetEquipSlot(Enum.ItemType type) {
        EquipItemSlotUIs[(int)type].ResetUI();
        EmptyIconObjs[(int)type].SetActive(true);
    }

    /// <summary>
    /// EQUIPアイテム 装置
    /// </summary>
    public void EquipItem(Enum.ItemType type, InventoryItem invItem, bool isEffect = true) {
        if(invItem.IsEmpty)
            return;

        EquipItemSlotUIs[(int)type].SetUI (
            invItem.Data.Type, 
            invItem.Data.Grade, 
            invItem.Data.ItemImg, 
            invItem.Quantity, 
            invItem.Lv, 
            invItem.RelicAbilities, 
            invItem.IsEquip
        );

        //* Euqipスロットは数量 表示しない
        foreach (var equipSlot in EquipItemSlotUIs)
            equipSlot.QuantityTxt.text = "";

        EmptyIconObjs[(int)type].SetActive(false);

        if(isEffect)
            EquipItemSlotUIs[(int)type].PlayScaleUIEF(
                EquipItemSlotUIs[(int)type], 
                invItem.Data.ItemImg
            );
    }

    /// <summary>
    /// インベントリーからEquipアイテムを装置・解除の時、先に０で初期化
    /// </summary>
    private void ResetEquipAbilityData() {
        AttackPer = 0; 
        SpeedPer = 0; 
        RangePer = 0; 
        CritPer = 0; 
        CritDmgPer = 0;
        ClearEtcPer = 0;
        ClearCoinPer = 0;
        StartMoney = 0;
        StartLife = 0;
        ItemDropPer = 0;
        // SkillPoint = 0;
        BonusCoinBy10Kill = 0;
        WarriorAttackPer = 0;
        ArcherAttackPer = 0;
        MagicianAttackPer = 0;
        WarriorUpgCostPer = 0;
        ArcherUpgCostPer = 0;
        MagicianUpgCostPer = 0;
    }

    /// <summary>
    /// インベントリーへ装置したアイテムの能力データをDBを反映
    /// </summary>
    private void SetEquipAbilityData(InventoryItem invEquipItem) {
        if(invEquipItem.IsEmpty) return;

        List<AbilityData> myAbilityDataList = new List<AbilityData>();

        //*「Relic」と「その他(Weapon, Shoes, Ring)」タイプに分け、Abilityデータリスト作成 : 形が違う
        if(invEquipItem.Data.Type == Enum.ItemType.Relic) {
            AbilityData[] abilityDb = invEquipItem.Data.Abilities;

            Array.ForEach(invEquipItem.RelicAbilities, relicAbtType => {
                foreach(var db in abilityDb) {
                    if(db.Type == relicAbtType) {
                        myAbilityDataList.Add(new AbilityData(relicAbtType, db.Val, db.UpgradeVal));
                        break;
                    }
                }
            });
        }

        else {
            //* １．その他 ➝ invItem.Data.Abilitiesへ宣言
            myAbilityDataList = invEquipItem.Data.Abilities.ToList();

            //* ２．Potential能力があったら
            if(invEquipItem.RelicAbilities?.Length == 1) {
                ItemSO[] relicDts = HM._.rwlm.RwdItemDt.RelicDatas;
                AbilityData[] relicAllAbilityDatas = Enum.GetRelicAllDts(invEquipItem.Data.Grade);

                //* EquipのPotential能力：InventoryItemでRelicAbilitiesの[0]Index追加
                AbilityType rType = invEquipItem.RelicAbilities[0];
                AbilityData potentialAbility = Array.Find(relicAllAbilityDatas, rAbility => rAbility.Type == rType);
                myAbilityDataList.Add(new AbilityData(potentialAbility.Type, potentialAbility.Val, potentialAbility.UpgradeVal));
            }
        }

        //* DBデータ設定
        int lvIdx = invEquipItem.Lv - 1;
        foreach(var abt in myAbilityDataList) {
            Debug.Log($"SetEquipAbilityData():: {invEquipItem.Data.Name}, ability= {abt.Type}");
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
                    StartMoney += (int)(abt.Val + (lvIdx * abt.UpgradeVal));
                    break;
                case AbilityType.StartLife:
                    StartLife += (int)(abt.Val + (lvIdx * abt.UpgradeVal));
                    break;
                case AbilityType.ItemDropPer:
                    ItemDropPer += abt.Val + (lvIdx * abt.UpgradeVal);
                    break;
                // case AbilityType.SkillPoint:
                //     SkillPoint += (int)(abt.Val + (lvIdx * abt.UpgradeVal));
                //     break;
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
        SetEquipAbilityData(FindEquipSlotItem(Enum.ItemType.Weapon));
        SetEquipAbilityData(FindEquipSlotItem(Enum.ItemType.Shoes));
        SetEquipAbilityData(FindEquipSlotItem(Enum.ItemType.Ring));
        SetEquipAbilityData(FindEquipSlotItem(Enum.ItemType.Relic));
    }
#endregion
}
