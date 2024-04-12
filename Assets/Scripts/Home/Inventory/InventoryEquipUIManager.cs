using System.Collections;
using System.Collections.Generic;
using Inventory.Model;
using Inventory.UI;
using UnityEngine;
using UnityEngine.UI;

public class InventoryEquipUIManager : MonoBehaviour {
    public InventoryUIItem WeaponInvUISlot;
    public GameObject WeaponEmptyIconObj;
    public InventoryUIItem ShoesInvUISlot;
    public GameObject ShoesEmptyIconObj;
    public InventoryUIItem RingInvUISlot;
    public GameObject RingEmptyIconObj;
    public InventoryUIItem RelicInvUISlot;
    public GameObject RelicEmptyIconObj;


    public void SetEquipEmptyIcon(Enum.ItemType type, bool isActive) {
        switch (type) {
            case Enum.ItemType.Weapon: WeaponEmptyIconObj.SetActive(isActive); break;
            case Enum.ItemType.Shoes: ShoesEmptyIconObj.SetActive(isActive); break;
            case Enum.ItemType.Ring: RingEmptyIconObj.SetActive(isActive); break;
            case Enum.ItemType.Relic: RelicEmptyIconObj.SetActive(isActive); break;
        }
    }
}
