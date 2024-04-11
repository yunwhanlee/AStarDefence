using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Inventory.Model 
{   
    #region ABILITY
    /// <summary>
    ///* 実際に保存するアイテムの能力データ (ランダムで設定するRelicタイプをため)
    /// </summary>
    [Serializable]
    public struct Ability {
        public AbilityType Type;
    }
    #endregion

    #region INVENTORY ITEM
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
        public bool IsEquip;

        /// <summary>
        ///* 必ず自分のインベントリーへ再代入しなければならない
        /// </summary>
        public InventoryItem ChangeQuantity(int newQuantity)
            => new InventoryItem {
                Quantity = newQuantity,
                Lv = this.Lv,
                Data = this.Data,
                RelicAbilities = this.RelicAbilities,
                IsEquip = this.IsEquip,
            };
        public InventoryItem ChangeLevel(int newLv)
            => new InventoryItem {
                Quantity = this.Quantity,
                Lv = newLv,
                Data = this.Data,
                RelicAbilities = this.RelicAbilities,
                IsEquip = this.IsEquip,
            };
        public InventoryItem ChangeItemData(ItemSO newItemDt)
            => new InventoryItem {
                Quantity = this.Quantity,
                Lv = this.Lv,
                Data = newItemDt,
                RelicAbilities = this.RelicAbilities,
                IsEquip = this.IsEquip,
            };
        public InventoryItem ChangeItemRelicAbilities(AbilityType[] newRelicAbilities)
            => new InventoryItem {
                Quantity = this.Quantity,
                Lv = this.Lv,
                Data = this.Data,
                RelicAbilities = newRelicAbilities,
                IsEquip = this.IsEquip,
            };
        public InventoryItem ChangeIsEquip(bool newIsEquip)
            => new InventoryItem {
                Quantity = this.Quantity,
                Lv = this.Lv,
                Data = this.Data,
                RelicAbilities = this.RelicAbilities,
                IsEquip = newIsEquip,
            };
        public static InventoryItem GetEmptyItem()
            => new InventoryItem {
                Quantity = 0,
                Lv = 0,
                Data = null,
                RelicAbilities = null,
                IsEquip = false,
            };
    }
    #endregion

    #region INVENTORY SO (DATA)
    [CreateAssetMenu]
    public class InventorySO : ScriptableObject {
        [SerializeField] public List<InventoryItem> ItemList;
        [field: SerializeField] public int Size {get; private set;} = 10;
        public event Action<Dictionary<int, InventoryItem>> OnInventoryUpdated;

        public void Init() {
            ItemList = new List<InventoryItem>();
            for(int i = 0; i < Size; i++)
                ItemList.Add(InventoryItem.GetEmptyItem());
        }

        public void ResetIsEquipData(Enum.ItemType type) {
            for(int i = 0; i < ItemList.Count; i++) {
                if(ItemList[i].IsEmpty)
                    continue;
                if(ItemList[i].Data.Type == type)
                    ItemList[i] = ItemList[i].ChangeIsEquip(false);
            }
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
        /// </summary>
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
        public void UpgradeEquipItem(ItemSO item, int quantity, int lv, AbilityType[] abilities, bool isEquip) {
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
                    HM._.ivm.UpdateDescription(HM._.ivm.CurItemIdx, item, quantity, lv, abilities, isEquip);
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

                    //* 数量を減る
                    int mergeCnt = item.Quantity / MERGE_UNIT;
                    ItemList[i] = item.ChangeQuantity(item.Quantity - mergeCnt * MERGE_UNIT);

                    //* もしマージしてから、以前のアイテム数量が０なら、削除（Empty）
                    if(ItemList[i].Quantity <= 0) 
                        ItemList[i] = InventoryItem.GetEmptyItem();

                    //* 次のレベルアイテム生成
                    var type = item.Data.Type;
                    int nextGrade = (int)item.Data.Grade + 1;
                    Debug.Log($"AutoMergeEquipItem():: {type}: {(int)item.Data.Grade} -> {nextGrade}");
                    // タイプ
                    ItemSO nextItemDt = (type == Enum.ItemType.Weapon)? HM._.rwlm.RwdItemDt.WeaponDatas[nextGrade]
                        : (type == Enum.ItemType.Shoes)? HM._.rwlm.RwdItemDt.ShoesDatas[nextGrade]
                        : (type == Enum.ItemType.Ring)? HM._.rwlm.RwdItemDt.RingDatas[nextGrade]
                        : (type == Enum.ItemType.Relic)? HM._.rwlm.RwdItemDt.RelicDatas[nextGrade - (int)Enum.Grade.Epic]
                        : null;

                    //* Relicなら、ランダムで能力
                    var relicAbilities = CheckRelicAbilitiesData(nextItemDt);

                    AddStackableItem(nextItemDt, mergeCnt, lv: 1, relicAbilities);
                }
            }
            //* イベントリーUI アップデート
            InformAboutChange();
            HM._.hui.ShowMsgNotice("자동합성 완료!");
        }

        public AbilityType[] CheckRelicAbilitiesData(ItemSO itemDt) {
            var abilities = new AbilityType[0];
            if(itemDt.Type == Enum.ItemType.Relic) {
                //* 配列のIndex数
                int len = (int)itemDt.Grade - 1;
                Debug.Log($"CheckRelicAbilitiesData():: Relic Grade= {itemDt.Grade}, len= {len}");
                abilities = new AbilityType[len];

                for(int j = 0; j < abilities.Length; j++) {
                    int rand = Random.Range(0, Util.GetEnumAbilityTypeLength());
                    var abilityTypeVals = Util.GetEnumAbilityTypeVals();
                    abilities[j] = abilityTypeVals[rand];
                    Debug.Log($"CheckRelicAbilitiesData():: RELIC:: abilities[{j}]= {abilities[j]}");
                }
            }
            return abilities;
        }

        public void DecreaseItem(int tgIdx, int decVal = -1) {
            ItemList[tgIdx] = ItemList[tgIdx].ChangeQuantity(ItemList[tgIdx].Quantity + decVal);
            // Debug.Log($"DecreaseItem():: ItemList[{tgIdx}]= {ItemList[tgIdx].Data.Name}, Quantity= {ItemList[tgIdx].Quantity}");

            //* アイテム数量が０なら、削除（Empty）
            if(ItemList[tgIdx].Quantity <= 0) {
                //* インベントリースロットのデータとUIリセット
                ItemList[tgIdx] = InventoryItem.GetEmptyItem();
                HM._.ivm.InvUIItemList[tgIdx].ResetData();
                //* アイテムがないので、開いたPopUpに可能性があることを全て非表示
                HM._.rwlm.RewardChestPopUp.SetActive(false);
                HM._.ivm.ConsumePopUp.SetActive(false);
                //* 整列
                SortInventory();
            }

            foreach (var invItemUI in HM._.ivm.InvUIItemList) {
                if(invItemUI.IsEmpty)
                    invItemUI.ResetData();
            }

            //* イベントリーUI アップデート
            InformAboutChange();
        }

        public void SortInventory() {
            Debug.Log("SortInventory()::");
            //* 整列
            ItemList.Sort((a, b) => {
                if (a.IsEmpty && b.IsEmpty)
                    return 0; 
                if (a.IsEmpty)
                    return 1; // aをb後ろへ
                if (b.IsEmpty)
                    return -1; // bをa後ろへ

                // １．タイプによって整列する
                int itemTypeComparison = a.Data.Type.CompareTo(b.Data.Type);
                if (itemTypeComparison != 0) {
                    return itemTypeComparison;
                }
                // ２．同じタイプの場合、名前で整列
                return a.Data.name.CompareTo(b.Data.name);
            });
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
    #endregion
}