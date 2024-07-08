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
        [field:SerializeField] public int CurCateIdx;
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
        [field:SerializeField] public GameObject GradeInfoPopUp {get; private set;}
        [field:SerializeField] public GameObject PotentialInfoPopUp {get; private set;}

        [field:SerializeField] public Animator EquipPopUpAnim {get; private set;}

        [field:SerializeField] public GameObject AutoMergeBtnAlertIcon {get; private set;}
        [field:SerializeField] public GameObject InvAlertIcon {get; private set;}
        [field:SerializeField] public TMP_Text InvAlertCntTxt {get; private set;}

        [field:SerializeField] public RectTransform CateUnderline {get; private set;}

        [field:SerializeField] public InventoryUIItem ItemPf {get; private set;}
        [field:SerializeField] public RectTransform Content {get; set;}
        [field:SerializeField] public InventoryDescription InvDesc {get; set;}
        // [field:SerializeField] public MouseFollower MouseFollower {get; set;}
        [field:SerializeField] public InventoryUIItem[] InvUIItemArr {get; set;}

        // private int curDraggedItemIdx = -1;

        //* Actionで使えるint ➝ Index
        public event Action<int> OnDescriptionRequested;
        // public event Action<int> OnItemActionRequested;
        // public event Action<int> OnStartDragging;
        // public event Action<int, int> OnSwapItems;

        void Awake() {
            CurCateIdx = -1;
            Hide();
            InvUIItemArr = new InventoryUIItem[HM._.ivCtrl.InventoryData.InvArr.Length];
            // MouseFollower.Toggle(false);
            InvDesc.ResetDescription();
        }

#region EVENT
    /// <summary>
    /// カテゴリーアイコンクリック
    /// </summary>
    /// <param name="cateIdx"> 0：WEAPON、1：SHOES、2：ACCESARY、3：CROWN, 4：ETC</param>
    public void OnClickCateMenuIconBtn(int cateIdx) {
        SM._.SfxPlay(SM.SFX.ClickSFX);
        CurCateIdx = cateIdx;

        //* Move Tap UnderLine
        const int ORIGIN_X = -440;
        const int MOVE_X_UNIT = 220;
        CateUnderline.anchoredPosition = new Vector2(ORIGIN_X + (cateIdx * MOVE_X_UNIT), CateUnderline.anchoredPosition.y);

        //* ItemList 表示
        switch(cateIdx) {
            // case 0: Array.ForEach(InvUIItemArr, item => item.gameObject.SetActive(true)); break; // 全て表示
            case 0: ActiveCategoryItemList(Enum.ItemType.Weapon); break;
            case 1: ActiveCategoryItemList(Enum.ItemType.Shoes);  break;
            case 2: ActiveCategoryItemList(Enum.ItemType.Ring); break;
            case 3: ActiveCategoryItemList(Enum.ItemType.Relic); break;
            case 4: ActiveCategoryItemList(Enum.ItemType.Etc); break;
        } 
    }

    /// <summary>
    /// カテゴリに合うリストのみ表示
    /// </summary>
    private void ActiveCategoryItemList(Enum.ItemType category) {
        for(int i = 0; i < InvUIItemArr.Length; i++) {
            Debug.Log($"ActiveCategoryItemList():: Item {i} Type: {InvUIItemArr[i].Type}, Expected Type: {category}");
            InvUIItemArr[i].gameObject.SetActive(InvUIItemArr[i].Type == category);
        }
    }

    public void OnClickInventoryIconBtn() {
        // 最初にWEAPONカテゴリに初期化
        if(CurCateIdx == -1)
            ActiveCategoryItemList(Enum.ItemType.Weapon);
        // インベントリー開く
        HM._.ivCtrl.ShowInventory();
    } 
    public void OnClickInventoryPopUpBackBtn() => HM._.ivCtrl.HideInventory();
    public void OnClickInvItemAutoMergeBtn() => HM._.ivCtrl.InventoryData.AutoMergeEquipItem();
    public void OnClickGradeInfoBtn() {
        SM._.SfxPlay(SM.SFX.ClickSFX);
        GradeInfoPopUp.SetActive(true);
    }
    public void OnClickGradeInfoPopUpCloseBtn() {
        SM._.SfxPlay(SM.SFX.ClickSFX);
        GradeInfoPopUp.SetActive(false);
    }
    public void OnClickPotentialInfoBtn() {
        SM._.SfxPlay(SM.SFX.ClickSFX);
        PotentialInfoPopUp.SetActive(true);
    }
    public void OnClickPotentialInfoPopUpCloseBtn() {
        SM._.SfxPlay(SM.SFX.ClickSFX);
        PotentialInfoPopUp.SetActive(false);
    }
#endregion

#region FUNC
        public void SetInvAlertIcon(int newItemCnt) {
            InvAlertIcon.SetActive(newItemCnt > 0);
            if(InvAlertIcon.activeSelf)
                InvAlertCntTxt.text = $"{newItemCnt}";
        }

        /// <summary>
        ///* インベントリUIスロット 生成
        /// </summary>
        public void InitInventoryUI() {
            Debug.Log($"AA InventoryUIManager():: InitInventoryUI():: InvUIItemArr= {InvUIItemArr.Length}");
            //* ETC スロット
            for(int i = 0; i < InvUIItemArr.Length; i++) {
                // UIスロット 生成
                InventoryUIItem itemUIIns = Instantiate(ItemPf, Content);
                itemUIIns.Type = HM._.ivCtrl.InventoryData.InvArr[i].Data.Type;
                InvUIItemArr[i] = itemUIIns;

                // タイプ 設定 ( UIスロット ← INVENTORY FIXED INDEXデータ )
                Debug.Log($"InitInventoryUI():: InvUIItemArr {i} Type: {InvUIItemArr[i].Type} = {InvUIItemArr[i].Type}");

                // 最初はWEAPONカテゴリ 指定
                Debug.Log($"InitInventoryUI():: InvUIItemArr{i} Type is Weapon? -> {InvUIItemArr[i].Type == Enum.ItemType.Weapon}");
                // InvUIItemArr[i].gameObject.SetActive(InvUIItemArr[i].Type == Enum.ItemType.Weapon);

                // イベント 登録
                itemUIIns.OnItemClicked += HandleItemSelection;
                itemUIIns.OnItemClickShortly += HandleShowItemInfoPopUp;
            }

            //* EQUIP スロット
            foreach (var equipSlot in HM._.ivEqu.EquipItemSlotUIs) {
                // イベント 登録
                equipSlot.OnItemClicked += HandleItemSelection;
                equipSlot.OnItemClickShortly += HandleShowItemInfoPopUp;
            }
        }
        public void Show() {
            HM._.hui.SetTopNavOrderInLayer(isLocateFront: true);
            WindowObj.SetActive(true);
            ResetSelection();
        }
        public void Hide() {
            WindowObj.SetActive(false);
            HM._.hui.SetTopNavOrderInLayer(isLocateFront: false);
            // ResetDraggedItem();
        }
        public void ResetSelection() {
            InvDesc.ResetDescription();
            DeselectAllSlot();
        }
        private void DeselectAllSlot() {
            foreach(InventoryUIItem item in InvUIItemArr)
                item.Deselect();
        }

        /// <summary>
        /// インベントリーUIスロット 表示
        /// </summary>
        /// <param name="itemIdx">InventoryItem.InvArrのINDEX値</param>
        /// <param name="item"></param>ItemSOデータ<summary>
        public void UpdateUI(int itemIdx, InventoryItem item) {
            Debug.Log($"<color=white>UpdateData():: itemIdx= {itemIdx}, type= {item.Data.Type}, item= {item.Data.Name})</color>");

            InvUIItemArr[itemIdx].SetUI (
                item.Data.Type, 
                item.Data.Grade, 
                item.Data.ItemImg, 
                item.Quantity, 
                item.Lv, 
                item.RelicAbilities,
                item.IsEquip,
                item.IsNewAlert
            );
        }

        // private void ResetDraggedItem() {
        //     MouseFollower.Toggle(false);
        //     // curDraggedItemIdx = -1;
        // }

        /// <summary>
        /// UIインベントリーから、実際のアイテム情報を受け取る
        /// </summary>
        /// <param name="invItemUI">インベントリーUIのアイテム</param>
        // public InventoryItem GetItemFromUI(InventoryUIItem invItemUI) {
        //     int idx = Array.IndexOf(InvUIItemArr, invItemUI);
        //     if(idx == -1)
        //         return InventoryItem.GetEmptyItem();
        //     return HM._.ivCtrl.InventoryData.GetItemAt(idx);
        // }

        public InventoryItem GetCurItemUIFromIdx(int idx) {
            Debug.Log($"GetCurItemUIFromIdx():: idx= {idx}");
            if(idx == -1) {
                return HM._.ivCtrl.InventoryData.InvArr[0].GetEmptyItem();
            }
            else {
                return HM._.ivCtrl.InventoryData.GetItemAt(idx);
            }
        }

        /// <summary>
        /// アイテムスロット情報POPUP 表示
        /// </summary>
        private void HandleShowItemInfoPopUp(InventoryUIItem invItemUI) {
            Debug.Log($"HandleShowItemInfoPopUp(invItemUI.name= {invItemUI.name})::");
            DeselectAllSlot();

            if(invItemUI.IsEmpty)
                return;

            //* 選択したら、NewAlertアイコン 非表示
            if(invItemUI.AlertRedDot) {
                invItemUI.AlertRedDot.SetActive(false);
                HM._.ivCtrl.InventoryData.InvArr[CurItemIdx] = CurInvItem.ChangeIsNewAlert(false);
            }

            if(CurInvItem.Data.Type == Enum.ItemType.Etc) {
                if(CurInvItem.Data.name == $"{Etc.ConsumableItem.ChestCommon}") return;
                if(CurInvItem.Data.name == $"{Etc.ConsumableItem.ChestDiamond}") return;
                if(CurInvItem.Data.name == $"{Etc.ConsumableItem.ChestEquipment}") return;
                if(CurInvItem.Data.name == $"{Etc.ConsumableItem.ChestGold}") return;
                if(CurInvItem.Data.name == $"{Etc.ConsumableItem.ChestPremium}") return;
                SM._.SfxPlay(SM.SFX.ClickSFX);
                ConsumePopUp.SetActive(true);
            }
            else {
                SM._.SfxPlay(SM.SFX.ClickSFX);
                EquipPopUp.SetActive(true);
            }
        }

        /// <summary>
        /// インベントリーUIスロット選択
        /// </summary>
        /// <param name="invItemUI"></param>クリックしたスロット<summary>
        public void HandleItemSelection(InventoryUIItem invItemUI) {
            //* アイテムのINDEX 習得
            int idx;
            switch(invItemUI.name) {
                //* Equip スロットなら
                case InventoryEquipUIManager.WEAPON_SLOT_OBJ_NAME : 
                    idx = HM._.ivCtrl.FindCurrentEquipItemIdx(Enum.ItemType.Weapon);
                    if(idx == -1) OnClickCateMenuIconBtn((int)Enum.ItemType.Weapon); // スロットが空なら、該当なカテゴリ表示
                    break;
                case InventoryEquipUIManager.SHOES_SLOT_OBJ_NAME : 
                    idx = HM._.ivCtrl.FindCurrentEquipItemIdx(Enum.ItemType.Shoes);
                    if(idx == -1) OnClickCateMenuIconBtn((int)Enum.ItemType.Shoes); // スロットが空なら、該当なカテゴリ表示
                    break;
                case InventoryEquipUIManager.RING_SLOT_OBJ_NAME : 
                    idx = HM._.ivCtrl.FindCurrentEquipItemIdx(Enum.ItemType.Ring);
                    if(idx == -1) OnClickCateMenuIconBtn((int)Enum.ItemType.Ring); // スロットが空なら、該当なカテゴリ表示
                    break;
                case InventoryEquipUIManager.RELIC_SLOT_OBJ_NAME : 
                    idx = HM._.ivCtrl.FindCurrentEquipItemIdx(Enum.ItemType.Relic);
                    if(idx == -1) OnClickCateMenuIconBtn((int)Enum.ItemType.Relic); // スロットが空なら、該当なカテゴリ表示
                    break;
                //* インベントリーアイテムなら
                default:
                    idx = Array.IndexOf(InvUIItemArr, invItemUI);
                    break;
            }

            //* 適用
            Debug.Log($"HandleItemSelection(invItemUI= {invItemUI.name}):: idx= {idx}");
            if(idx == -1)
                return;

            CurItemIdx = idx;
            CurInvItem = HM._.ivCtrl.InventoryData.InvArr[idx]; //GetCurItemUIFromIdx(idx);
            OnDescriptionRequested?.Invoke(idx);
        }

        public void UpdateDescription(int itemIdx, ItemSO item, int quantity, int lv, AbilityType[] relicAbilities, bool isEquip) {
            Debug.Log($"UpdateDescription():: itemIdx= {itemIdx}, item.Name= {item.Name}, quantity= {quantity}");
            InvDesc.SetDescription(item, quantity, lv, relicAbilities, isEquip);
            DeselectAllSlot();
            InvUIItemArr[itemIdx].Select();
        }

        public void ResetAllItems() {
            Debug.Log($"ResetAllItems():: InvUIItemList.Length= {InvUIItemArr.Length}");
            foreach (InventoryUIItem item in InvUIItemArr) {
                item.ResetUI();
                item.Deselect();
            }
        }

        public void InitEquipDimUI(Enum.ItemType type) {
            //* UI Elements
            var filterItemUIs = Array.FindAll(InvUIItemArr, elem => elem.Type == type);
            Array.ForEach(filterItemUIs, elem => elem.EquipDim.SetActive(false));
        }
#endregion
    }

}

