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
            get => DM._.DB.InvItemDBs;
        }

        void Start() {
            DM._.LoadDt();
            ivm = HM._.ivm;
            PrepareUI();
            PrepareInventoryData();
        }

        void OnDisable() {
            InventoryData.OnInventoryUpdated -= UpdateInventoryUI;
        }

    #region FUNC
        private void PrepareUI() {
            ivm.InitInventoryUI(InventoryData.Size);
            ivm.OnDescriptionRequested += HandleDescriptionRequest;
            ivm.OnSwapItems += HandleSwapItems;
            // ivm.OnStartDragging += HandleDragging;
            ivm.OnItemActionRequested += HandleItemActionRequest;
        }

        private void PrepareInventoryData() {
            //* InventorySOリストデータを初期化（ロードしたデータを実際に管理する場所）
            InventoryData.Init();
            //* インベントリUI初期化するメソッド機能を購読（まだ使わない）=> InventorySO::InformAboutChange()で処理
            InventoryData.OnInventoryUpdated += UpdateInventoryUI;
            //* DBの保存したインベントリデータを一個ずつ読みこみながら、インベントリSOリストへ追加
            Debug.Log($"PrepareInventoryData():: InitItems.Length= {InvItemDBs.Count}");
            foreach (InventoryItem item in InvItemDBs) {
                if(item.IsEmpty) continue;
                // item.Data.SetRelicAbility();
                InventoryData.AddItem(item);
            }
        }

        private void UpdateInventoryUI(Dictionary<int, InventoryItem> inventoryState) {
            Debug.Log("UpdateInventoryUI()::");
            ivm.ResetAllItems();
            foreach (var item in inventoryState)
                ivm.UpdateData(item.Key, item.Value);
        }



        private void HandleItemActionRequest(int itemIdx) {}

        // private void HandleDragging(int itemIdx) {
        //     InventoryItem item = InventoryData.GetItemAt(itemIdx);
        //     if(item.IsEmpty) return;
        //     ivm.CreateDraggedItem(item.Data.Type, item.Data.Grade, item.Data.ItemImg, item.Quantity, item.Lv);
        // }

        private void HandleSwapItems(int itemIdx1, int itemIdx2) {   
            Debug.Log($"HandleSwapItems():: itemIdx1= {itemIdx1}, itemIdx2= {itemIdx2}");
            InventoryData.SwapItems(itemIdx1, itemIdx2);
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
            Debug.Log("ShowInventory()::");
            ivm.Show();
            foreach (var item in InventoryData.GetCurrentInventoryState()) {
                ivm.UpdateData( item.Key, item.Value );
            }
        }

        public void HideInventory() {
            ivm.Hide();
        }

    #region EQUIP
        public int FindCurEquipItemIdx(Enum.ItemType type) {
            return InventoryData.ItemList.FindIndex(item
                => !item.IsEmpty
                && item.IsEquip
                && item.Data.Type == type
            );
        }

        public void UnEquipSlotUI() {
            InventoryEquipUIManager ivEqu = HM._.ivEqu;
            InventoryItem curInvItem = InventoryData.ItemList[HM._.ivm.CurItemIdx];
            Enum.ItemType type = curInvItem.Data.Type;

            //* インベントリ 初期化
            InventoryData.InitIsEquipData(type); // ItemDt.isEquip
            HM._.ivm.InitEquipDimUI(type); // 「装置中」DimUI 

            //* Equipスロット 初期化
            ivEqu.ResetEquipSlot(type);

            // HM._.ivEqu.SetEquipAbilityData(curInvItem, isUnEquip: true);
        }

        public void EquipItemSlotUI() {
            var ivEqu = HM._.ivEqu;
            InventoryItem  curInvItem = InventoryData.ItemList[HM._.ivm.CurItemIdx];
            Enum.ItemType type = curInvItem.Data.Type;

            //* 初期化
            InventoryData.InitIsEquipData(curInvItem.Data.Type); // 「装置中」DimUI 
            HM._.ivm.InitEquipDimUI(curInvItem.Data.Type); // ItemDt

            //* アップデート (現在着用したアイテム)
            InventoryData.ItemList[HM._.ivm.CurItemIdx] = curInvItem.ChangeIsEquip(true); // IsEquip：True
            HM._.ivm.InvUIItemList[HM._.ivm.CurItemIdx].EquipDim.SetActive(true); // DimUI 表示

            //* 装置スロットUI
            ivEqu.EquipItem(type, curInvItem);
            ivEqu.EquipUIEFs[(int)type].Play();
            // HM._.ivEqu.SetEquipAbilityData(curInvItem);
        }
    #endregion
    }
    #endregion
}