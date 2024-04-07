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
        public AbilityType Type;
    }

    /// <summary>
    ///* 実際に保存するアイテム表情
    /// </summary>
    [Serializable]
    public struct InventoryItem {
        public int Quantity;
        public int Lv;
        public ItemSO Data;
        public AbilityType[] RelicAbilities;
        public bool IsEmpty => Data == null;

        public InventoryItem ChangeQuantity(int newQuantity)
            => new InventoryItem {
                Quantity = newQuantity,
                Lv = this.Lv,
                Data = this.Data,
                RelicAbilities = this.RelicAbilities
            };
        public InventoryItem ChangeLevel(int newLv)
            => new InventoryItem {
                Quantity = this.Quantity,
                Lv = newLv,
                Data = this.Data,
                RelicAbilities = this.RelicAbilities
            };
        public InventoryItem ChangeItemData(ItemSO newItemDt)
            => new InventoryItem {
                Quantity = this.Quantity,
                Lv = this.Lv,
                Data = newItemDt,
                RelicAbilities = this.RelicAbilities
            };
        public InventoryItem ChangeItemRelicAbilities(AbilityType[] relicAbilities)
            => new InventoryItem {
                Quantity = this.Quantity,
                Lv = this.Lv,
                Data = this.Data,
                RelicAbilities = relicAbilities
            };
        public static InventoryItem GetEmptyItem()
            => new InventoryItem {
                Quantity = 1,
                Lv = 1,
                Data = null,
                RelicAbilities = null
            };
    }

    [CreateAssetMenu]
    public class InventorySO : ScriptableObject {
        [SerializeField] private ItemSO[] weaponItemDatas;
        [SerializeField] private ItemSO[] shoesItemDatas;
        [SerializeField] private ItemSO[] ringItemDatas;

        [SerializeField] public List<InventoryItem> ItemList;
        [field: SerializeField] public int Size {get; private set;} = 10;
        public event Action<Dictionary<int, InventoryItem>> OnInventoryUpdated;

        public void Init() {
            ItemList = new List<InventoryItem>();
            for(int i = 0; i < Size; i++)
                ItemList.Add(InventoryItem.GetEmptyItem());
        }

        public int AddItem(ItemSO item, int quantity, int lv, AbilityType[] relicAbilities) {
            // if(item.IsStackable == false) {
            //     Debug.Log($"InventorySO:: AddItem({item.name}, quantity= {quantity}, lv= {lv}, relicAbilities= {relicAbilities.Length})::");
            //     // 過去：Valが２なら、重ならないので１個しかできない複数に分ける
            //     /*
            //     for(int i = 0; i < inventoryItems.Count; i++) {
            //         while(val > 0 && IsInventoryFull() == false) {
            //             val -= AddItemToFirstFreeSlot(item, 1);
            //         }
            //         InformAboutChange();
            //         return val;
            //     }
            //     */
            //     //* 変更：EquipアイテムはValをレベルとして扱う
            //     quantity = AddItemToFirstFreeSlot(item, quantity, lv, null);
            //     InformAboutChange();
            //     return quantity;
            // }
            quantity = AddStackableItem(item, quantity, lv, relicAbilities);
            InformAboutChange();
            return quantity;
        }

        /// <summary>
        ///* 数えないアイテムとして追加 （今は使うことがない）
        /// </summary>
        private int AddItemToFirstFreeSlot(ItemSO itemDt, int quantity, int lv, AbilityType[] relicAbilities) {
            InventoryItem newItem = new InventoryItem {
                Data = itemDt,
                Quantity = quantity,
                Lv = lv,
                RelicAbilities = relicAbilities
            };

            Debug.Log($"newItem.RelicAbilities.Length= {newItem}");

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
        private int AddStackableItem(ItemSO item, int quantity, int lv, AbilityType[] relicAbilities) {
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
                AddItemToFirstFreeSlot(item, newQuantity, lv, relicAbilities);
            }

            return quantity;
        }

        /// <summary>
        /// インベントリーの装置アイテムをアップグレード
        /// </summary>
        public void UpgradeEquipItem(ItemSO item, int quantity, int lv, AbilityType[] abilities) {
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
            const int MERGE_UNIT = 10;
            //* マージすることがなかったら
            bool isExistMergable = ItemList.Exists (item => item.Data && item.Data.Type != Enum.ItemType.Etc &&  item.Quantity >= 10);
            if(!isExistMergable) {
                HM._.hui.ShowMsgError("합성할 아이템이 없습니다.");
                return;
            }

            //* マージ
            for(int i = 0; i < ItemList.Count; i++) {
                InventoryItem item = ItemList[i];
                if(item.Quantity >= MERGE_UNIT) {
                    if(item.Data.Grade == Enum.Grade.Prime) {
                        //TODO もし、最後までできて、またマージするとしたらどうする？
                        Debug.Log("最後の等級までしたので、もう次がない");
                        continue;
                    }

                    //* 数量を切り替えて適用
                    int mergeCnt = item.Quantity / MERGE_UNIT;
                    ItemList[i] = item.ChangeQuantity(item.Quantity - mergeCnt * MERGE_UNIT);

                    //* もしマージしてから、以前のアイテム数量が０なら、削除（Empty）
                    if(ItemList[i].Quantity <= 0) 
                        ItemList[i] = InventoryItem.GetEmptyItem();

                    //* 次のレベルアイテム生成
                    var type = item.Data.Type;
                    int nextGrade = (int)item.Data.Grade + 1;
                    // タイプ
                    ItemSO nextItemDt = (type == Enum.ItemType.Weapon)? weaponItemDatas[nextGrade]
                        : (type == Enum.ItemType.Shoes)? shoesItemDatas[nextGrade]
                        : (type == Enum.ItemType.Ring)? ringItemDatas[nextGrade]
                        : null; // TODO RELIC
                    AddStackableItem(nextItemDt, mergeCnt, lv: 1, relicAbilities: null);
                }
            }
            //* イベントリーUI アップデート
            InformAboutChange();
            HM._.hui.ShowMsgNotice("자동합성 완료!");
        }

        public void AddItem(InventoryItem item) {
            Debug.Log($"AddItem():: {item.Data.Name}, Lv= {item.Lv}, RelicAbilities.Length= {item.RelicAbilities.Length}");
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

        public void InformAboutChange() {
            Debug.Log("InformAboutChange()::");
            OnInventoryUpdated?.Invoke(GetCurrentInventoryState());
        }
    }
}