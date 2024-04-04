using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Inventory.Model 
{
    [Serializable]
    public struct Ability {
        public ItemAbilityType AbilityType;
        public float Val;
    }

    [Serializable]
    public struct InventoryItem {
        public int Val;
        public Ability[] Abilities;
        public ItemSO Data;
        public bool IsEmpty => Data == null;

        public InventoryItem ChangeValue(int newVal) {
            return new InventoryItem {
                Data = this.Data,
                Val = newVal
            };
        }
        public static InventoryItem GetEmptyItem()
            => new InventoryItem {
                Data = null,
                Val = 0
            };
    }

    [CreateAssetMenu]
    public class InventorySO : ScriptableObject {
        [SerializeField] private List<InventoryItem> inventoryItems;
        [field: SerializeField] public int Size {get; private set;} = 10;
        public event Action<Dictionary<int, InventoryItem>> OnInventoryUpdated;

        public void Init() {
            inventoryItems = new List<InventoryItem>();
            for(int i = 0; i < Size; i++)
                inventoryItems.Add(InventoryItem.GetEmptyItem());
        }

        public int AddItem(ItemSO item, int val) {
            if(item.IsStackable == false) {
                Debug.Log($"InventorySO:: AddItem({item.name}, val= {val})::");
                // 過去：Valが２なら、重ならないので１個しかできない複数に分ける
                // for(int i = 0; i < inventoryItems.Count; i++) {
                //     while(val > 0 && IsInventoryFull() == false) {
                //         val -= AddItemToFirstFreeSlot(item, 1);
                //     }
                //     InformAboutChange();
                //     return val;
                // }
                //! 変更：EquipアイテムはValをレベルとして扱う
                val = AddItemToFirstFreeSlot(item, val);
                InformAboutChange();
                return val;
            }
            val = AddStackableItem(item, val);
            InformAboutChange();
            return val;
        }

        private int AddItemToFirstFreeSlot(ItemSO item, int val) {
            InventoryItem newItem = new InventoryItem{
                Data = item,
                Val = val
            };

            for(int i = 0; i < inventoryItems.Count; i++) {
                if(inventoryItems[i].IsEmpty) {
                    inventoryItems[i] = newItem;
                    return val;
                }
            }
            return 0;
        }

        /// <summary>
        /// 一つでも空スロットがあったら、インベントリーがFullではない => False
        /// /// </summary>
        private bool IsInventoryFull()
            => inventoryItems.Where(item => item.IsEmpty).Any() == false;

        private int AddStackableItem(ItemSO item, int val) {
            for(int i = 0; i < inventoryItems.Count; i++) {
                if(inventoryItems[i].IsEmpty)
                    continue;
                if(inventoryItems[i].Data.ID == item.ID) {
                    int amountPossibleToTake = inventoryItems[i].Data.MaxStackSize - inventoryItems[i].Val;

                    if(val > amountPossibleToTake) {
                        inventoryItems[i] = inventoryItems[i].ChangeValue(inventoryItems[i].Data.MaxStackSize);
                        val -= amountPossibleToTake;
                    }
                    else {
                        inventoryItems[i] = inventoryItems[i].ChangeValue(inventoryItems[i].Val + val);
                        InformAboutChange();
                        return 0;
                    }
                }
            }

            while(val > 0 && IsInventoryFull() == false) {
                int newVal = Mathf.Clamp(val, 0, item.MaxStackSize);
                val -= newVal;
                AddItemToFirstFreeSlot(item, newVal);
            }

            return val;
        }

        /// <summary>
        /// アイテムのアップグレードや消費する
        /// </summary>
        public void UpgradeEquipItem(ItemSO item, int val, Ability[] abilities) {
            for(int i = 0; i < inventoryItems.Count; i++) {
                //* 同じIDを探して
                if(inventoryItems[i].Data.ID == item.ID) {
                    //* アップグレードのMAX制限
                    int max = (item.Type == Enum.ItemType.Relic)? Config.RELIC_UPGRADE_MAX : Config.EQUIP_UPGRADE_MAX;
                    val = Mathf.Min(val, max);
                    Debug.Log($"UpgradeEquipItem({inventoryItems[i].Data.ID} == {item.ID}):: val= {val}");

                    //* 増えたVal値を最新化
                    inventoryItems[i] = inventoryItems[i].ChangeValue(val);
                    //* イベントリーUI アップデート
                    InformAboutChange();
                    //* 情報表示ポップアップUI アップデート
                    HM._.ivm.UpdateDescription(HM._.ivm.CurItemIdx, item, val, abilities);
                    return;
                }
            }
        }

        public void AddItem(InventoryItem item) {
            AddItem(item.Data, item.Val);
        }

        public Dictionary<int, InventoryItem> GetCurrentInventoryState() {
            Dictionary<int, InventoryItem> invItemDic = new Dictionary<int, InventoryItem>();
            for(int i = 0; i < inventoryItems.Count; i++) {
                if(inventoryItems[i].IsEmpty)
                    continue;
                invItemDic[i] = inventoryItems[i];
            }
            return invItemDic;
        }

        /// <summary>
        /// 実際のインベントリーへあるアイテム情報を返す
        /// </summary>
        /// <param name="itemIdx"></param>
        public InventoryItem GetItemAt(int itemIdx)
            => inventoryItems[itemIdx];

        public void SwapItems(int itemIdx1, int itemIdx2) {
            InventoryItem item1 = inventoryItems[itemIdx1];
            inventoryItems[itemIdx1] = inventoryItems[itemIdx2];
            inventoryItems[itemIdx2] = item1;
            InformAboutChange();
        }

        public void InformAboutChange()
            => OnInventoryUpdated?.Invoke(GetCurrentInventoryState());
    }


}