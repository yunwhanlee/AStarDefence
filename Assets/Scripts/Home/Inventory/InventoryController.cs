using System;
using System.Collections;
using System.Collections.Generic;
using Inventory.Model;
using Inventory.UI;
using UnityEngine;

namespace Inventory 
{
    public class InventoryController : MonoBehaviour {
        [SerializeField] private InventoryUIManager invUI;
        [SerializeField] private InventorySO inventoryData;
        public List<InventoryItem> initItems = new List<InventoryItem>();

        void Start() {
            PrepareUI();
            PrepareInventoryData();
        }

        private void PrepareInventoryData()
        {
            inventoryData.Init();
            inventoryData.OnInventoryUpdated += UpdateInventoryUI;
            foreach (InventoryItem item in initItems) {
                if(item.IsEmpty)
                    continue;
                inventoryData.AddItem(item);
            }
        }

        private void UpdateInventoryUI(Dictionary<int, InventoryItem> inventoryState)
        {
            invUI.ResetAllItems();
            foreach (var item in inventoryState) {
                invUI.UpdateData(item.Key, item.Value);
            }
        }

        private void PrepareUI() {
            invUI.InitInventoryUI(inventoryData.Size);
            invUI.OnDescriptionRequested += HandleDescriptionRequest;
            invUI.OnSwapItems += HandleSwapItems;
            invUI.OnStartDragging += HandleDragging;
            invUI.OnItemActionRequested += HandleItemActionRequest;
        }

        private void HandleItemActionRequest(int itemIdx)
        {
            
        }

        private void HandleDragging(int itemIdx)
        {
            InventoryItem item = inventoryData.GetItemAt(itemIdx);
            if(item.IsEmpty)
                return;
            invUI.CreateDraggedItem(item.Data.Type, item.Data.Grade, item.Data.ItemImg, item.Val);
        }

        private void HandleSwapItems(int itemIdx1, int itemIdx2)
        {   
            Debug.Log($"HandleSwapItems():: itemIdx1= {itemIdx1}, itemIdx2= {itemIdx2}");
            inventoryData.SwapItems(itemIdx1, itemIdx2);
        }

        private void HandleDescriptionRequest(int itemIdx)
        {
            InventoryItem invItem = inventoryData.GetItemAt(itemIdx);
            if(invItem.IsEmpty) {
                invUI.ResetSelection();
                return;
            }
            ItemSO item = invItem.Data;
            invUI.UpdateDescription(itemIdx, item, invItem.Val);
        }

        public void ShowInventory() {
            invUI.Show();
            foreach (var item in inventoryData.GetCurrentInventoryState()) {
                invUI.UpdateData(
                    item.Key,
                    item.Value
                );
            }
        }

        public void HideInventory() {
            invUI.Hide();
        }
    }

}

