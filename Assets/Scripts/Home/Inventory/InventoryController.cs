using System;
using System.Collections;
using System.Collections.Generic;
using Inventory.Model;
using Inventory.UI;
using UnityEngine;

namespace Inventory 
{
    public class InventoryController : MonoBehaviour {
        [SerializeField] private InventoryUIManager InvUI;
        [SerializeField] public InventorySO InventoryData;
        [SerializeField] public List<InventoryItem> InitItems {
            get => DM._.DB.InvItemDBs;
        }

        void Start() {
            InvUI = HM._.ivm;
            PrepareUI();
            PrepareInventoryData();
        }

        private void PrepareInventoryData() {
            //* InventorySOリストデータを初期化（ロードしたデータを実際に管理する場所）
            InventoryData.Init(); 
            //* インベントリUI初期化するメソッド機能を購読（まだ使わない）=> InventorySO::InformAboutChange()で処理
            InventoryData.OnInventoryUpdated += UpdateInventoryUI;
            //* DBの保存したインベントリデータを一個ずつ読みこみながら、インベントリSOリストへ追加
            foreach (InventoryItem item in InitItems) {
                if(item.IsEmpty) continue;
                // item.Data.SetRelicAbility();
                InventoryData.AddItem(item);
            }
        }

        private void UpdateInventoryUI(Dictionary<int, InventoryItem> inventoryState) {
            Debug.Log("UpdateInventoryUI()::");
            InvUI.ResetAllItems();
            foreach (var item in inventoryState)
                InvUI.UpdateData(item.Key, item.Value);
        }

        private void PrepareUI() {
            InvUI.InitInventoryUI(InventoryData.Size);
            InvUI.OnDescriptionRequested += HandleDescriptionRequest;
            InvUI.OnSwapItems += HandleSwapItems;
            InvUI.OnStartDragging += HandleDragging;
            InvUI.OnItemActionRequested += HandleItemActionRequest;
        }

        private void HandleItemActionRequest(int itemIdx) {}

        private void HandleDragging(int itemIdx) {
            InventoryItem item = InventoryData.GetItemAt(itemIdx);
            if(item.IsEmpty) return;
            InvUI.CreateDraggedItem(item.Data.Type, item.Data.Grade, item.Data.ItemImg, item.Quantity, item.Lv);
        }

        private void HandleSwapItems(int itemIdx1, int itemIdx2) {   
            Debug.Log($"HandleSwapItems():: itemIdx1= {itemIdx1}, itemIdx2= {itemIdx2}");
            InventoryData.SwapItems(itemIdx1, itemIdx2);
        }

        private void HandleDescriptionRequest(int itemIdx) {
            InventoryItem invItem = InventoryData.GetItemAt(itemIdx);
            if(invItem.IsEmpty) {
                InvUI.ResetSelection();
                return;
            }
            ItemSO item = invItem.Data;
            InvUI.UpdateDescription(itemIdx, item, invItem.Quantity, invItem.Lv, invItem.RelicAbilities);
        }

        public void ShowInventory() {
            Debug.Log("ShowInventory()::");
            InvUI.Show();
            foreach (var item in InventoryData.GetCurrentInventoryState()) {
                InvUI.UpdateData( item.Key, item.Value );
            }
        }

        public void HideInventory() {
            InvUI.Hide();
        }
    }

}