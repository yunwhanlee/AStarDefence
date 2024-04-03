using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Inventory.UI
{
    public class InventoryUIItem : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IEndDragHandler, IDropHandler, IDragHandler {
        private Coroutine CorPushTimeID = null;
        private bool IsShortPush = false;
        [field:SerializeField] public Image ItemImg {get; set;}
        [field:SerializeField] public TMP_Text ValTxt {get; set;}
        [field:SerializeField] public Image BorderImg {get; set;}
        public event Action<InventoryUIItem> OnItemClicked, 
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
            BorderImg.color = Color.red;
            BorderImg.enabled = true;
        }

        public void Draggable() {
            BorderImg.color = Color.green;
        }

        private IEnumerator CoCheckPushTime() {
            IsShortPush = true;
            yield return Util.Time0_3;
            IsShortPush = false;
            Draggable();
        }

        public void OnPointerDown(PointerEventData eventData) {
            if(IsEmpty) return;
            Debug.Log("OnPointerDown");
            OnItemClicked?.Invoke(this);
            CorPushTimeID = StartCoroutine(CoCheckPushTime());
        }

        public void OnPointerUp(PointerEventData eventData) {
            Debug.Log("OnPointerUp");
            if(IsShortPush) {
                StopCoroutine(CorPushTimeID);
                OnRightMouseBtnClick?.Invoke(this);
            }
        }

        public void OnBeginDrag(PointerEventData eventData) {
            if(IsEmpty) return;
            if(IsShortPush) return;
            OnItemBeginDrag?.Invoke(this);
        }

        public void OnEndDrag(PointerEventData eventData) {
            if(IsShortPush) return;
            OnItemEndDrag?.Invoke(this);
        }

        public void OnDrop(PointerEventData eventData) {
            if(IsShortPush) return;
            OnItemDroppedOn?.Invoke(this);
        }

        public void OnDrag(PointerEventData eventData) {
            if(IsShortPush) return;
        }
    }
}
