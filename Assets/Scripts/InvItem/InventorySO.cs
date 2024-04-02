using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class InventorySO : ScriptableObject {
    [SerializeField] private List<InventoryItem> inventoryItems;
    [field: SerializeField] public int Size {get; private set;} = 10;

    public void Init() {
        inventoryItems = new List<InventoryItem>();
        for(int i = 0; i < Size; i++) {
            inventoryItems.Add(InventoryItem.GetEmptyItem());
        }
    }

    public void AddItem(ItemSO item, int val) {
        for(int i = 0; i < inventoryItems.Count; i++) {
            if(inventoryItems[i].IsEmpty) {
                inventoryItems[i] = new InventoryItem {
                    Item = item,
                    Val = val
                };
            }
        }
    }

    public Dictionary<int, InventoryItem> GetCurrentInventoryState() {
        Dictionary<int, InventoryItem> result = new Dictionary<int, InventoryItem>();
        for(int i = 0; i < inventoryItems.Count; i++) {
            if(inventoryItems[i].IsEmpty)
                continue;
            result[i] = inventoryItems[i];
        }
        return result;
    }

    public InventoryItem GetItemAt(int itemIdx) {
        return inventoryItems[itemIdx];
    }
}

[Serializable]
public struct InventoryItem {
    public int Val;
    public ItemSO Item;
    public bool IsEmpty => Item == null;

    public InventoryItem ChangeValue(int newVal) {
        return new InventoryItem {
            Item = this.Item,
            Val = newVal
        };
    }
    public static InventoryItem GetEmptyItem()
        => new InventoryItem {
            Item = null,
            Val = 0
        };
}
