using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryItem : MonoBehaviour {
    [field:SerializeField] private Image ItemImg {get; set;}
    [field:SerializeField] private TMP_Text ValTxt {get; set;}
    [field:SerializeField] private Image BorderImg {get; set;}
    public event Action<InventoryItem> OnItemClicked, 
        OnItemDroppedOn, 
        OnItemBeginDrag, 
        OnItemEndDrag,
        OnRightMouseBtnClick;
    bool IsEmpty = false;

    void Awake() {
        ResetData();
        Deselect();
    }
    public void ResetData() {
        ItemImg.gameObject.SetActive(false);
        IsEmpty = true;
    }
    public void Deselect() {
        BorderImg.enabled = false;
    }

    /// <summary> アイテムのデータ設定 </summary>
    /// <param name="val">Equipment : Lv, Consumable : Cnt</param>
    public void SetData(Sprite spr, int val) {
        ItemImg.gameObject.SetActive(true);
        ItemImg.sprite = spr;
        ValTxt.text = $"{val}";
        IsEmpty = false;
    }

    public void Select() {
        BorderImg.enabled = true;
    }

    public void OnBeginDrag() {
        if(IsEmpty) return;
        OnItemBeginDrag?.Invoke(this);
    }

    public void OnDrop() {
        OnItemDroppedOn?.Invoke(this);
    }
    public void OnEndDrag() {
        OnItemEndDrag?.Invoke(this);
    }

    public void OnPointerClick(BaseEventData data) {
        if(IsEmpty)
            return;
        PointerEventData pointerData = (PointerEventData)data;
        if(pointerData.button == PointerEventData.InputButton.Right)
            OnRightMouseBtnClick?.Invoke(this);
        else
            OnItemClicked?.Invoke(this);
    }
}
