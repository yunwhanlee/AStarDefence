using System.Collections;
using System.Collections.Generic;
using Inventory.Model;
using Inventory.UI;
using UnityEngine;
using UnityEngine.UI;

public class InventoryEquipUIManager : MonoBehaviour {
    public InventoryUIItem[] EquipItemSlotUIs;
    public GameObject[] EmptyIconObjs;

#region EVENT
    /// <summary>
    /// 装置スロットクリック イベント
    /// </summary>
    /// <param name="itemType">0: Weapon, 1: Shoes, 2: Ring, 3: Relic</param>
    public void OnClickEquipSlot(int itemType) {
        switch (itemType) {
            case (int)Enum.ItemType.Weapon:
                break;
            case (int)Enum.ItemType.Shoes:
                break;
            case (int)Enum.ItemType.Ring:
                break;
            case (int)Enum.ItemType.Relic:
                break;
        }
    }
#endregion

#region FUNC
    private void SetEquipEmptyIcon(Enum.ItemType type, bool isActive)
        => EmptyIconObjs[(int)type].SetActive(isActive);

    public void InitEquipSlot(Enum.ItemType type) {
        EquipItemSlotUIs[(int)type].ResetData();
        EmptyIconObjs[(int)type].SetActive(true);
        SetEquipEmptyIcon(type, true);
    }

    public void EquipItem(Enum.ItemType type, InventoryItem curInvItem) {
        ItemSO dt = curInvItem.Data;
        EquipItemSlotUIs[(int)type].SetUIData (
            dt.Type, 
            dt.Grade, 
            dt.ItemImg, 
            curInvItem.Quantity, 
            curInvItem.Lv, 
            curInvItem.RelicAbilities, 
            curInvItem.IsEquip
        );
        EquipItemSlotUIs[(int)type].PlayScaleUIEF(EquipItemSlotUIs[(int)type], dt.ItemImg);
        SetEquipEmptyIcon(type, false);
    }
#endregion
}
