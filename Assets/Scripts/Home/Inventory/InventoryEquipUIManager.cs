using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public InventoryUIItem[] EquipItemSlotUIs;
    public GameObject[] EmptyIconObjs;

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
        AttackPer = 0; SpeedPer = 0; RangePer = 0; CritPer = 0; CritDmgPer = 0;
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
                //TODO 他の全てのAbilityに関して宣言
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
