using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
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
        public readonly bool IsEmpty => Quantity == 0; //* ItemSOがNull(登録されていない)なら、true
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
        public InventoryItem GetEmptyItem()
            => new InventoryItem {
                Quantity = 0,
                Lv = 0,
                Data = this.Data, //* Dataは固定だから、さわらない！
                RelicAbilities = null,
                IsEquip = false,
                IsNewAlert = false,
            };
    }
    #endregion

    #region INVENTORY SO (DATA)
    [CreateAssetMenu]
    public class InventorySO : ScriptableObject {
        [field:Header("インベントリリスト")]
        [field: SerializeField] public InventoryItem[] InvArr;

        // [field: SerializeField] public static int Size {get; private set;} = 55;
        public event Action<Dictionary<int, InventoryItem>> OnInventoryUIUpdated;

        public void SetLoadData() {
            Debug.Log("InventorySO:: LoadData()::");
            InvArr = DM._.DB.InvItemDBList.ToArray();
        }

        public void InitIsEquipData(Enum.ItemType type) {
            for(int i = 0; i < InvArr.Length; i++) {
                if(InvArr[i].IsEmpty)
                    continue;
                if(InvArr[i].Data.Type == type)
                    InvArr[i] = InvArr[i].ChangeIsEquip(false);
            }
        }

        public int AddItem(ItemSO item, int quantity, int lv, AbilityType[] relicAbilities, bool isEquip = false, bool isNewAlert = false) {
            Debug.Log($"AddItem:: item.Name= {item.Name}, quantity= {quantity}, relicAbilities is {(relicAbilities == null? "NULL" : relicAbilities)}");
            // if(item.MaxStackSize > 1)
            quantity = AddStackableItem(item, quantity, lv, relicAbilities, isEquip, isNewAlert);
            // else {
            //     //* アイテム生成 (最初の初期化にも使う)
            //     while(quantity > 0 && IsInventoryFull() == false) {
            //         Debug.Log($"AddStackableItem():: アイテム生成= {item.name}");
            //         int newQuantity = Mathf.Clamp(quantity, 0, item.MaxStackSize);
            //         quantity -= newQuantity;
            //         AddItemToFirstFreeSlot(item, newQuantity, lv, relicAbilities, isEquip, isNewAlert);
            //     }
            // }

            // InformAboutChange();
            return quantity;
        }

        /// <summary>
        ///* 数えないアイテムとして追加 （今は使うことがない）
        /// </summary>
        // private int AddItemToFirstFreeSlot(ItemSO itemDt, int quantity, int lv, AbilityType[] relicAbilities, bool isEquip, bool isNewAlert = false) {
        //     InventoryItem newItem = new InventoryItem {
        //         Data = itemDt,
        //         Quantity = quantity,
        //         Lv = lv,
        //         RelicAbilities = relicAbilities,
        //         IsEquip = isEquip,
        //         IsNewAlert = isNewAlert
        //     };

        //     Debug.Log($"newItem.RelicAbilities.Length= {newItem}");
        //     for(int i = 0; i < invArr.Count; i++) {
        //         if(invArr[i].IsEmpty) {
        //             invArr[i] = newItem;
        //             return quantity;
        //         }
        //     }
        //     return 0;
        // }

        /// <summary>
        /// 一つでも空スロットがあったら、インベントリーがFullではない => False
        /// </summary>
        // private bool IsInventoryFull()
        //     => invArr.Where(item => item.IsEmpty).Any() == false;

        /// <summary>
        /// 数えるアイテムとして追加 (自動マージしたときも使う)
        /// </summary>
        private int AddStackableItem(ItemSO item, int quantity, int lv, AbilityType[] relicAbilities, bool isEquip, bool isNewAlert) {
            Debug.Log($"AddStackableItem():: item.ID= {item.ID}, item.Name= {item.Name}, quantity= {quantity}");
            InvArr[item.ID] = InvArr[item.ID].ChangeQuantity(InvArr[item.ID].Quantity + quantity);
            InvArr[item.ID] = InvArr[item.ID].ChangeLevel(lv);

            if(item.Type == Enum.ItemType.Relic)
                InvArr[item.ID] = InvArr[item.ID].ChangeItemRelicAbilities(relicAbilities);

            // InformAboutChange();
            return quantity;
        }

        /// <summary>
        /// インベントリーの装置アイテムをアップグレード
        /// </summary>
        public void UpgradeEquipItem(ItemSO curItem, int quantity, int lv, AbilityType[] abilities, bool isEquip) {
            for(int i = 0; i < InvArr.Length; i++) {
                try {
                    //* 同じIDを探して
                    if(InvArr[i].Data.ID == curItem.ID) {
                        //* アップグレードのMAX制限
                        int max = (curItem.Type == Enum.ItemType.Relic)? Config.RELIC_UPGRADE_MAX : Config.EQUIP_UPGRADE_MAX;
                        lv = Mathf.Min(lv, max);
                        Debug.Log($"UpgradeEquipItem({InvArr[i].Data.ID} == {curItem.ID}):: lv= {lv}");
                        //* 増えたVal値を最新化
                        InvArr[i] = InvArr[i].ChangeLevel(lv);
                        // //* イベントリーUI アップデート
                        // InformAboutChange();
                        // //* 情報表示ポップアップUI アップデート
                        // HM._.ivm.UpdateDescription(HM._.ivm.CurItemIdx, item, quantity, lv, abilities, isEquip);
                        return;
                    }
                }
                catch(Exception msg) {
                    Debug.LogWarning("(BUG) " + msg);
                    Debug.Log($"ItemList[{i}]= {InvArr[i]}");
                }
            }
        }

        /// <summary>
        /// 自動マージ
        /// </summary>
        public void AutoMergeEquipItem() {
            // フィルター(EQUIPアイテム別)
            InventoryItem[] weaponItems = Array.FindAll(InvArr, item => item.Data.Type == Enum.ItemType.Weapon);
            InventoryItem[] shoesItems = Array.FindAll(InvArr, item => item.Data.Type == Enum.ItemType.Shoes);
            InventoryItem[] ringItems = Array.FindAll(InvArr, item => item.Data.Type == Enum.ItemType.Ring);
            InventoryItem[] relicItems = Array.FindAll(InvArr, item => item.Data.Type == Enum.ItemType.Relic);

            //* マージ
            bool isWpMerge = Merge(weaponItems);
            bool isShMerge = Merge(shoesItems);
            bool isRgMerge = Merge(ringItems);
            bool isRlMerge = Merge(relicItems);

            //* 合成できるか 確認
            if(!isWpMerge && !isShMerge && !isRgMerge && !isRlMerge) {
                HM._.hui.ShowMsgError("합성할 아이템이 없습니다.");
                return;
            }

            //* 現在のカテゴリを再クリックして０になったアイテムスロット 非表示
            HM._.ivm.OnClickCateMenuIconBtn(HM._.ivm.CurCateIdx);

            //* イベントリーUI アップデート (数値 など)
            InformAboutChange();

            //* 周り等級アイテムの数量によって、現在装置したEquipアイテムが消えることもあるため、Equipスロット４つも全て最新化
            // HM._.ivEqu.UpdateAllEquipSlots();

            SM._.SfxPlay(SM.SFX.Merge3SFX);
            HM._.hui.ShowMsgNotice("자동합성 완료!");
        }

        /// <summary>
        /// EQUIPアイテム マージ
        /// </summary>
        /// <param name="equipItems">EQUIPアイテム配列</param>
        /// <returns>マージできるか結果</returns>
        private bool Merge(InventoryItem[] equipItems) {
            const int PRIME_IDX_CNT = 1;

            //* マージできるか アイテム数を確認
            bool isMergable = Array.Exists(equipItems, item => item.Quantity >= Config.EQUIP_MERGE_CNT);

            //* マージできる数がなかったら、そのまま終了
            if(!isMergable) {
                return false;
            }

            //* マージ 実行
            for(int i = 0; i < equipItems.Length - PRIME_IDX_CNT; i++) {
                // 現在と次LVのEQUIPアイテム INDEX
                int id = equipItems[i].Data.ID;
                int nextId = equipItems[i + 1].Data.ID;

                // マージカウント
                int mergeCnt = InvArr[id].Quantity / Config.EQUIP_MERGE_CNT;
                int removeQuantity = mergeCnt * Config.EQUIP_MERGE_CNT;

                // アイテム数 減る
                InvArr[id] = InvArr[id].ChangeQuantity(InvArr[id].Quantity - removeQuantity);

                // マージ後、数が０なら
                if(InvArr[id].Quantity <= 0) {
                    // 装置中なら
                    if(InvArr[id].IsEquip) {
                        Debug.Log($"MERGE WEAPON ITEM: i({i}), {InvArr[id].Data.Name}, isEquip= {InvArr[id].IsEquip}");
                        // IsEquip 初期化
                        InvArr[i] = InvArr[i].ChangeIsEquip(false);
                        // 「装置中」DimUI
                        HM._.ivm.InitEquipDimUI(InvArr[id].Data.Type);
                        // Equipスロット 初期化
                        HM._.ivEqu.ResetEquipSlot(InvArr[id].Data.Type);
                    }

                    // Empty初期化 + 非表示
                    InvArr[id] = InvArr[id].GetEmptyItem();
                    HM._.ivm.ActiveSlotUI(id, false);
                }

                // 結果
                // Relicなら、ランダムで能力
                var relicAbilities = CheckRelicAbilitiesData(InvArr[nextId].Data);

                // 次のLVアイテム生成
                AddStackableItem(InvArr[nextId].Data, mergeCnt, lv: 1, relicAbilities, InvArr[nextId].IsEquip, isNewAlert: true);

                // RELICなら、Ability追加
                if(InvArr[nextId].Data.Type == Enum.ItemType.Relic)
                    InvArr[nextId] = InvArr[nextId].ChangeItemRelicAbilities(relicAbilities);
            }

            return true;
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
            InvArr[tgIdx] = InvArr[tgIdx].ChangeQuantity(InvArr[tgIdx].Quantity + decVal);
            // Debug.Log($"DecreaseItem():: ItemList[{tgIdx}]= {ItemList[tgIdx].Data.Name}, Quantity= {ItemList[tgIdx].Quantity}");

            //* アイテム数量が０なら、削除（Empty）
            if(InvArr[tgIdx].Quantity <= 0) {
                // インベントリースロットのデータとUIリセット
                InvArr[tgIdx] = InvArr[tgIdx].GetEmptyItem();
                HM._.ivm.InvUIItemArr[tgIdx].ResetUI();

                if(InvArr[tgIdx].Quantity <= 0) {
                    HM._.ivm.ActiveSlotUI(tgIdx, false);
                }

                // アイテムがないので、開いたPopUpに可能性があることを全て非表示
                HM._.rwlm.RewardChestPopUp.SetActive(false);
                HM._.ivm.ConsumePopUp.SetActive(false);
            }

            // foreach (var invItemUI in HM._.ivm.InvUIItemArr) {
            //     if(invItemUI.IsEmpty)
            //         invItemUI.ResetUI();
            // }

            //* イベントリーUI アップデート
            InformAboutChange();
        }

        // public void SortInventory() {
        //     Debug.Log("SortInventory()::");
        //     //* 整列
        //     invArr.Sort((a, b) => {
        //         if (a.IsEmpty && b.IsEmpty)
        //             return 0; 
        //         if (a.IsEmpty)
        //             return 1; // aをb後ろへ
        //         if (b.IsEmpty)
        //             return -1; // bをa後ろへ

        //         // １．タイプによって整列する
        //         int itemTypeComparison = a.Data.Type.CompareTo(b.Data.Type);
        //         if (itemTypeComparison != 0) {
        //             return itemTypeComparison;
        //         }
        //         // ２．同じタイプの場合、名前で整列
        //         return a.Data.name.CompareTo(b.Data.name);
        //     });
        // }

        public void AddItem(InventoryItem item) {
            Debug.Log($"AddItem():: {item.Data.Name}, Lv= {item.Lv}, {(item.RelicAbilities != null? item.RelicAbilities.Length : "NULL")}, isEquip= {item.IsEquip}");
            AddItem(item.Data, item.Quantity, item.Lv, item.RelicAbilities, item.IsEquip, item.IsNewAlert);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        // public Dictionary<int, InventoryItem> GetCurrentInventoryState() {
        //     Dictionary<int, InventoryItem> invItemDic = new Dictionary<int, InventoryItem>();
        //     for(int i = 0; i < InvArr.Length; i++) {
        //         if(InvArr[i].IsEmpty)
        //             continue;
        //         invItemDic[i] = InvArr[i];
        //     }
        //     return invItemDic;
        // }

        /// <summary>
        /// 実際のインベントリーへあるアイテム情報を返す
        /// </summary>
        /// <param name="itemIdx"></param>
        public InventoryItem GetItemAt(int itemIdx)
            => InvArr[itemIdx];

        // public void SwapItems(int itemIdx1, int itemIdx2) {
        //     InventoryItem item1 = InvArr[itemIdx1];
        //     InvArr[itemIdx1] = InvArr[itemIdx2];
        //     InvArr[itemIdx2] = item1;
        //     InformAboutChange();
        // }

        public void InformAboutChange() {
            Debug.Log("InformAboutChange()::");
            for(int i = 0; i < HM._.ivCtrl.InventoryData.InvArr.Length; i++)
                HM._.ivm.UpdateUI(i, HM._.ivCtrl.InventoryData.InvArr[i]);
        }
    }
    #endregion
}