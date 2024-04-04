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
        public List<InventoryItem> InitItems = new List<InventoryItem>();

        void Start() {
            InvUI = HM._.ivm;
            PrepareUI();
            PrepareInventoryData();
        }

        private void PrepareInventoryData() {
            InventoryData.Init();
            InventoryData.OnInventoryUpdated += UpdateInventoryUI;
            foreach (InventoryItem item in InitItems) {
                if(item.IsEmpty)
                    continue;
                InventoryData.AddItem(item);
            }
        }

        private void UpdateInventoryUI(Dictionary<int, InventoryItem> inventoryState) {
            InvUI.ResetAllItems();
            foreach (var item in inventoryState) {
                InvUI.UpdateData(item.Key, item.Value);
            }
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
            InvUI.CreateDraggedItem(item.Data.Type, item.Data.Grade, item.Data.ItemImg, item.Val);
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
            InvUI.UpdateDescription(itemIdx, item, invItem.Val, invItem.Abilities);
        }

        public void ShowInventory() {
            InvUI.Show();
            foreach (var item in InventoryData.GetCurrentInventoryState()) {
                InvUI.UpdateData(
                    item.Key,
                    item.Value
                );
            }
        }

        public void HideInventory() {
            InvUI.Hide();
        }
    }

}