using System;
using System.Collections;
using System.Collections.Generic;
using Inventory.Model;
using Inventory.UI;
using UnityEngine;

namespace Inventory 
{
    public class InventoryController : MonoBehaviour {
        [SerializeField] private InventoryUIManager ivm;
        [SerializeField] public InventorySO InventoryData;
        [SerializeField] public List<InventoryItem> InvItemDBs {
            get => DM._.DB.InvItemDBList;
        }

        void Start() {
            ivm = HM._.ivm;
            PrepareUI();
            PrepareInventoryData();
            StartCoroutine(CoUpdateNewItemAlert());
            // UpdateConsumableInvItems();
        }

        void OnDisable() {
            InventoryData.OnInventoryUIUpdated -= UpdateInventoryUI;
        }

    #region FUNC
        /// <summary>
        /// 消費アイテムが０以下になったら、インベントリーから削除して最新化
        /// </summary>
        private void UpdateConsumableInvItems() {
            // bool isConsumeItemDelete = false;

            for(int i = 0; i < InventoryData.InvArr.Length; i++) {
                var itemList = InventoryData.InvArr[i];
                if(itemList.IsEmpty)
                    continue;

                //* 消費アイテム 最新化 (数量が０なら、削除)
                if((itemList.Data.name == $"{Etc.ConsumableItem.Clover}"
                || itemList.Data.name == $"{Etc.ConsumableItem.GoldClover}"
                || itemList.Data.name == $"{Etc.ConsumableItem.SteamPack0}"
                || itemList.Data.name == $"{Etc.ConsumableItem.SteamPack1}"
                || itemList.Data.name == $"{Etc.ConsumableItem.BizzardScroll}"
                || itemList.Data.name == $"{Etc.ConsumableItem.LightningScroll}")
                && itemList.Quantity <= 0) 
                {
                    InventoryData.InvArr[i] = InventoryData.InvArr[i].GetEmptyItem();
                    // isConsumeItemDelete = true;
                }

                //* 消費アイテムの中で削除された物があったら、インベントリースロット 整列
                // if(isConsumeItemDelete) {
                //     InventoryData.SortInventory();
                // }
            }
        }

        /// <summary>
        ///  EXPクロバーが活性化したら、一個減る
        /// </summary>
        public void CheckActiveClover() {
            for(int i = 0; i < InventoryData.InvArr.Length; i++) {
                var itemList = InventoryData.InvArr[i];
                if(itemList.IsEmpty)
                    continue;

                if(DM._.DB.IsCloverActive && itemList.Data.name == $"{Etc.ConsumableItem.Clover}") {
                    InventoryData.InvArr[i] = itemList.ChangeQuantity(itemList.Quantity - 1);
                    if(InventoryData.InvArr[i].Quantity <= 0)
                        DM._.DB.IsCloverActive = false;
                }
                else if(DM._.DB.IsGoldCloverActive && itemList.Data.name == $"{Etc.ConsumableItem.GoldClover}") {
                    InventoryData.InvArr[i] = itemList.ChangeQuantity(itemList.Quantity - 1);
                    if(InventoryData.InvArr[i].Quantity <= 0)
                        DM._.DB.IsGoldCloverActive = false;
                }
            }
        }

        /// <summary>
        /// インベントリーNewアイテムの数を赤い点で表示
        /// </summary>
        IEnumerator CoUpdateNewItemAlert() {
            int newItemCnt;
            while(true) {
                newItemCnt = 0;
                foreach (InventoryItem itemDt in InventoryData.InvArr)
                    if (itemDt.IsNewAlert) newItemCnt++;
                HM._.ivm.SetInvAlertIcon(newItemCnt);
                yield return Util.Time0_5;
            }
        }

        private void PrepareUI() {
            ivm.InitInventoryUI();
            ivm.OnDescriptionRequested += HandleDescriptionRequest;
        }

        private void PrepareInventoryData() {
            //* DBに保存したインベントリーデータ 設定
            InventoryData.SetLoadData();

            //* インベントリUI最新化イベント登録 (InventorySO::InformAboutChange()で使う)
            InventoryData.OnInventoryUIUpdated += UpdateInventoryUI;

            //* DBの保存したインベントリデータを一個ずつ読みこみながら、インベントリSOリストへ追加
            // Debug.Log($"PrepareInventoryData():: InitItems.Length= {InvItemDBs}");
            // foreach (InventoryItem item in InvItemDBs) {
            //     if(item.IsEmpty) continue;
            //     // item.Data.SetRelicAbility();
            //     InventoryData.AddItem(item);
            // }
        }

        /// <summary>
        /// INVENTORYのSlotUIを最新化
        /// </summary>
        /// <param name="inventoryState">DICIONARY化した最新のINVENTORYデータ</param>
        private void UpdateInventoryUI(Dictionary<int, InventoryItem> inventoryState) {
            Debug.Log("UpdateInventoryUI()::");
            ivm.ResetAllItems();
            // foreach (var item in inventoryState)
            for(int i = 0; i < HM._.ivCtrl.InventoryData.InvArr.Length; i++)
                ivm.UpdateUI(i, HM._.ivCtrl.InventoryData.InvArr[i]);
        }

        private void HandleDescriptionRequest(int itemIdx) {
            InventoryItem invItem = InventoryData.GetItemAt(itemIdx);
            if(invItem.IsEmpty) {
                ivm.ResetSelection();
                return;
            }
            ItemSO item = invItem.Data;
            ivm.UpdateDescription(itemIdx, item, invItem.Quantity, invItem.Lv, invItem.RelicAbilities, invItem.IsEquip);
        }

        public void ShowInventory() {
            HM._.hui.IsActivePopUp = true;
            SM._.SfxPlay(SM.SFX.ClickSFX);
            ivm.Show();
            // Debug.Log($"InventoryData.GetCurrentInventoryState().Count= {InventoryData.GetCurrentInventoryState().Count}");
            // foreach (var item in InventoryData.GetCurrentInventoryState()) {
            //     ivm.UpdateUI( item.Key, item.Value );
            // }
            for(int i = 0; i < HM._.ivCtrl.InventoryData.InvArr.Length; i++)
                ivm.UpdateUI(i, HM._.ivCtrl.InventoryData.InvArr[i]);
        }

        public void HideInventory() {
            HM._.hui.IsActivePopUp = false;
            SM._.SfxPlay(SM.SFX.ClickSFX);
            ivm.Hide();
            HM._.dailyMs.OnUpdateUI.Invoke();
        }

    #region EQUIP
        public int FindCurrentEquipItemIdx(Enum.ItemType type) {
            return Array.FindIndex(InventoryData.InvArr, item
                => item.IsEquip
                && item.Data.Type == type
                // && !item.IsEmpty
            );
        }

        public void UnEquipSlotUI() {
            SM._.SfxPlay(SM.SFX.InvUnEquipSFX);
            InventoryEquipUIManager ivEqu = HM._.ivEqu;
            InventoryItem curInvItem = InventoryData.InvArr[HM._.ivm.CurItemIdx];
            Enum.ItemType type = curInvItem.Data.Type;

            //* インベントリ 初期化
            InventoryData.InitIsEquipData(type); // ItemDt.isEquip
            HM._.ivm.InitEquipDimUI(type); // 「装置中」DimUI 

            //* Equipスロット 初期化
            ivEqu.ResetEquipSlot(type);

            // HM._.ivEqu.SetEquipAbilityData(curInvItem, isUnEquip: true);
        }

        public void EquipItemSlotUI() {
            SM._.SfxPlay(SM.SFX.InvEquipSFX);
            var ivEqu = HM._.ivEqu;
            InventoryItem  curInvItem = InventoryData.InvArr[HM._.ivm.CurItemIdx];
            Enum.ItemType type = curInvItem.Data.Type;

            //* 初期化
            InventoryData.InitIsEquipData(curInvItem.Data.Type); // ItemDt.isEquip
            HM._.ivm.InitEquipDimUI(curInvItem.Data.Type); // 「装置中」DimUI

            //* アップデート (現在着用したアイテム)
            InventoryData.InvArr[HM._.ivm.CurItemIdx] = curInvItem.ChangeIsEquip(true); // IsEquip：True
            HM._.ivm.InvUIItemArr[HM._.ivm.CurItemIdx].EquipDim.SetActive(true); // DimUI 表示

            //* 装置スロットUI
            ivEqu.EquipItem(type, curInvItem);
            ivEqu.EquipUIEFs[(int)type].Play();
            // HM._.ivEqu.SetEquipAbilityData(curInvItem);
        }

        public InventoryItem OpenCurrentEquipPotentialAbility() {
            if(ivm.CurInvItem.RelicAbilities != null && ivm.CurInvItem.RelicAbilities.Length > 0) {
                Debug.Log($"OpenCurrentEquipPotentialAbility():: 既にある");
                //* 能力がもう有ったら、同じものが出ない処理のため、引数を渡す
                InventoryData.InvArr[ivm.CurItemIdx] = ivm.CurInvItem.ChangeItemRelicAbilities(
                    new AbilityType[1] {Util.PickRandomAbilityType(ivm.CurInvItem.RelicAbilities[0])}
                );
            }
            else {
                Debug.Log($"OpenCurrentEquipPotentialAbility():: 新しく生成");
                //* 新しいRelic能力を一つランダム選択 -> InventoryDtへ反映
                InventoryData.InvArr[ivm.CurItemIdx] = ivm.CurInvItem.ChangeItemRelicAbilities(
                    new AbilityType[1] {Util.PickRandomAbilityType()}
                );
            }
            //* ★ CurInvItemDtへも反映(EquipPopUpが開いたままであれば、CurInvItemで表示するため、最新化必要)
            ivm.CurInvItem = InventoryData.InvArr[ivm.CurItemIdx];
            Debug.Log($"OpenCurrentEquipPotentialAbility():: Ability Type= {ivm.CurInvItem.RelicAbilities[0]}");
            return ivm.CurInvItem;
        }

        public InventoryItem ResetCurrentRelicAbilities() {
            Debug.Log("ResetCurrentRelicAbilities()::");
            var ivm = HM._.ivm;
            ItemSO itemDt = ivm.CurInvItem.Data;
            //* 新しい能力
            AbilityType[] newRelicAbilities = InventoryData.CheckRelicAbilitiesData(itemDt);
            //* Relicの能力 変更
            return InventoryData.InvArr[ivm.CurItemIdx] = ivm.CurInvItem.ChangeItemRelicAbilities(newRelicAbilities);
        }
    #endregion
    }
    #endregion
}