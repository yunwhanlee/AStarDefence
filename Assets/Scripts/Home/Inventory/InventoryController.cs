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
        [SerializeField] public List<InventoryItem> InitItems {
            get => DM._.DB.InvItemDBs;
        }

        void Start() {
            ivm = HM._.ivm;
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
            ivm.ResetAllItems();
            foreach (var item in inventoryState)
                ivm.UpdateData(item.Key, item.Value);
        }

        private void PrepareUI() {
            ivm.InitInventoryUI(InventoryData.Size);
            ivm.OnDescriptionRequested += HandleDescriptionRequest;
            ivm.OnSwapItems += HandleSwapItems;
            ivm.OnStartDragging += HandleDragging;
            ivm.OnItemActionRequested += HandleItemActionRequest;
        }

        private void HandleItemActionRequest(int itemIdx) {}

        private void HandleDragging(int itemIdx) {
            InventoryItem item = InventoryData.GetItemAt(itemIdx);
            if(item.IsEmpty) return;
            ivm.CreateDraggedItem(item.Data.Type, item.Data.Grade, item.Data.ItemImg, item.Quantity, item.Lv);
        }

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

        public void EquipItemSlot() {
            int curIdx = HM._.ivm.CurItemIdx;
            InventoryItem  curInvItem = InventoryData.ItemList[curIdx];

            //* 初期化
            InventoryData.ResetIsEquipData(curInvItem.Data.Type); // 「装置中」DimUI 
            HM._.ivm.ResetEquipDimUI(curInvItem.Data.Type); // ItemDt

            //* 現在着用したアイテムのアップデート
            InventoryData.ItemList[curIdx] = curInvItem.ChangeIsEquip(true); // IsEquip：True
            HM._.ivm.InvUIItemList[curIdx].EquipDim.SetActive(true); // DimUI 表示

            //* 装置スロットUI
            ItemSO dt = curInvItem.Data;
            switch(curInvItem.Data.Type) {
                case Enum.ItemType.Weapon:
                    HM._.ivEqu.WeaponInvUISlot.SetUIData(dt.Type, dt.Grade, dt.ItemImg, curInvItem.Quantity, curInvItem.Lv);
                    HM._.ivEqu.WeaponInvUISlot.ItemImgScaleUIEF.sprite = dt.ItemImg;
                    HM._.ivEqu.WeaponInvUISlot.ItemImgScaleUIEF.Play();
                    break;
                case Enum.ItemType.Shoes:
                    HM._.ivEqu.ShoesInvUISlot.SetUIData(dt.Type, dt.Grade, dt.ItemImg, curInvItem.Quantity, curInvItem.Lv);
                    HM._.ivEqu.ShoesInvUISlot.ItemImgScaleUIEF.sprite = dt.ItemImg;
                    HM._.ivEqu.ShoesInvUISlot.ItemImgScaleUIEF.Play();
                    break;
                case Enum.ItemType.Ring:
                    HM._.ivEqu.RingInvUISlot.SetUIData (dt.Type, dt.Grade, dt.ItemImg, curInvItem.Quantity, curInvItem.Lv);
                    HM._.ivEqu.RingInvUISlot.ItemImgScaleUIEF.sprite = dt.ItemImg;
                    HM._.ivEqu.RingInvUISlot.ItemImgScaleUIEF.Play();
                    break;
                case Enum.ItemType.Relic:
                    HM._.ivEqu.RelicInvUISlot.SetUIData (dt.Type, dt.Grade, dt.ItemImg, curInvItem.Quantity, curInvItem.Lv);
                    HM._.ivEqu.RelicInvUISlot.ItemImgScaleUIEF.sprite = dt.ItemImg;
                    HM._.ivEqu.RelicInvUISlot.ItemImgScaleUIEF.Play();
                    break;
            }
            
        }
    }

}