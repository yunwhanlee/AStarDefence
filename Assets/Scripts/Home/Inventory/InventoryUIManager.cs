using System;
using System.Collections;
using System.Collections.Generic;
using Inventory.Model;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Inventory.UI 
{
    public class InventoryUIManager : MonoBehaviour {
        [field:SerializeField] public int CurItemIdx;
        [field:SerializeField] public InventoryItem CurInvItem;

        [field: Header("RESOURCE")]
        [field:SerializeField] public Sprite NoneSpr;
        [field:SerializeField] public Color CommonSlotClr;
        [field:SerializeField] public Sprite NoneBgSpr;
        [field:SerializeField] public Sprite[] TypeSprs;
        [field:SerializeField] public Color[] GradeClrs;
        [field:SerializeField] public Color[] TopBgClrs;
        [field:SerializeField] public Color[] GlowBgClrs;
        [field:SerializeField] public Sprite[] GradeBgSprs;

        [field: Header("Elements")]
        [field:SerializeField] public GameObject WindowObj {get; private set;}
        [field:SerializeField] public GameObject EquipPopUp {get; private set;}
        [field:SerializeField] public GameObject ConsumePopUp {get; private set;}

        [field:SerializeField] public RectTransform CateUnderline {get; private set;}

        [field:SerializeField] public InventoryUIItem ItemPf {get; private set;}
        [field:SerializeField] public RectTransform Content {get; set;}
        [field:SerializeField] public InventoryDescription InvDesc {get; set;}
        [field:SerializeField] public MouseFollower MouseFollower {get; set;}
        [field:SerializeField] public List<InventoryUIItem> InvUIItemList = new List<InventoryUIItem>();

        private int curDraggedItemIdx = -1;

        //* Actionで使えるint ➝ Index
        public event Action<int> OnDescriptionRequested,
            OnItemActionRequested,
            OnStartDragging;
        public event Action<int, int> OnSwapItems;

        void Awake() {
            Hide();
            MouseFollower.Toggle(false);
            InvDesc.ResetDescription();
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
            case 0: InvUIItemList.ForEach(item => item.gameObject.SetActive(true)); break;
            case 1: ActiveCategoryItemList(Enum.ItemType.Weapon); break;
            case 2: ActiveCategoryItemList(Enum.ItemType.Shoes);  break;
            case 3: ActiveCategoryItemList(Enum.ItemType.Ring); break;
            case 4: ActiveCategoryItemList(Enum.ItemType.Relic); break;
            case 5: ActiveCategoryItemList(Enum.ItemType.Etc); break;
        } 
    }
    private void ActiveCategoryItemList(Enum.ItemType category) {
        //* 全て非表示
        InvUIItemList.ForEach(item => item.gameObject.SetActive(false));
        //* カテゴリに合うリストのみ表示
        var filterList = InvUIItemList.FindAll(item => item.Type == category);
        filterList.ForEach(item => item.gameObject.SetActive(true));
    }

    public void OnClickInventoryIconBtn() => HM._.ivCtrl.ShowInventory();
    public void OnClickInventoryPopUpBackBtn() => HM._.ivCtrl.HideInventory();
    public void OnClickInvItemAutoMergeBtn() {
        HM._.ivCtrl.InventoryData.AutoMergeEquipItem();
    }
#endregion

    #region FUNC
        /// <summary>
        ///* インベントリとEquipアイテムのスロットUI イベント登録
        /// </summary>
        public void InitInventoryUI(int invSize) {
            //* Inventory ItemUI Btn
            for(int i = 0; i < invSize; i++) {
                InventoryUIItem item = Instantiate(ItemPf, Content);
                InvUIItemList.Add(item);
                item.OnItemClicked += HandleItemSelection;
                item.OnItemClickShortly += HandleShowItemInfoPopUp;
                // item.OnItemBeginDrag += HandleBeginDrag;
                // item.OnItemDroppedOn += HandleSwap;
                // item.OnItemEndDrag += HandleEndDrag;
            }

            //* Equip Slot Btn
            foreach (var equipSlot in HM._.ivEqu.EquipItemSlotUIs) {
                equipSlot.OnItemClicked += HandleItemSelection;
                equipSlot.OnItemClickShortly += HandleShowItemInfoPopUp;
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
            InvDesc.ResetDescription();
            DeselectAllItems();
        }
        private void DeselectAllItems() {
            foreach(InventoryUIItem item in InvUIItemList)
                item.Deselect();
        }

        public void UpdateData(int itemIdx, InventoryItem item) {
            Debug.Log($"UpdateData(itemIdx= {itemIdx}, type= {item.Data.Type}, item= {item.Data.Name}):: -> SetUIData()");
            ItemSO dt = item.Data;
            if(InvUIItemList.Count > itemIdx)
                InvUIItemList[itemIdx].SetUIData (
                    dt.Type, 
                    dt.Grade, 
                    dt.ItemImg, 
                    item.Quantity, 
                    item.Lv, 
                    item.RelicAbilities,
                    item.IsEquip
                );
        }

        private void ResetDraggedItem() {
            MouseFollower.Toggle(false);
            curDraggedItemIdx = -1;
        }

        /// <summary>
        /// UIインベントリーから、実際のアイテム情報を受け取る
        /// </summary>
        /// <param name="invItemUI">インベントリーUIのアイテム</param>
        public InventoryItem GetItemFromUI(InventoryUIItem invItemUI) {
            int idx = InvUIItemList.IndexOf(invItemUI);
            if(idx == -1)
                return InventoryItem.GetEmptyItem();
            return HM._.ivCtrl.InventoryData.GetItemAt(idx);
        }
        public InventoryItem GetCurItemUIFromIdx(int idx) {
            if(idx == -1)
                return InventoryItem.GetEmptyItem();
            return HM._.ivCtrl.InventoryData.GetItemAt(idx);
        }

        /// <summary>
        /// インベントリの情報PopUp表示
        /// </summary>
        private void HandleShowItemInfoPopUp(InventoryUIItem invItemUI) {
            Debug.Log($"HandleShowItemInfoPopUp(invItemUI.name= {invItemUI.name})::");
            DeselectAllItems();
            if(invItemUI.IsEmpty)
                return;

            if(CurInvItem.Data.Type == Enum.ItemType.Etc) {
                if(CurInvItem.Data.name == $"{Etc.ConsumableItem.ChestCommon}") return;
                if(CurInvItem.Data.name == $"{Etc.ConsumableItem.ChestDiamond}") return;
                if(CurInvItem.Data.name == $"{Etc.ConsumableItem.ChestEquipment}") return;
                if(CurInvItem.Data.name == $"{Etc.ConsumableItem.ChestGold}") return;
                if(CurInvItem.Data.name == $"{Etc.ConsumableItem.ChestPremium}") return;
                ConsumePopUp.SetActive(true);
            }
            else {
                EquipPopUp.SetActive(true);
            }
        }

        // private void HandleEndDrag(InventoryUIItem invItemUI)
        //     => ResetDraggedItem();

        // private void HandleSwap(InventoryUIItem invItemUI) {
        //     int idx = InvUIItemList.IndexOf(invItemUI);
        //     if(idx == -1) return;
        //     if(curDraggedItemIdx == -1) return;
        //     OnSwapItems?.Invoke(curDraggedItemIdx, idx);
        //     HandleItemSelection(invItemUI);
        // }

        // private void HandleBeginDrag(InventoryUIItem invItemUI) {
        //     int idx = InvUIItemList.IndexOf(invItemUI);
        //     if(idx == -1) return;
        //     curDraggedItemIdx = idx;
        //     HandleItemSelection(invItemUI);
        //     OnStartDragging?.Invoke(idx);
        // }

        // public void CreateDraggedItem(Enum.ItemType type, Enum.Grade grade, Sprite spr, int quantity, int lv) {
        //     MouseFollower.Toggle(true);
        //     MouseFollower.SetUIData(type, grade, spr, quantity, lv);
        // } 

        public void HandleItemSelection(InventoryUIItem invItemUI) {
            //* アイテムのINDEX 習得
            int idx;
            switch(invItemUI.name) {
                //* Equip スロットなら
                case InventoryEquipUIManager.WEAPON_SLOT_OBJ_NAME : 
                    idx = HM._.ivCtrl.FindCurEquipItemIdx(Enum.ItemType.Weapon);
                    if(idx == -1) OnClickCateMenuIconBtn((int)Enum.ItemType.Weapon + 1); // スロットが空なら、該当なカテゴリ表示
                    break;
                case InventoryEquipUIManager.SHOES_SLOT_OBJ_NAME : 
                    idx = HM._.ivCtrl.FindCurEquipItemIdx(Enum.ItemType.Shoes);
                    if(idx == -1) OnClickCateMenuIconBtn((int)Enum.ItemType.Shoes + 1); // スロットが空なら、該当なカテゴリ表示
                    break;
                case InventoryEquipUIManager.RING_SLOT_OBJ_NAME : 
                    idx = HM._.ivCtrl.FindCurEquipItemIdx(Enum.ItemType.Ring);
                    if(idx == -1) OnClickCateMenuIconBtn((int)Enum.ItemType.Ring + 1); // スロットが空なら、該当なカテゴリ表示
                    break;
                case InventoryEquipUIManager.RELIC_SLOT_OBJ_NAME : 
                    idx = HM._.ivCtrl.FindCurEquipItemIdx(Enum.ItemType.Relic);
                    if(idx == -1) OnClickCateMenuIconBtn((int)Enum.ItemType.Relic + 1); // スロットが空なら、該当なカテゴリ表示
                    break;
                //* インベントリーアイテムなら
                default:
                    idx = InvUIItemList.IndexOf(invItemUI);
                    break;
            }
            //* 適用
            Debug.Log($"HandleItemSelection(invItemUI= {invItemUI.name}):: idx= {idx}");
            if(idx == -1) return;
            CurItemIdx = idx;
            CurInvItem = GetCurItemUIFromIdx(idx);
            OnDescriptionRequested?.Invoke(idx);
        }

        public void UpdateDescription(int itemIdx, ItemSO item, int quantity, int lv, AbilityType[] relicAbilities, bool isEquip) {
            InvDesc.SetDescription(item, quantity, lv, relicAbilities, isEquip);
            DeselectAllItems();
            InvUIItemList[itemIdx].Select();
        }

        public void ResetAllItems() {
            foreach (var item in InvUIItemList) {
                item.ResetData();
                item.Deselect();
            }
        }

        public void InitEquipDimUI(Enum.ItemType type) {
            //* UI Elements
            List<InventoryUIItem> filterItemUIs = InvUIItemList.FindAll(elem => elem.Type == type);
            filterItemUIs.ForEach(elem => elem.EquipDim.SetActive(false));
        }
        #endregion
    }

}

