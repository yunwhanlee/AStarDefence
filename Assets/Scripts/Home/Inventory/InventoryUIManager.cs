using System;
using System.Collections;
using System.Collections.Generic;
using Inventory.Model;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

namespace Inventory.UI 
{
    public class InventoryUIManager : MonoBehaviour {
        public InventoryController InvCtrl;

        [field: Header("RESOURCE")]
        [field:SerializeField] public Sprite NoneSpr;
        [field:SerializeField] public Sprite NoneBgSpr;
        [field:SerializeField] public Sprite[] TypeSprs;
        [field:SerializeField] public Color[] GradeClrs;
        [field:SerializeField] public Sprite[] GradeBgSprs;

        [field: Header("Elements")]
        [field:SerializeField] public GameObject WindowObj {get; private set;}
        [field:SerializeField] public GameObject EquipPopUp {get; private set;}
        [field:SerializeField] public GameObject ConsumePopUp {get; private set;}

        [field:SerializeField] public RectTransform CateUnderline {get; private set;}

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
            InvCtrl = GameObject.Find("InventoryController").GetComponent<InventoryController>();

            Hide();
            MouseFollower.Toggle(false);
            ItemDescription.ResetDescription();
        }

#region EVENT
    /// <summary>
    /// カテゴリーアイコンクリック
    /// </summary>
    /// <param name="cateIdx">０：全て、１：WEAPON、２：SHOES、３：ACCESARY、４：CROWN, 5：ETC</param>
    public void OnClickCateMenuIconBtn(int cateIdx) {
        //* Move Tap UnderLine
        const int ORIGIN_X = -450, MOVE_X_UNIT = 180;
        CateUnderline.anchoredPosition = new Vector2(ORIGIN_X + (cateIdx * MOVE_X_UNIT), CateUnderline.anchoredPosition.y);

        //* ItemList 表示
        switch(cateIdx) {
            case 0: ItemList.ForEach(item => item.gameObject.SetActive(true)); break;
            case 1: ActiveCategoryItemList(Enum.ItemType.Weapon); break;
            case 2: ActiveCategoryItemList(Enum.ItemType.Shoes);  break;
            case 3: ActiveCategoryItemList(Enum.ItemType.Accessories); break;
            case 4: ActiveCategoryItemList(Enum.ItemType.Relic); break;
            case 5: ActiveCategoryItemList(Enum.ItemType.Etc); break;
        } 
    }
    private void ActiveCategoryItemList(Enum.ItemType category) {
        //* 全て非表示
        ItemList.ForEach(item => item.gameObject.SetActive(false));
        //* カテゴリに合うリストのみ表示
        var filterList = ItemList.FindAll(item => item.Type == category);
        filterList.ForEach(item => item.gameObject.SetActive(true));
    }

    public void OnClickInventoryIconBtn() {
        InvCtrl.ShowInventory();
    }
    public void OnClickInventoryPopUpBackBtn() {
        InvCtrl.HideInventory();
    }
#endregion

    #region FUNC
        public void InitInventoryUI(int invSize) {
            for(int i = 0; i < invSize; i++) {
                InventoryUIItem item = Instantiate(ItemPf, Content);
                ItemList.Add(item);
                item.OnItemClicked += HandleItemSelection;
                item.OnItemBeginDrag += HandleBeginDrag;
                item.OnItemDroppedOn += HandleSwap;
                item.OnItemEndDrag += HandleEndDrag;
                item.OnItemClickShortly += HandleShowItemInfoPopUp;
            }
        }
        public void Show() {
            WindowObj.SetActive(true);
            ResetSelection();
        }
        public void Hide() {
            WindowObj.SetActive(false);
            ResetDraggedItem();
        }
        public void ResetSelection() {
            ItemDescription.ResetDescription();
            DeselectAllItems();
        }
        private void DeselectAllItems() {
            foreach(InventoryUIItem item in ItemList)
                item.Deselect();
        }

        public void UpdateData(int itemIdx, InventoryItem item) {
            ItemSO dt = item.Data;
            if(ItemList.Count > itemIdx)
                ItemList[itemIdx].SetData(dt.Type, dt.Grade, dt.ItemImg, item.Val);
        }
        private void ResetDraggedItem() {
            MouseFollower.Toggle(false);
            curDraggedItemIdx = -1;
        }

        /// <summary>
        ///* UIインベントリーから、実際のアイテム情報を受け取る
        /// </summary>
        /// <param name="invItemUI">インベントリーUIのアイテム</param>
        public InventoryItem GetItemFromUI(InventoryUIItem invItemUI) {
            int idx = ItemList.IndexOf(invItemUI);
            if(idx == -1)
                return InventoryItem.GetEmptyItem();
            return InvCtrl.InventoryData.GetItemAt(idx);            
        }

        private void HandleShowItemInfoPopUp(InventoryUIItem invItemUI) {
            DeselectAllItems();

            //* 実際のインベントリーへあるアイテム情報
            InventoryItem curInvItem = GetItemFromUI(invItemUI);

            if(curInvItem.Data.Type == Enum.ItemType.Etc)
                ConsumePopUp.SetActive(true);
            else
                EquipPopUp.SetActive(true);
        }
        private void HandleEndDrag(InventoryUIItem invItemUI)
            => ResetDraggedItem();

        private void HandleSwap(InventoryUIItem invItemUI) {
            int idx = ItemList.IndexOf(invItemUI);
            if(idx == -1) return;
            if(curDraggedItemIdx == -1) return;
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

        public void CreateDraggedItem(Enum.ItemType type, Enum.Grade grade, Sprite spr, int val) {
            MouseFollower.Toggle(true);
            MouseFollower.SetData(type, grade, spr, val);
        } 

        private void HandleItemSelection(InventoryUIItem invItemUI) {
            int idx = ItemList.IndexOf(invItemUI);
            if(idx == -1) return;
            OnDescriptionRequested?.Invoke(idx);
        }

        internal void UpdateDescription(int itemIdx, ItemSO item, int Val) {
            ItemDescription.SetDescription(item, Val);
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

