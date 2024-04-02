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

    [field:SerializeField] public Sprite Image {get; set;}
    [field:SerializeField] public int Val {get; set;}
    [field:SerializeField] public string Title {get; set;}
    [field:SerializeField] public string Description {get; set;}

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
        ItemDescription.ResetDescription();

        ItemList[0].SetData(Image, Val);
    }
    public void Hide() {
        WindowObj.SetActive(false);
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

    private void HandleShowItemActions(InventoryItem obj) {
        HM._.ivm.EquipPopUp.SetActive(true);
    }

    private void HandleEndDrag(InventoryItem obj) {
        MouseFollower.Toggle(false);
    }

    private void HandleSwap(InventoryItem obj) {
    }

    private void HandleItemSelection(InventoryItem obj) {
        ItemDescription.SetDescription(Image, Title, Description);
        ItemList[0].Select();
    }

    private void HandleBeginDrag(InventoryItem obj) {
        MouseFollower.Toggle(true);
        MouseFollower.SetData(Image, Val);
    }
    #endregion
}
