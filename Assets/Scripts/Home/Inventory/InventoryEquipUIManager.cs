using System.Collections;
using System.Collections.Generic;
using Inventory.Model;
using Inventory.UI;
using UnityEngine;
using UnityEngine.UI;

public class InventoryEquipUIManager : MonoBehaviour {
    public const string WEAPON_SLOT_OBJ_NAME = "WeaponInvItemUISlot";
    public const string SHOES_SLOT_OBJ_NAME = "ShoesInvItemUISlot";
    public const string RING_SLOT_OBJ_NAME = "RingInvItemUISlot";
    public const string RELIC_SLOT_OBJ_NAME = "RelicInvItemUISlot";

    public InventoryUIItem[] EquipItemSlotUIs;
    public GameObject[] EmptyIconObjs;

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
#endregion
}
