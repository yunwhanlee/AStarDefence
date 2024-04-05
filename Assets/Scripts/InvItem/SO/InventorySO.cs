using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Inventory.Model 
{   
    /// <summary>
    ///* 実際に保存するアイテムの能力データ (ランダムで設定するRelicタイプをため)
    /// </summary>
    [Serializable]
    public struct Ability {
        public ItemAbilityType Type;
    }

    /// <summary>
    ///* 実際に保存するアイテム表情
    /// </summary>
    [Serializable]
    public struct InventoryItem {
        public int Quantity;
        public int Lv;
        public ItemSO Data;
        public Ability[] RelicAbilities;
        public bool IsEmpty => Data == null;

        public InventoryItem ChangeQuantity(int newQuantity)
            => new InventoryItem {
                Data = this.Data,
                Quantity = newQuantity,
                Lv = this.Lv,
                RelicAbilities = this.RelicAbilities
            };
        public InventoryItem ChangeLevel(int newLv)
            => new InventoryItem {
                Data = this.Data,
                Quantity = this.Quantity,
                Lv = newLv,
                RelicAbilities = this.RelicAbilities
            };
        public static InventoryItem GetEmptyItem()
            => new InventoryItem {
                Data = null,
                Quantity = 0,
                Lv = 1,
                RelicAbilities = null
            };
    }

    [CreateAssetMenu]
    public class InventorySO : ScriptableObject {
        [SerializeField] private ItemSO[] weaponItemDatas;

        [SerializeField] public List<InventoryItem> ItemList;
        [field: SerializeField] public int Size {get; private set;} = 10;
        public event Action<Dictionary<int, InventoryItem>> OnInventoryUpdated;

        public void Init() {
            ItemList = new List<InventoryItem>();
            for(int i = 0; i < Size; i++)
                ItemList.Add(InventoryItem.GetEmptyItem());
        }

        public int AddItem(ItemSO item, int quantity, int lv, Ability[] abilities) {
            if(item.IsStackable == false) {
                Debug.Log($"InventorySO:: AddItem({item.name}, quantity= {quantity}, lv= {lv})::");
                // 過去：Valが２なら、重ならないので１個しかできない複数に分ける
                /*
                for(int i = 0; i < inventoryItems.Count; i++) {
                    while(val > 0 && IsInventoryFull() == false) {
                        val -= AddItemToFirstFreeSlot(item, 1);
                    }
                    InformAboutChange();
                    return val;
                }
                */
                //* 変更：EquipアイテムはValをレベルとして扱う
                quantity = AddItemToFirstFreeSlot(item, quantity, lv, abilities);
                InformAboutChange();
                return quantity;
            }
            quantity = AddStackableItem(item, quantity, lv);
            InformAboutChange();
            return quantity;
        }

        /// <summary>
        ///* 数えないアイテムとして追加 （今は使うことがない）
        /// </summary>        
        private int AddItemToFirstFreeSlot(ItemSO item, int quantity, int lv, Ability[] abilities = null ) {
            InventoryItem newItem = new InventoryItem {
                Data = item,
                Quantity = quantity,
                Lv = lv,
                RelicAbilities = abilities
            };

            for(int i = 0; i < ItemList.Count; i++) {
                if(ItemList[i].IsEmpty) {
                    ItemList[i] = newItem;
                    return quantity;
                }
            }
            return 0;
        }

        /// <summary>
        /// 一つでも空スロットがあったら、インベントリーがFullではない => False
        /// /// </summary>
        private bool IsInventoryFull()
            => ItemList.Where(item => item.IsEmpty).Any() == false;

        /// <summary>
        /// 数えるアイテムとして追加 (自動マージしたときも使う)
        /// </summary>
        private int AddStackableItem(ItemSO item, int quantity, int lv) {
            for(int i = 0; i < ItemList.Count; i++) {
                if(ItemList[i].IsEmpty)
                    continue;
                if(ItemList[i].Data.ID == item.ID) {
                    int amountPossibleToTake = ItemList[i].Data.MaxStackSize - ItemList[i].Quantity;

                    if(quantity > amountPossibleToTake) {
                        ItemList[i] = ItemList[i].ChangeQuantity(ItemList[i].Data.MaxStackSize);
                        quantity -= amountPossibleToTake;
                    }
                    else {
                        ItemList[i] = ItemList[i].ChangeQuantity(ItemList[i].Quantity + quantity);
                        InformAboutChange();
                        return 0;
                    }
                }
            }

            while(quantity > 0 && IsInventoryFull() == false) {
                int newQuantity = Mathf.Clamp(quantity, 0, item.MaxStackSize);
                quantity -= newQuantity;
                AddItemToFirstFreeSlot(item, newQuantity, lv);
            }

            return quantity;
        }

        /// <summary>
        /// アイテムのアップグレードや消費する
        /// </summary>
        public void UpgradeEquipItem(ItemSO item, int quantity, int lv, Ability[] abilities) {
            for(int i = 0; i < ItemList.Count; i++) {
                //* 同じIDを探して
                if(ItemList[i].Data.ID == item.ID) {
                    //* アップグレードのMAX制限
                    int max = (item.Type == Enum.ItemType.Relic)? Config.RELIC_UPGRADE_MAX : Config.EQUIP_UPGRADE_MAX;
                    lv = Mathf.Min(lv, max);
                    Debug.Log($"UpgradeEquipItem({ItemList[i].Data.ID} == {item.ID}):: lv= {lv}");
                    //* 増えたVal値を最新化
                    ItemList[i] = ItemList[i].ChangeLevel(lv);
                    //* イベントリーUI アップデート
                    InformAboutChange();
                    //* 情報表示ポップアップUI アップデート
                    HM._.ivm.UpdateDescription(HM._.ivm.CurItemIdx, item, quantity, lv, abilities);
                    return;
                }
            }
        }

        /// <summary>
        /// インベントリーの装置アイテムを次のレベルに自動マージ
        /// </summary>
        public void AutoMergeEquipItem() {
            const int LV_OFFSET = 1;
            for(int i = 0; i < ItemList.Count; i++) {
                var item = ItemList[i];
                if(item.Quantity >= 10) {
                    //* 数値を減って適用
                    ItemList[i] = item.ChangeQuantity(item.Quantity - 10);
                    //* 次のレベルアイテム生成
                    int nextLvIdx = item.Lv + 1 - LV_OFFSET;
                    AddStackableItem(weaponItemDatas[nextLvIdx], 1, 1);
                }
            }
            //* イベントリーUI アップデート
            InformAboutChange();
        }

        public void AddItem(InventoryItem item) {
            AddItem(item.Data, item.Quantity, item.Lv, item.RelicAbilities);
        }

        public Dictionary<int, InventoryItem> GetCurrentInventoryState() {
            Dictionary<int, InventoryItem> invItemDic = new Dictionary<int, InventoryItem>();
            for(int i = 0; i < ItemList.Count; i++) {
                if(ItemList[i].IsEmpty)
                    continue;
                invItemDic[i] = ItemList[i];
            }
            return invItemDic;
        }

        /// <summary>
        /// 実際のインベントリーへあるアイテム情報を返す
        /// </summary>
        /// <param name="itemIdx"></param>
        public InventoryItem GetItemAt(int itemIdx)
            => ItemList[itemIdx];

        public void SwapItems(int itemIdx1, int itemIdx2) {
            InventoryItem item1 = ItemList[itemIdx1];
            ItemList[itemIdx1] = ItemList[itemIdx2];
            ItemList[itemIdx2] = item1;
            InformAboutChange();
        }

        public void InformAboutChange()
            => OnInventoryUpdated?.Invoke(GetCurrentInventoryState());
    }


}