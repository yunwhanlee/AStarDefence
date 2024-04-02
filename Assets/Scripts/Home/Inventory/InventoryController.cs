using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class InventoryController : MonoBehaviour {
    [SerializeField] private InventoryUIManager invUI;
    [SerializeField] private InventorySO inventoryData;

    void Start()
    {
        PrepareUI();
        // inventoryData.Init();
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
        
    }

    private void HandleSwapItems(int itemIdx1, int itemIdx2)
    {
        
    }

    private void HandleDescriptionRequest(int itemIdx)
    {
        InventoryItem inventoryItem = inventoryData.GetItemAt(itemIdx);
        if(inventoryItem.IsEmpty) {
            invUI.ResetSelection();
            return;
        }
        ItemSO item = inventoryItem.Item;
        invUI.UpdateDescription(itemIdx, item.ItemImg, item.Name, item.Description);
    }

    public void ShowInventory() {
        invUI.Show();
        foreach (var item in inventoryData.GetCurrentInventoryState()) {
            invUI.UpdateData(item.Key,
                item.Value.Item.ItemImg,
                item.Value.Val
            );
        }
    }

    public void HideInventory() {
        invUI.Hide();
    }
}
