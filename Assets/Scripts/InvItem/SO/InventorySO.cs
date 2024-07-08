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

            if(item.Type == Enum.ItemType.Relic)
                InvArr[item.ID] = InvArr[item.ID].ChangeItemRelicAbilities(relicAbilities);
            
            // InformAboutChange();

            // //* 全てのインベントリースロットを回す
            // for(int i = 0; i < InvArr.Length; i++) {
            //     Debug.Log($"AddStackableItem():: ItemList[{i}].Empty= {InvArr[i].IsEmpty}");
            //     //* スロットにアイテムが有り、
            //     // if(!invArr[i].IsEmpty) {
            //     //* 同じアイテムなら、
            //     Debug.Log($"invArr[{i}].Data.Name({InvArr[i].Data.Name}) == item.Name({item.Name})");
            //     Debug.Log($"invArr[{i}].Data.ID({InvArr[i].Data.ID}) == item.ID({item.ID})");
            //     if(InvArr[i].Data.ID == item.ID) {
            //         //* アイテム数量 増加
            //         Debug.Log($"AddStackableItem():: EACH SAME {InvArr[i].Data.Name}(ID={InvArr[i].Data.ID}) == {item.Name}(ID={item.ID}), ItemList.Length= {InvArr.Length}");
            //         int amountPossibleToTake = InvArr[i].Data.MaxStackSize - InvArr[i].Quantity;

            //         if(quantity > amountPossibleToTake) {
            //             InvArr[i] = InvArr[i].ChangeQuantity(InvArr[i].Data.MaxStackSize);
            //             quantity -= amountPossibleToTake;
            //         }
            //         else {
            //             InvArr[i] = InvArr[i].ChangeQuantity(InvArr[i].Quantity + quantity);
            //             InformAboutChange();
            //             return 0;
            //         }
            //         break;
            //     }
                // }
                //* スロットが空いていて
                // else {
                //     //* インベントリーがいっぱい
                //     if(quantity > 0 && IsInventoryFull()) {
                //         HM._.hui.ShowMsgError("インベントリーに空いているスロットがないです。");
                //         continue; //* 次に進む
                //     }

                //     //! (BUG) アイテムの数量が０になる(Emptyスロット)と、Emptyスロットを埋めるのが優先になって同じ物が重複するバグがある
                //     //* そのため、ループで全てを回しながら、新しく生成する前に同じ物があるのかを検査する
                //     if(invArr.FindIndex(invSlot => !invSlot.IsEmpty && invSlot.Data.ID == item.ID) != -1) {
                //         Debug.Log($"<color=red>AddStackableItem():: EACH NEW ALREADY EXIST {item.Name}(ID={item.ID})</color>");
                //         //* もしあったら、数量を上げて終了
                //         invArr[i] = invArr[i].ChangeQuantity(invArr[i].Quantity + quantity);
                //         continue; //* 次に進む
                //     }

                //     //* 新しくアイテム生成 (最初の初期化にも使う)
                //     Debug.Log($"AddStackableItem():: EACH NEW {item.Name}(ID={item.ID}), ItemList.Count= {invArr.Count}");
                //     AddItemToFirstFreeSlot(item, quantity, lv, relicAbilities, isEquip, isNewAlert);
                //     break;
                // }
            // }

            //* アイテム生成 (最初の初期化にも使う)
            // while(quantity > 0 && IsInventoryFull() == false) {
            //     Debug.Log($"AddStackableItem():: アイテム生成= {item.name}");
            //     int newQuantity = Mathf.Clamp(quantity, 0, item.MaxStackSize);
            //     quantity -= newQuantity;
            //     AddItemToFirstFreeSlot(item, newQuantity, lv, relicAbilities, isEquip, isNewAlert);
            // }

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
        /// インベントリーの装置アイテムを次のレベルに自動マージ
        /// </summary>
        public void AutoMergeEquipItem() {
            bool isMergable = false;
            bool isMustUnEquip = false;
            string typeName = "";

            //* マージ状況 確認
            foreach(var item in InvArr) {
                if(item.IsEmpty) continue;
                if(item.Data.Type == Enum.ItemType.Etc) continue;
                if(item.Data.Type != Enum.ItemType.Etc && item.Data.Grade == Enum.Grade.Prime) continue;

                // 装置アイテム中で、マージができるのがあったら
                if(item.Quantity >= Config.EQUIPITEM_MERGE_CNT) {
                    isMergable = true;
                    if(item.IsEquip) {
                        isMustUnEquip = true;
                        typeName = Enum.GetItemTypeName(item.Data.Type);
                    }
                }

                // 着用した装置アイテムがあったら、同じタイプのアイテムを全て探す
                // if(item.IsEquip) {
                //     var type = item.Data.Type;
                //     List<InventoryItem> sameTypeEquipItems = invList.FindAll(invItem => !invItem.IsEmpty && invItem.Data?.Type != Enum.ItemType.Etc && invItem.Data?.Type == type);

                //     //* この中でマージできる物があったら、装置解除のお知らせトリガーをONにする
                //     sameTypeEquipItems.ForEach(item => Debug.Log($"sameTypeEquipItems item= {item.Data.Name}"));
                //     isMustUnEquip = sameTypeEquipItems.Exists(item => item.Quantity >= Config.EQUIPITEM_MERGE_CNT);
                //     typeName = Enum.GetItemTypeName(item.Data.Type);
                // }
            }

            //* 除外処理
            if(!isMergable) {
                HM._.hui.ShowMsgError("합성할 아이템이 없습니다.");
                return;
            }
            else if(isMustUnEquip) {
                HM._.hui.ShowMsgError($"{typeName}장착을 해제해주세요! (장착한 아이템 중 합성대상이 있습니다.)");
                return;
            }

            SM._.SfxPlay(SM.SFX.Merge3SFX);

            //* マージ
            for(int i = 0; i < InvArr.Length; i++) {
                InventoryItem item = InvArr[i];

                if(!item.IsEmpty
                && item.Data.Type != Enum.ItemType.Etc
                && item.Data.Type != Enum.ItemType.Empty
                && item.Quantity >= Config.EQUIPITEM_MERGE_CNT)
                {
                    Debug.Log($"AutoMergeEquipItem():: Merge item.Data= {item.Data}, item.Name= {item.Data.Name}, isEquip= {item.IsEquip}");

                    if(item.Data.Grade == Enum.Grade.Prime) {
                        Debug.Log("最後の等級なので、処理しない");
                        continue;
                    }

                    int mergeCnt = item.Quantity / Config.EQUIPITEM_MERGE_CNT;
                    int removeQuantity = mergeCnt * Config.EQUIPITEM_MERGE_CNT;

                    //* 数量 減る
                    InvArr[i] = item.ChangeQuantity(item.Quantity - removeQuantity);

                    //* マージしてアイテム数量が０なら、削除（Empty）
                    if(InvArr[i].Quantity <= 0) 
                        InvArr[i] = InvArr[i].GetEmptyItem();

                    //* 次のレベルアイテム生成
                    var type = item.Data.Type;
                    int nextGrade = (int)item.Data.Grade + 1;
                    Debug.Log($"AutoMergeEquipItem():: {type}: {(int)item.Data.Grade} -> {nextGrade}");

                    // タイプ
                    ItemSO nextLvItemDt = (type == Enum.ItemType.Weapon)? HM._.rwlm.RwdItemDt.WeaponDatas[nextGrade]
                        : (type == Enum.ItemType.Shoes)? HM._.rwlm.RwdItemDt.ShoesDatas[nextGrade]
                        : (type == Enum.ItemType.Ring)? HM._.rwlm.RwdItemDt.RingDatas[nextGrade]
                        : (type == Enum.ItemType.Relic)? HM._.rwlm.RwdItemDt.RelicDatas[nextGrade - (int)Enum.Grade.Epic]
                        : null;

                    if(nextLvItemDt != null)
                        continue;

                    //* Relicなら、ランダムで能力
                    var relicAbilities = CheckRelicAbilitiesData(nextLvItemDt);

                    //* 次のLVアイテム生成
                    // AddStackableItem(nextLvItemDt, mergeCnt, lv: 1, relicAbilities, item.IsEquip, isNewAlert: true);
                    InvArr[nextLvItemDt.ID] = InvArr[nextLvItemDt.ID].ChangeQuantity(mergeCnt);

                    //* RELICなら、Ability追加
                    if(type == Enum.ItemType.Relic)
                        InvArr[nextLvItemDt.ID] = InvArr[nextLvItemDt.ID].ChangeItemRelicAbilities(relicAbilities);
                }
            }
            //* 整列
            // SortInventory();

            //* イベントリーUI アップデート
            InformAboutChange();

            //* 周り等級アイテムの数量によって、現在装置したEquipアイテムが消えることもあるため、Equipスロット４つも全て最新化
            HM._.ivEqu.UpdateAllEquipSlots();

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
            InvArr[tgIdx] = InvArr[tgIdx].ChangeQuantity(InvArr[tgIdx].Quantity + decVal);
            // Debug.Log($"DecreaseItem():: ItemList[{tgIdx}]= {ItemList[tgIdx].Data.Name}, Quantity= {ItemList[tgIdx].Quantity}");

            //* アイテム数量が０なら、削除（Empty）
            if(InvArr[tgIdx].Quantity <= 0) {
                //* インベントリースロットのデータとUIリセット
                InvArr[tgIdx] = InvArr[tgIdx].GetEmptyItem();
                HM._.ivm.InvUIItemArr[tgIdx].ResetUI();
                //* アイテムがないので、開いたPopUpに可能性があることを全て非表示
                HM._.rwlm.RewardChestPopUp.SetActive(false);
                HM._.ivm.ConsumePopUp.SetActive(false);
                //* 整列
                // SortInventory();
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
            // OnInventoryUIUpdated?.Invoke(GetCurrentInventoryState());
        }
    }
    #endregion
}