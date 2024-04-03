using System.Collections;
using System.Collections.Generic;
using Inventory.Model;
using UnityEngine;

[System.Serializable]
public class RewardItem {
    [field: SerializeField] public ItemSO InventoryItem {get; private set;}
    [field: SerializeField] public int Val {get; set;} = 1;
}
