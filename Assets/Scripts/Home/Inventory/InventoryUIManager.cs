using System;
using System.Collections;
using System.Collections.Generic;
using Inventory.Model;
using UnityEngine;

namespace Inventory.UI 
{
    public class InventoryUIManager : MonoBehaviour {
        [SerializeField] InventoryController ivctrl;

        [field:SerializeField] public GameObject WindowObj {get; private set;}
        [field:SerializeField] public GameObject EquipPopUp {get; private set;}
        [field:SerializeField] public GameObject ConsumePopUp {get; private set;}
        [field:SerializeField] public InventoryUIItem ItemPf {get; private set;}
        [field:SerializeField] public RectTransform Content {get; set;}
        [field:SerializeField] public InventoryDescription ItemDescription {get; set;}
        [field:SerializeField] public MouseFollower MouseFollower {get; set;}
        [field:SerializeField] List<InventoryUIItem> ItemList = new List<InventoryUIItem>();

        private int curDraggedItemIdx = -1;

        //* Actionで使えるint ➝ Index
        public event Action<int> OnDescriptionRequested,
            OnItemActionRequested,
            OnStartDragging;

        public event Action<int, int> OnSwapItems;

        void Awake() {
            ivctrl = GameObject.Find("InventoryController").GetComponent<InventoryController>();

            Hide();
            MouseFollower.Toggle(false);
            ItemDescription.ResetDescription();
        }

    #region EVENT
        public void OnClickInventoryIconBtn() {
            ivctrl.ShowInventory();
        }
        public void OnClickInventoryPopUpBackBtn() {
            ivctrl.HideInventory();
        }
    #endregion

    #region FUNC
        public void Show() {
            WindowObj.SetActive(true);
            ResetSelection();
        }
        public void ResetSelection() {
            ItemDescription.ResetDescription();
            DeselectAllItems();
        }
        private void DeselectAllItems() {
            foreach(InventoryUIItem item in ItemList)
                item.Deselect();
        }
        public void Hide() {
            WindowObj.SetActive(false);
            ResetDraggedItem();
        }
        public void InitInventoryUI(int invSize) {
            for(int i = 0; i < invSize; i++) {
                InventoryUIItem item = Instantiate(ItemPf, Content);
                ItemList.Add(item);
                item.OnItemClicked += HandleItemSelection;
                item.OnItemBeginDrag += HandleBeginDrag;
                item.OnItemDroppedOn += HandleSwap;
                item.OnItemEndDrag += HandleEndDrag;
                item.OnRightMouseBtnClick += HandleShowItemInfoPopUp;
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

        private void HandleShowItemInfoPopUp(InventoryUIItem invItemUI) {
            EquipPopUp.SetActive(true);
            DeselectAllItems();
        }

        private void HandleEndDrag(InventoryUIItem invItemUI) {
            ResetDraggedItem();
        }

        private void HandleSwap(InventoryUIItem invItemUI) {
            int idx = ItemList.IndexOf(invItemUI);
            if(idx == -1) return;
            OnSwapItems?.Invoke(curDraggedItemIdx, idx);
            HandleItemSelection(invItemUI);
        }

        private void HandleBeginDrag(InventoryUIItem invItemUI) {
            int idx = ItemList.IndexOf(invItemUI);
            if(idx == -1) return;
            curDraggedItemIdx = idx;
            HandleItemSelection(invItemUI);
            OnStartDragging?.Invoke(idx);

        }

        public void CreateDraggedItem(Sprite spr, int val) {
            MouseFollower.Toggle(true);
            MouseFollower.SetData(spr, val);
        } 

        private void HandleItemSelection(InventoryUIItem invItemUI) {
            int idx = ItemList.IndexOf(invItemUI);
            if(idx == -1) return;
            OnDescriptionRequested?.Invoke(idx);
        }

        internal void UpdateDescription(int itemIdx, Sprite itemImg, string name, string description) {
            ItemDescription.SetDescription(itemImg, name, description);
            DeselectAllItems();
            ItemList[itemIdx].Select();
        }

        internal void ResetAllItems() {
            foreach (var item in ItemList) {
                item.ResetData();
                item.Deselect();
            }
        }

        #endregion
    }

}

