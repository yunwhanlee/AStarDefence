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
        public bool IsEmpty => Data == null; //* ItemSOがNull(登録されていない)なら、true
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
        [field:Header("インベントリリスト")]
        [field: SerializeField] public List<InventoryItem> invList;

        [field: SerializeField] public static int Size {get; private set;} = 50;
        public event Action<Dictionary<int, InventoryItem>> OnInventoryUpdated;

        public void Init() {
            Debug.Log("InventorySO:: Init():: (インベントリデータ) 初期化");
            invList = new List<InventoryItem>();
            for(int i = 0; i < Size; i++)
                invList.Add(InventoryItem.GetEmptyItem());
        }

        public void InitIsEquipData(Enum.ItemType type) {
            for(int i = 0; i < invList.Count; i++) {
                if(invList[i].IsEmpty)
                    continue;
                if(invList[i].Data.Type == type)
                    invList[i] = invList[i].ChangeIsEquip(false);
            }
        }

        public int AddItem(ItemSO item, int quantity, int lv, AbilityType[] relicAbilities, bool isEquip = false, bool isNewAlert = false) {
            Debug.Log($"AddItem:: AddStackableItem():: ItemList[0].Empty= {invList[0].IsEmpty}");
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
            for(int i = 0; i < invList.Count; i++) {
                if(invList[i].IsEmpty) {
                    invList[i] = newItem;
                    return quantity;
                }
            }
            return 0;
        }

        /// <summary>
        /// 一つでも空スロットがあったら、インベントリーがFullではない => False
        /// </summary>
        private bool IsInventoryFull()
            => invList.Where(item => item.IsEmpty).Any() == false;

        /// <summary>
        /// 数えるアイテムとして追加 (自動マージしたときも使う)
        /// </summary>
        private int AddStackableItem(ItemSO item, int quantity, int lv, AbilityType[] relicAbilities, bool isEquip, bool isNewAlert) {
            //* 全てのインベントリースロットを回す
            for(int i = 0; i < invList.Count; i++) {
                Debug.Log($"AddStackableItem():: ItemList[{i}].Empty= {invList[i].IsEmpty}");
                //* スロットにアイテムが有り、
                if(!invList[i].IsEmpty) {
                    //* 同じアイテムなら、
                    if(invList[i].Data.ID == item.ID) {
                        //* アイテム数量 増加
                        Debug.Log($"AddStackableItem():: EACH SAME {invList[i].Data.Name}(ID={invList[i].Data.ID}) == {item.Name}(ID={item.ID}), ItemList.Count= {invList.Count}");
                        int amountPossibleToTake = invList[i].Data.MaxStackSize - invList[i].Quantity;

                        if(quantity > amountPossibleToTake) {
                            invList[i] = invList[i].ChangeQuantity(invList[i].Data.MaxStackSize);
                            quantity -= amountPossibleToTake;
                        }
                        else {
                            invList[i] = invList[i].ChangeQuantity(invList[i].Quantity + quantity);
                            InformAboutChange();
                            return 0;
                        }
                        break;
                    }
                }
                //* スロットが空いていて
                else {
                    //* インベントリーがいっぱい
                    if(quantity > 0 && IsInventoryFull()) {
                        HM._.hui.ShowMsgError("インベントリーに空いているスロットがないです。");
                        continue; //* 次に進む
                    }

                    //! (BUG) アイテムの数量が０になる(Emptyスロット)と、Emptyスロットを埋めるのが優先になって同じ物が重複するバグがある
                    //* そのため、ループで全てを回しながら、新しく生成する前に同じ物があるのかを検査する
                    if(invList.FindIndex(invSlot => !invSlot.IsEmpty && invSlot.Data.ID == item.ID) != -1) {
                        Debug.Log($"<color=red>AddStackableItem():: EACH NEW ALREADY EXIST {item.Name}(ID={item.ID})</color>");
                        //* もしあったら、数量を上げて終了
                        invList[i] = invList[i].ChangeQuantity(invList[i].Quantity + quantity);
                        continue; //* 次に進む
                    }

                    //* 新しくアイテム生成 (最初の初期化にも使う)
                    Debug.Log($"AddStackableItem():: EACH NEW {item.Name}(ID={item.ID}), ItemList.Count= {invList.Count}");
                    AddItemToFirstFreeSlot(item, quantity, lv, relicAbilities, isEquip, isNewAlert);
                    break;
                }
            }

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
            for(int i = 0; i < invList.Count; i++) {
                try {
                    //* 同じIDを探して
                    if(invList[i].Data.ID == curItem.ID) {
                        //* アップグレードのMAX制限
                        int max = (curItem.Type == Enum.ItemType.Relic)? Config.RELIC_UPGRADE_MAX : Config.EQUIP_UPGRADE_MAX;
                        lv = Mathf.Min(lv, max);
                        Debug.Log($"UpgradeEquipItem({invList[i].Data.ID} == {curItem.ID}):: lv= {lv}");
                        //* 増えたVal値を最新化
                        invList[i] = invList[i].ChangeLevel(lv);
                        // //* イベントリーUI アップデート
                        // InformAboutChange();
                        // //* 情報表示ポップアップUI アップデート
                        // HM._.ivm.UpdateDescription(HM._.ivm.CurItemIdx, item, quantity, lv, abilities, isEquip);
                        return;
                    }
                }
                catch(Exception msg) {
                    Debug.LogWarning("(BUG) " + msg);
                    Debug.Log($"ItemList[{i}]= {invList[i]}");
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

            //* マージできるか状況 確認
            foreach(var item in invList) {
                if(item.IsEmpty) continue;
                if(item.Data.Type == Enum.ItemType.Etc) continue;

                if(item.Quantity >= Config.EQUIPITEM_MERGE_CNT) {
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
                HM._.hui.ShowMsgError($"{typeName}장착을 해제해주세요! (장착한 아이템 중 합성대상이 있어 불가)");
                return;
            }

            SM._.SfxPlay(SM.SFX.Merge3SFX);

            //* マージ
            for(int i = 0; i < invList.Count; i++) {
                InventoryItem item = invList[i];
                if(item.Quantity >= Config.EQUIPITEM_MERGE_CNT) {
                    if(item.Data.Grade == Enum.Grade.Prime) {
                        Debug.Log("最後の等級なので、処理しない");
                        continue;
                    }

                    int mergeCnt = item.Quantity / Config.EQUIPITEM_MERGE_CNT;
                    int removeQuantity = mergeCnt * Config.EQUIPITEM_MERGE_CNT;

                    //* 数量 減る
                    invList[i] = item.ChangeQuantity(item.Quantity - removeQuantity);

                    //* マージしてアイテム数量が０なら、削除（Empty）
                    if(invList[i].Quantity <= 0) 
                        invList[i] = InventoryItem.GetEmptyItem();

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

                    //* 次の等級アイテム生成
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
            invList[tgIdx] = invList[tgIdx].ChangeQuantity(invList[tgIdx].Quantity + decVal);
            // Debug.Log($"DecreaseItem():: ItemList[{tgIdx}]= {ItemList[tgIdx].Data.Name}, Quantity= {ItemList[tgIdx].Quantity}");

            //* アイテム数量が０なら、削除（Empty）
            if(invList[tgIdx].Quantity <= 0) {
                //* インベントリースロットのデータとUIリセット
                invList[tgIdx] = InventoryItem.GetEmptyItem();
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
            invList.Sort((a, b) => {
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
            for(int i = 0; i < invList.Count; i++) {
                if(invList[i].IsEmpty)
                    continue;
                invItemDic[i] = invList[i];
            }
            return invItemDic;
        }

        /// <summary>
        /// 実際のインベントリーへあるアイテム情報を返す
        /// </summary>
        /// <param name="itemIdx"></param>
        public InventoryItem GetItemAt(int itemIdx)
            => invList[itemIdx];

        public void SwapItems(int itemIdx1, int itemIdx2) {
            InventoryItem item1 = invList[itemIdx1];
            invList[itemIdx1] = invList[itemIdx2];
            invList[itemIdx2] = item1;
            InformAboutChange();
        }

        public void InformAboutChange() {
            Debug.Log("InformAboutChange()::");
            OnInventoryUpdated?.Invoke(GetCurrentInventoryState());
        }
    }
    #endregion
}