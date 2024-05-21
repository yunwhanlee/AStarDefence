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
        public bool IsNewAlert;

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
                IsNewAlert = this.IsNewAlert,
            };
        public InventoryItem ChangeLevel(int newLv)
            => new InventoryItem {
                Quantity = this.Quantity,
                Lv = newLv,
                Data = this.Data,
                RelicAbilities = this.RelicAbilities,
                IsEquip = this.IsEquip,
                IsNewAlert = this.IsNewAlert,
            };
        public InventoryItem ChangeItemData(ItemSO newItemDt)
            => new InventoryItem {
                Quantity = this.Quantity,
                Lv = this.Lv,
                Data = newItemDt,
                RelicAbilities = this.RelicAbilities,
                IsEquip = this.IsEquip,
                IsNewAlert = this.IsNewAlert,
            };
        public InventoryItem ChangeItemRelicAbilities(AbilityType[] newRelicAbilities)
            => new InventoryItem {
                Quantity = this.Quantity,
                Lv = this.Lv,
                Data = this.Data,
                RelicAbilities = newRelicAbilities,
                IsEquip = this.IsEquip,
                IsNewAlert = this.IsNewAlert,
            };
        public InventoryItem ChangeIsEquip(bool newIsEquip)
            => new InventoryItem {
                Quantity = this.Quantity,
                Lv = this.Lv,
                Data = this.Data,
                RelicAbilities = this.RelicAbilities,
                IsEquip = newIsEquip,
                IsNewAlert = this.IsNewAlert,
            };
        public InventoryItem ChangeIsNewAlert(bool isNewAlert)
            => new InventoryItem {
                Quantity = this.Quantity,
                Lv = this.Lv,
                Data = this.Data,
                RelicAbilities = this.RelicAbilities,
                IsEquip = this.IsEquip,
                IsNewAlert = isNewAlert,
            };
        public static InventoryItem GetEmptyItem()
            => new InventoryItem {
                Quantity = 0,
                Lv = 0,
                Data = null,
                RelicAbilities = null,
                IsEquip = false,
                IsNewAlert = false,
            };
    }
    #endregion

    #region INVENTORY SO (DATA)
    [CreateAssetMenu]
    public class InventorySO : ScriptableObject {
        [SerializeField] public List<InventoryItem> ItemList;
        [field: SerializeField] public static int Size {get; private set;} = 50;
        public event Action<Dictionary<int, InventoryItem>> OnInventoryUpdated;

        public void Init() {
            Debug.Log("InventorySO:: Init():: (インベントリデータ) 初期化");
            ItemList = new List<InventoryItem>();
            for(int i = 0; i < Size; i++)
                ItemList.Add(InventoryItem.GetEmptyItem());
        }

        public void InitIsEquipData(Enum.ItemType type) {
            for(int i = 0; i < ItemList.Count; i++) {
                if(ItemList[i].IsEmpty)
                    continue;
                if(ItemList[i].Data.Type == type)
                    ItemList[i] = ItemList[i].ChangeIsEquip(false);
            }
        }

        public int AddItem(ItemSO item, int quantity, int lv, AbilityType[] relicAbilities, bool isEquip = false, bool isNewAlert = false) {
            quantity = AddStackableItem(item, quantity, lv, relicAbilities, isEquip, isNewAlert);
            InformAboutChange();
            return quantity;
        }

        /// <summary>
        ///* 数えないアイテムとして追加 （今は使うことがない）
        /// </summary>
        private int AddItemToFirstFreeSlot(ItemSO itemDt, int quantity, int lv, AbilityType[] relicAbilities, bool isEquip, bool isNewAlert = false) {
            InventoryItem newItem = new InventoryItem {
                Data = itemDt,
                Quantity = quantity,
                Lv = lv,
                RelicAbilities = relicAbilities,
                IsEquip = isEquip,
                IsNewAlert = isNewAlert
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
        private int AddStackableItem(ItemSO item, int quantity, int lv, AbilityType[] relicAbilities, bool isEquip, bool isNewAlert) {
            for(int i = 0; i < ItemList.Count; i++) {
                if(ItemList[i].IsEmpty)
                    continue;

                //* 同じアイテムが有ったら
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

            //* アイテム生成 (最初の初期化にも使う)
            while(quantity > 0 && IsInventoryFull() == false) {
                int newQuantity = Mathf.Clamp(quantity, 0, item.MaxStackSize);
                quantity -= newQuantity;
                AddItemToFirstFreeSlot(item, newQuantity, lv, relicAbilities, isEquip, isNewAlert);
            }

            return quantity;
        }

        /// <summary>
        /// インベントリーの装置アイテムをアップグレード
        /// </summary>
        public void UpgradeEquipItem(ItemSO curItem, int quantity, int lv, AbilityType[] abilities, bool isEquip) {
            for(int i = 0; i < ItemList.Count; i++) {
                try {
                    //* 同じIDを探して
                    if(ItemList[i].Data.ID == curItem.ID) {
                        //* アップグレードのMAX制限
                        int max = (curItem.Type == Enum.ItemType.Relic)? Config.RELIC_UPGRADE_MAX : Config.EQUIP_UPGRADE_MAX;
                        lv = Mathf.Min(lv, max);
                        Debug.Log($"UpgradeEquipItem({ItemList[i].Data.ID} == {curItem.ID}):: lv= {lv}");
                        //* 増えたVal値を最新化
                        ItemList[i] = ItemList[i].ChangeLevel(lv);
                        // //* イベントリーUI アップデート
                        // InformAboutChange();
                        // //* 情報表示ポップアップUI アップデート
                        // HM._.ivm.UpdateDescription(HM._.ivm.CurItemIdx, item, quantity, lv, abilities, isEquip);
                        return;
                    }
                }
                catch(Exception msg) {
                    Debug.LogWarning("(BUG) " + msg);
                    Debug.Log($"ItemList[{i}]= {ItemList[i]}");
                }
            }
        }

        /// <summary>
        /// インベントリーの装置アイテムを次のレベルに自動マージ
        /// </summary>
        public void AutoMergeEquipItem() {
            const int MERGE_UNIT = 10;

            bool isMergable = false;
            bool isMustUnEquip = false;
            string typeName = "";

            //* マージできるか状況 確認
            foreach(var item in ItemList) {
                if(item.IsEmpty) continue;
                if(item.Data.Type == Enum.ItemType.Etc) continue;

                if(item.Quantity >= 10) {
                    isMergable = true;
                    if(item.IsEquip) {
                        isMustUnEquip = true;
                        typeName = Enum.GetItemTypeName(item.Data.Type);
                    }
                }
            }

            //* 除外
            if(!isMergable) {
                HM._.hui.ShowMsgError("합성할 아이템이 없습니다.");
                return;
            }
            else if(isMustUnEquip) {
                
                HM._.hui.ShowMsgError($"{typeName}의 장착을 해제해주세요! (장착한 아이템 중 합성대상이 있어 불가)");
                return;
            }

            SM._.SfxPlay(SM.SFX.Merge3SFX);

            //* マージ
            for(int i = 0; i < ItemList.Count; i++) {
                InventoryItem item = ItemList[i];
                if(item.Quantity >= MERGE_UNIT) {
                    if(item.Data.Grade == Enum.Grade.Prime) {
                        Debug.Log("最後の等級なので、処理しない");
                        continue;
                    }

                    //* 数量を減る
                    int mergeCnt = item.Quantity / MERGE_UNIT;
                    int removeQuantity = mergeCnt * MERGE_UNIT;
                    ItemList[i] = item.ChangeQuantity(item.Quantity - removeQuantity);

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

                    if(nextItemDt == null)
                        continue;

                    //* Relicなら、ランダムで能力
                    var relicAbilities = CheckRelicAbilitiesData(nextItemDt);

                    //* アイテムマージ
                    AddStackableItem(nextItemDt, mergeCnt, lv: 1, relicAbilities, item.IsEquip, isNewAlert: true);
                    
                }
            }
            //* 整列
            SortInventory();
            //* イベントリーUI アップデート
            InformAboutChange();
            //* 現在カテゴリ表示を再ロード ➝ ずれたスロットリストを正しく合わせる
            HM._.ivm.OnClickCateMenuIconBtn(HM._.ivm.CurCateIdx);

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
                    int start = (int)AbilityType.Critical; //* RelicでAttack、Speed、Rangeは対応しない！
                    int end = Util.GetSize_AbilityType();
                    int randIdx = Random.Range(start, end);
                    AbilityType[] abilityTypeArr = Util.GetEnumArray_AbilityType();
                    abilities[j] = abilityTypeArr[randIdx];
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
                HM._.ivm.InvUIItemList[tgIdx].ResetUI();
                //* アイテムがないので、開いたPopUpに可能性があることを全て非表示
                HM._.rwlm.RewardChestPopUp.SetActive(false);
                HM._.ivm.ConsumePopUp.SetActive(false);
                //* 整列
                SortInventory();
            }

            foreach (var invItemUI in HM._.ivm.InvUIItemList) {
                if(invItemUI.IsEmpty)
                    invItemUI.ResetUI();
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
            Debug.Log($"AddItem():: {item.Data.Name}, Lv= {item.Lv}, RelicAbilities.Length= {item.RelicAbilities.Length}, isEquip= {item.IsEquip}");
            AddItem(item.Data, item.Quantity, item.Lv, item.RelicAbilities, item.IsEquip, item.IsNewAlert);
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