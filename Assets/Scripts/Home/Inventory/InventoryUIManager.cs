using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

public class InventoryUIManager : MonoBehaviour {
    [field:SerializeField] public GameObject WindowObj {get; private set;}
    [field:SerializeField] public GameObject EquipPopUp {get; private set;}
    [field:SerializeField] public GameObject ConsumePopUp {get; private set;}
    [field:SerializeField] public InventoryItem ItemPf {get; private set;}
    [field:SerializeField] public RectTransform Content {get; set;}
    [field:SerializeField] public InventoryDescription ItemDescription {get; set;}
    [field:SerializeField] public MouseFollower MouseFollower {get; set;}
    [field:SerializeField] List<InventoryItem> ItemList = new List<InventoryItem>();

    private int curDraggedItemIdx = -1;

    //* Actionで使えるint ➝ Index
    public event Action<int> OnDescriptionRequested,
        OnItemActionRequested,
        OnStartDragging;
    public event Action<int, int> OnSwapItems;



    void Awake() {
        Hide();
        MouseFollower.Toggle(false);
        ItemDescription.ResetDescription();
    }

#region EVENT
    public void OnClickInventoryIconBtn() {
        Show();
    }
    public void OnClickInventoryPopUpBackBtn() {
        Hide();
    }
#endregion

#region FUNC
    public void Show() {
        WindowObj.SetActive(true);
        ResetSelection();
    }
    private void ResetSelection() {
        ItemDescription.ResetDescription();
        DeselectAllItems();
    }
    private void DeselectAllItems() {
        foreach(InventoryItem item in ItemList) {
            item.Deselect();
        }
    }
    public void Hide() {
        WindowObj.SetActive(false);
        ResetDraggedItem();
    }
    public void InitInventoryUI(int invSize) {
        for(int i = 0; i < invSize; i++) {
            InventoryItem item = Instantiate(ItemPf, Content);
            ItemList.Add(item);

            item.OnItemClicked += HandleItemSelection;
            item.OnItemBeginDrag += HandleBeginDrag;
            item.OnItemDroppedOn += HandleSwap;
            item.OnItemEndDrag += HandleEndDrag;
            item.OnRightMouseBtnClick += HandleShowItemActions;
        }
    }

    public void UpdateData(int itemIdx, Sprite itemSpr, int itemVal) {
        if(ItemList.Count > itemIdx) {
            ItemList[itemIdx].SetData(itemSpr, itemVal);
        }
    }
    private void ResetDraggedItem() {
        MouseFollower.Toggle(false);
        curDraggedItemIdx = -1;
    }

    private void HandleShowItemActions(InventoryItem invItem) {
        HM._.ivm.EquipPopUp.SetActive(true);
    }

    private void HandleEndDrag(InventoryItem invItem) {
        ResetDraggedItem();
    }

    private void HandleSwap(InventoryItem invItem) {
        int idx = ItemList.IndexOf(invItem);
        if(idx == -1) {
            return;
        }
        OnSwapItems?.Invoke(curDraggedItemIdx, idx);
    }

    private void HandleBeginDrag(InventoryItem invItem) {
        int idx = ItemList.IndexOf(invItem);
        if(idx == -1) return;
        curDraggedItemIdx = idx;
        HandleItemSelection(invItem);
        OnStartDragging?.Invoke(idx);

    }

    public void CreateDraggedItem(Sprite spr, int val) {
        MouseFollower.Toggle(true);
        MouseFollower.SetData(spr, val);
    } 

    private void HandleItemSelection(InventoryItem invItem) {
        int idx = ItemList.IndexOf(invItem);
        if(idx == -1) return;
        OnDescriptionRequested?.Invoke(idx);
    }

    #endregion
}
